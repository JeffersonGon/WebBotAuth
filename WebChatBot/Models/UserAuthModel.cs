using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebChatBot.Models
{
    public class UserAuthModel
    {
        public string Email { get; set; }
        public string Senha { get; set; }
        public bool Log { get; set; }
    }
}
