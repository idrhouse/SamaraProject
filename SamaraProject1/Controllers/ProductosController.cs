using ClosedXML.Excel;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        // Método mejorado para obtener emprendedores por categoría (para AJAX)
        [HttpGet]
        public async Task<IActionResult> ObtenerEmprendedoresPorCategoria(int idCategoria)
        {
            if (idCategoria <= 0)
            {
                return Json(new List<object>());
            }

            var emprendedores = await _context.Emprendedores
                .Where(e => e.IdCategoria == idCategoria)
                .Select(e => new {
                    idEmprendedor = e.IdEmprendedor,
                    nombreEmprendedor = $"{e.NombreEmprendedor} {e.Apellidos} - {e.NombreNegocio}"
                })
                .ToListAsync();

            return Json(emprendedores);
        }

        // GET: Producto/Crear
        public async Task<IActionResult> Crear(int? idCategoria)
        {
            // Cargar tipos de productos y categorías
            var tipoProductos = await _context.TipoProducto.ToListAsync();
            var categorias = await _context.Categorias.ToListAsync();

            if (!tipoProductos.Any())
            {
                ModelState.AddModelError("", "No hay tipos de productos registrados.");
            }

            // Cargar emprendedores inicialmente (todos o filtrados por categoría si se proporciona)
            var emprendedores = await _context.Emprendedores
                .Where(e => idCategoria == null || e.IdCategoria == idCategoria)
                .ToListAsync();

            ViewBag.Emprendedores = emprendedores;
            ViewBag.TipoProductos = tipoProductos;
            ViewBag.Categorias = new SelectList(categorias, "IdCategoria", "NombreCategoria", idCategoria);

            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ObtenerImagen(int id)
        {
            var productos = _context.Productos.FirstOrDefault(e => e.IdProducto == id);

            if (productos == null || productos.ImagenDatos == null)
            {
                var rutaDefault = Path.Combine(_webHostEnvironment.WebRootPath, "imagenes/default-producto.png");
                var defaultImage = System.IO.File.ReadAllBytes(rutaDefault);
                return File(defaultImage, "image/jpeg");
            }

            return File(productos.ImagenDatos, "image/jpeg");
        }

        // POST: Producto/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Producto producto, int[] SelectedEmprendedores, IFormFile? imagen)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Manejo de la imagen
                    if (imagen != null && imagen.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await imagen.CopyToAsync(memoryStream);
                            producto.ImagenDatos = memoryStream.ToArray();
                        }
                    }

                    // Guardar el producto
                    _context.Add(producto);
                    await _context.SaveChangesAsync();

                    // Asociar emprendedores al producto
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
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Ocurrió un error al guardar el producto: " + ex.Message);
                }
            }

            // Si llegamos aquí, algo falló, volvemos a cargar los datos para el formulario
            var tipoProductos = await _context.TipoProducto.ToListAsync();
            var categorias = await _context.Categorias.ToListAsync();

            // Cargar emprendedores basados en la categoría seleccionada (si hay alguna)
            int? idCategoria = null;
            if (Request.Form.ContainsKey("IdCategoria") && int.TryParse(Request.Form["IdCategoria"], out int parsedId))
            {
                idCategoria = parsedId;
            }

            var emprendedores = await _context.Emprendedores
                .Where(e => idCategoria == null || e.IdCategoria == idCategoria)
                .ToListAsync();

            ViewBag.Emprendedores = emprendedores;
            ViewBag.TipoProductos = tipoProductos;
            ViewBag.Categorias = new SelectList(categorias, "IdCategoria", "NombreCategoria", idCategoria);

            return View(producto);
        }

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

            // Obtener la categoría del primer emprendedor asociado (si hay alguno)
            int? idCategoria = null;
            if (producto.ProductoEmprendedores != null && producto.ProductoEmprendedores.Any())
            {
                var primerEmprendedor = await _context.Emprendedores
                    .FirstOrDefaultAsync(e => e.IdEmprendedor == producto.ProductoEmprendedores.First().IdEmprendedor);

                if (primerEmprendedor != null)
                {
                    idCategoria = primerEmprendedor.IdCategoria;
                }
            }

            var categorias = await _context.Categorias.ToListAsync();
            ViewBag.Categorias = new SelectList(categorias, "IdCategoria", "NombreCategoria", idCategoria);
            ViewBag.Emprendedores = await _context.Emprendedores
                .Where(e => idCategoria == null || e.IdCategoria == idCategoria)
                .ToListAsync();
            ViewBag.TipoProductos = await _context.TipoProducto.ToListAsync();
            ViewBag.SelectedEmprendedores = producto.ProductoEmprendedores?.Select(pe => pe.IdEmprendedor).ToList() ?? new List<int>();

            return View(producto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, Producto producto, List<int> SelectedEmprendedores, IFormFile? imagen)
        {
            if (id != producto.IdProducto)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingProduct = await _context.Productos
                        .Include(p => p.ProductoEmprendedores)
                        .FirstOrDefaultAsync(p => p.IdProducto == id);

                    if (existingProduct == null)
                    {
                        return NotFound();
                    }

                    existingProduct.Nombre_Producto = producto.Nombre_Producto;
                    existingProduct.Descripcion = producto.Descripcion;
                    existingProduct.IdTipoProducto = producto.IdTipoProducto;

                    if (imagen != null && imagen.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await imagen.CopyToAsync(memoryStream);
                            existingProduct.ImagenDatos = memoryStream.ToArray();
                        }
                    }

                    _context.ProductoEmprendedores.RemoveRange(existingProduct.ProductoEmprendedores);

                    if (SelectedEmprendedores != null)
                    {
                        foreach (var emprendedorId in SelectedEmprendedores)
                        {
                            _context.ProductoEmprendedores.Add(new ProductoEmprendedor
                            {
                                IdProducto = existingProduct.IdProducto,
                                IdEmprendedor = emprendedorId
                            });
                        }
                    }

                    await _context.SaveChangesAsync();

                    TempData["Message"] = "Producto actualizado exitosamente.";
                    return RedirectToAction(nameof(Lista));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductoExists(producto.IdProducto))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Ocurrió un error al actualizar el producto: " + ex.Message);
                }
            }

            // Si llegamos aquí, algo falló, volvemos a cargar los datos para el formulario
            int? idCategoria = null;
            if (Request.Form.ContainsKey("IdCategoria") && int.TryParse(Request.Form["IdCategoria"], out int parsedId))
            {
                idCategoria = parsedId;
            }

            var categorias = await _context.Categorias.ToListAsync();
            ViewBag.Categorias = new SelectList(categorias, "IdCategoria", "NombreCategoria", idCategoria);
            ViewBag.Emprendedores = await _context.Emprendedores
                .Where(e => idCategoria == null || e.IdCategoria == idCategoria)
                .ToListAsync();
            ViewBag.TipoProductos = await _context.TipoProducto.ToListAsync();
            ViewBag.SelectedEmprendedores = SelectedEmprendedores;

            return View(producto);
        }

        public async Task<IActionResult> Eliminar(int id)
        {
            var producto = await _context.Productos
                .Include(p => p.ProductoEmprendedores)
                    .ThenInclude(pe => pe.Emprendedor)
                .Include(p => p.TipoProducto)
                .FirstOrDefaultAsync(m => m.IdProducto == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarEliminar(int id)
        {
            var producto = await _context.Productos
                .Include(p => p.ProductoEmprendedores)
                .FirstOrDefaultAsync(p => p.IdProducto == id);

            if (producto == null)
            {
                return NotFound();
            }

            _context.ProductoEmprendedores.RemoveRange(producto.ProductoEmprendedores);
            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Producto eliminado exitosamente.";
            return RedirectToAction(nameof(Lista));
        }

        private bool ProductoExists(int id)
        {
            return _context.Productos.Any(e => e.IdProducto == id);
        }


        // GET: Producto/GenerarReporte
        public IActionResult GenerarReporte()
        {
            return View();
        }

        // POST: Producto/GenerarReporteExcel
        [HttpPost]
        public async Task<IActionResult> GenerarReporteExcel(DateTime fechaInicio, DateTime fechaFin)
        {
            DateTime fechaInicioUtc = DateTime.SpecifyKind(fechaInicio.Date, DateTimeKind.Utc);
            DateTime fechaFinUtc = DateTime.SpecifyKind(fechaFin.Date.AddDays(1).AddTicks(-1), DateTimeKind.Utc);

            var productos = await _context.Productos
                .Include(p => p.TipoProducto)
                .Include(p => p.ProductoEmprendedores)
                .ThenInclude(pe => pe.Emprendedor)
                .Where(p => p.IdProducto > 0) // Modificar según el criterio de fecha si es necesario
                .ToListAsync();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Productos");
                worksheet.Cell(1, 1).Value = "ID";
                worksheet.Cell(1, 2).Value = "Nombre";
                worksheet.Cell(1, 3).Value = "Descripción";
                worksheet.Cell(1, 4).Value = "Tipo de Producto";
                worksheet.Cell(1, 5).Value = "Emprendedores";

                var headerRow = worksheet.Row(1);
                headerRow.Style.Font.Bold = true;
                headerRow.Style.Fill.BackgroundColor = XLColor.LightBlue;

                int row = 2;
                foreach (var producto in productos)
                {
                    worksheet.Cell(row, 1).Value = producto.IdProducto;
                    worksheet.Cell(row, 2).Value = producto.Nombre_Producto;
                    worksheet.Cell(row, 3).Value = producto.Descripcion;
                    worksheet.Cell(row, 4).Value = producto.TipoProducto?.NombreTipo ?? "Sin tipo";

                    string emprendedores = producto.ProductoEmprendedores != null && producto.ProductoEmprendedores.Any()
                        ? string.Join(", ", producto.ProductoEmprendedores.Select(pe => pe.Emprendedor.NombreEmprendedor))
                        : "Sin emprendedores";
                    worksheet.Cell(row, 5).Value = emprendedores;
                    row++;
                }

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Position = 0;
                    string fileName = $"Productos_{fechaInicio:yyyyMMdd}_a_{fechaFin:yyyyMMdd}.xlsx";
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }

        // POST: Producto/GenerarReportePDF
        [HttpPost]
        public async Task<IActionResult> GenerarReportePDF(DateTime fechaInicio, DateTime fechaFin)
        {
            DateTime fechaInicioUtc = DateTime.SpecifyKind(fechaInicio.Date, DateTimeKind.Utc);
            DateTime fechaFinUtc = DateTime.SpecifyKind(fechaFin.Date.AddDays(1).AddTicks(-1), DateTimeKind.Utc);

            var productos = await _context.Productos
                .Include(p => p.TipoProducto)
                .Include(p => p.ProductoEmprendedores)
                .ThenInclude(pe => pe.Emprendedor)
                .Where(p => p.IdProducto > 0)
                .ToListAsync();

            using (var memoryStream = new MemoryStream())
            {
                Document document = new Document(PageSize.A4, 25, 25, 30, 30);
                PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
                document.Open();

                Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18, BaseColor.BLUE);
                Paragraph title = new Paragraph($"Reporte de Productos", titleFont)
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingAfter = 20
                };
                document.Add(title);

                PdfPTable table = new PdfPTable(5) { WidthPercentage = 100 };
                table.SetWidths(new float[] { 0.5f, 1.5f, 2f, 1.5f, 2f });

                Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.WHITE);
                BaseColor headerBackground = new BaseColor(51, 122, 183);

                string[] headers = { "ID", "Nombre", "Descripción", "Tipo de Producto", "Emprendedores" };
                foreach (var header in headers)
                {
                    AddCellToTable(table, header, headerFont, headerBackground);
                }

                Font dataFont = FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.BLACK);
                BaseColor evenRowColor = new BaseColor(240, 240, 240);
                BaseColor oddRowColor = BaseColor.WHITE;

                int rowNum = 0;
                foreach (var producto in productos)
                {
                    BaseColor rowColor = (rowNum % 2 == 0) ? evenRowColor : oddRowColor;
                    rowNum++;

                    AddCellToTable(table, producto.IdProducto.ToString(), dataFont, rowColor);
                    AddCellToTable(table, producto.Nombre_Producto ?? "N/A", dataFont, rowColor);
                    AddCellToTable(table, producto.Descripcion ?? "N/A", dataFont, rowColor);
                    AddCellToTable(table, producto.TipoProducto?.NombreTipo ?? "Sin tipo", dataFont, rowColor);

                    string emprendedores = producto.ProductoEmprendedores != null && producto.ProductoEmprendedores.Any()
                        ? string.Join(", ", producto.ProductoEmprendedores.Select(pe => pe.Emprendedor.NombreEmprendedor))
                        : "Sin emprendedores";
                    AddCellToTable(table, emprendedores, dataFont, rowColor);
                }

                document.Add(table);
                document.Close();
                writer.Close();

                string fileName = $"Productos_{fechaInicio:yyyyMMdd}_a_{fechaFin:yyyyMMdd}.pdf";
                return File(memoryStream.ToArray(), "application/pdf", fileName);
            }
        }

        private void AddCellToTable(PdfPTable table, string text, Font font, BaseColor backgroundColor)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text, font))
            {
                BackgroundColor = backgroundColor,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                Padding = 5
            };
            table.AddCell(cell);
        }
    }
}