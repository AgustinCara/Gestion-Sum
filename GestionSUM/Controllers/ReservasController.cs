using GestionSUM.Data;
using GestionSUM.Models;
using GestionSUM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GestionSUM.Controllers
{
    [Authorize]
    public class ReservasController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<Usuario> _userManager;
        private readonly IEmailService _emailService;

        public ReservasController(AppDbContext context, UserManager<Usuario> userManager, IEmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            _emailService = emailService;
        }

        // GET: Reservas
        public async Task<IActionResult> Index()
        {
            var hoy = DateTime.Today;
            var reservas = _context.Reservas
              .Include(r => r.Turno)
              .Include(r => r.Usuario)
              .Where(r => !r.Cancelada)
              .Where(r => r.Fecha >= hoy);

            return View(await reservas.ToListAsync());

        }

        // GET: Reservas/Create


        public IActionResult Create(DateTime? fecha, string origen)
        {
            var reserva = new Reserva();
            ViewBag.Origen = origen;
            ViewBag.SumInfo = _context.SumInfos.FirstOrDefault();

            reserva.Fecha = fecha ?? DateTime.Today;


            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (int.TryParse(userIdString, out int userIdInt))
            {
                var usuarioLogueado = _context.Users.Where(u => u.Id == userIdInt).ToList();

                ViewBag.Usuarios = new SelectList(
                    usuarioLogueado,
                    "Id",
                    "NombreCompleto",
                    userIdInt
                );
            }

            ViewBag.Turnos = new SelectList(
              _context.Turnos.ToList(),
              "Id",
              "MomentoDelDia",
              reserva.TurnoId
            );

            if (fecha.HasValue)
                reserva.Fecha = fecha.Value;

            return View(reserva);
        }

        // POST: Reservas/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        // Cambiamos a async Task para manejar el envío de mail sin bloquear el servidor
        public async Task<IActionResult> Create(Reserva reserva)
        {
            if (reserva.Fecha.Date < DateTime.Today)
            {
                ModelState.AddModelError("", "No se pueden crear reservas en fechas pasadas.");
            }

            var turno = _context.Turnos.FirstOrDefault(t => t.Id == reserva.TurnoId);

            if (turno != null)
            {
                var inicioReserva = reserva.Fecha.Date + turno.HoraInicio;
                var horasFaltantes = (inicioReserva - DateTime.Now).TotalHours;

                if (horasFaltantes < 6)
                {
                    ModelState.AddModelError("", "No se puede reservar con menos de 6 horas de anticipación.");
                }
            }

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
                await _context.SaveChangesAsync(); // Usamos la versión asíncrona

                // --- LÓGICA DE ENVÍO DE MAIL ---
                try
                {
                    // Buscamos el usuario para obtener su Email y Nombre
                    var usuario = await _context.Users.FindAsync(reserva.UsuarioId);

                    if (usuario != null && turno != null)
                    {
                        // Llamamos al servicio (asegurate de haberlo inyectado en el constructor)
                        await _emailService.EnviarConfirmacionReservaAsync(
                            usuario.Email,
                            usuario.NombreCompleto,
                            reserva.Fecha.ToShortDateString(),
                            turno.MomentoDelDia
                        );
                    }
                }
                catch (Exception ex)
                {
                    // Si el mail falla, no queremos que el usuario piense que la reserva no se hizo.
                    // La reserva YA se guardó arriba, así que solo logueamos el error.
                    Console.WriteLine($"Error al enviar el mail: {ex.Message}");
                }

                return RedirectToAction(nameof(Index));
            }

            // Si llegamos acá es porque hubo un error, recargamos los ViewBags
            ViewBag.SumInfo = _context.SumInfos.FirstOrDefault();

            var usuarioLogueado = _context.Users.Where(u => u.Id == reserva.UsuarioId).ToList();
            ViewBag.Usuarios = new SelectList(usuarioLogueado, "Id", "NombreCompleto", reserva.UsuarioId);
            ViewBag.Turnos = new SelectList(_context.Turnos.ToList(), "Id", "MomentoDelDia", reserva.TurnoId);

            return View(reserva);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Create(Reserva reserva)
        //{

        //    if (reserva.Fecha.Date < DateTime.Today)
        //    {
        //        ModelState.AddModelError("", "No se pueden crear reservas en fechas pasadas.");
        //    }

        //    // Obtener el turno para conocer la hora
        //    var turno = _context.Turnos.FirstOrDefault(t => t.Id == reserva.TurnoId);

        //    if (turno != null)
        //    {
        //        var inicioReserva = reserva.Fecha.Date + turno.HoraInicio;
        //        var horasFaltantes = (inicioReserva - DateTime.Now).TotalHours;

        //        if (horasFaltantes < 6)
        //        {
        //            ModelState.AddModelError("", "No se puede reservar con menos de 6 horas de anticipación.");
        //        }
        //    }


        //    bool ocupado = _context.Reservas.Any(r =>
        //      r.Fecha == reserva.Fecha &&
        //      r.TurnoId == reserva.TurnoId &&
        //      !r.Cancelada
        //    );

        //    if (ocupado)
        //    {
        //        ModelState.AddModelError("", "Ese turno ya está reservado para ese día.");
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        _context.Reservas.Add(reserva);
        //        _context.SaveChanges();
        //        //ModelState.AddModelError("", "Turno reservado con exito.");

        //        return RedirectToAction(nameof(Index));
        //    }

        //    ViewBag.SumInfo = _context.SumInfos.FirstOrDefault();

        //    var usuarioLogueado = _context.Users.Where(u => u.Id == reserva.UsuarioId).ToList();
        //    ViewBag.Usuarios = new SelectList(usuarioLogueado, "Id", "NombreCompleto", reserva.UsuarioId);

        //    ViewBag.Turnos = new SelectList(_context.Turnos.ToList(), "Id", "MomentoDelDia", reserva.TurnoId);

        //    return View(reserva);
        //}





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


            var hoy = DateTime.Today;
            var inicioMes = new DateTime(hoy.Year, hoy.Month, 1);
            var finMes = inicioMes.AddMonths(1);

            ViewBag.ReservasCanceladasMes = _context.Reservas
              .Count(r =>
                r.Fecha >= inicioMes &&
                r.Fecha < finMes &&
                r.Cancelada
              );


            var reservasCanceladas = await _context.Reservas
              .Include(r => r.Usuario)
              .Include(r => r.Turno)
              .Where(r => r.Cancelada)
              .OrderByDescending(r => r.FechaCancelacion)
              .ToListAsync();

            return View(reservasCanceladas);
        }

        public async Task<IActionResult> HistorialReservas()
        {
            var reservas = _context.Reservas
              .Include(r => r.Turno)
              .Include(r => r.Usuario)
              .Where(r => !r.Cancelada)
              .OrderByDescending(r => r.Fecha);

            return View(await reservas.ToListAsync());

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
              .Where(r => !r.Cancelada && r.Turno != null)
              .ToListAsync();

            //Transformación 
            var eventos = reservas.Select(r => {
                DateTime inicio = r.Fecha.Date.Add(r.Turno.HoraInicio);
                DateTime fin = r.Fecha.Date.Add(r.Turno.HoraFin);

                if (r.Turno.HoraFin < r.Turno.HoraInicio)
                    fin = fin.AddDays(1);

                string titulo = UsuarioActual.Rol == RolUsuario.Administrador
                  ? $"{(r.Usuario?.NombreCompleto ?? "S/N")} ({(r.Usuario?.Departamento ?? "S/D")})"
                  : "Reservado";

                return new
                {
                    id = r.Id,
                    title = titulo,
                    start = inicio.ToString("yyyy-MM-ddTHH:mm:ss"),
                    end = fin.ToString("yyyy-MM-ddTHH:mm:ss"),
                    color = "#dc3545",
                    allDay = false
                };
            });

            return Json(eventos);
        }

    }
}
