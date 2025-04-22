using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace SamaraProject1.Controllers
{
    public class VideoController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public VideoController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // GET: /Video/GetThumbnail?videoId=VIDEO_ID
        [HttpGet]
        [ResponseCache(Duration = 86400)] // Caché por 24 horas
        public async Task<IActionResult> GetThumbnail(string videoId)
        {
            if (string.IsNullOrEmpty(videoId))
            {
                return BadRequest("ID de video no proporcionado");
            }

            try
            {
                // Intentar obtener la miniatura de alta calidad
                var thumbnailUrl = $"https://img.youtube.com/vi/{videoId}/maxresdefault.jpg";
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync(thumbnailUrl);

                // Si no existe la miniatura de alta calidad, usar la estándar
                if (!response.IsSuccessStatusCode)
                {
                    thumbnailUrl = $"https://img.youtube.com/vi/{videoId}/hqdefault.jpg";
                    response = await client.GetAsync(thumbnailUrl);
                }

                if (response.IsSuccessStatusCode)
                {
                    var imageBytes = await response.Content.ReadAsByteArrayAsync();
                    return File(imageBytes, "image/jpeg");
                }
                else
                {
                    return NotFound("No se pudo obtener la miniatura del video");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener la miniatura: {ex.Message}");
            }
        }
    }
}
