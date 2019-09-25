using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class AutorizacaoContext : DbContext
    {
        public AutorizacaoContext(DbContextOptions<AutorizacaoContext> options) : base(options)
        {
        }
        public DbSet<Autorizacao> Autorizacoes { get; set; }
    }
}
