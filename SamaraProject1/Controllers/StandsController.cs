using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SamaraProject1.Models;

namespace SamaraProject1.Controllers
{
    public class StandsController : Controller
    {
        private readonly SamaraMarketContext _samaraMarketContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public StandsController(SamaraMarketContext samaraMarketContext, IWebHostEnvironment webHostEnvironment)
        {
            _samaraMarketContext = samaraMarketContext;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            List<Stands> lista = await _samaraMarketContext.Stands
                .Include(s => s.Emprendedor)
                .ToListAsync();
            return View(lista);
        }

        [HttpGet]
        public IActionResult Nuevo()
        {
            ViewBag.Emprendedores = _samaraMarketContext.Emprendedores.ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Nuevo(Stands stands, IFormFile? imagen)
        {
            if (ModelState.IsValid)
            {
                if (imagen != null && imagen.Length > 0)
                {
                    string folder = "imagenes/stands";
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

                    stands.ImagenUrl = $"/{folder}/{uniqueFileName}";
                }

                await _samaraMarketContext.Stands.AddAsync(stands);
                await _samaraMarketContext.SaveChangesAsync();
                return RedirectToAction(nameof(Lista));
            }
            ViewBag.Emprendedores = _samaraMarketContext.Emprendedores.ToList();
            return View(stands);
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            Stands stands = await _samaraMarketContext.Stands
                .FirstOrDefaultAsync(s => s.IdStand == id);
            if (stands == null)
            {
                return NotFound();
            }
            ViewBag.Emprendedores = _samaraMarketContext.Emprendedores.ToList();
            return View(stands);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(Stands stands, IFormFile? imagen)
        {
            if (ModelState.IsValid)
            {
                if (imagen != null && imagen.Length > 0)
                {
                    if (!string.IsNullOrEmpty(stands.ImagenUrl))
                    {
                        string oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, stands.ImagenUrl.TrimStart('/'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    string folder = "imagenes/stands";
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

                    stands.ImagenUrl = $"/{folder}/{uniqueFileName}";
                }

                _samaraMarketContext.Stands.Update(stands);
                await _samaraMarketContext.SaveChangesAsync();
                return RedirectToAction(nameof(Lista));
            }
            ViewBag.Emprendedores = _samaraMarketContext.Emprendedores.ToList();
            return View(stands);
        }

        [HttpGet]
        public async Task<IActionResult> Eliminar(int id)
        {
            Stands stands = await _samaraMarketContext.Stands
                .Include(s => s.Emprendedor)
                .FirstOrDefaultAsync(s => s.IdStand == id);
            if (stands == null)
            {
                return NotFound();
            }
            return View(stands);
        }

        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarEliminar(int id)
        {
            Stands stands = await _samaraMarketContext.Stands
                .FirstOrDefaultAsync(s => s.IdStand == id);
            if (stands == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(stands.ImagenUrl))
            {
                string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, stands.ImagenUrl.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            _samaraMarketContext.Stands.Remove(stands);
            await _samaraMarketContext.SaveChangesAsync();
            return RedirectToAction(nameof(Lista));
        }
    }
}