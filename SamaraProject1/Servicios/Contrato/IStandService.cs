using System.Collections.Generic;
using System.Threading.Tasks;
using SamaraProject1.Models;

namespace SamaraProject1.Servicios.Contrato
{
    public interface IStandService
    {
        Task<List<Stands>> GetStandsAsync();
        Task<Stands> GetStandByIdAsync(int id);
        Task<Stands> SaveStandAsync(Stands stand);
        Task<Stands> EditarStandAsync(Stands stand);
        Task<Stands> EliminarStandAsync(Stands stand);
    }
}