using GestionSUM.Data;
using GestionSUM.Filters;
using GestionSUM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity; 
namespace GestionSUM.Controllers
{
    [AdminOnly]
    public class UsuariosController : Controller
    {
        private readonly UserManager<Usuario> _userManager;

        public UsuariosController(UserManager<Usuario> userManager)
        {
            _userManager = userManager;
        }

        // GET: Usuarios
        public async Task<IActionResult> Index()
        {
            return View(await _userManager.Users.ToListAsync());
        }

        // GET: Usuarios/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Usuarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Usuario usuario)
        {
            if (ModelState.IsValid)
            {

                usuario.UserName = usuario.Email;

                // CreateAsync se encarga de hashear la password y guardar en DB
                var result = await _userManager.CreateAsync(usuario, "Password123!");

                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }

                // Si hay errores (ej: email duplicado), los agregamos al modelo
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(usuario);
        }
    }
}