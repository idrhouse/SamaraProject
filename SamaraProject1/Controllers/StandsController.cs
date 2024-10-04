using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SamaraProject1.Models;

namespace SamaraProject1.Controllers
{
    public class StandsController : Controller
    {
        private readonly SamaraMarketContext _samaraMarketContext;

        public StandsController(SamaraMarketContext samaraMarketContext)
        {
            _samaraMarketContext = samaraMarketContext;
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
        public async Task<IActionResult> Nuevo(Stands stands)
        {
            if (ModelState.IsValid)
            {
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
        public async Task<IActionResult> Editar(Stands stands)
        {
            if (ModelState.IsValid)
            {
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
            _samaraMarketContext.Stands.Remove(stands);
            await _samaraMarketContext.SaveChangesAsync();
            return RedirectToAction(nameof(Lista));
        }
    }
}