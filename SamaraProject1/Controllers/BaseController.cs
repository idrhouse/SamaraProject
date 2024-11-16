using Microsoft.AspNetCore.Mvc;
using SamaraProject1.Models;
using Microsoft.EntityFrameworkCore;

public class BaseController : Controller
{
    private readonly SamaraMarketContext _samaraMarketContext;

    public BaseController(SamaraMarketContext samaraMarketContext)
    {
        _samaraMarketContext = samaraMarketContext;
    }

    protected async Task SetNombreAdministradorAsync()
    {
        var usuarioId = User.Identity?.Name;
        string nombreAdministrador = "Administrador";

        if (!string.IsNullOrEmpty(usuarioId))
        {
            var administrador = await _samaraMarketContext.Administrador.FirstOrDefaultAsync(a => a.Correo == usuarioId);

            if (administrador != null)
            {
                nombreAdministrador = administrador.NombreAdministrador;
            }
        }

        ViewData["NombreAdministrador"] = nombreAdministrador;
    }
}
