using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using webapi;

namespace stresstests
{
    public class HttpRequestsManager
    {
        private string _url;
        private HttpClient _client;
        public HttpRequestsManager(string url, bool allowInvalidCerts)
        {
            _url = url;

            HttpClientHandler handler = new HttpClientHandler();
            // allowing invalid SSL certificates for testing here
            if (allowInvalidCerts)
            {
                handler.ClientCertificateOptions = ClientCertificateOption.Manual;
                handler.ServerCertificateCustomValidationCallback =
                    (httpRequestMessage, cert, cetChain, policyErrors) =>
                {
                    return true;
                };

                _client = new HttpClient(handler);
            }
            else
            {
                _client = new HttpClient();
            }


        }

        public async Task<string> sendData(InfluxTelemetry inobj)// RSACryptoServiceProvider RSAalg)
        {
            //InfluxTelemetry inobj = generateRandomData(RSAalg);
            var json = JsonConvert.SerializeObject(inobj);
            var response = await Request(HttpMethod.Post, json, _client);
            string ress = response.StatusCode.ToString();
            //string responseText = await response.Content.ReadAsStringAsync();
            //Console.WriteLine(ress);
            return ress;
        }

        async Task<HttpResponseMessage> Request(HttpMethod pMethod, string pJsonContent, HttpClient _Client)
        {

            var httpRequestMessage = new HttpRequestMessage();
            httpRequestMessage.Method = pMethod;
            httpRequestMessage.RequestUri = new Uri(_url);
            HttpContent httpContent = new StringContent(pJsonContent, Encoding.UTF8, "application/json");
            httpRequestMessage.Content = httpContent;
            return await _Client.SendAsync(httpRequestMessage);
        }
    }
}