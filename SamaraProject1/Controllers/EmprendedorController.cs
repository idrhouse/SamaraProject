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
using ClosedXML.Excel; // Para generar Excel
using iTextSharp.text; // Para generar PDF
using iTextSharp.text.pdf;
using System.Collections.Generic;

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
            var emprendedores = await _context.Emprendedores.Include(e => e.Stands).Include(e => e.Categoria).ToListAsync();
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

        // GET: Emprendedor/Detalles/5
        [AllowAnonymous]
        public async Task<IActionResult> Detalles(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var emprendedor = await _context.Emprendedores
                .Include(e => e.Categoria)
                .Include(e => e.Stands)
                .Include(e => e.Productos)
                    .ThenInclude(p => p.TipoProducto)
                .FirstOrDefaultAsync(m => m.IdEmprendedor == id);

            if (emprendedor == null)
            {
                return NotFound();
            }

            return View(emprendedor);
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
            // Asegurar que la fecha de creación esté en UTC
            emprendedor.FechaCreacion = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

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

            // Validar si la cédula ya existe
            var cedulaExistente = await _context.Emprendedores
                .FirstOrDefaultAsync(e => e.Cedula == emprendedor.Cedula);

            if (cedulaExistente != null)
            {
                ModelState.AddModelError("Cedula", "Ya existe un emprendedor con esta cédula.");
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
            var categorias = await _context.Categorias.ToListAsync();
            var categoriasSelectList = categorias.Select(c => new SelectListItem
            {
                Value = c.IdCategoria.ToString(),
                Text = c.NombreCategoria,
                Selected = (c.IdCategoria == emprendedor.IdCategoria)
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
                var categorias = await _context.Categorias.ToListAsync();
                var categoriasSelectList = categorias.Select(c => new SelectListItem
                {
                    Value = c.IdCategoria.ToString(),
                    Text = c.NombreCategoria,
                    Selected = (c.IdCategoria == emprendedor.IdCategoria)
                }).ToList();
                ViewBag.Categorias = categoriasSelectList;

                return View(emprendedor);
            }

            // Validar si la cédula ya existe en otro emprendedor
            var cedulaExistente = await _context.Emprendedores
                .FirstOrDefaultAsync(e => e.Cedula == emprendedor.Cedula && e.IdEmprendedor != id);

            if (cedulaExistente != null)
            {
                ModelState.AddModelError("Cedula", "Ya existe otro emprendedor con esta cédula.");
                var categorias = await _context.Categorias.ToListAsync();
                var categoriasSelectList = categorias.Select(c => new SelectListItem
                {
                    Value = c.IdCategoria.ToString(),
                    Text = c.NombreCategoria,
                    Selected = (c.IdCategoria == emprendedor.IdCategoria)
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
                // Corregido: Convertir a SelectListItem
                var categorias = await _context.Categorias.ToListAsync();
                var categoriasSelectList = categorias.Select(c => new SelectListItem
                {
                    Value = c.IdCategoria.ToString(),
                    Text = c.NombreCategoria,
                    Selected = (c.IdCategoria == emprendedor.IdCategoria)
                }).ToList();
                ViewBag.Categorias = categoriasSelectList;
                return View(emprendedor);
            }

            var emprendedorExistente = await _context.Emprendedores.FindAsync(id);

            if (emprendedorExistente == null)
            {
                return NotFound();
            }

            // Mantener la fecha de creación original pero asegurarse de que esté en UTC
            DateTime fechaCreacionOriginal = emprendedorExistente.FechaCreacion;
            if (fechaCreacionOriginal.Kind != DateTimeKind.Utc)
            {
                fechaCreacionOriginal = DateTime.SpecifyKind(fechaCreacionOriginal, DateTimeKind.Utc);
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
                // Mantener la imagen existente - obtenerla de la base de datos
                var emprendedorConImagen = await _context.Emprendedores
                    .AsNoTracking()
                    .Where(e => e.IdEmprendedor == id)
                    .Select(e => new { e.ImagenDatos })
                    .FirstOrDefaultAsync();

                if (emprendedorConImagen != null)
                {
                    emprendedorExistente.ImagenDatos = emprendedorConImagen.ImagenDatos;
                }
            }

            // Actualizar los datos del emprendedor existente
            emprendedorExistente.NombreEmprendedor = emprendedor.NombreEmprendedor;
            emprendedorExistente.Apellidos = emprendedor.Apellidos;
            emprendedorExistente.Correo = emprendedor.Correo;
            emprendedorExistente.DescripcionNegocio = emprendedor.DescripcionNegocio;
            emprendedorExistente.NombreNegocio = emprendedor.NombreNegocio;
            emprendedorExistente.Telefono = emprendedor.Telefono;
            emprendedorExistente.IdCategoria = emprendedor.IdCategoria;
            emprendedorExistente.Cedula = emprendedor.Cedula; // Actualizar la cédula
            emprendedorExistente.Categoria = categoriaExistente; // Asignar la categoría
            emprendedorExistente.FechaCreacion = fechaCreacionOriginal; // Restaurar la fecha de creación original en UTC

            try
            {
                _context.Update(emprendedorExistente);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Lista));
            }
            catch (Exception ex)
            {
                // Obtener el mensaje de la excepción interna si existe
                string errorMessage = ex.Message;
                Exception innerException = ex.InnerException;
                while (innerException != null)
                {
                    errorMessage += " - " + innerException.Message;
                    innerException = innerException.InnerException;
                }

                // Manejar errores de base de datos
                ModelState.AddModelError("", "Ocurrió un error al actualizar el emprendedor: " + errorMessage);
                // Corregido: Convertir a SelectListItem
                var categorias = await _context.Categorias.ToListAsync();
                var categoriasSelectList = categorias.Select(c => new SelectListItem
                {
                    Value = c.IdCategoria.ToString(),
                    Text = c.NombreCategoria,
                    Selected = (c.IdCategoria == emprendedor.IdCategoria)
                }).ToList();
                ViewBag.Categorias = categoriasSelectList;
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

        // GET: Emprendedor/GenerarReporte
        public IActionResult GenerarReporte()
        {
            return View();
        }

        // POST: Emprendedor/GenerarReporteExcel
        [HttpPost]
        public async Task<IActionResult> GenerarReporteExcel(DateTime fechaInicio, DateTime fechaFin)
        {
            // Obtener emprendedores filtrados por fecha de creación
            // Convertir fechas a UTC para la consulta
            DateTime fechaInicioUtc = DateTime.SpecifyKind(fechaInicio.Date, DateTimeKind.Utc);
            DateTime fechaFinUtc = DateTime.SpecifyKind(fechaFin.Date.AddDays(1).AddTicks(-1), DateTimeKind.Utc);

            var emprendedores = await _context.Emprendedores
                .Include(e => e.Categoria)
                .Include(e => e.Stands)
                .Where(e => e.FechaCreacion >= fechaInicioUtc && e.FechaCreacion <= fechaFinUtc)
                .ToListAsync();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Emprendedores");

                // Encabezados
                worksheet.Cell(1, 1).Value = "ID";
                worksheet.Cell(1, 2).Value = "Cédula"; // Nuevo encabezado
                worksheet.Cell(1, 3).Value = "Nombre";
                worksheet.Cell(1, 4).Value = "Apellidos";
                worksheet.Cell(1, 5).Value = "Negocio";
                worksheet.Cell(1, 6).Value = "Teléfono";
                worksheet.Cell(1, 7).Value = "Correo";
                worksheet.Cell(1, 8).Value = "Categoría";
                worksheet.Cell(1, 9).Value = "Stands";

                // Estilo para encabezados
                var headerRow = worksheet.Row(1);
                headerRow.Style.Font.Bold = true;
                headerRow.Style.Fill.BackgroundColor = XLColor.LightBlue;

                // Datos
                int row = 2;
                foreach (var emprendedor in emprendedores)
                {
                    worksheet.Cell(row, 1).Value = emprendedor.IdEmprendedor;
                    worksheet.Cell(row, 2).Value = emprendedor.Cedula; // Nuevo campo
                    worksheet.Cell(row, 3).Value = emprendedor.NombreEmprendedor;
                    worksheet.Cell(row, 4).Value = emprendedor.Apellidos;
                    worksheet.Cell(row, 5).Value = emprendedor.NombreNegocio;
                    worksheet.Cell(row, 6).Value = emprendedor.Telefono;
                    worksheet.Cell(row, 7).Value = emprendedor.Correo;
                    worksheet.Cell(row, 8).Value = emprendedor.Categoria?.NombreCategoria ?? "Sin categoría";

                    string stands = emprendedor.Stands != null && emprendedor.Stands.Any()
                        ? string.Join(", ", emprendedor.Stands.Select(s => s.Numero_Stand))
                        : "Sin asignar";
                    worksheet.Cell(row, 9).Value = stands;

                    row++;
                }

                // Ajustar columnas al contenido
                worksheet.Columns().AdjustToContents();

                // Preparar el stream para descargar
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Position = 0;

                    string fileName = $"Emprendedores_{fechaInicio:yyyyMMdd}_a_{fechaFin:yyyyMMdd}.xlsx";
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }

        // POST: Emprendedor/GenerarReportePDF
        [HttpPost]
        public async Task<IActionResult> GenerarReportePDF(DateTime fechaInicio, DateTime fechaFin)
        {
            // Obtener emprendedores filtrados por fecha de creación
            // Convertir fechas a UTC para la consulta
            DateTime fechaInicioUtc = DateTime.SpecifyKind(fechaInicio.Date, DateTimeKind.Utc);
            DateTime fechaFinUtc = DateTime.SpecifyKind(fechaFin.Date.AddDays(1).AddTicks(-1), DateTimeKind.Utc);

            var emprendedores = await _context.Emprendedores
                .Include(e => e.Categoria)
                .Include(e => e.Stands)
                .Where(e => e.FechaCreacion >= fechaInicioUtc && e.FechaCreacion <= fechaFinUtc)
                .ToListAsync();

            using (var memoryStream = new MemoryStream())
            {
                Document document = new Document(PageSize.A4, 25, 25, 30, 30);
                PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
                document.Open();

                // Agregar título
                Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18, BaseColor.BLUE);
                Paragraph title = new Paragraph($"Reporte de Emprendedores ({fechaInicio:dd/MM/yyyy} - {fechaFin:dd/MM/yyyy})", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                title.SpacingAfter = 20;
                document.Add(title);

                // Crear tabla
                PdfPTable table = new PdfPTable(8); // Aumentar a 8 columnas
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 0.5f, 1f, 1.5f, 1.5f, 1.5f, 1f, 1.5f, 1.5f }); // Ajustar anchos

                // Estilo para encabezados
                Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.WHITE);
                BaseColor headerBackground = new BaseColor(51, 122, 183);

                // Encabezados
                AddCellToTable(table, "ID", headerFont, headerBackground);
                AddCellToTable(table, "Cédula", headerFont, headerBackground); // Nuevo encabezado
                AddCellToTable(table, "Nombre", headerFont, headerBackground);
                AddCellToTable(table, "Apellidos", headerFont, headerBackground);
                AddCellToTable(table, "Negocio", headerFont, headerBackground);
                AddCellToTable(table, "Teléfono", headerFont, headerBackground);
                AddCellToTable(table, "Correo", headerFont, headerBackground);
                AddCellToTable(table, "Categoría", headerFont, headerBackground);

                // Estilo para datos
                Font dataFont = FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.BLACK);
                BaseColor evenRowColor = new BaseColor(240, 240, 240);
                BaseColor oddRowColor = BaseColor.WHITE;

                // Datos
                int rowNum = 0;
                foreach (var emprendedor in emprendedores)
                {
                    BaseColor rowColor = (rowNum % 2 == 0) ? evenRowColor : oddRowColor;
                    rowNum++;

                    AddCellToTable(table, emprendedor.IdEmprendedor.ToString(), dataFont, rowColor);
                    AddCellToTable(table, emprendedor.Cedula, dataFont, rowColor); // Nuevo campo
                    AddCellToTable(table, emprendedor.NombreEmprendedor, dataFont, rowColor);
                    AddCellToTable(table, emprendedor.Apellidos, dataFont, rowColor);
                    AddCellToTable(table, emprendedor.NombreNegocio, dataFont, rowColor);
                    AddCellToTable(table, emprendedor.Telefono ?? "N/A", dataFont, rowColor);
                    AddCellToTable(table, emprendedor.Correo ?? "N/A", dataFont, rowColor);
                    AddCellToTable(table, emprendedor.Categoria?.NombreCategoria ?? "Sin categoría", dataFont, rowColor);
                }

                document.Add(table);

                // Agregar información adicional
                Paragraph info = new Paragraph($"Total de emprendedores: {emprendedores.Count}",
                    FontFactory.GetFont(FontFactory.HELVETICA, 12, BaseColor.DARK_GRAY));
                info.SpacingBefore = 15;
                document.Add(info);

                // Agregar fecha de generación
                Paragraph dateInfo = new Paragraph($"Reporte generado el {DateTime.Now:dd/MM/yyyy HH:mm:ss}",
                    FontFactory.GetFont(FontFactory.HELVETICA, 10, Font.ITALIC, BaseColor.GRAY));
                dateInfo.Alignment = Element.ALIGN_RIGHT;
                dateInfo.SpacingBefore = 20;
                document.Add(dateInfo);

                document.Close();
                writer.Close();

                string fileName = $"Emprendedores_{fechaInicio:yyyyMMdd}_a_{fechaFin:yyyyMMdd}.pdf";
                return File(memoryStream.ToArray(), "application/pdf", fileName);
            }
        }

        private void AddCellToTable(PdfPTable table, string text, Font font, BaseColor backgroundColor)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text, font));
            cell.BackgroundColor = backgroundColor;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Padding = 5;
            table.AddCell(cell);
        }
    }
}