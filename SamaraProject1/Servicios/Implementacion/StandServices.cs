using Microsoft.EntityFrameworkCore;
using SamaraProject1.Models;
using SamaraProject1.Servicios.Contrato;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SamaraProject1.Servicios.Implementacion
{
    public class StandService : IStandService
    {
        private readonly SamaraMarketContext _dbcontext;

        public StandService(SamaraMarketContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<List<Stands>> GetStandsAsync()
        {
            return await _dbcontext.Stands.Include(s => s.Emprendedor).ToListAsync();
        }

        public async Task<Stands> GetStandByIdAsync(int id)
        {
            return await _dbcontext.Stands.Include(s => s.Emprendedor).FirstOrDefaultAsync(s => s.IdStand == id);
        }

        public async Task<Stands> SaveStandAsync(Stands stand)
        {
            _dbcontext.Stands.Add(stand);
            await _dbcontext.SaveChangesAsync();
            return stand;
        }

        public async Task<Stands> EditarStandAsync(Stands stand)
        {
            _dbcontext.Stands.Update(stand);
            await _dbcontext.SaveChangesAsync();
            return stand;
        }

        public async Task<Stands> EliminarStandAsync(Stands stand)
        {
            _dbcontext.Stands.Remove(stand);
            await _dbcontext.SaveChangesAsync();
            return stand;
        }
    }
}