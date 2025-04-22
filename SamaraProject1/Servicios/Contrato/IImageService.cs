using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Webp;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace SamaraProject1.Servicios
{
    public interface IImageService
    {
        Task<byte[]> OptimizeImageAsync(byte[] imageData, int maxWidth = 1200, int quality = 80);
        Task<byte[]> OptimizeImageAsync(Stream imageStream, int maxWidth = 1200, int quality = 80);
        Task<byte[]> OptimizeImageAsWebPAsync(Stream imageStream, int maxWidth = 1200, int quality = 80);
        Task<string> GetImageFormatAsync(IFormFile file);
        Task<bool> IsAnimatedGifAsync(Stream stream);
    }

    public class ImageService : IImageService
    {
        private readonly ILogger<ImageService> _logger;

        public ImageService(ILogger<ImageService> logger)
        {
            _logger = logger;
        }

        public async Task<byte[]> OptimizeImageAsync(byte[] imageData, int maxWidth = 1200, int quality = 80)
        {
            try
            {
                using (var stream = new MemoryStream(imageData))
                {
                    return await OptimizeImageAsync(stream, maxWidth, quality);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al optimizar imagen desde bytes");
                return imageData; // Devolver la imagen original en caso de error
            }
        }

        public async Task<byte[]> OptimizeImageAsync(Stream imageStream, int maxWidth = 1200, int quality = 80)
        {
            try
            {
                // Asegurarse de que el stream esté en la posición inicial
                if (imageStream.CanSeek)
                {
                    imageStream.Position = 0;
                }

                // Verificar si es un GIF animado
                bool isAnimatedGif = await IsAnimatedGifAsync(imageStream);
                if (isAnimatedGif)
                {
                    // Para GIFs animados, devolver el original sin procesar
                    if (imageStream.CanSeek)
                    {
                        imageStream.Position = 0;
                    }

                    using (var ms = new MemoryStream())
                    {
                        await imageStream.CopyToAsync(ms);
                        return ms.ToArray();
                    }
                }

                // Resetear la posición del stream
                if (imageStream.CanSeek)
                {
                    imageStream.Position = 0;
                }

                using (var image = await Image.LoadAsync(imageStream))
                {
                    // Redimensionar solo si la imagen es más ancha que maxWidth
                    if (image.Width > maxWidth)
                    {
                        // Mantener la relación de aspecto
                        var ratio = (double)maxWidth / image.Width;
                        var newHeight = (int)(image.Height * ratio);

                        image.Mutate(x => x.Resize(maxWidth, newHeight));
                    }

                    // Guardar con la calidad especificada
                    using (var outputStream = new MemoryStream())
                    {
                        await image.SaveAsJpegAsync(outputStream, new JpegEncoder { Quality = quality });
                        return outputStream.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al optimizar imagen desde stream");

                // En caso de error, intentar devolver la imagen original
                if (imageStream.CanSeek)
                {
                    imageStream.Position = 0;
                    using (var ms = new MemoryStream())
                    {
                        await imageStream.CopyToAsync(ms);
                        return ms.ToArray();
                    }
                }

                throw; // Si no podemos recuperar la imagen original, propagar el error
            }
        }

        public async Task<byte[]> OptimizeImageAsWebPAsync(Stream imageStream, int maxWidth = 1200, int quality = 80)
        {
            try
            {
                // Asegurarse de que el stream esté en la posición inicial
                if (imageStream.CanSeek)
                {
                    imageStream.Position = 0;
                }

                using (var image = await Image.LoadAsync(imageStream))
                {
                    // Redimensionar solo si la imagen es más ancha que maxWidth
                    if (image.Width > maxWidth)
                    {
                        // Mantener la relación de aspecto
                        var ratio = (double)maxWidth / image.Width;
                        var newHeight = (int)(image.Height * ratio);

                        image.Mutate(x => x.Resize(maxWidth, newHeight));
                    }

                    // Guardar como WebP con la calidad especificada
                    using (var outputStream = new MemoryStream())
                    {
                        await image.SaveAsWebpAsync(outputStream, new WebpEncoder { Quality = quality });
                        return outputStream.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al optimizar imagen como WebP");

                // En caso de error, intentar optimizar como JPEG
                return await OptimizeImageAsync(imageStream, maxWidth, quality);
            }
        }

        public async Task<string> GetImageFormatAsync(IFormFile file)
        {
            try
            {
                using (var stream = file.OpenReadStream())
                {
                    var image = await Image.IdentifyAsync(stream);
                    return image.Metadata.DecodedImageFormat?.Name ?? "unknown";
                }
            }
            catch
            {
                return "unknown";
            }
        }

        public async Task<bool> IsAnimatedGifAsync(Stream stream)
        {
            try
            {
                if (stream.CanSeek)
                {
                    stream.Position = 0;
                }

                // Leer los primeros bytes para identificar si es un GIF
                byte[] header = new byte[6];
                await stream.ReadAsync(header, 0, 6);

                // Verificar si es un GIF
                if (header[0] == 'G' && header[1] == 'I' && header[2] == 'F')
                {
                    if (stream.CanSeek)
                    {
                        stream.Position = 0;
                    }

                    // Leer todo el archivo para buscar múltiples frames
                    using (var ms = new MemoryStream())
                    {
                        await stream.CopyToAsync(ms);
                        byte[] data = ms.ToArray();

                        // Buscar múltiples frames (0x21, 0xF9, 0x04 seguido por 0x00, 0x2C)
                        int frameCount = 0;
                        for (int i = 0; i < data.Length - 4; i++)
                        {
                            if (data[i] == 0x21 && data[i + 1] == 0xF9 && data[i + 2] == 0x04)
                            {
                                frameCount++;
                                if (frameCount > 1)
                                {
                                    return true; // Más de un frame, es animado
                                }
                            }
                        }
                    }
                }

                if (stream.CanSeek)
                {
                    stream.Position = 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar si es un GIF animado");
                return false;
            }
        }
    }
}
