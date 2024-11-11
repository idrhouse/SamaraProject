﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SamaraProject1.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace SamaraProject1.Controllers
{
    public class StandsController : Controller
    {
        private readonly SamaraMarketContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public StandsController(SamaraMarketContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Stands
        public async Task<IActionResult> Lista()
        {
            var stands = await _context.Stands.Include(s => s.Emprendedor).ToListAsync();
            return View(stands);
        }

        // GET: Stands/Detalles/5
        public async Task<IActionResult> Detalles(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stand = await _context.Stands
                .Include(s => s.Emprendedor)
                .FirstOrDefaultAsync(m => m.IdStand == id);
            if (stand == null)
            {
                return NotFound();
            }

            return View(stand);
        }

        // GET: Stands/Crear
        public IActionResult Crear()
        {
            ViewBag.Emprendedores = GetEmprendedoresSelectList();
            return View();
        }

        // POST: Stands/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear([Bind("Numero_Stand,Descripcion_Stand,IdEmprendedor")] Stands stand, IFormFile imagen)
        {
            if (ModelState.IsValid)
            {
                if (imagen != null && imagen.Length > 0)
                {
                    var fileName = Path.GetFileName(imagen.FileName);
                    var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "imagenes", "stands", fileName);

                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imagen.CopyToAsync(fileStream);
                    }

                    stand.ImagenUrl = "/imagenes/stands/" + fileName;
                }

                _context.Add(stand);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Lista));
            }

            ViewBag.Emprendedores = GetEmprendedoresSelectList();
            return View(stand);
        }

        // GET: Stands/Editar/5
        public async Task<IActionResult> Editar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stand = await _context.Stands.FindAsync(id);
            if (stand == null)
            {
                return NotFound();
            }

            ViewBag.Emprendedores = GetEmprendedoresSelectList();
            return View(stand);
        }

        // POST: Stands/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, [Bind("IdStand,Numero_Stand,Descripcion_Stand,IdEmprendedor,ImagenUrl")] Stands stand, IFormFile imagen)
        {
            if (id != stand.IdStand)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (imagen != null && imagen.Length > 0)
                    {
                        var fileName = Path.GetFileName(imagen.FileName);
                        var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "imagenes", "stands", fileName);

                        Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await imagen.CopyToAsync(fileStream);
                        }

                        // Eliminar la imagen anterior si existe
                        if (!string.IsNullOrEmpty(stand.ImagenUrl))
                        {
                            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, stand.ImagenUrl.TrimStart('/'));
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        stand.ImagenUrl = "/imagenes/stands/" + fileName;
                    }

                    _context.Update(stand);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StandExists(stand.IdStand))
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

            ViewBag.Emprendedores = GetEmprendedoresSelectList();
            return View(stand);
        }

        // GET: Stands/Eliminar/5
        public async Task<IActionResult> Eliminar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stand = await _context.Stands
                .Include(s => s.Emprendedor)
                .FirstOrDefaultAsync(m => m.IdStand == id);
            if (stand == null)
            {
                return NotFound();
            }

            return View(stand);
        }

        // POST: Stands/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            var stand = await _context.Stands.FindAsync(id);
            if (stand != null)
            {
                // Eliminar la imagen si existe
                if (!string.IsNullOrEmpty(stand.ImagenUrl))
                {
                    var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, stand.ImagenUrl.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                _context.Stands.Remove(stand);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Lista));
        }

        private bool StandExists(int id)
        {
            return _context.Stands.Any(e => e.IdStand == id);
        }

        private List<SelectListItem> GetEmprendedoresSelectList()
        {
            return _context.Emprendedores
                .Select(e => new SelectListItem
                {
                    Value = e.IdEmprendedor.ToString(),
                    Text = e.NombreEmprendedor
                })
                .ToList();
        }
    }
}