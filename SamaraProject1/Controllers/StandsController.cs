using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SamaraProject1.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SamaraProject1.Controllers
{
    [Authorize]
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

        [AllowAnonymous]
        public async Task<IActionResult> ListaPublica()
        {
            var stands = await _context.Stands.Include(s => s.Emprendedor).ToListAsync();
            return View(stands);
        }
        // GET: Stands/Crear
        public IActionResult Crear()
        {
            ViewBag.Emprendedores = new SelectList(_context.Emprendedores.ToList(), "IdEmprendedor", "NombreEmprendedor");
            return View();
        }

        // POST: Stands/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Stands stand, IFormFile imagen)
        {
            if (ModelState.IsValid)
            {
                if (imagen != null)
                {
                    stand.ImagenUrl = await GuardarImagen(imagen);
                }

                _context.Add(stand);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Lista));
            }
            ViewBag.Emprendedores = new SelectList(_context.Emprendedores.ToList(), "IdEmprendedor", "NombreEmprendedor", stand.IdEmprendedor);
            return View(stand);
        }

        // GET: Stands/Editar/5
        public async Task<IActionResult> Editar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stand = await _context.Stands
                .Include(s => s.Emprendedor)
                .FirstOrDefaultAsync(s => s.IdStand == id);

            if (stand == null)
            {
                return NotFound();
            }

            ViewBag.Emprendedores = await _context.Emprendedores.ToListAsync();
            return View(stand);
        }

        // POST: Stands/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, [Bind("IdStand,Numero_Stand,Descripcion_Stand,IdEmprendedor,ImagenUrl")] Stands stand, IFormFile? imagen)
        {
            if (id != stand.IdStand)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Emprendedores = await _context.Emprendedores.ToListAsync();
                return View(stand);
            }

            try
            {
                var existingStand = await _context.Stands
                    .AsNoTracking()
                    .FirstOrDefaultAsync(s => s.IdStand == id);

                if (existingStand == null)
                {
                    return NotFound();
                }

                if (imagen != null && imagen.Length > 0)
                {
                    if (!string.IsNullOrEmpty(existingStand.ImagenUrl))
                    {
                        var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, existingStand.ImagenUrl.TrimStart('/'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    stand.ImagenUrl = await GuardarImagen(imagen);
                }
                else
                {
                    stand.ImagenUrl = existingStand.ImagenUrl;
                }

                _context.Entry(existingStand).State = EntityState.Detached;
                _context.Entry(stand).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Lista));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StandExists(stand.IdStand))
                {
                    return NotFound();
                }
                throw;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Ha ocurrido un error al guardar los cambios: " + ex.Message);
                ViewBag.Emprendedores = await _context.Emprendedores.ToListAsync();
                return View(stand);
            }
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
                if (!string.IsNullOrEmpty(stand.ImagenUrl))
                {
                    var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, stand.ImagenUrl.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }
                _context.Stands.Remove(stand);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Lista));
        }

        private bool StandExists(int id)
        {
            return _context.Stands.Any(e => e.IdStand == id);
        }

        private async Task<string> GuardarImagen(IFormFile imagen)
        {
            string nombreUnico = Guid.NewGuid().ToString() + Path.GetExtension(imagen.FileName);
            string rutaCarpeta = Path.Combine(_webHostEnvironment.WebRootPath, "imagenes", "stands");

            if (!Directory.Exists(rutaCarpeta))
            {
                Directory.CreateDirectory(rutaCarpeta);
            }

            string rutaCompleta = Path.Combine(rutaCarpeta, nombreUnico);

            using (var stream = new FileStream(rutaCompleta, FileMode.Create))
            {
                await imagen.CopyToAsync(stream);
            }

            return "/imagenes/stands/" + nombreUnico;
        }
    }
}