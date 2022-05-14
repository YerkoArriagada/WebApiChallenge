using Microsoft.EntityFrameworkCore;
using WebApiChallenge.Entidades;

namespace WebApiChallenge
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UsuarioItem>()
                .HasKey(ui => new { ui.itemId, ui.usuarioId });
        }

        public DbSet<Item> Items { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<UsuarioItem> UsuariosItems { get; set; }
    }
}