using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebChatBot.Models
{
    public class LuisApiConfig
    {
        public string Url { get; set; }
        public string Query { get; set; }
        public LuisApiConfig(string url, string query)
        {
            Url = url;
            Query = query;
        }
        public HttpClient ObterClient()
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri(Url + Query)
            };
            return client;
        }
        public string ObterUrlCompleta()
        {
            return Url + Query;
        }
    }
}
