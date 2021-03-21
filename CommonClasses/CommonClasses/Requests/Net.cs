using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using CommonClasses.Serializers;
using CommonClasses.Parsers;
using System.Collections.Specialized;
using System.IO;

namespace CommonClasses.Request
{
    public class Net : IRequest
    {
        public const string Post = "POST";
        public const string Get = "GET";

        public V Request<T, V>(string url, string method = Get, T data = null, IAuthSerializer auth = null) where T : class where V : class
        {
            string response;
            var json = new JsonParser();
            using (var client = new WebClient())
            {

                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                client.Encoding = Encoding.UTF8;
                try
                {
                    if (auth != null)
                    {
                        Auth(client, auth);
                    }

                    if (method == Post)
                    {
                        var param = json.ParseObjToStr(data);
                        response = client.UploadString(url, param);
                    }
                    else
                    {
                        client.QueryString = SetParameters(data);
                        response = client.DownloadString(url);
                    }
                }
                catch (WebException ex)
                {
                    dynamic servResp = ex.Response;
                    if (ex.Response == null)
                    {
                        response = "{\"detail\":[\"Невозможно подключиться к серверу\"]}";
                    }
                    else
                    {
                        response = "{\"detail\":[\"Статус запроса " + servResp.StatusCode + "\"]}";
                    }
                }

            }

            return json.ParseStrToObj<V>(response);
        }

        public T Request<T>(string url, string method = Get, T data = null, IAuthSerializer auth = null) where T : class
        {
            string response;
            var json = new JsonParser();
            using (var client = new WebClient())
            {

                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                client.Encoding = Encoding.UTF8;
                try
                {
                    if (auth != null)
                    {
                        Auth(client, auth);
                    }


                    if (method == Post)
                    {
                        var param = json.ParseObjToStr(data);
                        response = client.UploadString(url, param);
                    }
                    else
                    {
                        client.QueryString = SetParameters(data);
                        response = client.DownloadString(url);
                    }

                }
                catch (WebException ex)
                {
                    dynamic servResp = ex.Response;
                    if (ex.Response == null)
                    {
                        response = "{\"detail\":[\"Невозможно подключиться к серверу\"]}";
                    }
                    else
                    {
                        response = "{\"detail\":[\"Статус запроса " + servResp.StatusCode +"\"]}";
                    }
                    
                }
            }

            return json.ParseStrToObj<T>(response);
        }

        public async Task<V> RequestAsync<T, V>(string url, string method = Get, T data = null, IAuthSerializer auth = null, DownloadProgressChangedEventHandler progressGet = null, UploadProgressChangedEventHandler progressPost = null) where T : class where V : class
        {
            string response;
            var json = new JsonParser();
            using (var client = new WebClient())
            {

                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                client.Encoding = Encoding.UTF8;
                try
                {
                    if (auth != null)
                    {
                        Auth(client, auth);
                    }

                    if (method == Post)
                    {
                        client.UploadProgressChanged += progressPost;
                        var param = json.ParseObjToStr(data);
                        response = await client.UploadStringTaskAsync(new Uri(url), param);
                    }
                    else
                    {
                        client.DownloadProgressChanged += progressGet;
                        client.QueryString = SetParameters(data);
                        response = await client.DownloadStringTaskAsync(url);
                    }

                }
                catch (WebException ex)
                {
                    dynamic servResp = ex.Response;
                    if (ex.Response == null)
                    {
                        response = "{\"detail\":[\"Невозможно подключиться к серверу\"]}";
                    }
                    else
                    {
                        response = "{\"detail\":[\"Статус запроса " + servResp.StatusCode + "\"]}";
                    }
                }

            }

            return json.ParseStrToObj<V>(response);
        }

        public async Task<T> RequestAsync<T>(string url, string method = Get, T data = null, IAuthSerializer auth = null, DownloadProgressChangedEventHandler progressGet = null, UploadProgressChangedEventHandler progressPost = null) where T : class
        {
            string response;
            var json = new JsonParser();
            using (var client = new WebClient())
            {

                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                client.Encoding = Encoding.UTF8;
                try
                {
                    if (auth != null)
                    {
                        Auth(client, auth);
                    }

                    if (method == Post)
                    {
                        client.UploadProgressChanged += progressPost;
                        var param = json.ParseObjToStr(data);
                        response = await client.UploadStringTaskAsync(new Uri(url), param);
                    }
                    else
                    {
                        client.DownloadProgressChanged += progressGet;
                        client.QueryString = SetParameters(data);
                        response = await client.DownloadStringTaskAsync(url);
                    }

                }
                catch (WebException ex)
                {
                    dynamic servResp = ex.Response;
                    if (ex.Response == null)
                    {
                        response = "{\"detail\":[\"Невозможно подключиться к серверу\"]}";
                    }
                    else
                    {
                        response = "{\"detail\":[\"Статус запроса " + servResp.StatusCode + "\"]}";
                    }
                }

            }

            return json.ParseStrToObj<T>(response);
        }

        private void Auth(WebClient client, IAuthSerializer auth)
        {
            var bytes = Encoding.UTF8.GetBytes(auth.username + ":" + auth.password);
            client.Headers.Add("Authorization", $"Basic {Convert.ToBase64String(bytes)}");
        }

        private NameValueCollection SetParameters<T>(T data) where T : class
        {
            var keyVal = new NameValueCollection();
            var json = new JsonParser();
            foreach (var dataParam in data.GetType().GetProperties())
            {
                if (typeof(IEnumerable<object>).IsAssignableFrom(dataParam.PropertyType))
                {
                    var val = json.ParseObjToStr(dataParam.GetValue(data));
                    keyVal.Set(dataParam.Name, val);
                }
                else
                {
                    keyVal.Set(dataParam.Name, dataParam.GetValue(data).ToString());
                }
            }
            return keyVal;
        }
    }
}
