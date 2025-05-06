using Microsoft.AspNetCore.Mvc;
using SamaraProject1.Servicios.Contrato;
using System.Threading.Tasks;

namespace SamaraProject1.Controllers
{
    public class DonacionController : Controller
    {
        private readonly ISiteSettingService _siteSettingService;

        public DonacionController(ISiteSettingService siteSettingService)
        {
            _siteSettingService = siteSettingService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("api/donacion/sinpe-number")]
        public async Task<IActionResult> GetSinpeNumber()
        {
            var number = await _siteSettingService.GetSettingValueAsync("SinpeMovilNumber", "88630334");
            return Json(new { number });
        }
    }
}
