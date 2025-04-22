using Microsoft.AspNetCore.Mvc;
using SamaraProject1.Servicios;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace SamaraProject1.Controllers
{
    public class ImageController : Controller
    {
        private readonly IImageService _imageService;
        private readonly ILogger<ImageController> _logger;

        public ImageController(IImageService imageService, ILogger<ImageController> logger)
        {
            _imageService = imageService;
            _logger = logger;
        }

        [HttpGet]
        [ResponseCache(Duration = 86400)] // Caché por 24 horas
        public async Task<IActionResult> Optimize(string url, int width = 800, int quality = 80)
        {
            if (string.IsNullOrEmpty(url))
            {
                return BadRequest("URL no proporcionada");
            }

            try
            {
                // Verificar si la URL es relativa y convertirla a absoluta
                if (url.StartsWith("/"))
                {
                    url = $"{Request.Scheme}://{Request.Host}{url}";
                }

                // Descargar la imagen
                using (var httpClient = new HttpClient())
                {
                    var imageBytes = await httpClient.GetByteArrayAsync(url);

                    // Optimizar la imagen
                    var optimizedBytes = await _imageService.OptimizeImageAsync(imageBytes, width, quality);

                    // Determinar el tipo de contenido
                    string contentType = "image/jpeg";
                    if (url.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                    {
                        contentType = "image/png";
                    }
                    else if (url.EndsWith(".gif", StringComparison.OrdinalIgnoreCase))
                    {
                        contentType = "image/gif";
                    }
                    else if (url.EndsWith(".webp", StringComparison.OrdinalIgnoreCase))
                    {
                        contentType = "image/webp";
                    }

                    // Devolver la imagen optimizada
                    return File(optimizedBytes, contentType);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al optimizar imagen desde URL: {Url}", url);
                return StatusCode(500, "Error al procesar la imagen");
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> OptimizeBulk()
        {
            try
            {
                // Obtener todas las imágenes en wwwroot/imagenes
                var webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var imageDirectory = Path.Combine(webRootPath, "imagenes");

                if (!Directory.Exists(imageDirectory))
                {
                    return NotFound("Directorio de imágenes no encontrado");
                }

                int optimizedCount = 0;
                long savedBytes = 0;

                // Procesar todas las imágenes en el directorio y subdirectorios
                foreach (var file in Directory.GetFiles(imageDirectory, "*.*", SearchOption.AllDirectories))
                {
                    var extension = Path.GetExtension(file).ToLowerInvariant();
                    if (extension == ".jpg" || extension == ".jpeg" || extension == ".png")
                    {
                        var originalSize = new FileInfo(file).Length;
                        var imageData = await System.IO.File.ReadAllBytesAsync(file);

                        // Optimizar la imagen
                        var optimizedData = await _imageService.OptimizeImageAsync(imageData, 1200, 80);

                        // Guardar solo si la imagen optimizada es más pequeña
                        if (optimizedData.Length < originalSize)
                        {
                            await System.IO.File.WriteAllBytesAsync(file, optimizedData);
                            optimizedCount++;
                            savedBytes += (originalSize - optimizedData.Length);
                        }
                    }
                }

                return Json(new
                {
                    success = true,
                    message = $"Se han optimizado {optimizedCount} imágenes, ahorrando {savedBytes / 1024} KB"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al optimizar imágenes en lote");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
