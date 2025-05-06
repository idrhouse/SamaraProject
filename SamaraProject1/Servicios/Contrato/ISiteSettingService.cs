using SamaraProject1.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SamaraProject1.Servicios.Contrato
{
    public interface ISiteSettingService
    {
        Task<List<SiteSetting>> GetAllSettingsAsync();
        Task<SiteSetting> GetSettingByKeyAsync(string key);
        Task<bool> UpdateSettingAsync(string key, string value, string updatedBy);
        Task<string> GetSettingValueAsync(string key, string defaultValue = "");
    }
}
