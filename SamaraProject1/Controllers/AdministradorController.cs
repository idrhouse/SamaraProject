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
        public async Task<IActionResult> Lista()
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
        public async Task<IActionResult> Editar(int id)
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

        // Método para obtener el nombre del Administrador autenticado
        private async Task<string> ObtenerNombreAdministrador()
        {
            var usuarioId = User.Identity.Name;
            Console.WriteLine($"Usuario autenticado: {usuarioId}");

            if (string.IsNullOrEmpty(usuarioId))
            {
                Console.WriteLine("El usuario no está autenticado.");
                return "No autenticado";
            }

            var administrador = await _samaraMarketContext.Administrador
                .FirstOrDefaultAsync(a => a.Correo == usuarioId);

            if (administrador == null)
            {
                Console.WriteLine($"No se encontró administrador con el correo: {usuarioId}");
            }

            return administrador?.NombreAdministrador ?? "Administrador";
        }



        // Método para pasarlo a la vista en el Layout
        public async Task<IActionResult> Layout()
        {
            // Obtener el nombre del administrador autenticado
            var nombreAdministrador = await ObtenerNombreAdministrador();

            // Verificar si el valor es correcto
            if (string.IsNullOrEmpty(nombreAdministrador))
            {
                // Si no se encuentra el nombre, puedes manejarlo de alguna forma
                nombreAdministrador = "Administrador";
            }

            ViewData["NombreAdministrador"] = nombreAdministrador;

            return View();
        }

    }

}

