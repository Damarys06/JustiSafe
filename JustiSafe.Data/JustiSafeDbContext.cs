using Microsoft.EntityFrameworkCore;
using JustiSafe.Data.Entities;

namespace JustiSafe.Data
{
    public class JustiSafeDbContext : DbContext
    {
        // Estas propiedades DbSet se convertirán en tus tablas en SQL Server
        public DbSet<User> Users { get; set; }
        public DbSet<Case> Cases { get; set; }
        public DbSet<Verdict> Verdicts { get; set; }

        // El constructor obligatorio para pasar la configuración desde el proyecto Web
        public JustiSafeDbContext(DbContextOptions<JustiSafeDbContext> options)
            : base(options)
        {
        }

        // Configuración adicional (Opcional, pero buena práctica para asegurar relaciones)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Asegurar que el AnonCode sea único en la base de datos
            modelBuilder.Entity<Case>()
                .HasIndex(c => c.AnonCode)
                .IsUnique();
        }
    }
}