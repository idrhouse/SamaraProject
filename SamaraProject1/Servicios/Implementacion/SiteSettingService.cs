using Microsoft.EntityFrameworkCore;
using SamaraProject1.Models;
using SamaraProject1.Servicios.Contrato;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SamaraProject1.Servicios
{
    public class SiteSettingService : ISiteSettingService
    {
        private readonly SamaraMarketContext _context;

        public SiteSettingService(SamaraMarketContext context)
        {
            _context = context;
        }

        public async Task<List<SiteSetting>> GetAllSettingsAsync()
        {
            return await _context.SiteSettings.OrderBy(s => s.SettingKey).ToListAsync();
        }

        public async Task<SiteSetting> GetSettingByKeyAsync(string key)
        {
            return await _context.SiteSettings.FirstOrDefaultAsync(s => s.SettingKey == key);
        }

        public async Task<string> GetSettingValueAsync(string key, string defaultValue = "")
        {
            var setting = await _context.SiteSettings.FirstOrDefaultAsync(s => s.SettingKey == key);
            return setting?.SettingValue ?? defaultValue;
        }

        public async Task<bool> UpdateSettingAsync(string key, string value, string updatedBy)
        {
            try
            {
                var setting = await _context.SiteSettings.FirstOrDefaultAsync(s => s.SettingKey == key);

                if (setting == null)
                {
                    // Si no existe, crear uno nuevo
                    setting = new SiteSetting
                    {
                        SettingKey = key,
                        SettingValue = value,
                        Description = "Número de SINPE Móvil para donaciones",
                        LastUpdated = DateTime.Now,
                        UpdatedBy = updatedBy
                    };
                    _context.SiteSettings.Add(setting);
                }
                else
                {
                    // Actualizar solo el valor y los campos de auditoría
                    setting.SettingValue = value;
                    setting.LastUpdated = DateTime.Now;
                    setting.UpdatedBy = updatedBy;
                }

                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                // Registrar el error para depuración
                Console.WriteLine($"Error en UpdateSettingAsync: {ex.ToString()}");
                throw; // Re-lanzar la excepción para que el controlador pueda manejarla
            }
        }
    }
}
