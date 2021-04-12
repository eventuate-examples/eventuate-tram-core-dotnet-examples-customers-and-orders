using ServiceCommon.Custom;
using ServiceCommon.OrderHistoryCommon;
using ServiceCommon.OrderHistoryTextSearchCommon;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ServiceCommon.Helpers
{
    public static class WebApiHelper
    {
        public static T WebApiCall<T>(string httpMethod, string uri, string body)
        {
            T result;
            // Do the http api call
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = httpMethod;
            if ((httpMethod == "GET") || (httpMethod == "DELETE"))
            {
                Task<WebResponse> response = httpWebRequest.GetResponseAsync();
                response.Wait();
                HttpWebResponse httpResponse = (HttpWebResponse)response.Result;
                using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    string resultString = streamReader.ReadToEnd();
                    result = JsonSerializer.Deserialize<T>(resultString, SerializerOptions());
                }
            }
            else if ((httpMethod == "POST") || (httpMethod == "PUT"))
            {
                // Send body
                Task<Stream> stream = httpWebRequest.GetRequestStreamAsync();
                stream.Wait();
                using (StreamWriter streamWriter = new StreamWriter(stream.Result))
                {
                    streamWriter.Write(body);
                    streamWriter.Flush();
                    Task<WebResponse> response = httpWebRequest.GetResponseAsync();
                    response.Wait();
                    HttpWebResponse httpResponse = (HttpWebResponse)response.Result;
                    using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        string resultString = streamReader.ReadToEnd();
                        result = JsonSerializer.Deserialize<T>(resultString, SerializerOptions());
                    }
                }
            }
            else
            {
                throw new InvalidOperationException("Invalid HTTP Method");
            }
            return result;
        }

        public static List<T> WebApiTextSearch<T>(string httpMethod, string uri)
        {
            // Do the http api call
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = httpMethod;
            Task<WebResponse> response = httpWebRequest.GetResponseAsync();
            response.Wait();
            HttpWebResponse httpResponse = (HttpWebResponse)response.Result;
            using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                string resultString = streamReader.ReadToEnd();
                return JsonSerializer.Deserialize<List<T>>(resultString, SerializerOptions());
            }

        }
        private static JsonSerializerOptions SerializerOptions()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            options.Converters.Add(new CustomDictionaryJsonConverter<long, OrderInfo>());
            return options;
        }
    }
}
