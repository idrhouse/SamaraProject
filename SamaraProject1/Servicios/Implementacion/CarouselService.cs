using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SamaraProject1.Models;
using SamaraProject1.Servicios.Contrato;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SamaraProject1.Servicios.Implementacion
{
    public class CarouselService : ICarouselService
    {
        private readonly SamaraMarketContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<CarouselService> _logger;

        public CarouselService(SamaraMarketContext context, IWebHostEnvironment webHostEnvironment, ILogger<CarouselService> logger)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }

        public async Task<List<CarouselImage>> GetAllCarouselImages()
        {
            return await _context.CarouselImages
                .OrderBy(i => i.Order)
                .ToListAsync();
        }

        public async Task<CarouselImage> GetCarouselImageById(int id)
        {
            return await _context.CarouselImages.FindAsync(id);
        }

        public async Task<CarouselImage> UploadCarouselImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("No se ha proporcionado ningún archivo");
            }

            // Verificar el tamaño máximo (5MB)
            if (file.Length > 5 * 1024 * 1024)
            {
                throw new ArgumentException("La imagen excede el tamaño máximo permitido de 5MB");
            }

            // Verificar el tipo de archivo
            var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif" };
            if (!allowedTypes.Contains(file.ContentType.ToLower()))
            {
                throw new ArgumentException("Solo se permiten archivos de imagen (JPG, PNG, GIF)");
            }

            try
            {
                // Crear directorio si no existe
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "carousel");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Generar nombre único para el archivo
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Guardar el archivo
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                // Obtener el último orden
                int lastOrder = await _context.CarouselImages
                    .OrderByDescending(i => i.Order)
                    .Select(i => i.Order)
                    .FirstOrDefaultAsync();

                // Crear la entidad CarouselImage
                var carouselImage = new CarouselImage
                {
                    Url = "/images/carousel/" + uniqueFileName,
                    Alt = "",
                    Order = lastOrder + 1,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                // Guardar en la base de datos
                _context.CarouselImages.Add(carouselImage);
                await _context.SaveChangesAsync();

                return carouselImage;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al subir imagen del carrusel");
                throw;
            }
        }

        public async Task SaveCarouselImages(List<CarouselImage> images)
        {
            if (images == null || !images.Any())
            {
                return;
            }

            try
            {
                foreach (var image in images)
                {
                    var existingImage = await _context.CarouselImages.FindAsync(image.IdCarouselImage);
                    if (existingImage != null)
                    {
                        existingImage.Alt = image.Alt;
                        existingImage.Order = image.Order;
                        existingImage.UpdatedAt = DateTime.UtcNow;
                    }
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar imágenes del carrusel");
                throw;
            }
        }

        public async Task DeleteCarouselImage(int id)
        {
            var image = await _context.CarouselImages.FindAsync(id);
            if (image == null)
            {
                throw new KeyNotFoundException($"No se encontró la imagen con ID {id}");
            }

            try
            {
                // Eliminar el archivo físico
                if (!string.IsNullOrEmpty(image.Url))
                {
                    string filePath = Path.Combine(_webHostEnvironment.WebRootPath, image.Url.TrimStart('/'));
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                }

                // Eliminar de la base de datos
                _context.CarouselImages.Remove(image);
                await _context.SaveChangesAsync();

                // Reordenar las imágenes restantes
                var remainingImages = await _context.CarouselImages
                    .OrderBy(i => i.Order)
                    .ToListAsync();

                for (int i = 0; i < remainingImages.Count; i++)
                {
                    remainingImages[i].Order = i + 1;
                    remainingImages[i].UpdatedAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar imagen del carrusel con ID {id}");
                throw;
            }
        }
    }
}