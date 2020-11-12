using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace remitservice.Models
{
    public class SymexDbContext:DbContext
    {
        public SymexDbContext() { }

        public SymexDbContext( DbContextOptions<SymexDbContext> options) : base(options)
        {
        }

        public DbSet<RemitterModel> remitterData { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=CTINLT006;initial catalog=NewSymex;integrated security=true");
        }
    }
}
