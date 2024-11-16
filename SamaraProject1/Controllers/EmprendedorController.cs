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
            ViewBag.Administradores = _context.Administrador.ToList();
            return View();
        }

        // POST: Emprendedor/Crear        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Emprendedor emprendedor, IFormFile imagen)
        {
            // Verificar si se está recibiendo un archivo
            if (imagen != null)
            {
                Console.WriteLine($"Archivo recibido: {imagen.FileName}");
                if (imagen.Length > 0)
                {
                    Console.WriteLine($"Tamaño del archivo: {imagen.Length} bytes");
                    // Proceder con el almacenamiento de la imagen
                }
                else
                {
                    Console.WriteLine("El archivo recibido está vacío.");
                }
            }
            else
            {
                Console.WriteLine("No se recibió ningún archivo.");
            }


            // Verificar si el modelo es válido
            if (!ModelState.IsValid)
            {
                // Mostrar errores de validación en la consola
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Error: {error.ErrorMessage}");
                }

                // Si hay errores, retornar a la vista con los errores
                ViewBag.Administradores = await _context.Administrador.ToListAsync();
                return View(emprendedor);
            }

            // Verificar si se subió una imagen
            if (imagen != null && imagen.Length > 0)
            {
                // Guardar la imagen
                emprendedor.ImagenUrl = await GuardarImagen(imagen);
                Console.WriteLine($"Imagen subida: {imagen.FileName}");  // Mensaje de depuración
            }
            else
            {
                // Si no se subió una imagen, asignar una imagen predeterminada
                emprendedor.ImagenUrl = "/imagenes/emprendedores/default-emprendedor.jpg";
                Console.WriteLine("No se subió imagen, se usará imagen predeterminada.");
            }

            // Verificar si el correo ya existe
            var correoExistente = await _context.Emprendedores
                .FirstOrDefaultAsync(e => e.Correo == emprendedor.Correo);

            if (correoExistente != null)
            {
                // Si el correo existe, mostrar error
                ModelState.AddModelError("Correo", "Ya existe un emprendedor con este correo.");
                ViewBag.Administradores = await _context.Administrador.ToListAsync();
                return View(emprendedor);
            }

            // Guardar el nuevo emprendedor en la base de datos
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
            ViewBag.Administradores = _context.Administrador.ToList();
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

            var emprendedorExistente = await _context.Emprendedores.AsNoTracking().FirstOrDefaultAsync(e => e.IdEmprendedor == id);

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
                    // Si ya existe el correo, agregar el error y retornar la vista con el mensaje
                    ModelState.AddModelError("Correo", "Ya existe un emprendedor con este correo.");
                    ViewBag.Administradores = await _context.Administrador.ToListAsync();
                    return View(emprendedor);
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Manejo de la imagen
                    if (imagen != null && imagen.Length > 0)
                    {
                        emprendedor.ImagenUrl = await GuardarImagen(imagen);
                    }
                    else
                    {
                        // Si no se sube una nueva imagen, mantener la imagen existente
                        emprendedor.ImagenUrl = emprendedorExistente.ImagenUrl;
                    }

                    // Actualizar solo los valores que han cambiado
                    _context.Entry(emprendedorExistente).CurrentValues.SetValues(emprendedor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmprendedorExists(emprendedor.IdEmprendedor))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Lista));
            }

            // Si no es válido, retornar la vista con los errores
            ViewBag.Administradores = await _context.Administrador.ToListAsync();
            return View(emprendedor);
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
