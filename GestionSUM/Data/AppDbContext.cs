using GestionSUM.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GestionSUM.Data
{
    public class AppDbContext : IdentityDbContext<Usuario, IdentityRole<int>, int>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Reserva> Reservas { get; set; }
        public DbSet<Turno> Turnos { get; set; }
        public DbSet<SumInfo> SumInfos { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Tablas de Identity
            builder.Entity<Usuario>().ToTable("Usuarios");
            builder.Entity<IdentityRole<int>>().ToTable("Roles");
            builder.Entity<IdentityUserRole<int>>().ToTable("UsuariosRoles");
            builder.Entity<IdentityUserClaim<int>>().ToTable("UsuariosClaims");
            builder.Entity<IdentityUserLogin<int>>().ToTable("UsuariosLogins");
            builder.Entity<IdentityUserToken<int>>().ToTable("UsuariosTokens");
            builder.Entity<IdentityRoleClaim<int>>().ToTable("RolesClaims");

            // Tablas de la aplicación
            builder.Entity<Reserva>().ToTable("Reservas");
            builder.Entity<Turno>().ToTable("Turnos");
            builder.Entity<SumInfo>().ToTable("SumInfos");
        }
    }
}