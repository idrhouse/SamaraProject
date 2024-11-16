using Microsoft.AspNetCore.Mvc;
using SamaraProject1.Models;
using Microsoft.EntityFrameworkCore;
using SamaraProject1.Recursos;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace SamaraProject1.Controllers
{
    [Authorize] 
    public class DashboardController : Controller
    {
        private readonly SamaraMarketContext _context;

        public DashboardController(SamaraMarketContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Estadísticas generales
            var totalEmprendedores = _context.Emprendedores.Count();
            var totalProductos = _context.Productos.Count();
            var totalEventos = _context.Eventos.Count();
            var totalStands = _context.Stands.Count();

            // Productos por tipo (para gráfica)
            var productosPorTipo = _context.Productos
                .GroupBy(p => p.TipoProducto!.NombreTipo)
                .Select(g => new { Tipo = g.Key, Cantidad = g.Count() })
                .ToList();

            // Disponibilidad de stands (para gráfica)
            var standsDisponibilidad = new
            {
                Disponibles = _context.Stands.Count(s => s.Disponible),
                Ocupados = _context.Stands.Count(s => !s.Disponible)
            };

            // Eventos próximos
            var eventosFuturos = _context.Eventos
                .Where(e => e.Fecha > DateTime.UtcNow)
                .OrderBy(e => e.Fecha)
                .Take(5)
                .Select(e => new { e.Nombre, Fecha = e.Fecha.ToString("dd/MM/yyyy") })
                .ToList();

            // Pasar datos a la vista
            var model = new
            {
                TotalEmprendedores = totalEmprendedores,
                TotalProductos = totalProductos,
                TotalEventos = totalEventos,
                TotalStands = totalStands,
                ProductosPorTipo = productosPorTipo,
                StandsDisponibilidad = standsDisponibilidad,
                EventosFuturos = eventosFuturos
            };

            return View(model);
        }
    }
}
