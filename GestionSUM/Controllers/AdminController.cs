using GestionSUM.Data;
using GestionSUM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionSUM.Controllers
{
    public class AdminSumController : Controller
    {
        private readonly AppDbContext _context;

        public AdminSumController(AppDbContext context)
        {
            _context = context;
        }

        // GET
        public async Task<IActionResult> Edit()
        {
            var info = await _context.SumInfos.FirstOrDefaultAsync();

            if (info == null)
            {
                info = new SumInfo();
                _context.SumInfos.Add(info);
                await _context.SaveChangesAsync();
            }

            return View(info);
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SumInfo model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _context.Update(model);
            await _context.SaveChangesAsync();

            TempData["Ok"] = "Información del SUM actualizada correctamente";

            return RedirectToAction("Edit");
        }
    }
}