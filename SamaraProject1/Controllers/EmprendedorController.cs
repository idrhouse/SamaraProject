using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
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
                return View(emprendedor);
            }

            if (imagen != null && imagen.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await imagen.CopyToAsync(memoryStream);
                    emprendedor.ImagenDatos = memoryStream.ToArray();
                }
            }

            // Validar si el correo ya existe
            var correoExistente = await _context.Emprendedores
                .FirstOrDefaultAsync(e => e.Correo == emprendedor.Correo);

            if (correoExistente != null)
            {
                ModelState.AddModelError("Correo", "Ya existe un emprendedor con este correo.");
                return View(emprendedor);
            }

            // Si no se sube una imagen, deja el campo null o lógica de imagen predeterminada
            if (emprendedor.ImagenDatos == null)
            {
                emprendedor.ImagenDatos = null; // O mantenlo como null para usar un endpoint de imagen predeterminada
            }

            _context.Add(emprendedor);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Lista));
        }


        // GET: Emprendedor/Editar/5
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

            var emprendedorExistente = await _context.Emprendedores.FindAsync(id);

            if (emprendedorExistente == null)
            {
                return NotFound();
            }

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
                // If no new image is uploaded, keep the existing image URL
                emprendedor.ImagenDatos = emprendedorExistente.ImagenDatos;
            }

            emprendedorExistente.NombreEmprendedor = emprendedor.NombreEmprendedor;
            emprendedorExistente.Apellidos = emprendedor.Apellidos;
            emprendedorExistente.Correo = emprendedor.Correo;
            emprendedorExistente.DescripcionNegocio = emprendedor.DescripcionNegocio;
            emprendedorExistente.NombreNegocio = emprendedor.NombreNegocio;
            emprendedorExistente.Telefono = emprendedor.Telefono;

            _context.Update(emprendedorExistente);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Lista));
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
