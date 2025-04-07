using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SamaraProject1.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SamaraProject1.Controllers
{
    public class TestimonioController : Controller
    {
        private readonly SamaraMarketContext _context;

        public TestimonioController(SamaraMarketContext context)
        {
            _context = context;
        }

        // GET: Testimonio/Index
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var testimonios = await _context.Testimonios
                .Where(t => t.Aprobado)
                .OrderByDescending(t => t.FechaCreacion)
                .ToListAsync();

            return View(testimonios);
        }
        // GET: Testimonio/GetApprovedTestimonios
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetApprovedTestimonios()
        {
            var testimonios = await _context.Testimonios
                .Where(t => t.Aprobado)
                .OrderByDescending(t => t.FechaCreacion)
                .ToListAsync();

            return Json(testimonios);
        }

        // GET: Testimonio/Crear
        [AllowAnonymous]
        public IActionResult Crear()
        {
            return View();
        }

        // POST: Testimonio/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Crear(Testimonio testimonio)
        {
            if (ModelState.IsValid)
            {
                testimonio.FechaCreacion = DateTime.UtcNow;
                testimonio.Aprobado = false; // Los testimonios requieren aprobación

                _context.Add(testimonio);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "¡Gracias por compartir tu experiencia! Tu testimonio será revisado y publicado pronto.";
                return RedirectToAction(nameof(Gracias));
            }
            return View(testimonio);
        }

        // GET: Testimonio/Gracias
        [AllowAnonymous]
        public IActionResult Gracias()
        {
            return View();
        }

        // GET: Testimonio/Administrar (solo para administradores)
        [Authorize]
        public async Task<IActionResult> Administrar()
        {
            var testimonios = await _context.Testimonios
                .OrderByDescending(t => t.FechaCreacion)
                .ToListAsync();

            return View(testimonios);
        }

        // POST: Testimonio/Aprobar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Aprobar(int id)
        {
            var testimonio = await _context.Testimonios.FindAsync(id);
            if (testimonio == null)
            {
                return NotFound();
            }

            testimonio.Aprobado = true;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Administrar));
        }

        // POST: Testimonio/Rechazar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Rechazar(int id)
        {
            var testimonio = await _context.Testimonios.FindAsync(id);
            if (testimonio == null)
            {
                return NotFound();
            }

            _context.Testimonios.Remove(testimonio);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Administrar));
        }
    }
}