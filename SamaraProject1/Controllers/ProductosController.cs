using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SamaraProject1.Models;

namespace SamaraProject1.Controllers
{
    public class ProductoController : Controller
    {
        private readonly SamaraMarketContext _samaraMarketContext;

        public ProductoController(SamaraMarketContext samaraMarketContext)
        {
            _samaraMarketContext = samaraMarketContext;
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
        public async Task<IActionResult> Nuevo(Producto producto)
        {
            if (ModelState.IsValid)
            {
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
        public async Task<IActionResult> Editar(Producto producto)
        {
            if (ModelState.IsValid)
            {
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
            _samaraMarketContext.Productos.Remove(producto);
            await _samaraMarketContext.SaveChangesAsync();
            return RedirectToAction(nameof(Lista));
        }
    }
}