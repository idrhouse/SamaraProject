using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SamaraProject1.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SamaraProject1.Controllers
{
    [Authorize]
    public class EmprendedorController : Controller
    {
        private readonly SamaraMarketContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EmprendedorController(SamaraMarketContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Emprendedor
        public async Task<IActionResult> Lista()
        {
            var emprendedores = await _context.Emprendedores.Include(e => e.Stands).ToListAsync();
            return View(emprendedores);
        }

        [AllowAnonymous]
        public async Task<IActionResult> ListaPublica()
        {
            var emprendedores = await _context.Emprendedores
                .ToListAsync();
            return View(emprendedores);
        }

        // GET: Emprendedor/Crear
        public IActionResult Crear()
        {
            var categorias = _context.Categorias
                .Select(c => new SelectListItem
                {
                    Value = c.IdCategoria.ToString(),
                    Text = c.NombreCategoria
                })
                .ToList();

            if (!categorias.Any())
            {
                ModelState.AddModelError("", "No hay categorías disponibles.");
                return View();
            }

            ViewBag.Categorias = categorias;
            return View();
        }

        //Obtener Imagen
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ObtenerImagen(int id)
        {
            var emprendedor = _context.Emprendedores.FirstOrDefault(e => e.IdEmprendedor == id);

            if (emprendedor == null || emprendedor.ImagenDatos == null)
            {
                // Devuelve una imagen predeterminada si no existe la imagen
                var rutaDefault = Path.Combine(_webHostEnvironment.WebRootPath, "imagenes/default-profile.png");
                var defaultImage = System.IO.File.ReadAllBytes(rutaDefault);
                return File(defaultImage, "image/jpeg");
            }

            // Devuelve la imagen almacenada en la base de datos
            return File(emprendedor.ImagenDatos, "image/jpeg");
        }

        // POST: Emprendedor/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Emprendedor emprendedor, IFormFile? imagen)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categorias = _context.Categorias
                    .Select(c => new SelectListItem
                    {
                        Value = c.IdCategoria.ToString(),
                        Text = c.NombreCategoria
                    })
                    .ToList();

                return View(emprendedor);
            }

            // Validar si la categoría existe
            var categoriaExistente = await _context.Categorias
                .FirstOrDefaultAsync(c => c.IdCategoria == emprendedor.IdCategoria);

            if (categoriaExistente == null)
            {
                ModelState.AddModelError("IdCategoria", "La categoría seleccionada no es válida.");
                ViewBag.Categorias = _context.Categorias
                    .Select(c => new SelectListItem
                    {
                        Value = c.IdCategoria.ToString(),
                        Text = c.NombreCategoria
                    })
                    .ToList();
                return View(emprendedor);
            }

            // Asignar la categoría al emprendedor
            emprendedor.Categoria = categoriaExistente;

            // Validar si el correo ya existe
            var correoExistente = await _context.Emprendedores
                .FirstOrDefaultAsync(e => e.Correo == emprendedor.Correo);

            if (correoExistente != null)
            {
                ModelState.AddModelError("Correo", "Ya existe un emprendedor con este correo.");
                ViewBag.Categorias = _context.Categorias
                    .Select(c => new SelectListItem
                    {
                        Value = c.IdCategoria.ToString(),
                        Text = c.NombreCategoria
                    })
                    .ToList();
                return View(emprendedor);
            }

            // Manejo de la imagen
            if (imagen != null && imagen.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await imagen.CopyToAsync(memoryStream);
                    emprendedor.ImagenDatos = memoryStream.ToArray();
                }
            }

            try
            {
                _context.Add(emprendedor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Lista));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Ocurrió un error al guardar el emprendedor: " + ex.Message);
                ViewBag.Categorias = _context.Categorias
                    .Select(c => new SelectListItem
                    {
                        Value = c.IdCategoria.ToString(),
                        Text = c.NombreCategoria
                    })
                    .ToList();
                return View(emprendedor);
            }
        }


        // GET: Emprendedor/Editar
        public async Task<IActionResult> Editar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var emprendedor = await _context.Emprendedores.FindAsync(id);
            if (emprendedor == null)
            {
                return NotFound();
            }

            // Convierte la lista de categorías en una lista de SelectListItem
            var categorias = _context.Categorias.ToList();
            var categoriasSelectList = categorias.Select(c => new SelectListItem
            {
                Value = c.IdCategoria.ToString(),
                Text = c.NombreCategoria
            }).ToList();
            ViewBag.Categorias = categoriasSelectList;

            return View(emprendedor);
        }


        // POST: Emprendedor/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, Emprendedor emprendedor, IFormFile? imagen)
        {
            if (id != emprendedor.IdEmprendedor)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                // Recargar las categorías en el ViewBag si el modelo no es válido
                var categorias = _context.Categorias.ToList();
                var categoriasSelectList = categorias.Select(c => new SelectListItem
                {
                    Value = c.IdCategoria.ToString(),
                    Text = c.NombreCategoria
                }).ToList();
                ViewBag.Categorias = categoriasSelectList;

                return View(emprendedor);
            }

            // Validar si la categoría existe
            var categoriaExistente = await _context.Categorias
                .FirstOrDefaultAsync(c => c.IdCategoria == emprendedor.IdCategoria);

            if (categoriaExistente == null)
            {
                ModelState.AddModelError("IdCategoria", "La categoría seleccionada no es válida.");
                ViewBag.Categorias = await _context.Categorias.ToListAsync();
                return View(emprendedor);
            }

            var emprendedorExistente = await _context.Emprendedores.FindAsync(id);

            if (emprendedorExistente == null)
            {
                return NotFound();
            }

            // Manejo de la imagen
            if (imagen != null && imagen.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await imagen.CopyToAsync(memoryStream);
                    emprendedorExistente.ImagenDatos = memoryStream.ToArray();
                }
            }
            else
            {
                emprendedorExistente.ImagenDatos = emprendedor.ImagenDatos;
            }

            // Actualizar los datos del emprendedor existente
            emprendedorExistente.NombreEmprendedor = emprendedor.NombreEmprendedor;
            emprendedorExistente.Apellidos = emprendedor.Apellidos;
            emprendedorExistente.Correo = emprendedor.Correo;
            emprendedorExistente.DescripcionNegocio = emprendedor.DescripcionNegocio;
            emprendedorExistente.NombreNegocio = emprendedor.NombreNegocio;
            emprendedorExistente.Telefono = emprendedor.Telefono;
            emprendedorExistente.IdCategoria = emprendedor.IdCategoria;
            emprendedorExistente.Categoria = categoriaExistente; // Asignar la categoría

            try
            {
                _context.Update(emprendedorExistente);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Lista));
            }
            catch (Exception ex)
            {
                // Manejar errores de base de datos
                ModelState.AddModelError("", "Ocurrió un error al actualizar el emprendedor: " + ex.Message);
                ViewBag.Categorias = await _context.Categorias.ToListAsync();
                return View(emprendedor);
            }
        }

        // GET: Emprendedor/Eliminar
        public async Task<IActionResult> Eliminar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var emprendedor = await _context.Emprendedores
                .Include(e => e.Productos)
                .Include(e => e.Stands)
                .FirstOrDefaultAsync(m => m.IdEmprendedor == id);

            if (emprendedor == null)
            {
                return NotFound();
            }

            ViewBag.TieneProductos = emprendedor.Productos.Any();
            ViewBag.TieneStands = emprendedor.Stands.Any();

            return View(emprendedor);
        }

        // POST: Emprendedor/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            var emprendedor = await _context.Emprendedores
                .Include(e => e.Productos)
                .Include(e => e.Stands)
                .FirstOrDefaultAsync(e => e.IdEmprendedor == id);

            if (emprendedor == null)
            {
                return NotFound();
            }


            if (emprendedor.Productos.Any() || emprendedor.Stands.Any())
            {
                ModelState.AddModelError("", "No se puede eliminar el emprendedor porque tiene stands asociados.");
                ViewBag.TieneProductos = emprendedor.Productos.Any();
                ViewBag.TieneStands = emprendedor.Stands.Any();
                return View(emprendedor);
            }

            try
            {
                // Eliminar el emprendedor de la base de datos
                _context.Emprendedores.Remove(emprendedor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Lista));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Ocurrió un error al intentar eliminar el emprendedor: " + ex.Message);
                ViewBag.TieneProductos = emprendedor.Productos.Any();
                ViewBag.TieneStands = emprendedor.Stands.Any();
                return View(emprendedor);
            }
        }

    }
}
