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

        // POST: Emprendedor/Crear        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Emprendedor emprendedor, IFormFile? imagen)
        {
            // Validar el modelo
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Error: {error.ErrorMessage}");
                }
                return View(emprendedor);
            }

            // Manejo de la imagen
            if (imagen != null && imagen.Length > 0)
            {
                emprendedor.ImagenUrl = await GuardarImagen(imagen);
                Console.WriteLine($"Imagen subida: {imagen.FileName}");
            }
            else
            {
                // Imagen predeterminada
                emprendedor.ImagenUrl = "/imagenes/emprendedores/default-emprendedor.jpg";
                Console.WriteLine("No se subió imagen, se usará imagen predeterminada.");
            }

            // Validar si el correo ya existe
            var correoExistente = await _context.Emprendedores
                .FirstOrDefaultAsync(e => e.Correo == emprendedor.Correo);

            if (correoExistente != null)
            {
                ModelState.AddModelError("Correo", "Ya existe un emprendedor con este correo.");
                return View(emprendedor);
            }

            // Guardar el emprendedor
            try
            {
                _context.Add(emprendedor);
                await _context.SaveChangesAsync();
                Console.WriteLine("Nuevo emprendedor creado exitosamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al guardar en la base de datos: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Hubo un error al guardar los datos.");
                return View(emprendedor);
            }

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

            // Verificar si el correo ha cambiado y si ya existe otro emprendedor con el mismo correo
            if (emprendedor.Correo != emprendedorExistente.Correo)
            {
                var correoExistente = await _context.Emprendedores
                    .FirstOrDefaultAsync(e => e.Correo == emprendedor.Correo && e.IdEmprendedor != id);

                if (correoExistente != null)
                {
                    ModelState.AddModelError("Correo", "Ya existe un emprendedor con este correo.");
                    return View(emprendedor); // Devolvemos solo el emprendedor
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Manejo de la imagen
                    if (imagen != null && imagen.Length > 0)
                    {
                        emprendedorExistente.ImagenUrl = await GuardarImagen(imagen);
                    }

                    // Actualizar solo los valores modificados manualmente
                    emprendedorExistente.NombreEmprendedor = emprendedor.NombreEmprendedor;
                    emprendedorExistente.Apellidos = emprendedor.Apellidos;
                    emprendedorExistente.Correo = emprendedor.Correo;
                    emprendedorExistente.DescripcionNegocio = emprendedor.DescripcionNegocio;
                    emprendedorExistente.NombreNegocio = emprendedor.NombreNegocio;
                    emprendedorExistente.Telefono = emprendedor.Telefono;

                    _context.Update(emprendedorExistente); // Adjuntar y marcar como modificado
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmprendedorExists(emprendedor.IdEmprendedor))
                    {
                        return NotFound();
                    }
                    throw; // Relanzar la excepción para errores inesperados
                }

                return RedirectToAction(nameof(Lista));
            }

            return View(emprendedor); // Devolvemos solo el emprendedor
        }



        // GET: Emprendedor/Eliminar/5
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
                ModelState.AddModelError("", "No se puede eliminar el emprendedor porque tiene productos o stands asociados.");
                ViewBag.TieneProductos = emprendedor.Productos.Any();
                ViewBag.TieneStands = emprendedor.Stands.Any();
                return View(emprendedor);
            }

            try
            {
                if (!string.IsNullOrEmpty(emprendedor.ImagenUrl))
                {
                    var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, emprendedor.ImagenUrl.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

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

        private bool EmprendedorExists(int id)
        {
            return _context.Emprendedores.Any(e => e.IdEmprendedor == id);
        }

        private async Task<string> GuardarImagen(IFormFile imagen)
        {
            string nombreUnico = Guid.NewGuid().ToString() + Path.GetExtension(imagen.FileName);
            string rutaCarpeta = Path.Combine(_webHostEnvironment.WebRootPath, "imagenes", "emprendedores");

            if (!Directory.Exists(rutaCarpeta))
            {
                Directory.CreateDirectory(rutaCarpeta);
            }

            string rutaCompleta = Path.Combine(rutaCarpeta, nombreUnico);

            using (var stream = new FileStream(rutaCompleta, FileMode.Create))
            {
                await imagen.CopyToAsync(stream);
            }

            return "/imagenes/emprendedores/" + nombreUnico;
        }
    }
}
