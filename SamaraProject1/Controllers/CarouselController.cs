using Microsoft.AspNetCore.Mvc;
using SamaraProject1.Models;
using SamaraProject1.Servicios.Contrato;
using SamaraProject1.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;

namespace SamaraProject1.Controllers
{
    [Authorize]
    public class CarouselController : Controller
    {
        private readonly ICarouselService _carouselService;
        private readonly IImageService _imageService;
        private readonly SamaraMarketContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CarouselController(ICarouselService carouselService, IImageService imageService, SamaraMarketContext context, IWebHostEnvironment webHostEnvironment)
        {
            _carouselService = carouselService;
            _imageService = imageService;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: /Carousel
        public async Task<IActionResult> Administrar()
        {
            try
            {
                var images = await _carouselService.GetAllCarouselImages();
                return View(images);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar las imágenes: {ex.Message}";
                return View(new List<CarouselImage>());
            }
        }

        // GET: /Carousel/GetCarouselImages
        [HttpGet]
        public async Task<IActionResult> GetCarouselImages()
        {
            try
            {
                var images = await _carouselService.GetAllCarouselImages();

                // Transformar a formato esperado por el frontend
                var result = images.Select(img => new
                {
                    url = img.Url,
                    alt = img.Alt ?? "Imagen de carrusel",
                    order = img.Order
                }).ToList();

                return Json(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // GET: /Carousel/Add
        public IActionResult Add()
        {
            return View();
        }

        // POST: /Carousel/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(string alt, IFormFile image)
        {
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

                // Intentar subir la imagen directamente sin optimización
                try
                {
                    var carouselImage = await _carouselService.UploadCarouselImage(image);

                    // Actualizar el texto alternativo
                    if (!string.IsNullOrEmpty(alt))
                    {
                        carouselImage.Alt = alt;
                        await _carouselService.SaveCarouselImages(new List<CarouselImage> { carouselImage });
                    }

                    TempData["Success"] = "Imagen agregada correctamente";
                    return RedirectToAction(nameof(Administrar));
                }
                catch (Exception ex)
                {
                    // Si falla el método directo, intentar con AddSimple
                    TempData["Error"] = $"Error al subir la imagen: {ex.Message}. Intentando método alternativo...";
                    return await AddSimple(alt, image);
                }
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

        // Método alternativo simple para subir imágenes
        [NonAction]
        public async Task<IActionResult> AddSimple(string alt, IFormFile image)
        {
            if (image == null || image.Length == 0)
            {
                TempData["Error"] = "Debe seleccionar una imagen";
                return View("Add");
            }

            try
            {
                // Generar un nombre único para la imagen
                var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
                var newFileName = $"{Guid.NewGuid()}{extension}";
                var relativePath = Path.Combine("imagenes", "carousel", newFileName).Replace("\\", "/");
                var absolutePath = Path.Combine(_webHostEnvironment.WebRootPath, relativePath);

                // Asegurar que el directorio existe
                var directory = Path.GetDirectoryName(absolutePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Guardar la imagen directamente
                using (var fileStream = new FileStream(absolutePath, FileMode.Create))
                {
                    await image.CopyToAsync(fileStream);
                }

                // Verificar que el archivo se haya creado correctamente
                if (!System.IO.File.Exists(absolutePath))
                {
                    throw new IOException($"No se pudo verificar la creación del archivo en: {absolutePath}");
                }

                // Obtener el siguiente número de orden
                int nextOrder = 1;
                try
                {
                    nextOrder = await _carouselService.GetNextOrderNumber();
                }
                catch
                {
                    // Si falla, usar 1 como valor predeterminado
                }

                // Crear manualmente el objeto CarouselImage
                var carouselImage = new CarouselImage
                {
                    Url = $"/{relativePath}",
                    Alt = alt,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Order = nextOrder
                };

                // Intentar guardar en la base de datos
                try
                {
                    _context.CarouselImages.Add(carouselImage);
                    await _context.SaveChangesAsync();
                }
                catch (Exception dbEx)
                {
                    TempData["Error"] = $"La imagen se guardó en el sistema de archivos pero no se pudo registrar en la base de datos: {dbEx.Message}";
                    return View("Add");
                }

                TempData["Success"] = "Imagen agregada correctamente (método alternativo)";
                return RedirectToAction(nameof(Administrar));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al subir la imagen (método alternativo): {ex.Message}";
                if (ex.InnerException != null)
                {
                    TempData["Error"] += $" Detalle: {ex.InnerException.Message}";
                }
                return View("Add");
            }
        }

        // GET: /Carousel/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var image = await _carouselService.GetCarouselImageById(id);
                if (image == null)
                {
                    TempData["Error"] = "Imagen no encontrada";
                    return RedirectToAction(nameof(Administrar));
                }

                return View(image);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar la imagen: {ex.Message}";
                return RedirectToAction(nameof(Administrar));
            }
        }

        // POST: /Carousel/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CarouselImage model, IFormFile ImageFile)
        {
            if (model == null)
            {
                TempData["Error"] = "Datos inválidos";
                return RedirectToAction(nameof(Administrar));
            }

            try
            {
                // Obtener la imagen actual
                var currentImage = await _carouselService.GetCarouselImageById(model.IdCarouselImage);
                if (currentImage == null)
                {
                    TempData["Error"] = "Imagen no encontrada";
                    return RedirectToAction(nameof(Administrar));
                }

                // Actualizar propiedades
                currentImage.Alt = model.Alt;
                currentImage.UpdatedAt = DateTime.UtcNow;

                // Si se proporciona una nueva imagen, subirla directamente
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    try
                    {
                        // Eliminar la imagen anterior
                        var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, currentImage.Url.TrimStart('/'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }

                        // Generar un nombre único para la nueva imagen
                        var extension = Path.GetExtension(ImageFile.FileName).ToLowerInvariant();
                        var newFileName = $"{Guid.NewGuid()}{extension}";
                        var relativePath = Path.Combine("imagenes", "carousel", newFileName).Replace("\\", "/");
                        var absolutePath = Path.Combine(_webHostEnvironment.WebRootPath, relativePath);

                        // Asegurar que el directorio existe
                        var directory = Path.GetDirectoryName(absolutePath);
                        if (!Directory.Exists(directory))
                        {
                            Directory.CreateDirectory(directory);
                        }

                        // Guardar la nueva imagen
                        using (var fileStream = new FileStream(absolutePath, FileMode.Create))
                        {
                            await ImageFile.CopyToAsync(fileStream);
                        }

                        // Verificar que el archivo se haya creado correctamente
                        if (!System.IO.File.Exists(absolutePath))
                        {
                            throw new IOException($"No se pudo verificar la creación del archivo en: {absolutePath}");
                        }

                        // Actualizar la URL
                        currentImage.Url = $"/{relativePath}";
                    }
                    catch (Exception ex)
                    {
                        TempData["Error"] = $"Error al actualizar la imagen: {ex.Message}";
                        return View(model);
                    }
                }

                // Guardar los cambios
                await _carouselService.SaveCarouselImages(new List<CarouselImage> { currentImage });

                TempData["Success"] = "Imagen actualizada correctamente";
                return RedirectToAction(nameof(Administrar));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al actualizar la imagen: {ex.Message}";
                return View(model);
            }
        }

        // POST: /Carousel/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _carouselService.DeleteCarouselImage(id);
                TempData["Success"] = "Imagen eliminada correctamente";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al eliminar la imagen: {ex.Message}";
            }

            return RedirectToAction(nameof(Administrar));
        }

        // POST: /Carousel/UpdateOrder
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOrder([FromBody] List<int> imageIds)
        {
            if (imageIds == null || imageIds.Count == 0)
            {
                return BadRequest("No se proporcionaron IDs de imágenes");
            }

            try
            {
                // Obtener todas las imágenes
                var allImages = await _carouselService.GetAllCarouselImages();
                var imagesToUpdate = new List<CarouselImage>();

                // Actualizar el orden según la lista recibida
                for (int i = 0; i < imageIds.Count; i++)
                {
                    var image = allImages.FirstOrDefault(img => img.IdCarouselImage == imageIds[i]);
                    if (image != null)
                    {
                        image.Order = i + 1;
                        image.UpdatedAt = DateTime.UtcNow;
                        imagesToUpdate.Add(image);
                    }
                }

                // Guardar los cambios
                await _carouselService.SaveCarouselImages(imagesToUpdate);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // Método para verificar permisos de directorio
        [NonAction]
        private bool CheckDirectoryPermissions(string path)
        {
            try
            {
                var directory = Path.GetDirectoryName(path);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Intentar crear un archivo de prueba
                var testFile = Path.Combine(directory, "test.txt");
                System.IO.File.WriteAllText(testFile, "Test");
                System.IO.File.Delete(testFile);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error de permisos: {ex.Message}");
                return false;
            }
        }
    }
}