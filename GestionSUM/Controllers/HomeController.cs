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
                .Include(r => r.Turno)
                .Where(r => r.Fecha >= hoy && !r.Cancelada)
                .OrderBy(r => r.Fecha)
                .ThenBy(r => r.Turno.HoraInicio)
                .Take(5)
                .ToListAsync();

            return View(proximasReservas);
        }

        //public IActionResult SobreElSum()
        //{
        //    return View();
        //}

        public IActionResult SobreElSum()
        {
            var info = _context.SumInfos.FirstOrDefault();

            // Si no existe todavía, la creamos
            if (info == null)
            {
                info = new SumInfo
                {
                    InformacionGeneral = "",
                    Capacidad = "",
                    Equipamiento = "",
                    Reglas = ""
                };

                _context.SumInfos.Add(info);
                _context.SaveChanges();
            }

            return View(info);
        }
    }
}
