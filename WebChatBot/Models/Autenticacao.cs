using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebChatBot.Models
{
    public class Autenticacao
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public double HorasTrab { get; set; }
        public Autenticacao(int id, string email, string senha, double horas)
        {
            Id = id;
            Email = email;
            Senha = senha;
            HorasTrab = horas;
        }
    }
}
