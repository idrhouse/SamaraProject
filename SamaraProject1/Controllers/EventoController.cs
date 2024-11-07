using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SamaraProject1.Models;

namespace SamaraProject1.Controllers
{
    public class EventoController : Controller
    {
        private readonly SamaraMarketContext _samaraMarketContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EventoController(SamaraMarketContext samaraMarketContext, IWebHostEnvironment webHostEnvironment)
        {
            _samaraMarketContext = samaraMarketContext;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            List<Evento> lista = await _samaraMarketContext.Eventos.ToListAsync();
            return View(lista);
        }

        [HttpGet]
        public IActionResult Nuevo()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Nuevo(Evento evento, IFormFile? imagen)
        {
            if (ModelState.IsValid)
            {
                if (imagen != null && imagen.Length > 0)
                {
                    string folder = "imagenes/eventos";
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, folder);

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + imagen.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imagen.CopyToAsync(fileStream);
                    }

                    evento.ImagenUrl = $"/{folder}/{uniqueFileName}";
                }

                await _samaraMarketContext.Eventos.AddAsync(evento);
                await _samaraMarketContext.SaveChangesAsync();
                return RedirectToAction(nameof(Lista));
            }
            return View(evento);
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            Evento evento = await _samaraMarketContext.Eventos.FirstOrDefaultAsync(e => e.IdEvento == id);
            if (evento == null)
            {
                return NotFound();
            }
            return View(evento);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(Evento evento, IFormFile? imagen)
        {
            if (ModelState.IsValid)
            {
                if (imagen != null && imagen.Length > 0)
                {
                    if (!string.IsNullOrEmpty(evento.ImagenUrl))
                    {
                        string oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, evento.ImagenUrl.TrimStart('/'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    string folder = "imagenes/eventos";
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, folder);

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + imagen.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imagen.CopyToAsync(fileStream);
                    }

                    evento.ImagenUrl = $"/{folder}/{uniqueFileName}";
                }

                _samaraMarketContext.Eventos.Update(evento);
                await _samaraMarketContext.SaveChangesAsync();
                return RedirectToAction(nameof(Lista));
            }
            return View(evento);
        }

        [HttpGet]
        public async Task<IActionResult> Eliminar(int id)
        {
            Evento evento = await _samaraMarketContext.Eventos.FirstOrDefaultAsync(e => e.IdEvento == id);
            if (evento == null)
            {
                return NotFound();
            }
            return View(evento);
        }

        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarEliminar(int id)
        {
            Evento evento = await _samaraMarketContext.Eventos.FirstOrDefaultAsync(e => e.IdEvento == id);
            if (evento == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(evento.ImagenUrl))
            {
                string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, evento.ImagenUrl.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            _samaraMarketContext.Eventos.Remove(evento);
            await _samaraMarketContext.SaveChangesAsync();
            return RedirectToAction(nameof(Lista));
        }
    }
}