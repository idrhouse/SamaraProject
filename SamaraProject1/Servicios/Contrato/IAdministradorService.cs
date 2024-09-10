using Microsoft.EntityFrameworkCore;
using SamaraProject1.Models;

namespace SamaraProject1.Servicios.Contrato
{
    public interface IAdministradorService
    {
        Task<Administrador> GetAdministrador(string correo, string clave);
        Task<Administrador> SaveAdministrador(Administrador modelo);
        Task<Administrador> EditarAdministrador(Administrador modelo);
        Task<Administrador> EliminarAdministrador(Administrador modelo);

    }
}
