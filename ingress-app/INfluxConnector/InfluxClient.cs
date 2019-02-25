

using System;
using System.Net;
using System.Net.Http;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Collections.Generic;


namespace webapi.Controllers
{
    public class InfluxClient : IInfluxClient
    {
        private readonly ISubject<string> synSubject;
        private readonly IDisposable subscription;
        private readonly HttpClientHandler httpClientHandler;
        private readonly HttpClient httpClient;
        private readonly string _requestUri;
        private bool _disposedValue;

        public int LastInsertCount { get; private set; } = 0;

        public InfluxClient(LineProtocolConParams params_obj)
        {
            //populate necessary fields for Influx Connection
            httpClientHandler = new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate };
            httpClient = new HttpClient(httpClientHandler) { BaseAddress = params_obj.Address };

            _requestUri = $"write?db={Uri.EscapeDataString(params_obj.DBName)}";
            if (!string.IsNullOrEmpty(params_obj.User))
            {
                _requestUri += $"&u={Uri.EscapeDataString(params_obj.User)}&p={Uri.EscapeDataString(params_obj.Password)}";
            }

            //populate necessary fields for buffer
            Subject<string> subject = new Subject<string>();
            synSubject = Subject.Synchronize(subject);

            subscription = synSubject
                .Buffer(TimeSpan.FromSeconds(params_obj.FlushBufferSeconds), params_obj.FlushBufferItemsSize)
                .Subscribe(onNext: async (pointsList) => await SendToInflux(pointsList));
        }

        private async Task<string> SendToInflux(IList<string> content, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (content == null || content.Count == 0)
                return "";

            HttpResponseMessage response;
            var stringContent = new StringContent(string.Join(System.Environment.NewLine, content), Encoding.UTF8, "application/json");
            response = await Post(_requestUri, stringContent, cancellationToken);


            string httpStatusCode = ((int)response.StatusCode).ToString();

            if (response.IsSuccessStatusCode){
                LastInsertCount+=content.Count;
                return httpStatusCode;}

            string errorMessage = await response.Content.ReadAsStringAsync();
            //What to to in case of influx connection error, should reschedual items in queue?
            Enqueue(content);

            return httpStatusCode;

        }

        private Task<HttpResponseMessage> Post(string endpoint, HttpContent httpContent, CancellationToken cToken)
        {
            Task<HttpResponseMessage> responseTask = httpClient.PostAsync(endpoint, httpContent, cToken);
            return responseTask;
        }

        public void Enqueue(IList<string> pointsList)
        {
            foreach (var point in pointsList)
                synSubject.OnNext(point);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    httpClient?.Dispose();
                    httpClientHandler?.Dispose();
                    subscription?.Dispose();
                    
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