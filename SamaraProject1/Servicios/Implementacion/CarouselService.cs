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

            try
            {
                // Generar un nombre único para la imagen
                var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
                var fileName = $"{Guid.NewGuid()}{extension}";
                var relativePath = Path.Combine("imagenes", "carousel", fileName).Replace("\\", "/");
                var absolutePath = Path.Combine(_webHostEnvironment.WebRootPath, relativePath);

                // Asegurar que el directorio existe
                var directory = Path.GetDirectoryName(absolutePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Verificar permisos de escritura
                try
                {
                    var testFilePath = Path.Combine(directory, "test.txt");
                    File.WriteAllText(testFilePath, "Test");
                    File.Delete(testFilePath);
                }
                catch (Exception ex)
                {
                    throw new IOException($"No se tienen permisos de escritura en el directorio: {directory}. Error: {ex.Message}");
                }

                // Guardar la imagen
                using (var stream = new FileStream(absolutePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                // Verificar que el archivo se haya creado correctamente
                if (!File.Exists(absolutePath))
                {
                    throw new IOException($"No se pudo verificar la creación del archivo en: {absolutePath}");
                }

                // Crear y guardar el registro en la base de datos
                var carouselImage = new CarouselImage
                {
                    Url = $"/{relativePath}",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Order = await GetNextOrderNumber()
                };

                _context.CarouselImages.Add(carouselImage);
                await _context.SaveChangesAsync();

                return carouselImage;
            }
            catch (Exception ex)
            {
                // Registrar el error detallado
                Console.WriteLine($"Error al subir imagen: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Error interno: {ex.InnerException.Message}");
                }

                // Relanzar la excepción con más detalles
                throw new Exception($"Error al subir la imagen: {ex.Message}", ex);
            }
        }

        public async Task<CarouselImage> UploadCarouselImageData(byte[] imageData, string fileName)
        {
            if (imageData == null || imageData.Length == 0)
            {
                throw new ArgumentException("No se han proporcionado datos de imagen válidos");
            }

            try
            {
                // Generar un nombre único para la imagen
                var extension = Path.GetExtension(fileName).ToLowerInvariant();
                var newFileName = $"{Guid.NewGuid()}{extension}";
                var relativePath = Path.Combine("imagenes", "carousel", newFileName).Replace("\\", "/");
                var absolutePath = Path.Combine(_webHostEnvironment.WebRootPath, relativePath);

                // Asegurar que el directorio existe
                var directory = Path.GetDirectoryName(absolutePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Verificar permisos de escritura
                try
                {
                    var testFilePath = Path.Combine(directory, "test.txt");
                    File.WriteAllText(testFilePath, "Test");
                    File.Delete(testFilePath);
                }
                catch (Exception ex)
                {
                    throw new IOException($"No se tienen permisos de escritura en el directorio: {directory}. Error: {ex.Message}");
                }

                // Guardar la imagen
                await File.WriteAllBytesAsync(absolutePath, imageData);

                // Verificar que el archivo se haya creado correctamente
                if (!File.Exists(absolutePath))
                {
                    throw new IOException($"No se pudo verificar la creación del archivo en: {absolutePath}");
                }

                // Crear y guardar el registro en la base de datos
                var carouselImage = new CarouselImage
                {
                    Url = $"/{relativePath}",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Order = await GetNextOrderNumber()
                };

                _context.CarouselImages.Add(carouselImage);
                await _context.SaveChangesAsync();

                return carouselImage;
            }
            catch (Exception ex)
            {
                // Registrar el error detallado
                Console.WriteLine($"Error al subir imagen: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Error interno: {ex.InnerException.Message}");
                }

                // Relanzar la excepción con más detalles
                throw new Exception($"Error al subir la imagen: {ex.Message}", ex);
            }
        }

        public async Task DeleteCarouselImage(int id)
        {
            var image = await _context.CarouselImages.FindAsync(id);
            if (image == null)
            {
                throw new ArgumentException($"No se encontró la imagen con ID {id}");
            }

            try
            {
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
            catch (Exception ex)
            {
                throw new Exception($"Error al eliminar la imagen: {ex.Message}", ex);
            }
        }

        public async Task SaveCarouselImages(List<CarouselImage> images)
        {
            if (images == null || images.Count == 0)
            {
                return;
            }

            try
            {
                foreach (var image in images)
                {
                    _context.Entry(image).State = image.IdCarouselImage == 0
                        ? EntityState.Added
                        : EntityState.Modified;
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al guardar las imágenes: {ex.Message}", ex);
            }
        }

        public async Task<int> GetNextOrderNumber()
        {
            try
            {
                var maxOrder = await _context.CarouselImages.MaxAsync(i => (int?)i.Order) ?? 0;
                return maxOrder + 1;
            }
            catch (Exception ex)
            {
                // Si hay un error al obtener el máximo orden, devolver 1
                Console.WriteLine($"Error al obtener el siguiente número de orden: {ex.Message}");
                return 1;
            }
        }
    }
}