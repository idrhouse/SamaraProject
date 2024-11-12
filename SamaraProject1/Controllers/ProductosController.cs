using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SamaraProject1.Models;
using System.IO;

namespace SamaraProject1.Controllers
{
    [Authorize]
    public class ProductoController : Controller
    {
        private readonly SamaraMarketContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductoController(SamaraMarketContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Lista()
        {
            var productos = await _context.Productos.Include(p => p.Emprendedor).ToListAsync();
            return View(productos);
        }

        public async Task<IActionResult> Crear()
        {
            ViewBag.Emprendedores = await _context.Emprendedores.ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Producto producto, IFormFile? imagen)
        {
            if (ModelState.IsValid)
            {
                if (imagen != null && imagen.Length > 0)
                {
                    producto.ImagenUrl = await GuardarImagen(imagen);
                }

                _context.Add(producto);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Producto creado exitosamente.";
                return RedirectToAction(nameof(Lista));
            }
            ViewBag.Emprendedores = await _context.Emprendedores.ToListAsync();
            return View(producto);
        }

        public async Task<IActionResult> Editar(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }
            ViewBag.Emprendedores = await _context.Emprendedores.ToListAsync();
            return View(producto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, Producto producto, IFormFile? imagen)
        {
            if (id != producto.IdProducto)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (imagen != null && imagen.Length > 0)
                    {
                        await EliminarImagenAnterior(producto.ImagenUrl);
                        producto.ImagenUrl = await GuardarImagen(imagen);
                    }

                    _context.Update(producto);
                    await _context.SaveChangesAsync();
                    TempData["Message"] = "Producto actualizado exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductoExists(producto.IdProducto))
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
            ViewBag.Emprendedores = await _context.Emprendedores.ToListAsync();
            return View(producto);
        }

        public async Task<IActionResult> Eliminar(int id)
        {
            var producto = await _context.Productos
                .Include(p => p.Emprendedor)
                .FirstOrDefaultAsync(m => m.IdProducto == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarEliminar(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto != null)
            {
                await EliminarImagenAnterior(producto.ImagenUrl);
                _context.Productos.Remove(producto);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Producto eliminado exitosamente.";
            }
            return RedirectToAction(nameof(Lista));
        }

        private bool ProductoExists(int id)
        {
            return _context.Productos.Any(e => e.IdProducto == id);
        }

        private async Task<string> GuardarImagen(IFormFile imagen)
        {
            string folder = "imagenes/productos";
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
            return $"/{folder}/{uniqueFileName}";
        }

        private async Task EliminarImagenAnterior(string? imagenUrl)
        {
            if (!string.IsNullOrEmpty(imagenUrl))
            {
                string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, imagenUrl.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }
        }
    }
}