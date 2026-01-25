using JustiSafe.Identity.API.Models;
using Microsoft.EntityFrameworkCore;

namespace JustiSafe.Identity.API.Data
{
    public class IdentityDbContext : DbContext
    {
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
    }
}
