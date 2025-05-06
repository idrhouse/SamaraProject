using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SamaraProject1.Models;
using SamaraProject1.Servicios.Contrato;
using System;
using System.Threading.Tasks;

namespace SamaraProject1.Controllers
{
    [Authorize] // Permitir acceso a cualquier usuario autenticado durante pruebas
    public class SiteSettingsController : Controller
    {
        private readonly ISiteSettingService _siteSettingService;
        private readonly SamaraMarketContext _context;

        public SiteSettingsController(ISiteSettingService siteSettingService, SamaraMarketContext context)
        {
            _siteSettingService = siteSettingService;
            _context = context;
        }

        // GET: SiteSettings
        public async Task<IActionResult> Index()
        {
            var settings = await _siteSettingService.GetAllSettingsAsync();
            return View(settings);
        }

        // GET: SiteSettings/Edit/SinpeMovilNumber
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var setting = await _context.SiteSettings.FirstOrDefaultAsync(s => s.SettingKey == id);
            if (setting == null)
            {
                return NotFound();
            }

            return View(setting);
        }

        // POST: SiteSettings/Edit/SinpeMovilNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, string SettingValue)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            try
            {
                // Buscar por SettingKey
                var setting = await _context.SiteSettings.FirstOrDefaultAsync(s => s.SettingKey == id);
                if (setting == null)
                {
                    return NotFound();
                }

                // Actualizar solo el valor y los campos de auditoría
                setting.SettingValue = SettingValue;

                // Importante: Convertir DateTime a UTC para PostgreSQL
                setting.LastUpdated = DateTime.UtcNow; // Usar UtcNow en lugar de Now
                setting.UpdatedBy = User.Identity.Name ?? "Sistema";

                // Guardar los cambios
                await _context.SaveChangesAsync();

                TempData["Success"] = "Configuración actualizada correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Registrar el error completo para depuración
                Console.WriteLine($"Error al guardar: {ex.ToString()}");

                // Agregar el mensaje de error para mostrar al usuario
                ModelState.AddModelError("", $"Error al guardar: {ex.Message}");

                // Si hay una excepción interna, mostrarla también
                if (ex.InnerException != null)
                {
                    ModelState.AddModelError("", $"Detalle: {ex.InnerException.Message}");
                }
            }

            // Si llegamos aquí, algo falló, volver a mostrar el formulario
            var settingToDisplay = await _context.SiteSettings.FirstOrDefaultAsync(s => s.SettingKey == id);
            if (settingToDisplay == null)
            {
                settingToDisplay = new SiteSetting
                {
                    SettingKey = id,
                    SettingValue = SettingValue,
                    Description = "Número de SINPE Móvil para donaciones",
                    LastUpdated = DateTime.UtcNow, // Usar UtcNow aquí también
                    UpdatedBy = User.Identity.Name ?? "Sistema"
                };
            }
            else
            {
                settingToDisplay.SettingValue = SettingValue;
            }

            return View(settingToDisplay);
        }

        // API para obtener configuraciones (usado por el frontend)
        [HttpGet]
        [AllowAnonymous]
        [Route("api/settings/{key}")]
        public async Task<IActionResult> GetSetting(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return BadRequest("Se requiere una clave de configuración");
            }

            var setting = await _context.SiteSettings.FirstOrDefaultAsync(s => s.SettingKey == key);
            var value = setting?.SettingValue ?? "";

            return Json(new { key, value });
        }
    }
}