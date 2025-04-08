using Microsoft.AspNetCore.Mvc;
using SamaraProject1.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Authentication;

public class BaseController : Controller
{
    private readonly SamaraMarketContext _samaraMarketContext;

    public BaseController(SamaraMarketContext samaraMarketContext)
    {
        _samaraMarketContext = samaraMarketContext;
    }

    protected async Task<bool> ValidarSesionUsuarioAsync()
    {
        var correo = User.Identity?.Name;

        if (string.IsNullOrEmpty(correo))
            return false;

        var existe = await _samaraMarketContext.Administrador.AnyAsync(a => a.Correo == correo);
        return existe;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        base.OnActionExecuting(context);

        var correo = User.Identity?.Name;

        if (!string.IsNullOrEmpty(correo))
        {
            var existe = _samaraMarketContext.Administrador.Any(a => a.Correo == correo);

            if (!existe)
            {
                context.Result = new RedirectToActionResult("CerrarSesion", "Acceso", null);
            }

        }
    }


}

