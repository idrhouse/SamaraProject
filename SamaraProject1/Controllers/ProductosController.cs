﻿using Microsoft.AspNetCore.Authorization;
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

        // GET: Crear
        public async Task<IActionResult> Crear()
        {
            var emprendedores = await _context.Emprendedores.ToListAsync();
            var tipoProductos = await _context.TipoProducto.ToListAsync();

            if (!emprendedores.Any())
            {
                ModelState.AddModelError("", "No hay emprendedores registrados.");
            }

            if (!tipoProductos.Any())
            {
                ModelState.AddModelError("", "No hay tipos de productos registrados.");
            }

            // Asignar datos correctamente al ViewBag
            ViewBag.Emprendedores = emprendedores;
            ViewBag.TipoProductos = tipoProductos; 

            return View();
        }

        //Obtener Imagen
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ObtenerImagen(int id)
        {
            var productos = _context.Productos.FirstOrDefault(e => e.IdProducto == id);

            if (productos == null || productos.ImagenDatos == null)
            {
                // Devuelve una imagen predeterminada si no existe la imagen
                var rutaDefault = Path.Combine(_webHostEnvironment.WebRootPath, "imagenes/default-producto.png");
                var defaultImage = System.IO.File.ReadAllBytes(rutaDefault);
                return File(defaultImage, "image/jpeg");
            }

            // Devuelve la imagen almacenada en la base de datos
            return File(productos.ImagenDatos, "image/jpeg");
        }


        //POST Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Producto producto, int[] SelectedEmprendedores, IFormFile? imagen)
        {
            if (ModelState.IsValid)
            {
                if (!ModelState.IsValid)
                {
                    return View(producto);
                }

            if (imagen != null && imagen.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await imagen.CopyToAsync(memoryStream);
                        producto.ImagenDatos = memoryStream.ToArray();
                }
            }

            // Create the product without assigning Emprendedores yet
            _context.Add(producto);
            await _context.SaveChangesAsync();

            // Now add the relationships
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

            // If we got this far, something failed; redisplay form
            ViewBag.Emprendedores = await _context.Emprendedores.ToListAsync();
            ViewBag.TipoProductos = await _context.TipoProducto.ToListAsync();
            return View(producto);
        }


        //GET Editar
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

        //POST Editar
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
                    // Fetch the existing product from the database
                    var existingProduct = await _context.Productos.AsNoTracking().FirstOrDefaultAsync(p => p.IdProducto == id);

                    if (existingProduct == null)
                    {
                        return NotFound();
                    }

                    if (imagen != null && imagen.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await imagen.CopyToAsync(memoryStream);
                            existingProduct.ImagenDatos = memoryStream.ToArray();
                        }
                    }
                    else
                    {
                        // If no new image is uploaded, keep the existing image URL
                        producto.ImagenDatos = existingProduct.ImagenDatos;
                    }

                    // Update the product
                    _context.Update(producto);

                    // Remove existing relationships
                    var existingRelationships = await _context.ProductoEmprendedores
                        .Where(pe => pe.IdProducto == producto.IdProducto)
                        .ToListAsync();
                    _context.ProductoEmprendedores.RemoveRange(existingRelationships);

                    // Add new relationships
                    if (SelectedEmprendedores != null)
                    {
                        foreach (var emprendedorId in SelectedEmprendedores)
                        {
                            _context.ProductoEmprendedores.Add(new ProductoEmprendedor
                            {
                                IdProducto = producto.IdProducto,
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

        //DELETE
        public async Task<IActionResult> Eliminar(int id)
        {
            var producto = await _context.Productos
                .Include(p => p.ProductoEmprendedores)
                    .ThenInclude(pe => pe.Emprendedor)
                .FirstOrDefaultAsync(m => m.IdProducto == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        //Post Eliminar
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarEliminar(int id)
        {
            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
            {
                return NotFound();
            }

            if (producto != null)
            {
                _context.Productos.Remove(producto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Lista));
            }
            return RedirectToAction(nameof(Lista));
        }

        private bool ProductoExists(int id)
        {
            return _context.Productos.Any(e => e.IdProducto == id);
        }

        

    }
}