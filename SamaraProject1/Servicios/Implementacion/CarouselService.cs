using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SamaraProject1.Models;
using SamaraProject1.Servicios.Contrato;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SamaraProject1.Servicios
{
    public class CarouselService : ICarouselService
    {
        private readonly SamaraMarketContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CarouselService(SamaraMarketContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<List<CarouselImage>> GetAllCarouselImages()
        {
            return await _context.CarouselImages.OrderBy(i => i.Order).ToListAsync();
        }

        public async Task<CarouselImage> GetCarouselImageById(int id)
        {
            return await _context.CarouselImages.FindAsync(id);
        }

        public async Task<CarouselImage> UploadCarouselImage(IFormFile image)
        {
            if (image == null || image.Length == 0)
            {
                throw new ArgumentException("No se ha proporcionado una imagen válida");
            }

            // Generar un nombre único para la imagen
            var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
            var fileName = $"{Guid.NewGuid()}{extension}";
            var relativePath = Path.Combine("imagenes", "carousel", fileName);
            var absolutePath = Path.Combine(_webHostEnvironment.WebRootPath, relativePath);

            // Asegurar que el directorio existe
            Directory.CreateDirectory(Path.GetDirectoryName(absolutePath));

            // Guardar la imagen
            using (var stream = new FileStream(absolutePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            // Crear y guardar el registro en la base de datos
            var carouselImage = new CarouselImage
            {
                Url = $"/{relativePath.Replace("\\", "/")}",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Order = await GetNextOrderNumber()
            };

            _context.CarouselImages.Add(carouselImage);
            await _context.SaveChangesAsync();

            return carouselImage;
        }

        public async Task<CarouselImage> UploadCarouselImageData(byte[] imageData, string fileName)
        {
            if (imageData == null || imageData.Length == 0)
            {
                throw new ArgumentException("No se han proporcionado datos de imagen válidos");
            }

            // Generar un nombre único para la imagen
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            var newFileName = $"{Guid.NewGuid()}{extension}";
            var relativePath = Path.Combine("imagenes", "carousel", newFileName);
            var absolutePath = Path.Combine(_webHostEnvironment.WebRootPath, relativePath);

            // Asegurar que el directorio existe
            Directory.CreateDirectory(Path.GetDirectoryName(absolutePath));

            // Guardar la imagen
            await File.WriteAllBytesAsync(absolutePath, imageData);

            // Crear y guardar el registro en la base de datos
            var carouselImage = new CarouselImage
            {
                Url = $"/{relativePath.Replace("\\", "/")}",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Order = await GetNextOrderNumber()
            };

            _context.CarouselImages.Add(carouselImage);
            await _context.SaveChangesAsync();

            return carouselImage;
        }

        public async Task DeleteCarouselImage(int id)
        {
            var image = await _context.CarouselImages.FindAsync(id);
            if (image == null)
            {
                throw new ArgumentException($"No se encontró la imagen con ID {id}");
            }

            // Eliminar el archivo físico
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, image.Url.TrimStart('/'));
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            // Eliminar el registro de la base de datos
            _context.CarouselImages.Remove(image);
            await _context.SaveChangesAsync();
        }

        public async Task SaveCarouselImages(List<CarouselImage> images)
        {
            if (images == null || images.Count == 0)
            {
                return;
            }

            foreach (var image in images)
            {
                _context.Entry(image).State = image.IdCarouselImage == 0
                    ? EntityState.Added
                    : EntityState.Modified;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<int> GetNextOrderNumber()
        {
            var maxOrder = await _context.CarouselImages.MaxAsync(i => (int?)i.Order) ?? 0;
            return maxOrder + 1;
        }
    }
}
