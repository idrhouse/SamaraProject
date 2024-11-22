using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SamaraProject1.Models;
using System.IO;

namespace SamaraProject1.Controllers
{
    [Authorize]
    public class ProductoController : Controller
    {
        private readonly SamaraMarketContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductoController(SamaraMarketContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Lista()
        {
            var productos = await _context.Productos
                .Include(p => p.ProductoEmprendedores)
                    .ThenInclude(pe => pe.Emprendedor)
                .Include(p => p.TipoProducto)
                .ToListAsync();
            return View(productos);
        }

        [AllowAnonymous]
        public async Task<IActionResult> ListaPublica()
        {
            var productos = await _context.Productos
                .Include(p => p.ProductoEmprendedores)
                    .ThenInclude(pe => pe.Emprendedor)
                .Include(p => p.TipoProducto)
                .Where(p => p.ProductoEmprendedores.Any())
                .ToListAsync();
            return View(productos);
        }

        // GET: Crear
        public async Task<IActionResult> Crear()
        {
            var emprendedores = await _context.Emprendedores.ToListAsync();
            var tipoProductos = await _context.TipoProducto.ToListAsync();

            if (!emprendedores.Any())
            {
                ModelState.AddModelError("", "No hay emprendedores registrados.");
            }

            if (!tipoProductos.Any())
            {
                ModelState.AddModelError("", "No hay tipos de productos registrados.");
            }

            // Asignar datos correctamente al ViewBag
            ViewBag.Emprendedores = emprendedores;
            ViewBag.TipoProductos = tipoProductos; 

            return View();
        }

        //Obtener Imagen
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ObtenerImagen(int id)
        {
            var productos = _context.Productos.FirstOrDefault(e => e.IdProducto == id);

            if (productos == null || productos.ImagenDatos == null)
            {
                // Devuelve una imagen predeterminada si no existe la imagen
                var rutaDefault = Path.Combine(_webHostEnvironment.WebRootPath, "imagenes/default-producto.png");
                var defaultImage = System.IO.File.ReadAllBytes(rutaDefault);
                return File(defaultImage, "image/jpeg");
            }

            // Devuelve la imagen almacenada en la base de datos
            return File(productos.ImagenDatos, "image/jpeg");
        }


        //POST Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Producto producto, int[] SelectedEmprendedores, IFormFile? imagen)
        {
            if (ModelState.IsValid)
            {
                if (!ModelState.IsValid)
                {
                    return View(producto);
                }

            if (imagen != null && imagen.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await imagen.CopyToAsync(memoryStream);
                        producto.ImagenDatos = memoryStream.ToArray();
                }
            }

            // Create the product without assigning Emprendedores yet
            _context.Add(producto);
            await _context.SaveChangesAsync();

            // Now add the relationships
            if (SelectedEmprendedores != null && SelectedEmprendedores.Any())
            {
                foreach (var idEmprendedor in SelectedEmprendedores)
                {
                    var productoEmprendedor = new ProductoEmprendedor
                    {
                        IdProducto = producto.IdProducto,
                        IdEmprendedor = idEmprendedor
                    };
                    _context.Add(productoEmprendedor);
                }
                await _context.SaveChangesAsync();
            }

            TempData["Message"] = "Producto creado exitosamente.";
            return RedirectToAction(nameof(Lista));
            }

            // If we got this far, something failed; redisplay form
            ViewBag.Emprendedores = await _context.Emprendedores.ToListAsync();
            ViewBag.TipoProductos = await _context.TipoProducto.ToListAsync();
            return View(producto);
        }


        //GET Editar
        public async Task<IActionResult> Editar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos
                .Include(p => p.ProductoEmprendedores)
                .ThenInclude(pe => pe.Emprendedor)
                .FirstOrDefaultAsync(m => m.IdProducto == id);

            if (producto == null)
            {
                return NotFound();
            }

            ViewBag.Emprendedores = await _context.Emprendedores.ToListAsync();
            ViewBag.TipoProductos = await _context.TipoProducto.ToListAsync();
            ViewBag.SelectedEmprendedores = producto.ProductoEmprendedores?.Select(pe => pe.IdEmprendedor).ToList() ?? new List<int>();

            return View(producto);
        }

        //POST Editar
        // POST: Evento/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, Evento evento, IFormFile? imagen)
        {
            if (id != evento.IdEvento)
            {
                return NotFound();
            }

            // Obtener el evento existente desde la base de datos
            var eventoExistente = await _context.Eventos.FirstOrDefaultAsync(e => e.IdEvento == id);

            if (eventoExistente == null)
            {
                return NotFound();
            }

            // Validaciones personalizadas
            if (evento.Fecha < DateTime.Today)
            {
                ModelState.AddModelError("Fecha", "No se puede agregar una fecha anterior al día de hoy.");
                return View(evento);
            }

            if (evento.HoraInicio >= evento.HoraFin)
            {
                ModelState.AddModelError("HoraInicio", "La hora de inicio no puede ser mayor o igual a la hora de finalización.");
                return View(evento);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Actualizar datos del evento existente
                    eventoExistente.Nombre = evento.Nombre;
                    eventoExistente.Descripcion = evento.Descripcion;
                    eventoExistente.Fecha = DateTime.SpecifyKind(evento.Fecha.Date + evento.HoraInicio, DateTimeKind.Utc);
                    eventoExistente.HoraInicio = evento.HoraInicio;
                    eventoExistente.HoraFin = evento.HoraFin;

                    // Manejo de imagen
                    if (imagen != null && imagen.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await imagen.CopyToAsync(memoryStream);
                            eventoExistente.ImagenDatos = memoryStream.ToArray();
                        }
                    }

                    // Guardar los cambios en la base de datos
                    _context.Update(eventoExistente);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Lista));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventoExists(evento.IdEvento))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return View(evento);
        }


        private bool EventoExists(int id)
        {
            return _context.Eventos.Any(e => e.IdEvento == id);
        }


        //DELETE
        public async Task<IActionResult> Eliminar(int id)
        {
            var producto = await _context.Productos
                .Include(p => p.ProductoEmprendedores)
                    .ThenInclude(pe => pe.Emprendedor)
                .FirstOrDefaultAsync(m => m.IdProducto == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        //Post Eliminar
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarEliminar(int id)
        {
            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
            {
                return NotFound();
            }

            if (producto != null)
            {
                _context.Productos.Remove(producto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Lista));
            }
            return RedirectToAction(nameof(Lista));
        }

        private bool ProductoExists(int id)
        {
            return _context.Productos.Any(e => e.IdProducto == id);
        }

        

    }
}