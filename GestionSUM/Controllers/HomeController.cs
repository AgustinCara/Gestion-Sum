using System.Diagnostics;
using GestionSUM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionSUM.Data;

namespace GestionSUM.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var hoy = DateTime.Today;

            var proximasReservas = await _context.Reservas
                .Include(r => r.Usuario)
                .Where(r => r.Fecha >= hoy)
                .OrderBy(r => r.Fecha)
                .ThenBy(r => r.Turno)
                .Take(5) // mostramos solo las próximas 5
                .ToListAsync();

            return View(proximasReservas);
        }
    }
}
