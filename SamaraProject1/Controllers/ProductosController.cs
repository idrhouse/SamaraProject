using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
            var productos = await _context.Productos
                .Include(p => p.ProductoEmprendedores)
                    .ThenInclude(pe => pe.Emprendedor)
                .Include(p => p.TipoProducto)
                .ToListAsync();
            return View(productos);
        }

        [AllowAnonymous]
        public async Task<IActionResult> ListaPublica()
        {
            var productos = await _context.Productos
                .Include(p => p.ProductoEmprendedores)
                    .ThenInclude(pe => pe.Emprendedor)
                .Include(p => p.TipoProducto)
                .Where(p => p.ProductoEmprendedores.Any())
                .ToListAsync();
            return View(productos);
        }

        public async Task<IActionResult> ObtenerEmprendedoresPorCategoria(int idCategoria)
        {
            var emprendedores = await _context.Emprendedores
                .Where(e => e.IdCategoria == idCategoria)
                .Select(e => new { e.IdEmprendedor, e.NombreEmprendedor })
                .ToListAsync();

            return Json(emprendedores);
        }

        public async Task<IActionResult> Crear(int? idCategoria)
        {
            var emprendedores = await _context.Emprendedores
                .Where(e => idCategoria == null || e.IdCategoria == idCategoria)
                .ToListAsync();
            var tipoProductos = await _context.TipoProducto.ToListAsync();
            var categorias = await _context.Categorias.ToListAsync();

            if (!emprendedores.Any())
            {
                ModelState.AddModelError("", "No hay emprendedores registrados.");
            }

            if (!tipoProductos.Any())
            {
                ModelState.AddModelError("", "No hay tipos de productos registrados.");
            }

            ViewBag.Emprendedores = emprendedores;
            ViewBag.TipoProductos = tipoProductos;
            ViewBag.Categorias = new SelectList(categorias, "IdCategoria", "NombreCategoria");

            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ObtenerImagen(int id)
        {
            var productos = _context.Productos.FirstOrDefault(e => e.IdProducto == id);

            if (productos == null || productos.ImagenDatos == null)
            {
                var rutaDefault = Path.Combine(_webHostEnvironment.WebRootPath, "imagenes/default-producto.png");
                var defaultImage = System.IO.File.ReadAllBytes(rutaDefault);
                return File(defaultImage, "image/jpeg");
            }

            return File(productos.ImagenDatos, "image/jpeg");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Producto producto, int[] SelectedEmprendedores, IFormFile? imagen)
        {
            if (ModelState.IsValid)
            {
                if (imagen != null && imagen.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await imagen.CopyToAsync(memoryStream);
                        producto.ImagenDatos = memoryStream.ToArray();
                    }
                }

                _context.Add(producto);
                await _context.SaveChangesAsync();

                if (SelectedEmprendedores != null && SelectedEmprendedores.Any())
                {
                    foreach (var idEmprendedor in SelectedEmprendedores)
                    {
                        var productoEmprendedor = new ProductoEmprendedor
                        {
                            IdProducto = producto.IdProducto,
                            IdEmprendedor = idEmprendedor
                        };
                        _context.Add(productoEmprendedor);
                    }
                    await _context.SaveChangesAsync();
                }

                TempData["Message"] = "Producto creado exitosamente.";
                return RedirectToAction(nameof(Lista));
            }

            ViewBag.Emprendedores = await _context.Emprendedores.ToListAsync();
            ViewBag.TipoProductos = await _context.TipoProducto.ToListAsync();
            return View(producto);
        }

        public async Task<IActionResult> Editar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos
                .Include(p => p.ProductoEmprendedores)
                .ThenInclude(pe => pe.Emprendedor)
                .FirstOrDefaultAsync(m => m.IdProducto == id);

            if (producto == null)
            {
                return NotFound();
            }

            ViewBag.Emprendedores = await _context.Emprendedores.ToListAsync();
            ViewBag.TipoProductos = await _context.TipoProducto.ToListAsync();
            ViewBag.SelectedEmprendedores = producto.ProductoEmprendedores?.Select(pe => pe.IdEmprendedor).ToList() ?? new List<int>();

            return View(producto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, Producto producto, List<int> SelectedEmprendedores, IFormFile? imagen)
        {
            if (id != producto.IdProducto)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingProduct = await _context.Productos
                        .Include(p => p.ProductoEmprendedores)
                        .FirstOrDefaultAsync(p => p.IdProducto == id);

                    if (existingProduct == null)
                    {
                        return NotFound();
                    }

                    existingProduct.Nombre_Producto = producto.Nombre_Producto;
                    existingProduct.Descripcion = producto.Descripcion;
                    existingProduct.IdTipoProducto = producto.IdTipoProducto;

                    if (imagen != null && imagen.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await imagen.CopyToAsync(memoryStream);
                            existingProduct.ImagenDatos = memoryStream.ToArray();
                        }
                    }

                    _context.ProductoEmprendedores.RemoveRange(existingProduct.ProductoEmprendedores);

                    if (SelectedEmprendedores != null)
                    {
                        foreach (var emprendedorId in SelectedEmprendedores)
                        {
                            _context.ProductoEmprendedores.Add(new ProductoEmprendedor
                            {
                                IdProducto = existingProduct.IdProducto,
                                IdEmprendedor = emprendedorId
                            });
                        }
                    }

                    await _context.SaveChangesAsync();

                    TempData["Message"] = "Producto actualizado exitosamente.";
                    return RedirectToAction(nameof(Lista));
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
            }

            ViewBag.Emprendedores = await _context.Emprendedores.ToListAsync();
            ViewBag.TipoProductos = await _context.TipoProducto.ToListAsync();
            ViewBag.SelectedEmprendedores = SelectedEmprendedores;

            return View(producto);
        }

        public async Task<IActionResult> Eliminar(int id)
        {
            var producto = await _context.Productos
                .Include(p => p.ProductoEmprendedores)
                    .ThenInclude(pe => pe.Emprendedor)
                .Include(p => p.TipoProducto)
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
            var producto = await _context.Productos
                .Include(p => p.ProductoEmprendedores)
                .FirstOrDefaultAsync(p => p.IdProducto == id);

            if (producto == null)
            {
                return NotFound();
            }

            _context.ProductoEmprendedores.RemoveRange(producto.ProductoEmprendedores);
            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Producto eliminado exitosamente.";
            return RedirectToAction(nameof(Lista));
        }

        private bool ProductoExists(int id)
        {
            return _context.Productos.Any(e => e.IdProducto == id);
        }
    }
}

