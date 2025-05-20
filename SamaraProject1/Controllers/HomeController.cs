using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using SamaraProject1.Models;
using System.Diagnostics;
using System.IO;
using System.Security.Claims;
using System.Text.Json;

namespace SamaraProject1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _environment;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            ClaimsPrincipal claimuser = HttpContext.User;
            string nombreUsuario = "";
            if (claimuser.Identity.IsAuthenticated)
            {
                nombreUsuario = claimuser.Claims.Where(c => c.Type == ClaimTypes.Name)
                    .Select(c => c.Value).SingleOrDefault();
            }
            ViewData["nombreUsuario"] = nombreUsuario;

            return View();
        }

        [AllowAnonymous]
        public IActionResult GetCarouselImages()
        {
            try
            {
                var dataFilePath = Path.Combine(_environment.WebRootPath, "data", "carouselImages.json");
                if (!System.IO.File.Exists(dataFilePath))
                {
                    // Si el archivo no existe, crear el directorio y un archivo vacío
                    var directory = Path.GetDirectoryName(dataFilePath);
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                    System.IO.File.WriteAllText(dataFilePath, "[]");
                    return Json(new List<CarouselImage>());
                }

                var json = System.IO.File.ReadAllText(dataFilePath);
                var images = JsonSerializer.Deserialize<List<CarouselImage>>(json) ?? new List<CarouselImage>();
                return Json(images.OrderBy(i => i.Order));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las imágenes del carrusel");
                return Json(new List<CarouselImage>());
            }
        }

        [AllowAnonymous]
        public IActionResult ComoUnirte()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult QuienesSomos()
        {
            return View();
        }

        public IActionResult Donacion()
        {
            return View();
        }

        // Other actions remain unchanged

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> CerrarSesion()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}