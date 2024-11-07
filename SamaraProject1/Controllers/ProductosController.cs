﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SamaraProject1.Models;

namespace SamaraProject1.Controllers
{
    public class ProductoController : Controller
    {
        private readonly SamaraMarketContext _samaraMarketContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductoController(SamaraMarketContext samaraMarketContext, IWebHostEnvironment webHostEnvironment)
        {
            _samaraMarketContext = samaraMarketContext;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            List<Producto> lista = await _samaraMarketContext.Productos
                .Include(p => p.Emprendedor)
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
        public async Task<IActionResult> Nuevo(Producto producto, IFormFile? imagen)
        {
            if (ModelState.IsValid)
            {
                if (imagen != null && imagen.Length > 0)
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

                    producto.ImagenUrl = $"/{folder}/{uniqueFileName}";
                }

                await _samaraMarketContext.Productos.AddAsync(producto);
                await _samaraMarketContext.SaveChangesAsync();
                return RedirectToAction(nameof(Lista));
            }
            ViewBag.Emprendedores = _samaraMarketContext.Emprendedores.ToList();
            return View(producto);
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            Producto producto = await _samaraMarketContext.Productos
                .FirstOrDefaultAsync(p => p.IdProducto == id);
            if (producto == null)
            {
                return NotFound();
            }
            ViewBag.Emprendedores = _samaraMarketContext.Emprendedores.ToList();
            return View(producto);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(Producto producto, IFormFile? imagen)
        {
            if (ModelState.IsValid)
            {
                if (imagen != null && imagen.Length > 0)
                {
                    if (!string.IsNullOrEmpty(producto.ImagenUrl))
                    {
                        string oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, producto.ImagenUrl.TrimStart('/'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

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

                    producto.ImagenUrl = $"/{folder}/{uniqueFileName}";
                }

                _samaraMarketContext.Productos.Update(producto);
                await _samaraMarketContext.SaveChangesAsync();
                return RedirectToAction(nameof(Lista));
            }
            ViewBag.Emprendedores = _samaraMarketContext.Emprendedores.ToList();
            return View(producto);
        }

        [HttpGet]
        public async Task<IActionResult> Eliminar(int id)
        {
            Producto producto = await _samaraMarketContext.Productos
                .Include(p => p.Emprendedor)
                .FirstOrDefaultAsync(p => p.IdProducto == id);
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
            Producto producto = await _samaraMarketContext.Productos
                .FirstOrDefaultAsync(p => p.IdProducto == id);
            if (producto == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(producto.ImagenUrl))
            {
                string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, producto.ImagenUrl.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            _samaraMarketContext.Productos.Remove(producto);
            await _samaraMarketContext.SaveChangesAsync();
            return RedirectToAction(nameof(Lista));
        }
    }
}