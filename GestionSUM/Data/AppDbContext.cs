using Microsoft.EntityFrameworkCore;
using GestionSUM.Models;


namespace GestionSUM.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) 
        { }


        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Reserva> Reservas { get; set; }
        public DbSet<Turno> Turnos { get; set; }

    }

}
