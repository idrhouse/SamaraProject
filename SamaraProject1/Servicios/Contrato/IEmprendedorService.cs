using Microsoft.EntityFrameworkCore;
using SamaraProject1.Models;
namespace SamaraProject1.Servicios.Contrato
{
    public interface IEmprendedorService
    {
        Task<List<Emprendedor>> GetEmprendedorAsync();// <List<Emprendedor>>
        Task<Emprendedor> GetEmprendedorByIdAsync(int id);// <Emprendedor>
        Task<Emprendedor> SaveEmprendedorAsync(Emprendedor emprendedor);// <Emprendedor>
        Task<Emprendedor> EditarEmprendedorAsync(Emprendedor emprendedor);// <Emprendedor>
        Task<Emprendedor> EliminarEmprendedorAsync(Emprendedor emprendedor);// <Emprendedor>
    }
}
