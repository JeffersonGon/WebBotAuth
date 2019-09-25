using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebChatBot.Models
{
    public class Intencao
    {
        public string Nome { get; set; }
        public Intencao(string nome)
        {
            Nome = nome;
        }
    }
}
