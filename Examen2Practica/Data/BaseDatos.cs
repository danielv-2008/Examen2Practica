using Examen2Practica.Models;
using Microsoft.EntityFrameworkCore;

namespace Examen2Practica.Data
{
    public class BaseDatos : DbContext
    {
        public BaseDatos(DbContextOptions<BaseDatos> options) : base(options) { }

        public DbSet<Tutor> tutores { get; set; }
        public DbSet<Tesis> tesis { get; set; }
        public DbSet<Usuario> usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Un tutor no puede estar duplicado: nombre y correo únicos (sin distinguir mayúsculas)
            modelBuilder.Entity<Tutor>(tutor =>
            {
                tutor.HasIndex(u => u.Nombre).IsUnique();
                tutor.HasIndex(u => u.Correo).IsUnique();
            });

            // Una tesis no puede estar duplicada: el título es único
            modelBuilder.Entity<Tesis>(tesis =>
            {
                tesis.HasIndex(t => t.Titulo).IsUnique();

                // Muchas tesis -> un tutor
                tesis.HasOne(t => t.Tutor)
                     .WithMany(tu => tu.Tesis)
                     .HasForeignKey(t => t.IdTutor)
                     .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
