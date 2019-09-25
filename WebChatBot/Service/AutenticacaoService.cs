using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebChatBot.Models;

namespace WebChatBot.LuisApiService
{
    public static class AutenticacaoService
    {
        public static bool AutenticarUsuario(string email, string senha)
        {
            var config = ConfigurarApi();
            var autenticacao = ObterAutenticacao(config);
            foreach (var autenticacoes in autenticacao)
            {
                if (autenticacoes.Email == email && autenticacoes.Senha == senha)
                    return true;
                return false;
            }
            return false;

        }
        private static List<Autenticacao> ObterAutenticacao(ApiConfig config)
        {
            var autenticacao = ApiService.ObterAutenticacao(config);
            return autenticacao;
        }
        private static ApiConfig ConfigurarApi()
        {
            var url = "https://apiwebchatbot.azurewebsites.net/";
            var api = "api/values";
            var config = new ApiConfig(url, api);
            return config;
        }
    }
}
