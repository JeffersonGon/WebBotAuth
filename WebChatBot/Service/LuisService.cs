using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WebChatBot.Models;

namespace WebChatBot.LuisApiService
{
    public static class LuisService
    {
        public static string ObterIntencao(string query)
        {
            var config = ConfigurarLuisApi(query);
            var response = ObterValorGet(config);
            var intencao = ConverterParaIntencao(response).Nome;
            return intencao;
        }
        private static string ObterValorGet(LuisApiConfig config)
        {
            using (HttpClient client = config.ObterClient())
            {
                var response = client.GetAsync(config.ObterUrlCompleta()).Result.Content.ReadAsStringAsync().Result;
                return response;
            }
        }
        private static Intencao ConverterParaIntencao(string apiBody)
        {
            var jResponse = JToken.Parse(apiBody);
            var jIntencao = jResponse["topScoringIntent"];
            var intencao = new Intencao(jIntencao["intent"].ToString());
            return intencao;
        }
        private static LuisApiConfig ConfigurarLuisApi(string query)
        {
            var url = "https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/7faa9c91-f7ef-41dd-8cf6-140a12bf304a?verbose=true&timezoneOffset=0&subscription-key=09e3af3648734585b3ccb0167fdc2148&q=";
            var config = new LuisApiConfig(url, query);
            return config;
        }
    }
}
