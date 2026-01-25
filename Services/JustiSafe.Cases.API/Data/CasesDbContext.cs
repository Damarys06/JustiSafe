using JustiSafe.Cases.API.Models;
using Microsoft.EntityFrameworkCore;

namespace JustiSafe.Cases.API.Data
{
    public class CasesDbContext : DbContext
    {
        public CasesDbContext(DbContextOptions<CasesDbContext> options) : base(options) { }

        public DbSet<Case> Cases { get; set; }
        public DbSet<Verdict> Verdicts { get; set; }
    }
}
