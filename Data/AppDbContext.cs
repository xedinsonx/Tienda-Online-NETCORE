using Microsoft.EntityFrameworkCore;
using CrudDemoPro.Models;

namespace CrudDemoPro.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 🔹 Forzar nombre de tabla en minúsculas
            modelBuilder.Entity<Usuario>().ToTable("usuarios");

            base.OnModelCreating(modelBuilder);
        }
    }
}
