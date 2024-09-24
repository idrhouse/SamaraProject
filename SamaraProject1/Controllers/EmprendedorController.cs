using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SamaraProject1.Models;

namespace SamaraProject1.Controllers
{
    public class EmprendedorController : Controller
    {
        private readonly SamaraMarketContext _samaraMarketContext;

        public EmprendedorController(SamaraMarketContext samaraMarketContext)
        {
            _samaraMarketContext = samaraMarketContext;
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
        public async Task<IActionResult> Nuevo(Emprendedor emprendedor)
        {
            if (ModelState.IsValid)
            {
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
        public async Task<IActionResult> Editar(Emprendedor emprendedor)
        {
            if (ModelState.IsValid)
            {
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
            _samaraMarketContext.Emprendedores.Remove(emprendedor);
            await _samaraMarketContext.SaveChangesAsync();
            return RedirectToAction(nameof(Lista));
        }
    }
}
