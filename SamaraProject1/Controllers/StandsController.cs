using ClosedXML.Excel;
using iTextSharp.text.pdf;
using iTextSharp.text;
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

                // Corregido: el stand será NO disponible si tiene emprendedor, disponible si no tiene.
                stand.Disponible = stand.IdEmprendedor == null;

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

        // GET: Stands/GenerarReporte
        public IActionResult GenerarReporte()
        {
            return View();
        }

        // POST: Stands/GenerarReporteExcel
        [HttpPost]
        public async Task<IActionResult> GenerarReporteExcel(DateTime fechaInicio, DateTime fechaFin)
        {
            var stands = await _context.Stands
                .Include(s => s.Emprendedor)
                .Where(s => s.IdStand > 0) // Modificar según el criterio de fecha si es necesario
                .ToListAsync();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Stands");
                worksheet.Cell(1, 1).Value = "ID";
                worksheet.Cell(1, 2).Value = "Número de Stand";
                worksheet.Cell(1, 3).Value = "Descripción";
                worksheet.Cell(1, 4).Value = "Disponible";
                worksheet.Cell(1, 5).Value = "Emprendedor";

                var headerRow = worksheet.Row(1);
                headerRow.Style.Font.Bold = true;
                headerRow.Style.Fill.BackgroundColor = XLColor.LightBlue;

                int row = 2;
                foreach (var stand in stands)
                {
                    worksheet.Cell(row, 1).Value = stand.IdStand;
                    worksheet.Cell(row, 2).Value = stand.Numero_Stand;
                    worksheet.Cell(row, 3).Value = stand.Descripcion_Stand ?? "N/A";
                    worksheet.Cell(row, 4).Value = stand.Disponible ? "Sí" : "No";
                    worksheet.Cell(row, 5).Value = stand.Emprendedor?.NombreEmprendedor ?? "Sin emprendedor";
                    row++;
                }

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Position = 0;
                    string fileName = $"Stands_{fechaInicio:yyyyMMdd}_a_{fechaFin:yyyyMMdd}.xlsx";
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }

        // POST: Stands/GenerarReportePDF
        [HttpPost]
        public async Task<IActionResult> GenerarReportePDF(DateTime fechaInicio, DateTime fechaFin)
        {
            var stands = await _context.Stands
                .Include(s => s.Emprendedor)
                .Where(s => s.IdStand > 0)
                .ToListAsync();

            using (var memoryStream = new MemoryStream())
            {
                Document document = new Document(PageSize.A4, 25, 25, 30, 30);
                PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
                document.Open();

                Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18, BaseColor.BLUE);
                Paragraph title = new Paragraph("Reporte de Stands", titleFont)
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingAfter = 20
                };
                document.Add(title);

                PdfPTable table = new PdfPTable(5) { WidthPercentage = 100 };
                table.SetWidths(new float[] { 0.5f, 1.5f, 2f, 1f, 2f });

                Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.WHITE);
                BaseColor headerBackground = new BaseColor(51, 122, 183);

                string[] headers = { "ID", "Número de Stand", "Descripción", "Disponible", "Emprendedor" };
                foreach (var header in headers)
                {
                    AddCellToTable(table, header, headerFont, headerBackground);
                }

                Font dataFont = FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.BLACK);
                BaseColor evenRowColor = new BaseColor(240, 240, 240);
                BaseColor oddRowColor = BaseColor.WHITE;

                int rowNum = 0;
                foreach (var stand in stands)
                {
                    BaseColor rowColor = (rowNum % 2 == 0) ? evenRowColor : oddRowColor;
                    rowNum++;

                    AddCellToTable(table, stand.IdStand.ToString(), dataFont, rowColor);
                    AddCellToTable(table, stand.Numero_Stand.ToString(), dataFont, rowColor);
                    AddCellToTable(table, stand.Descripcion_Stand ?? "N/A", dataFont, rowColor);
                    AddCellToTable(table, stand.Disponible ? "Sí" : "No", dataFont, rowColor);
                    AddCellToTable(table, stand.Emprendedor?.NombreEmprendedor ?? "Sin emprendedor", dataFont, rowColor);
                }

                document.Add(table);
                document.Close();
                writer.Close();

                string fileName = $"Stands_{fechaInicio:yyyyMMdd}_a_{fechaFin:yyyyMMdd}.pdf";
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