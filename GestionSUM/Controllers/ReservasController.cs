using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using GestionSUM.Data;
using GestionSUM.Models;
using GestionSUM.Services;

namespace GestionSUM.Controllers
{
    public class ReservasController : Controller
    {
        private readonly AppDbContext _context;

        public ReservasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Reservas
        public async Task<IActionResult> Index()
        {
            var reservas = _context.Reservas
                .Include(r => r.Turno)
                .Include(r => r.Usuario)
                .Where(r => !r.Cancelada);

            return View(await reservas.ToListAsync());
        }

        // GET: Reservas/Create


        public IActionResult Create(DateTime? fecha)
        {

            ViewBag.Usuarios = new SelectList(
                _context.Usuarios.ToList(),
                "Id",
                "Nombre"
            );

            ViewBag.Turnos = new SelectList(
                _context.Turnos.ToList(),
                "Id",
                "MomentoDelDia"
            );

            var reserva = new Reserva();

            if (fecha.HasValue)
                reserva.Fecha = fecha.Value;

            return View(reserva);
        }

        // POST: Reservas/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Reserva reserva)
        {
            bool ocupado = _context.Reservas.Any(r =>
                r.Fecha == reserva.Fecha &&
                r.TurnoId == reserva.TurnoId &&
                !r.Cancelada
            );

            if (ocupado)
            {
                ModelState.AddModelError("", "Ese turno ya está reservado para ese día.");
            }

            if (ModelState.IsValid)
            {
                _context.Reservas.Add(reserva);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Usuarios = new SelectList(_context.Usuarios, "Id", "Nombre");
            ViewBag.Turnos = new SelectList(_context.Turnos, "Id", "MomentoDelDia");

            return View(reserva);
        }



        //GET cancelar reservas
        public async Task<IActionResult> Delete(int id)
        {
            var reserva = await _context.Reservas
                .Include(r => r.Turno)
                .Include(r => r.Usuario)
                .FirstOrDefaultAsync(r => r.Id == id);


            if (reserva == null)
                return NotFound();

            if (UsuarioActual.Rol != RolUsuario.Administrador)
            {
                if (reserva.UsuarioId != UsuarioActual.UsuarioId)
                {
                    return RedirectToAction("Index", "Home");
                }

                var inicioReserva =
                    reserva.Fecha.Date + reserva.Turno.HoraInicio;

                var horasFaltantes =
                    (inicioReserva - DateTime.Now).TotalHours;


                if (horasFaltantes < 2)
                {
                    TempData["Error"] = "No se puede cancelar la reserva con menos de 2 horas de anticipacion.";
                    return RedirectToAction(nameof(Index));
                }
            }

            return View(reserva);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, string motivoCancelacion)
        {
            var reserva = await _context.Reservas
                .Include(r => r.Turno)
                .FirstOrDefaultAsync(r => r.Id == id);


            if (reserva == null)
                return NotFound();

            //if (
            //    UsuarioActual.Rol != RolUsuario.Administrador &&
            //    reserva.UsuarioId != UsuarioActual.UsuarioId
            //)
            //{
            //    return RedirectToAction("Index", "Home");
            //}

            if (UsuarioActual.Rol != RolUsuario.Administrador)
            {
                if (reserva.UsuarioId != UsuarioActual.UsuarioId)
                {
                    return RedirectToAction("Index", "Home");
                }

                var inicioReserva =
                    reserva.Fecha.Date + reserva.Turno.HoraInicio;

                var horasFaltantes =
                    (inicioReserva - DateTime.Now).TotalHours;


                if (horasFaltantes < 2)
                {
                    TempData["Error"] = "No se puede cancelar la reserva con menos de 2 horas de anticipación.";
                    return RedirectToAction(nameof(Index));
                }
            }

            reserva.Cancelada = true;
            reserva.MotivoCancelacion = motivoCancelacion;
            reserva.FechaCancelacion = DateTime.Now;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Historial()
        {
            if (UsuarioActual.Rol != RolUsuario.Administrador)
            {
                return RedirectToAction("Index", "Home");
            }

            var reservasCanceladas = await _context.Reservas
                .Include(r => r.Usuario)
                .Where(r => r.Cancelada)
                .OrderByDescending(r => r.FechaCancelacion)
                .ToListAsync(); 

            return View(reservasCanceladas);
        }


        public IActionResult Calendario()
        {
            return View();
        }

        public async Task<IActionResult> ReservasCalendario()
        {
            var reservas = await _context.Reservas
                .Include(r => r.Usuario)
                .Include(r => r.Turno)
                .Where(r => !r.Cancelada)
                .ToListAsync();

            var eventos = reservas.Select(r => new
            {
                id = r.Id,
                title = UsuarioActual.Rol == RolUsuario.Administrador
                    ? $"{r.Usuario.Nombre} ({r.Usuario.Departamento})"
                    : "Reservado",

                start = (r.Fecha.Date + r.Turno.HoraInicio)
                    .ToString("yyyy-MM-ddTHH:mm:ss"),

                end = (r.Fecha.Date + r.Turno.HoraFin)
                    .ToString("yyyy-MM-ddTHH:mm:ss"),

                color = "#dc3545"
            });


            return Json(eventos);
        }

    }
}
