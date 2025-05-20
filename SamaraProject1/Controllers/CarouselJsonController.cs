using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using SamaraProject1.Models;
using Microsoft.AspNetCore.Authorization;

namespace SamaraProject1.Controllers
{
    [Authorize]
    [Route("CarouselJson")]
    public class CarouselJsonController : Controller
    {
        private readonly IWebHostEnvironment _environment;
        private readonly string _dataFilePath;

        public CarouselJsonController(IWebHostEnvironment environment)
        {
            _environment = environment;
            _dataFilePath = Path.Combine(_environment.WebRootPath, "data", "carouselImages.json");
            EnsureDataFileExists();
        }

        // Asegurar que el archivo JSON existe
        private void EnsureDataFileExists()
        {
            var directory = Path.GetDirectoryName(_dataFilePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (!System.IO.File.Exists(_dataFilePath))
            {
                System.IO.File.WriteAllText(_dataFilePath, "[]"); // Inicializa con una lista vacía en formato JSON
            }
        }

        // Cargar imágenes desde el archivo JSON
        private List<CarouselImage> LoadImages()
        {
            try
            {
                var json = System.IO.File.ReadAllText(_dataFilePath);
                return JsonSerializer.Deserialize<List<CarouselImage>>(json) ?? new List<CarouselImage>();
            }
            catch
            {
                return new List<CarouselImage>(); // Si ocurre un error, retorna una lista vacía
            }
        }

        // Guardar imágenes al archivo JSON
        private void SaveImages(List<CarouselImage> images)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(images, options);
            System.IO.File.WriteAllText(_dataFilePath, json);
        }

        [AllowAnonymous]
        [HttpGet("GetImages")]
        public IActionResult GetCarouselImages()
        {
            var images = LoadImages();
            return Json(images);
        }

        [HttpGet("Administrar")]
        public IActionResult Administrar()
        {
            var images = LoadImages();
            return View(images);
        }

        [HttpGet("Add")]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost("Add")]
        [ValidateAntiForgeryToken]
        public IActionResult Add([FromForm] IFormFile image, [FromForm] string alt)
        {
            var images = LoadImages();

            if (image == null || image.Length == 0)
            {
                TempData["Error"] = "Debe seleccionar una imagen";
                return View();
            }

            try
            {
                // Verificar tamaño máximo (5MB)
                if (image.Length > 5 * 1024 * 1024)
                {
                    TempData["Error"] = "La imagen no debe superar los 5MB";
                    return View();
                }

                // Verificar tipo de archivo
                var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif" };
                if (!string.IsNullOrEmpty(image.ContentType) && !allowedTypes.Contains(image.ContentType.ToLower()))
                {
                    TempData["Error"] = "Solo se permiten archivos JPG, PNG y GIF";
                    return View();
                }

                // Generar un nombre único para la imagen
                var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
                var fileName = $"{Guid.NewGuid()}{extension}";
                var relativePath = Path.Combine("imagenes", "carousel", fileName).Replace("\\", "/");
                var absolutePath = Path.Combine(_environment.WebRootPath, relativePath);

                // Asegurar que el directorio existe
                var directory = Path.GetDirectoryName(absolutePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Guardar la imagen
                using (var stream = new FileStream(absolutePath, FileMode.Create))
                {
                    image.CopyTo(stream);
                }

                // Verificar que el archivo se haya creado correctamente
                if (!System.IO.File.Exists(absolutePath))
                {
                    throw new IOException($"No se pudo verificar la creación del archivo en: {absolutePath}");
                }

                // Crear y guardar el registro en el archivo JSON
                var carouselImage = new CarouselImage
                {
                    IdCarouselImage = images.Count > 0 ? images.Max(i => i.IdCarouselImage) + 1 : 1,
                    Url = $"/{relativePath}",
                    Alt = alt ?? "",
                    Order = images.Count > 0 ? images.Max(i => i.Order) + 1 : 1,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                images.Add(carouselImage);
                SaveImages(images);

                TempData["Success"] = "Imagen agregada correctamente";
                return RedirectToAction(nameof(Administrar));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al subir la imagen: {ex.Message}";
                if (ex.InnerException != null)
                {
                    TempData["Error"] += $" Detalle: {ex.InnerException.Message}";
                }
                return View();
            }
        }

        [HttpGet("Edit/{id}")]
        public IActionResult Edit(int id)
        {
            var images = LoadImages();
            var image = images.FirstOrDefault(i => i.IdCarouselImage == id);
            if (image == null)
            {
                TempData["Error"] = "Imagen no encontrada";
                return RedirectToAction(nameof(Administrar));
            }

            return View(image);
        }

        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [FromForm] IFormFile image, [FromForm] string alt)
        {
            var images = LoadImages();
            var carouselImage = images.FirstOrDefault(i => i.IdCarouselImage == id);
            if (carouselImage == null)
            {
                TempData["Error"] = "Imagen no encontrada";
                return RedirectToAction(nameof(Administrar));
            }

            try
            {
                // Actualizar el texto alternativo
                carouselImage.Alt = alt ?? "";
                carouselImage.UpdatedAt = DateTime.UtcNow;

                // Si se proporciona una nueva imagen, subirla
                if (image != null && image.Length > 0)
                {
                    // Eliminar la imagen anterior
                    var oldImagePath = Path.Combine(_environment.WebRootPath, carouselImage.Url.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }

                    // Generar un nombre único para la nueva imagen
                    var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
                    var fileName = $"{Guid.NewGuid()}{extension}";
                    var relativePath = Path.Combine("imagenes", "carousel", fileName).Replace("\\", "/");
                    var absolutePath = Path.Combine(_environment.WebRootPath, relativePath);

                    // Asegurar que el directorio existe
                    var directory = Path.GetDirectoryName(absolutePath);
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    // Guardar la nueva imagen
                    using (var stream = new FileStream(absolutePath, FileMode.Create))
                    {
                        image.CopyTo(stream);
                    }

                    // Verificar que el archivo se haya creado correctamente
                    if (!System.IO.File.Exists(absolutePath))
                    {
                        throw new IOException($"No se pudo verificar la creación del archivo en: {absolutePath}");
                    }

                    // Actualizar la URL
                    carouselImage.Url = $"/{relativePath}";
                }

                SaveImages(images);

                TempData["Success"] = "Imagen actualizada correctamente";
                return RedirectToAction(nameof(Administrar));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al actualizar la imagen: {ex.Message}";
                return View(carouselImage);
            }
        }
        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            try
            {
                var images = LoadImages();
                var image = images.FirstOrDefault(i => i.IdCarouselImage == id);

                if (image == null)
                {
                    return Json(new { success = false, message = "Imagen no encontrada" });
                }

                try
                {
                    // Eliminar el archivo físico
                    var filePath = Path.Combine(_environment.WebRootPath, image.Url.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }

                    // Eliminar el registro del archivo JSON
                    images.Remove(image);
                    SaveImages(images);

                    return Json(new { success = true, message = "Imagen eliminada correctamente" });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = $"Error al eliminar archivo: {ex.Message}" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error al eliminar la imagen: {ex.Message}" });
            }
        }
    }
}