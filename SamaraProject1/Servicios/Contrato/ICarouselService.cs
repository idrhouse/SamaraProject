using Microsoft.AspNetCore.Http;
using SamaraProject1.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SamaraProject1.Servicios.Contrato
{
    public interface ICarouselService
    {
        Task<List<CarouselImage>> GetAllCarouselImages();
        Task<CarouselImage> GetCarouselImageById(int id);
        Task<CarouselImage> UploadCarouselImage(IFormFile image);
        Task<CarouselImage> UploadCarouselImageData(byte[] imageData, string fileName);
        Task DeleteCarouselImage(int id);
        Task SaveCarouselImages(List<CarouselImage> images);
        Task<int> GetNextOrderNumber();
    }
}
