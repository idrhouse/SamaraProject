using Microsoft.EntityFrameworkCore;
using SamaraProject1.Models;
using SamaraProject1.Servicios.Contrato;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SamaraProject1.Servicios.Implementacion
{
    public class EventoService : IEventoService
    {
        private readonly SamaraMarketContext _dbcontext;

        public EventoService(SamaraMarketContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<List<Evento>> GetEventosAsync()
        {
            return await _dbcontext.Eventos.ToListAsync(); // Obtener todos los eventos
        }

        public async Task<Evento> GetEventoByIdAsync(int id)
        {
            return await _dbcontext.Eventos.FirstOrDefaultAsync(e => e.IdEvento == id); // Obtener un evento por ID
        }

        public async Task<Evento> SaveEventoAsync(Evento evento)
        {
            await _dbcontext.Eventos.AddAsync(evento); // Agregar un nuevo evento
            await _dbcontext.SaveChangesAsync(); // Guardar cambios
            return evento;
        }

        public async Task<Evento> EditarEventoAsync(Evento evento)
        {
            _dbcontext.Eventos.Update(evento); // Actualizar el evento
            await _dbcontext.SaveChangesAsync(); // Guardar cambios
            return evento;
        }

        public async Task<Evento> EliminarEventoAsync(int id)
        {
            var evento = await GetEventoByIdAsync(id); // Obtener el evento por ID
            if (evento != null)
            {
                _dbcontext.Eventos.Remove(evento); // Eliminar el evento
                await _dbcontext.SaveChangesAsync(); // Guardar cambios
            }
            return evento;
        }
    }
}