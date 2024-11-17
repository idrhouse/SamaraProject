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
    public class TipoProductoController : Controller
    {
        private readonly SamaraMarketContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public TipoProductoController(SamaraMarketContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: TipoProducto
        public async Task<IActionResult> Lista()
        {
            var TipoProducto = await _context.TipoProducto  
                                       .OrderBy(s => s.IdTipoProducto)
                                       .ToListAsync();
            return View(TipoProducto);
        }

        // GET: TipoProducto/Crear
        public IActionResult Crear()
        {
            ViewBag.TipoProducto = new SelectList(_context.TipoProducto.ToList(), "IdTipoProducto", "NombreTipo");
            return View();
        }

        // POST: TipoProducto/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(TipoProducto tipoProducto)
        {
            if (ModelState.IsValid)
            {
                // Verificar si ya existe un stand con el mismo número
                var existeTipoProducto = await _context.TipoProducto.AnyAsync(s => s.NombreTipo == tipoProducto.NombreTipo);
                if (existeTipoProducto)
                {
                    ModelState.AddModelError("", "Ya existe esa categoria, ingresa otra.");
                    ViewBag.TipoProducto = new SelectList(_context.TipoProducto.ToList(), "IdTipoProducto", "NombreTipo", tipoProducto.IdTipoProducto);
                    return View(tipoProducto);
                }
                _context.Add(tipoProducto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Lista));
            }
            ViewBag.TipoProducto = new SelectList(_context.TipoProducto.ToList(), "IdTipoProducto", "NombreTipo", tipoProducto.IdTipoProducto);
            return View(tipoProducto);
        }


        // GET: TipoProducto/Editar/5
        public async Task<IActionResult> Editar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoProducto = await _context.TipoProducto
                .FirstOrDefaultAsync(s => s.IdTipoProducto == id);

            if (tipoProducto == null)
            {
                return NotFound();
            }

            ViewBag.TipoProducto = await _context.TipoProducto.ToListAsync();
            return View(tipoProducto);
        }

        // TipoProductoExist
        private bool TipoProductoExists(int id)
        {
            return _context.TipoProducto.Any(e => e.IdTipoProducto == id);
        }

        // POST: TipoProducto/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, [Bind("IdTipoProducto,NombreTipo")] TipoProducto tipoProducto)
        {
            if (id != tipoProducto.IdTipoProducto)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.TipoProducto = await _context.TipoProducto.ToListAsync();
                return View(tipoProducto);
            }

            try
            {
                var existeTipoProducto = await _context.TipoProducto.AsNoTracking().FirstOrDefaultAsync(s => s.IdTipoProducto == id);
                if (existeTipoProducto == null)
                {
                    return NotFound();
                }

                // Solo se validará la reasignación si el IdEmprendedor o el Numero_Stand cambian
                if (tipoProducto.IdTipoProducto != existeTipoProducto.IdTipoProducto)
                {
                    ModelState.AddModelError("", "Ya existe esa categoria, ingresa otra.");
                    ViewBag.TipoProducto = await _context.TipoProducto.ToListAsync();
                    return View(tipoProducto);
                }

                _context.Entry(existeTipoProducto).State = EntityState.Detached;
                _context.Entry(tipoProducto).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Lista));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TipoProductoExists(tipoProducto.IdTipoProducto))
                {
                    return NotFound();
                }
                throw;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Ha ocurrido un error al guardar los cambios: " + ex.Message);
                ViewBag.TipoProducto = await _context.TipoProducto.ToListAsync();
                return View(tipoProducto);
            }
        }


        // GET: Stands/Eliminar/5
        public async Task<IActionResult> Eliminar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoProducto = await _context.TipoProducto
                .FirstOrDefaultAsync(m => m.IdTipoProducto == id);
            if (tipoProducto == null)
            {
                return NotFound();
            }

            return View(tipoProducto);
        }

        // POST: Stands/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            var tipoProducto = await _context.TipoProducto.FindAsync(id);
            if (tipoProducto != null)
            {
                _context.TipoProducto.Remove(tipoProducto);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Lista));
        }
    }
}