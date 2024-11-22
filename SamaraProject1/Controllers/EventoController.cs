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
        public IActionResult Calendario()
        {
            return View();
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

        //Obtener Imagen
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ObtenerImagen(int id)
        {
            var evento = _context.Eventos.FirstOrDefault(e => e.IdEvento == id);

            if (evento == null || evento.ImagenDatos == null)
            {
                // Devuelve una imagen predeterminada si no existe la imagen
                var rutaDefault = Path.Combine(_webHostEnvironment.WebRootPath, "imagenes/default-evento.jpg");
                var defaultImage = System.IO.File.ReadAllBytes(rutaDefault);
                return File(defaultImage, "image/jpeg");
            }

            // Devuelve la imagen almacenada en la base de datos
            return File(evento.ImagenDatos, "image/jpeg");
        }

        // POST: Evento/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Evento evento, IFormFile? imagen)
        {
            // Validar el modelo
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Error: {error.ErrorMessage}");
                }
                return View(evento);
            }

            // Validación personalizada del archivo
            if (imagen != null && imagen.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await imagen.CopyToAsync(memoryStream);
                    evento.ImagenDatos = memoryStream.ToArray();
                }
            }

            if (evento.Fecha < DateTime.Today)
            {
                ModelState.AddModelError("Fecha", "No se puede agregar una fecha anterior al día de hoy.");
                return View(evento);
            }

            if (evento.HoraInicio >= evento.HoraFin)
            {
                ModelState.AddModelError("HoraInicio", "La hora de inicio no puede ser mayor o igual a la hora de finalización.");
                return View(evento);
            }            

            // Convertir la fecha y hora a UTC antes de guardar
            evento.Fecha = DateTime.SpecifyKind(evento.Fecha.Date + evento.HoraInicio, DateTimeKind.Utc);

            try
            {
                _context.Add(evento);
                await _context.SaveChangesAsync();
                Console.WriteLine("Evento creado exitosamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al guardar el evento: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Hubo un error al guardar los datos.");
                return View(evento);
            }

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

            // Buscar el evento existente
            var eventoExistente = await _context.Eventos.FindAsync(id);

            if (eventoExistente == null)
            {
                return NotFound();
            }

            // Validaciones personalizadas
            if (evento.Fecha < DateTime.Today)
            {
                ModelState.AddModelError("Fecha", "No se puede agregar una fecha anterior al día de hoy.");
                return View(evento);
            }

            if (evento.HoraInicio >= evento.HoraFin)
            {
                ModelState.AddModelError("HoraInicio", "La hora de inicio no puede ser mayor o igual a la hora de finalización.");
                return View(evento);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Actualizar datos del evento existente
                    eventoExistente.Nombre = evento.Nombre;
                    eventoExistente.Descripcion = evento.Descripcion;
                    eventoExistente.Fecha = DateTime.SpecifyKind(evento.Fecha.Date + evento.HoraInicio, DateTimeKind.Utc);
                    eventoExistente.HoraInicio = evento.HoraInicio;
                    eventoExistente.HoraFin = evento.HoraFin;

                    // Manejar la imagen
                    if (imagen != null && imagen.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await imagen.CopyToAsync(memoryStream);
                            eventoExistente.ImagenDatos = memoryStream.ToArray();
                        }
                    }

                    // Actualizar el evento en el contexto
                    _context.Update(eventoExistente);
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
                if (evento != null)                    
                {
                    _context.Eventos.Remove(evento);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Lista));
                }

                
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Ocurrió un error al intentar eliminar el evento: " + ex.Message);
                return View(evento);
            }
            return RedirectToAction(nameof(Lista));

        }

        private bool EventoExists(int id)
        {
            return _context.Eventos.Any(e => e.IdEvento == id);
        }
                
    }
}