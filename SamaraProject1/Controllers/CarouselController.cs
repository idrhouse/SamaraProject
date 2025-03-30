﻿using Microsoft.AspNetCore.Mvc;
using SamaraProject1.Models;
using SamaraProject1.Servicios.Contrato;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SamaraProject1.Controllers
{
    public class CarouselController : Controller
    {
        private readonly ICarouselService _carouselService;

        public CarouselController(ICarouselService carouselService)
        {
            _carouselService = carouselService;
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
        public async Task<IActionResult> Add(string alt, Microsoft.AspNetCore.Http.IFormFile image)
        {
            if (image == null || image.Length == 0)
            {
                TempData["Error"] = "Debe seleccionar una imagen";
                return View();
            }

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
                TempData["Error"] = $"Error al subir la imagen: {ex.Message}";
                return View();
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
        public async Task<IActionResult> Edit(CarouselImage model, Microsoft.AspNetCore.Http.IFormFile ImageFile)
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

                // Si se proporciona una nueva imagen, subirla
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    // Eliminar la imagen anterior y subir la nueva
                    await _carouselService.DeleteCarouselImage(model.IdCarouselImage);
                    var newImage = await _carouselService.UploadCarouselImage(ImageFile);

                    // Mantener el mismo ID y orden
                    newImage.IdCarouselImage = model.IdCarouselImage;
                    newImage.Order = currentImage.Order;
                    newImage.Alt = model.Alt;

                    await _carouselService.SaveCarouselImages(new List<CarouselImage> { newImage });
                }
                else
                {
                    // Guardar los cambios
                    await _carouselService.SaveCarouselImages(new List<CarouselImage> { currentImage });
                }

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
    }
}