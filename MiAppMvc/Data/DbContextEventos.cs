using MiAppMvc.Models;
using Microsoft.EntityFrameworkCore;

namespace MiAppMvc.Data
{
    public class DbContextEventos : DbContext
    {
        public DbContextEventos(DbContextOptions<DbContextEventos> options) : base(options){ }

        public DbSet<Eventos> Eventos { get; set; }
        public DbSet<Usuarios> Usuarios { get; set; }
        public DbSet<Empresa> Empresas { get; set; }
        public DbSet<EventosUsuarios> EventosUsuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<EventosUsuarios>()
                .HasKey(eu => new { eu.UsuarioId, eu.EventoId });

            modelBuilder.Entity<EventosUsuarios>()
                .HasOne(eu => eu.Usuario)
                .WithMany(u => u.EventosUsuarios)
                .HasForeignKey(eu => eu.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict); // Cambiar de CASCADE a NO ACTION

            modelBuilder.Entity<EventosUsuarios>()
                .HasOne(eu => eu.Evento)
                .WithMany(e => e.EventosUsuarios)
                .HasForeignKey(eu => eu.EventoId)
                .OnDelete(DeleteBehavior.Restrict); // Cambiar de CASCADE a NO ACTION

            modelBuilder.Entity<RolesUsuarios>().HasData(
                new RolesUsuarios { RoleId = 1, RolName = "Usuario" },
                new RolesUsuarios { RoleId = 2, RolName = "Administrador" },
                new RolesUsuarios { RoleId = 3, RolName = "Empresa" }
            );
        }
    }
}
