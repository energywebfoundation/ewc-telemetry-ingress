using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace webapi.Controllers
{
    public class LineProtocolClient : IDisposable
    {
        private readonly HttpClientHandler _httpClientHandler;
        private readonly HttpClient _httpClient;
        private readonly string _requestUri;

        private bool _disposedValue;

        public LineProtocolClient(LineProtocolConParams params_obj)
        {
            _httpClientHandler = new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate };
            _httpClient = new HttpClient(_httpClientHandler) { BaseAddress = params_obj.Address };

            _requestUri = GetUri(params_obj);
        }

        public async Task<string> WriteAsync(string content,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            HttpResponseMessage response;
            var stringContent = new StringContent(content.ToString(), Encoding.UTF8, "application/json");
            response = await Post(_requestUri, stringContent, cancellationToken);


            string httpStatusCode = ((int)response.StatusCode).ToString();

            if (response.IsSuccessStatusCode)
                return httpStatusCode;

            string errorMessage = await response.Content.ReadAsStringAsync();

            return httpStatusCode;

        }

        protected virtual Task<HttpResponseMessage> Post(string endpoint, HttpContent httpContent, CancellationToken cancellationToken)
        {
            Task<HttpResponseMessage> responseTask = _httpClient.PostAsync(endpoint, httpContent, cancellationToken);
            return responseTask;
        }

        private string GetUri(LineProtocolConParams params_obj)
        {
            string uri = $"write?db={Uri.EscapeDataString(params_obj.DBName)}";
            if (!string.IsNullOrEmpty(params_obj.User))
            {
                uri += $"&u={Uri.EscapeDataString(params_obj.User)}&p={Uri.EscapeDataString(params_obj.Password)}";
            }
            return uri;
        }


        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _httpClient?.Dispose();
                    _httpClientHandler?.Dispose();
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