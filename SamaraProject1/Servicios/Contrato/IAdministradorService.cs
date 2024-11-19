using SamaraProject1.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SamaraProject1.Servicios.Contrato
{
    public interface IAdministradorService
    {
        Task<Administrador> GetAdministrador(string correo, string clave);
        Task<Administrador> SaveAdministrador(Administrador modelo);
        Task<Administrador> EditarAdministrador(Administrador modelo);
        Task<bool> EliminarAdministrador(int id);
        Task<Administrador> GetAdministradorPorCorreo(string correo);
        Task<Administrador> GetAdministradorPorToken(string token);
        Task<bool> ActualizarAdministrador(Administrador administrador);
        Task<List<Administrador>> GetAllAdministradores();
        Task<Administrador> GetAdministradorPorId(int id);
        Task<bool> ExisteAdministradorConCorreo(string correo, int? idExcluir = null);
    }
}