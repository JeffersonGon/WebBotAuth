using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/values")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly AutorizacaoContext _context;
        public ValuesController(AutorizacaoContext context) 
        {
            _context = context;
            if (_context.Autorizacoes.Count() == 0)
            {
                _context.Autorizacoes.Add(new Autorizacao { Email = "jeffinho@konia.com.br", Senha = "123456", HorasTrab = 40 });
                _context.Autorizacoes.Add(new Autorizacao { Email = "arthurito@konia.com.br", Senha = "123456", HorasTrab = 80 });
                _context.SaveChanges();
            }
        }
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<Autorizacao>> Get()
        {
            return _context.Autorizacoes.ToList();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
