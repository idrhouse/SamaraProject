using Microsoft.AspNetCore.Mvc;
using SamaraProject1.Models;
using SamaraProject1.Recursos;
using SamaraProject1.Servicios.Contrato;


using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace SamaraProject1.Controllers
{
    public class InicioController : Controller
    {

        private readonly IAdministradorService _administradorService;
        public InicioController(IAdministradorService administradorService)
        {
            _administradorService = administradorService;
        }

        [Authorize]
        public IActionResult Registrarse()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Registrarse(Administrador modelo)
        {
            modelo.Clave = Utilidades.Cifrar(modelo.Clave);
            Administrador administrador_creado = await _administradorService.SaveAdministrador(modelo);

            if (administrador_creado.IdAdministrador > 0)

                return RedirectToAction("IniciarSesion", "Inicio");

            ViewData["MENSAJE"] = "NO SE PUDO CREAR EL Administrador";


            return View();

        }

        public IActionResult IniciarSesion()
        {
            ViewData["SuccessMessage"] = TempData["SuccessMessage"];
            ViewData["ErrorMessage"] = TempData["ErrorMessage"];
            return View();
        }

        [HttpPost]
        public async Task< IActionResult>IniciarSesion(string correo, string clave)
        {
            Administrador administrador_encontrado = await _administradorService.GetAdministrador(correo, Utilidades.Cifrar(clave));
         
            if (administrador_encontrado == null)
            {
                ViewData["MENSAJE"] = "No se encontraron coincidencias";
                return View();
            }

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, administrador_encontrado.NombreAdministrador)

            };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            AuthenticationProperties properties = new AuthenticationProperties()
            {
                AllowRefresh = true
            };

            await HttpContext.SignInAsync(

            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            properties);
            return RedirectToAction("Index", "Dashboard");
        }
 
            }
        }
   
