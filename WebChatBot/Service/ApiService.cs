using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WebChatBot.Models;

namespace WebChatBot.LuisApiService
{
    public static class ApiService
    {
        public static List<Autenticacao> ObterAutenticacao(ApiConfig config)
        {
            var response = ObterResponseApi(config);
            var autenticacao = ConverterParaAutenticacao(response);
            return autenticacao;
        }
        private static string ObterResponseApi(ApiConfig config)
        {
            using (HttpClient client = config.ObterClient())
            {
                var response = client.GetAsync(config.ObterUrlCompleta()).Result.Content.ReadAsStringAsync().Result;
                return response;
            }
        }
        private static List<Autenticacao> ConverterParaAutenticacao(string apiBody)
        {
            var autenticacao = new List<Autenticacao>();
            var jResponse = JToken.Parse(apiBody);
            var jAutenticacao = (JArray)jResponse;
            foreach (var jAutenticacoes in jAutenticacao)
            {
                autenticacao.Add(new Autenticacao(int.Parse(jAutenticacoes["id"].ToString()) ,jAutenticacoes["email"].ToString(), jAutenticacoes["senha"].ToString(), Double.Parse(jAutenticacoes["horasTrab"].ToString())));
            }
            return autenticacao;
        }
    }
}
