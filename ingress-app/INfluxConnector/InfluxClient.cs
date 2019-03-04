

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
    - Optimazation testing - buffer flush settings time and # of requests
    - Making buffer fail safe
    - Influx response based buffer behavior ( for minor faliures handle internally)
    - Application level Logging
 */
namespace webapi.Controllers
{
    public class InfluxClient : IInfluxClient
    {
        private readonly ISubject<string> _synSubject;
        private readonly IDisposable _subscription;
        private readonly HttpClientHandler _httpClientHandler;
        private readonly HttpClient _httpClient;
        private readonly string _requestUri;
        private bool _disposedValue;

        public int LastInsertCount { get; private set; } = 0;

        public InfluxClient(LineProtocolConnectionParameters paramsObj)
        {
            //populate necessary fields for Influx Connection
            _httpClientHandler = new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate };
            _httpClient = new HttpClient(_httpClientHandler) { BaseAddress = paramsObj.Address };

            _requestUri = $"write?db={Uri.EscapeDataString(paramsObj.DBName)}";
            if (!string.IsNullOrEmpty(paramsObj.User))
            {
                _requestUri += $"&u={Uri.EscapeDataString(paramsObj.User)}&p={Uri.EscapeDataString(paramsObj.Password)}";
            }

            //populate necessary fields for buffer
            Subject<string> subject = new Subject<string>();
            _synSubject = Subject.Synchronize(subject);

            _subscription = _synSubject
                .Buffer(TimeSpan.FromSeconds(paramsObj.FlushBufferSeconds), paramsObj.FlushBufferItemsSize)
                .Subscribe(onNext: async (pointsList) => await SendToInflux(pointsList));
        }

        private async Task<string> SendToInflux(IList<string> content, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (content == null || content.Count == 0)
            {
                return "";
            }

            var stringContent = new StringContent(
                string.Join(Environment.NewLine, content), 
                Encoding.UTF8, 
                "application/json");

            HttpResponseMessage response = null;
            try
            {
                response = await Post(_requestUri, stringContent, cancellationToken);


                string httpStatusCode = ((int)response.StatusCode).ToString();

                if (!response.IsSuccessStatusCode)
                {
                    throw new WebException("Got " + httpStatusCode + " from database, " +
                                           response.Content.ReadAsStringAsync().Result);
                }

                LastInsertCount=content.Count;
                return httpStatusCode;

            }
            catch (Exception e)
            {
                string errorMessage = $"Unable to insert into database: {e.Message}";
                if (response != null)
                {
                    errorMessage += await response.Content.ReadAsStringAsync();    
                }
                
                //What to to in case of influx connection error, should reschedual items in queue?
                Enqueue(content);

                Console.WriteLine("ERROR: " + errorMessage);
            }

            return "error";

        }

        private Task<HttpResponseMessage> Post (string endpoint, HttpContent httpContent, CancellationToken cToken) 
        {
            return  _httpClient.PostAsync(endpoint, httpContent, cToken);
        }

        public void Enqueue(IList<string> pointsList)
        {
            foreach (var point in pointsList)
            {
                _synSubject.OnNext(point);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _httpClient?.Dispose();
                    _httpClientHandler?.Dispose();
                    _subscription?.Dispose();
                    
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }

}