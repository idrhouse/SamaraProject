using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using SamaraProject1.Models;
using Microsoft.AspNetCore.Authorization;


namespace SamaraProject1.Controllers
{
    [Authorize]
    [Route("FeaturedProducts")]
    public class FeaturedProductsController : Controller
    {
        private readonly IWebHostEnvironment _environment;
        private readonly string _dataFilePath;

        public FeaturedProductsController(IWebHostEnvironment environment)
        {
            _environment = environment;
            _dataFilePath = Path.Combine(_environment.WebRootPath, "data", "featuredProducts.json");
            EnsureDataFileExists();
        }

        // Asegurar que el archivo JSON existe
        private void EnsureDataFileExists()
        {
            if (!System.IO.File.Exists(_dataFilePath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_dataFilePath));
                System.IO.File.WriteAllText(_dataFilePath, "[]"); // Inicializa con una lista vacía en formato JSON
            }
        }


        // Cargar productos desde el archivo JSON
        private List<FeaturedProduct> LoadProducts()
        {
            try
            {
                var json = System.IO.File.ReadAllText(_dataFilePath);
                return JsonSerializer.Deserialize<List<FeaturedProduct>>(json) ?? new List<FeaturedProduct>();
            }
            catch
            {
                return new List<FeaturedProduct>(); // Si ocurre un error, retorna una lista vacía
            }
        }


        // Guardar productos al archivo JSON
        private void SaveProducts(List<FeaturedProduct> products)
        {
            var json = JsonSerializer.Serialize(products);
            System.IO.File.WriteAllText(_dataFilePath, json);
        }

        [HttpGet]
        public IActionResult GetFeaturedProducts()
        {
            var products = LoadProducts();
            return Json(products);
        }

        [HttpGet("List")]
        public IActionResult ListFeaturedProducts()
        {
            var products = LoadProducts();
            return View("List", products);
        }

        [HttpGet("Add")]
        public IActionResult AddFeaturedProductView()
        {
            return View("Add");
        }



        [HttpPost("Add")]
        public IActionResult AddFeaturedProduct([FromForm] IFormFile image, [FromForm] string name)
        {
            var products = LoadProducts();

            if (image == null || image.Length == 0)
            {
                TempData["Error"] = "No se subió ninguna imagen.";
                return RedirectToAction("Add");
            }

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
            var filePath = Path.Combine("imagenes/featuredproducts", fileName);
            var absolutePath = Path.Combine(_environment.WebRootPath, filePath);

            using (var stream = new FileStream(absolutePath, FileMode.Create))
            {
                image.CopyTo(stream);
            }

            var newProduct = new FeaturedProduct
            {
                Id = products.Count > 0 ? products.Max(p => p.Id) + 1 : 1,
                Name = name,
                ImageUrl = $"/{filePath}"
            };

            products.Add(newProduct);
            SaveProducts(products);

            TempData["Success"] = "El producto fue agregado exitosamente.";
            return RedirectToAction("List");
        }

        [HttpGet("Edit/{id}")]
        public IActionResult EditFeaturedProductView(int id)
        {
            var products = LoadProducts();
            var product = products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                TempData["Error"] = "Producto no encontrado.";
                return RedirectToAction("Edit");
            }
            return View("Edit", product);
        }

        [HttpPost("Edit/{id}")]
        public IActionResult EditFeaturedProduct(int id, [FromForm] IFormFile image, [FromForm] string name)
        {
            var products = LoadProducts();
            var product = products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                TempData["Error"] = "Producto no encontrado.";
                return RedirectToAction("Edit");
            }

            if (!string.IsNullOrEmpty(name))
            {
                product.Name = name;
            }

            if (image != null && image.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                var filePath = Path.Combine("imagenes/featuredproducts", fileName);
                var absolutePath = Path.Combine(_environment.WebRootPath, filePath);

                using (var stream = new FileStream(absolutePath, FileMode.Create))
                {
                    image.CopyTo(stream);
                }

                product.ImageUrl = $"/{filePath}";
            }

            SaveProducts(products);

            TempData["Success"] = "Producto de carrousel actualizado correctamente.";
            return RedirectToAction("List");
        }

        [HttpPost("Delete/{id}")]
        public IActionResult DeleteFeaturedProduct(int id)
        {
            var products = LoadProducts();
            var product = products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                TempData["Error"] = "Producto no encontrado.";
                return RedirectToAction("Edit");
            }

            // Eliminar la imagen física del servidor
            var absolutePath = Path.Combine(_environment.WebRootPath, product.ImageUrl.TrimStart('/'));
            if (System.IO.File.Exists(absolutePath))
            {
                System.IO.File.Delete(absolutePath);
            }

            products.Remove(product);
            SaveProducts(products);

            TempData["Success"] = "Imagen de carrousel eliminado correctamente.";
            return RedirectToAction("List");
        }
    }
}
