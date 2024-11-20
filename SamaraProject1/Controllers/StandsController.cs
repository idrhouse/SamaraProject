using Microsoft.AspNetCore.Authorization;
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
    public class StandsController : Controller
    {
        private readonly SamaraMarketContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public StandsController(SamaraMarketContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Stands
        public async Task<IActionResult> Lista()
        {
            var stands = await _context.Stands
                                       .Include(s => s.Emprendedor)  // Asegúrate de incluir la relación Emprendedor
                                       .OrderBy(s => s.Numero_Stand)
                                       .ToListAsync();
            return View(stands);
        }


        [AllowAnonymous]
        public async Task<IActionResult> ListaPublica()
        {
            var stands = await _context.Stands.OrderBy(s => s.Numero_Stand).ToListAsync();
            return View(stands);
        }
        // GET: Stands/Crear
        public IActionResult Crear()
        {
            ViewBag.Emprendedores = new SelectList(_context.Emprendedores.ToList(), "IdEmprendedor", "NombreEmprendedor");
            return View();
        }

        // POST: Stands/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Stands stand)
        {
            if (ModelState.IsValid)
            {
                // Verificar si ya existe un stand con el mismo número
                var standExistenteNumero = await _context.Stands.AnyAsync(s => s.Numero_Stand == stand.Numero_Stand);
                if (standExistenteNumero)
                {
                    ModelState.AddModelError("", "Ya existe un stand con este número. Por favor, elige un número diferente.");
                    ViewBag.Emprendedores = new SelectList(_context.Emprendedores.ToList(), "IdEmprendedor", "NombreEmprendedor", stand.IdEmprendedor);
                    return View(stand);
                }

                stand.Disponible = true; // Marcar como no disponible al asignarse
                _context.Add(stand);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Lista));
            }
            ViewBag.Emprendedores = new SelectList(_context.Emprendedores.ToList(), "IdEmprendedor", "NombreEmprendedor", stand.IdEmprendedor);
            return View(stand);
        }


        // GET: Stands/Editar/5
        public async Task<IActionResult> Editar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stand = await _context.Stands
                .Include(s => s.Emprendedor)
                .FirstOrDefaultAsync(s => s.IdStand == id);

            if (stand == null)
            {
                return NotFound();
            }

            ViewBag.Emprendedores = await _context.Emprendedores.ToListAsync();
            return View(stand);
        }

        // StandExist
        private bool StandExists(int id)
        {
            return _context.Stands.Any(e => e.IdStand == id);
        }


        // POST: Stands/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, [Bind("IdStand,Numero_Stand,Descripcion_Stand,IdEmprendedor,Disponible")] Stands stand)
        {
            if (id != stand.IdStand)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Emprendedores = await _context.Emprendedores.ToListAsync();
                return View(stand);
            }

            try
            {
                var existingStand = await _context.Stands.AsNoTracking().FirstOrDefaultAsync(s => s.IdStand == id);
                if (existingStand == null)
                {
                    return NotFound();
                }

                // Solo se validará la reasignación si el IdEmprendedor o el Numero_Stand cambian
                if (existingStand.Numero_Stand != stand.Numero_Stand ||
                    (stand.IdEmprendedor != existingStand.IdEmprendedor && !existingStand.Disponible))
                {
                    ModelState.AddModelError("", $"El stand número {stand.Numero_Stand} ya está creado.");
                    ViewBag.Emprendedores = await _context.Emprendedores.ToListAsync();
                    return View(stand);
                }

                stand.Disponible = stand.IdEmprendedor == null; // Disponible si no tiene emprendedor asignado
                _context.Entry(existingStand).State = EntityState.Detached;
                _context.Entry(stand).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Lista));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StandExists(stand.IdStand))
                {
                    return NotFound();
                }
                throw;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Ha ocurrido un error al guardar los cambios: " + ex.Message);
                ViewBag.Emprendedores = await _context.Emprendedores.ToListAsync();
                return View(stand);
            }
        }


        // GET: Stands/Eliminar/5
        public async Task<IActionResult> Eliminar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stand = await _context.Stands
                .Include(s => s.Emprendedor)
                .FirstOrDefaultAsync(m => m.IdStand == id);
            if (stand == null)
            {
                return NotFound();
            }

            return View(stand);
        }

        // POST: Stands/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            var stand = await _context.Stands.FindAsync(id);
            if (stand != null)
            {
                _context.Stands.Remove(stand);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Lista));
        }

        // Acción para quitar el emprendedor de un stand y marcarlo como disponible
        public async Task<IActionResult> QuitarEmprendedor(int id)
        {
            var stand = await _context.Stands
                .Include(s => s.Emprendedor)  // Incluimos la relación con el emprendedor
                .FirstOrDefaultAsync(s => s.IdStand == id);

            if (stand == null)
            {
                return NotFound();  // Si el stand no existe, devolvemos un error
            }

            // Actualizar las propiedades del stand
            stand.IdEmprendedor = null;  // Quitar el emprendedor (IdEmprendedor se vuelve NULL)
            stand.Disponible = true;     // Marcar como disponible

            _context.Update(stand);  // Actualizar el stand en el contexto
            await _context.SaveChangesAsync();  // Guardar los cambios en la base de datos

            TempData["Message"] = "El emprendedor ha sido quitado y el stand está disponible nuevamente.";  // Mensaje de éxito
            return RedirectToAction(nameof(Lista));  // Redirigir a la lista de stands
        }


    }
}