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

        [AllowAnonymous]
        public async Task<IActionResult> ObtenerEventos()
        {
            var eventos = await _context.Eventos.ToListAsync();
            return Json(eventos);
        }

        // GET: Evento/Crear
        public IActionResult Crear()
        {
            return View();
        }

        // POST: Evento/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Evento evento, IFormFile imagen)
        {
            if (imagen != null)
            {
                Console.WriteLine($"Archivo recibido: {imagen.FileName}");
                if (imagen.Length > 0)
                {
                    Console.WriteLine($"Tamaño del archivo: {imagen.Length} bytes");
                }
                else
                {
                    Console.WriteLine("El archivo recibido está vacío.");
                }
            }
            else
            {
                Console.WriteLine("No se recibió ningún archivo.");
            }

            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Error: {error.ErrorMessage}");
                }
                return View(evento);
            }

            if (imagen != null && imagen.Length > 0)
            {
                evento.ImagenUrl = await GuardarImagen(imagen);
                Console.WriteLine($"Imagen subida: {imagen.FileName}");
            }
            else
            {
                evento.ImagenUrl = "/imagenes/eventos/1e197cea-904c-49ea-a11a-b862881a9ea2_135011440_202779264886845_4680377247879112035_n";
                Console.WriteLine("No se subió imagen, se usará imagen predeterminada.");
            }

            // Convertir la fecha y hora a UTC antes de guardar
            evento.Fecha = DateTime.SpecifyKind(evento.Fecha.Date + evento.HoraInicio, DateTimeKind.Utc);

            _context.Add(evento);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Lista));
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

            // Establecer HoraInicio y HoraFin para la vista
            evento.HoraInicio = evento.Fecha.TimeOfDay;
            return View(evento);
        }

        // POST: Evento/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, Evento evento, IFormFile? imagen)
        {
            if (id != evento.IdEvento)
            {
                return NotFound();
            }

            var eventoExistente = await _context.Eventos
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.IdEvento == id);

            if (eventoExistente == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (imagen != null && imagen.Length > 0)
                    {
                        evento.ImagenUrl = await GuardarImagen(imagen);
                    }
                    else
                    {
                        evento.ImagenUrl = eventoExistente.ImagenUrl;
                    }

                    // Convertir la fecha y hora a UTC antes de guardar
                    evento.Fecha = DateTime.SpecifyKind(evento.Fecha.Date + evento.HoraInicio, DateTimeKind.Utc);

                    _context.Update(evento);
                    await _context.SaveChangesAsync();
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

            try
            {
                if (!string.IsNullOrEmpty(evento.ImagenUrl) &&
                    !evento.ImagenUrl.Equals("/imagenes/eventos/1e197cea-904c-49ea-a11a-b862881a9ea2_135011440_202779264886845_4680377247879112035_n", StringComparison.OrdinalIgnoreCase))
                {
                    var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, evento.ImagenUrl.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                _context.Eventos.Remove(evento);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Lista));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Ocurrió un error al intentar eliminar el evento: " + ex.Message);
                return View(evento);
            }
        }

        private bool EventoExists(int id)
        {
            return _context.Eventos.Any(e => e.IdEvento == id);
        }

        private async Task<string> GuardarImagen(IFormFile imagen)
        {
            string nombreUnico = Guid.NewGuid().ToString() + Path.GetExtension(imagen.FileName);
            string rutaCarpeta = Path.Combine(_webHostEnvironment.WebRootPath, "imagenes", "eventos");

            if (!Directory.Exists(rutaCarpeta))
            {
                Directory.CreateDirectory(rutaCarpeta);
            }

            string rutaCompleta = Path.Combine(rutaCarpeta, nombreUnico);

            using (var stream = new FileStream(rutaCompleta, FileMode.Create))
            {
                await imagen.CopyToAsync(stream);
            }

            return "/imagenes/eventos/" + nombreUnico;
        }
    }
}