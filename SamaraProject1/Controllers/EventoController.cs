using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SamaraProject1.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SamaraProject1.Controllers
{
    [Authorize]
    public class EventoController : Controller
    {
        private readonly SamaraMarketContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EventoController(SamaraMarketContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Evento
        public async Task<IActionResult> Lista()
        {
            var eventos = await _context.Eventos.ToListAsync();
            return View(eventos);
        }

        // GET: Evento/Crear
        public IActionResult Crear()
        {
            return View();
        }

        // POST: Evento/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear([Bind("Nombre,Descripcion,Fecha")] Evento evento, IFormFile imagen)
        {
            if (ModelState.IsValid)
            {
                // Convert to UTC before saving
                evento.Fecha = DateTime.SpecifyKind(evento.Fecha, DateTimeKind.Utc);

                if (imagen != null && imagen.Length > 0)
                {
                    var fileName = Path.GetFileName(imagen.FileName);
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + fileName;
                    var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "imagenes", "eventos", uniqueFileName);

                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imagen.CopyToAsync(fileStream);
                    }

                    evento.ImagenUrl = "/imagenes/eventos/" + uniqueFileName;
                }

                _context.Add(evento);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Evento creado exitosamente.";
                return RedirectToAction(nameof(Lista));
            }
            return View(evento);
        }

        // GET: Evento/Editar/5
        public async Task<IActionResult> Editar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var evento = await _context.Eventos.FindAsync(id);
            if (evento == null)
            {
                return NotFound();
            }
            return View(evento);
        }

        // POST: Evento/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, [Bind("IdEvento,Nombre,Descripcion,Fecha,ImagenUrl")] Evento evento, IFormFile imagen)
        {
            if (id != evento.IdEvento)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Convert to UTC before saving
                    evento.Fecha = DateTime.SpecifyKind(evento.Fecha, DateTimeKind.Utc);

                    // Rest of your existing code...
                    if (imagen != null && imagen.Length > 0)
                    {
                        // Your existing image handling code...
                    }

                    _context.Update(evento);
                    await _context.SaveChangesAsync();
                    TempData["Message"] = "Evento actualizado exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventoExists(evento.IdEvento))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Lista));
            }
            return View(evento);
        }

        // GET: Evento/Eliminar/5
        public async Task<IActionResult> Eliminar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var evento = await _context.Eventos
                .FirstOrDefaultAsync(m => m.IdEvento == id);
            if (evento == null)
            {
                return NotFound();
            }

            return View(evento);
        }

        // POST: Evento/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            var evento = await _context.Eventos.FindAsync(id);
            if (evento == null)
            {
                return NotFound();
            }

            // Delete image if exists
            if (!string.IsNullOrEmpty(evento.ImagenUrl))
            {
                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, evento.ImagenUrl.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            _context.Eventos.Remove(evento);
            await _context.SaveChangesAsync();
            TempData["Message"] = "Evento eliminado exitosamente.";
            return RedirectToAction(nameof(Lista));
        }

        private bool EventoExists(int id)
        {
            return _context.Eventos.Any(e => e.IdEvento == id);
        }
    }
}