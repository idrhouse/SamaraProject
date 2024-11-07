using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SamaraProject1.Models;

namespace SamaraProject1.Controllers
{
    public class EmprendedorController : Controller
    {
        private readonly SamaraMarketContext _samaraMarketContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EmprendedorController(SamaraMarketContext samaraMarketContext, IWebHostEnvironment webHostEnvironment)
        {
            _samaraMarketContext = samaraMarketContext;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Lista(Emprendedor emprendedor)
        {
            List<Emprendedor> lista = await _samaraMarketContext.Emprendedores.Include(e => e.Administrador).ToListAsync();
            return View(lista);
        }

        [HttpGet]
        public IActionResult Nuevo()
        {
            ViewBag.Administradores = _samaraMarketContext.Administrador.ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Nuevo(Emprendedor emprendedor, IFormFile? imagen)
        {
            if (ModelState.IsValid)
            {
                if (imagen != null && imagen.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "imagenes/emprendedores");
                    Directory.CreateDirectory(uploadsFolder); // Crear directorio si no existe

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + imagen.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imagen.CopyToAsync(fileStream);
                    }

                    emprendedor.ImagenUrl = "/imagenes/emprendedores/" + uniqueFileName;
                }

                await _samaraMarketContext.Emprendedores.AddAsync(emprendedor);
                await _samaraMarketContext.SaveChangesAsync();
                return RedirectToAction(nameof(Lista));
            }
            ViewBag.Administradores = _samaraMarketContext.Administrador.ToList();
            return View(emprendedor);
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            Emprendedor emprendedor = await _samaraMarketContext.Emprendedores.FirstAsync(a => a.IdEmprendedor == id);
            if (emprendedor == null)
            {
                return NotFound();
            }
            ViewBag.Administradores = _samaraMarketContext.Administrador.ToList();
            return View(emprendedor);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(Emprendedor emprendedor, IFormFile? imagen)
        {
            if (ModelState.IsValid)
            {
                if (imagen != null && imagen.Length > 0)
                {
                    // Eliminar imagen anterior si existe
                    if (!string.IsNullOrEmpty(emprendedor.ImagenUrl))
                    {
                        var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, emprendedor.ImagenUrl.TrimStart('/'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "imagenes/emprendedores");
                    Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + imagen.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imagen.CopyToAsync(fileStream);
                    }

                    emprendedor.ImagenUrl = "/imagenes/emprendedores/" + uniqueFileName;
                }

                _samaraMarketContext.Emprendedores.Update(emprendedor);
                await _samaraMarketContext.SaveChangesAsync();
                return RedirectToAction(nameof(Lista));
            }
            ViewBag.Administradores = _samaraMarketContext.Administrador.ToList();
            return View(emprendedor);
        }

        [HttpGet]
        public async Task<IActionResult> Eliminar(int id)
        {
            Emprendedor emprendedor = await _samaraMarketContext.Emprendedores.FirstAsync(a => a.IdEmprendedor == id);
            if (emprendedor == null)
            {
                return NotFound();
            }
            return View(emprendedor);
        }

        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarEliminar(int id)
        {
            Emprendedor emprendedor = await _samaraMarketContext.Emprendedores.FirstAsync(a => a.IdEmprendedor == id);
            if (emprendedor == null)
            {
                return NotFound();
            }

            // Eliminar imagen si existe
            if (!string.IsNullOrEmpty(emprendedor.ImagenUrl))
            {
                var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, emprendedor.ImagenUrl.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            _samaraMarketContext.Emprendedores.Remove(emprendedor);
            await _samaraMarketContext.SaveChangesAsync();
            return RedirectToAction(nameof(Lista));
        }
    }
}