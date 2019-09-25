using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace WebChatBot.Extensions
{
    public static class ApiExtension
    {
        public static HttpClient SetAuthorizationHeader(this HttpClient client, string token)
        {
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format(":{0}", token))));
            return client;
        }
        public static HttpClient SetAuthorizationEndPointKey(this HttpClient client, string token)
        {
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("EndpointKey " + token);
            return client;
        }
    }
}
