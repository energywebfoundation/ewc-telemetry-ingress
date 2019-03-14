

using System;
using System.Net;
using System.Net.Http;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Collections.Generic;

/*
TODOs:
    - Define Influx RTO, according to that write requests on disk and once Influx is up read requests and resent to Influx
    - Optimization testing - buffer flush settings time and # of requests
 */

namespace webapi.Controllers
{
    /// <summary>
    /// The Class having functions for data queuing, HTTP connection to Influx and data persistance to Influx Influx DB.
    /// </summary>
    public class InfluxClient : IInfluxClient
    {
        private readonly ISubject<string> _synSubject;
        private readonly IDisposable _subscription;
        private readonly ISubject<string> _synSubjectSecondQueue;
        private readonly IDisposable _subscriptionSecondQueue;
        private readonly HttpClientHandler _httpClientHandler;
        private readonly HttpClient _httpClient;
        private readonly string _requestUri;
        private bool _disposedValue;

        /// <summary>
        /// LastInsertCount tracks number of points inserted into Influx in last Flush trigger.Its used for unit testing only.
        /// </summary>
        public int LastInsertCount { get; private set; } = 0;

        /// <summary>
        /// InfluxClient Constructor. The constructor expects LineProtocolConnectionParameters in params. It does initialization of HTTP Client and HTTP Client Handler.
        /// It also does initialization of worker buffer and failure handler buffer.
        /// </summary>
        /// <param name="paramsObj">The reference of Line Protocol Connection parameters.</param>
        /// <returns>returns instance of InfluxClient</returns>
        public InfluxClient(LineProtocolConnectionParameters paramsObj)
        {
            //1. populate necessary fields for Influx Connection

            //create HTTP Client for connection to Influx
            _httpClientHandler = new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate };
            _httpClient = new HttpClient(_httpClientHandler) { BaseAddress = paramsObj.Address };

            //Create Influx POST request URL
            _requestUri = $"write?db={Uri.EscapeDataString(paramsObj.DBName)}";
            if (!string.IsNullOrEmpty(paramsObj.User))
            {
                _requestUri += $"&u={Uri.EscapeDataString(paramsObj.User)}&p={Uri.EscapeDataString(paramsObj.Password)}";
            }


            //2. populate necessary fields for buffer ( Using reactive Programming Rx )

            //Create Subject for first buffer (worker buffer)
            Subject<string> subject = new Subject<string>();
            _synSubject = Subject.Synchronize(subject);

            //Create first buffer ( worker buffer) with provided buffer flush time or buffer item limit for flush
            _subscription = _synSubject
                .Buffer(TimeSpan.FromSeconds(paramsObj.FlushBufferSeconds), paramsObj.FlushBufferItemsSize)
                .Subscribe(onNext: async (pointsList) => await SendToInflux(pointsList, true));



            //Second buffer in case first fails. If requests fail in first buffer it will queue in 2nd buffer
            //Create Subject for second buffer (failure handler buffer)
            Subject<string> subjectSecondQueue = new Subject<string>();
            _synSubjectSecondQueue = Subject.Synchronize(subjectSecondQueue);

            //Create second buffer ( failure handler buffer) with provided buffer flush time or buffer item limit for flush
            _subscriptionSecondQueue = _synSubjectSecondQueue
                .Buffer(TimeSpan.FromSeconds(paramsObj.FlushSecondBufferSeconds), paramsObj.FlushSecondBufferItemsSize)
                .Subscribe(onNext: async (pointsListSecondQueue) => await SendToInflux(pointsListSecondQueue, false));
        }

        /// <summary>
        /// Buffer data flush method. It is triggered when one of specified buffer condition is true
        /// In case worker buffer fails then data is flushed to failure handler buffer. Buffers flushing settings can be configured using appsettings.json
        /// </summary>
        /// <param name="content">The List of Influx points</param>
        /// <param name="workerQueue">The Flag for indication of worker buffer or failure handler buffer</param>
        /// <param name="cancellationToken">The Flag for CancellationToken</param>
        /// <returns>returns instance of SignatureVerifyException</returns>
        private async Task<string> SendToInflux(IList<string> content, bool workerQueue, CancellationToken cancellationToken = default(CancellationToken))
        {
            //this function is called when buffer flush is triggered based on provided time or item size limit
            if (content == null || content.Count == 0)
            {
                return "";
            }

            // creating Influx Points contents by joining flushed data
            var stringContent = new StringContent(
                string.Join(Environment.NewLine, content),
                Encoding.UTF8,
                "application/json");
            Console.WriteLine("{0} Buffer flush call", (workerQueue ? "Worker " : "Failure Handler "));

            HttpResponseMessage response = null;
            try
            {
                // HTTP Post Request to Influx
                response = await _httpClient.PostAsync(_requestUri, stringContent, cancellationToken);

                // Status code returned from Influx
                string httpStatusCode = ((int)response.StatusCode).ToString();

                if (!response.IsSuccessStatusCode)
                {
                    throw new WebException("Got " + httpStatusCode + " from database, " +
                                           response.Content.ReadAsStringAsync().Result);
                }

                LastInsertCount = content.Count;

                //Success will always return with following
                return httpStatusCode;

            }
            catch (Exception e)
            {
                string errorMessage = $"Unable to insert into database: {e.Message}";
                if (response != null)
                {
                    errorMessage += await response.Content.ReadAsStringAsync();
                }

                //In case of failure worker queue data will be placed in failure handler queue
                if (workerQueue)
                {
                    Enqueue(content, !workerQueue);
                }
                /*else{
                    // TODO What to do when failure handler queue fails
                }*/

                Console.WriteLine("ERROR: {0}", errorMessage);
            }

            return "Error";

        }

        /// <summary>
        ///  Enqueue method for adding data to buffer.
        /// </summary>
        /// <param name="pointsList">The List of Influx points</param>
        /// <param name="workerQueue">The Flag for indication if provided points should be enqueued into worker buffer or failure handler buffer</param>
        public void Enqueue(IList<string> pointsList, bool workerQueue)
        {
            // Enqueue method for putting data into buffer, workerQueue is flag for putting data into worker or failure handler buffers
            ISubject<string> sub = workerQueue ? _synSubject : _synSubjectSecondQueue;

            //iterates on incoming list and push data in buffer
            foreach (var point in pointsList)
            {
                //Verify Influx Point, if it is invalid just ignore that
                if (InfluxPointVerifier.verifyPoint(point))
                    sub.OnNext(point);
            }
        }

        /// <summary>
        ///  Enqueue method for adding single point to buffer.
        /// </summary>
        /// <param name="point">The single Influx point</param>
        /// <param name="workerQueue">The Flag for indication if provided points should be enqueued into worker buffer or failure handler buffer</param>
        public void Enqueue(string point, bool workerQueue)
        {
            // Enqueue method for putting data into buffer, workerQueue is flag for putting data into worker or failure handler buffers
            ISubject<string> sub = workerQueue ? _synSubject : _synSubjectSecondQueue;

            //Verify Influx Point, if it is invalid just ignore that
            if (InfluxPointVerifier.verifyPoint(point))
                sub.OnNext(point);
        }

        /// <summary>
        ///  Method for Disposal of InfluxClient and calling dispose for underlying http client, http client handler, subscription and subscription for second queue
        /// </summary>
        /// <param name="disposing">The single Influx point</param>
        protected virtual void Dispose(bool disposing)
        {
            //dispose allocated resources
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _httpClient?.Dispose();
                    _httpClientHandler?.Dispose();
                    _subscription?.Dispose();
                    _subscriptionSecondQueue?.Dispose();

                }

                _disposedValue = true;
            }
        }

        /// <summary>
        ///  Method for Disposal of InfluxClient 
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }
    }

}