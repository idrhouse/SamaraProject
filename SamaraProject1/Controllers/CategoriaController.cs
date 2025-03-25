using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using SamaraProject1.Models; // Replace YourProjectName with your actual project name

namespace SamaraProject1.Controllers // Replace YourProjectName with your actual project name
{
    public class CategoriaController : Controller
    {
        private readonly SamaraMarketContext _context;

        public CategoriaController(SamaraMarketContext context)
        {
            _context = context;
        }

        // GET: Categoria
        public async Task<IActionResult> Lista()
        {
            return View(await _context.Categorias.ToListAsync());
        }

        // GET: Categoria/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoria = await _context.Categorias
                .FirstOrDefaultAsync(m => m.IdCategoria == id);
            if (categoria == null)
            {
                return NotFound();
            }

            return View(categoria);
        }

        // GET: Categoria/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categoria/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdCategoria,NombreCategoria")] Categoria categoria)
        {
            if (ModelState.IsValid)
            {
                _context.Add(categoria);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Categoría creada correctamente.";
                return RedirectToAction(nameof(Lista));
            }
            return View(categoria);
        }

        // GET: Categoria/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null)
            {
                return NotFound();
            }
            return View(categoria);
        }

        // POST: Categoria/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdCategoria,NombreCategoria")] Categoria categoria)
        {
            if (id != categoria.IdCategoria)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(categoria);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Categoría actualizada correctamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoriaExists(categoria.IdCategoria))
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
            return View(categoria);
        }

        // GET: Categoria/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoria = await _context.Categorias
                .FirstOrDefaultAsync(m => m.IdCategoria == id);
            if (categoria == null)
            {
                return NotFound();
            }

            return View(categoria);
        }

        // POST: Categoria/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null)
            {
                return NotFound();
            }

            // Verificar si hay emprendedores asociados a esta categoría
            bool tieneEmprendedores = await _context.Emprendedores.AnyAsync(e => e.IdCategoria == id);

            if (tieneEmprendedores)
            {
                // Si hay emprendedores asociados, mostrar un mensaje de error
                TempData["Error"] = "No se puede eliminar la categoría porque hay emprendedores asociados a ella. Reasigne los emprendedores a otra categoría antes de eliminar esta.";
                return RedirectToAction(nameof(Lista));
            }

            try
            {
                _context.Categorias.Remove(categoria);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Categoría eliminada correctamente.";
                return RedirectToAction(nameof(Lista));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar la categoría: " + ex.Message;
                return RedirectToAction(nameof(Lista));
            }
        }

        private bool CategoriaExists(int id)
        {
            return _context.Categorias.Any(e => e.IdCategoria == id);
        }
    }
}

