using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WebChatBot.Extensions;

namespace WebChatBot.Models
{
    public class ApiConfig
    {
        public string Url { get; set; }
        public string Api { get; set; }
        public string Token { get; set; }
        public ApiConfig(string url, string api)
        {
            Url = url;
            Api = api;
        }
        public ApiConfig(string url, string api, string token)
        {
            Url = url;
            Api = api;
            Token = token;
        }
        public HttpClient ObterClient()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(Url + Api);
            if (Token != null)
            {
                client.SetAuthorizationHeader(Token);
            }
            return client;
        }
        public string ObterUrlCompleta()
        {
            return Url + Api;
        }
    }
}
