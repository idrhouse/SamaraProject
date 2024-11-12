using Microsoft.AspNetCore.Mvc;
using SamaraProject1.Models;
using Microsoft.EntityFrameworkCore;
using SamaraProject1.Recursos;
using Microsoft.AspNetCore.Authorization;
namespace SamaraProject1.Controllers

{
    [Authorize]
    public class AdministradorController : Controller
    {
        private readonly SamaraMarketContext _samaraMarketContext;

        public AdministradorController(SamaraMarketContext samaraMarketContext)
        {
            _samaraMarketContext = samaraMarketContext;
        }
        [HttpGet]
        public async Task< IActionResult> Lista()
        {
            List<Administrador> lista = await _samaraMarketContext.Administrador.ToListAsync();
            return View(lista);
        }
        [HttpGet]
        public IActionResult Nuevo()
        {
           return View();
        }
        [HttpPost]
        public async Task<IActionResult> Nuevo(Administrador administrador)
        {
           await _samaraMarketContext.Administrador.AddAsync(administrador);
           await _samaraMarketContext.SaveChangesAsync();
           return RedirectToAction(nameof(Lista));
        }
        [HttpGet]
        public async Task <IActionResult> Editar(int id)
        {
            Administrador administrador = await _samaraMarketContext.Administrador.FirstAsync(a => a.IdAdministrador == id);
            return View(administrador);
        }
        [HttpPost]
        public async Task<IActionResult> Editar(Administrador administrador)
        {
             _samaraMarketContext.Administrador.Update(administrador);
            await _samaraMarketContext.SaveChangesAsync();
            return RedirectToAction(nameof(Lista));
        }
        [HttpGet]
        public async Task<IActionResult> Eliminar(int id)
        {
            Administrador administrador = await _samaraMarketContext.Administrador.FirstAsync(a => a.IdAdministrador == id);
            _samaraMarketContext.Administrador.Remove(administrador);
            await _samaraMarketContext.SaveChangesAsync();
            return RedirectToAction(nameof(Lista));
        }

    }
    }

