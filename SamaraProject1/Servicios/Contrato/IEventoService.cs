using System.Collections.Generic;
using System.Threading.Tasks;
using SamaraProject1.Models;

namespace SamaraProject1.Servicios.Contrato
{
    public interface IEventoService
    {
        Task<List<Evento>> GetEventosAsync(); 
        Task<Evento> GetEventoByIdAsync(int id); 
        Task<Evento> SaveEventoAsync(Evento evento); 
        Task<Evento> EditarEventoAsync(Evento evento); 
        Task<Evento> EliminarEventoAsync(int id); 
    }
}