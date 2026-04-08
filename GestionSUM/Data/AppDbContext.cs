using GestionSUM.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GestionSUM.Data
{
    // Heredamos de IdentityDbContext en lugar de DbContext
    public class AppDbContext : IdentityDbContext<Usuario, IdentityRole<int>, int>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }
        public DbSet<Reserva> Reservas { get; set; }
        public DbSet<Turno> Turnos { get; set; }
        public DbSet<SumInfo> SumInfos { get; set; }
    
    }
}