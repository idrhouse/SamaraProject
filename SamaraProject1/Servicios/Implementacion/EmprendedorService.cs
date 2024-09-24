using Microsoft.EntityFrameworkCore;
using SamaraProject1.Models;
using SamaraProject1.Servicios.Contrato;

namespace SamaraProject1.Servicios.Implementacion
{
    public class EmprendedorService : IEmprendedorService
    {
        private readonly SamaraMarketContext _dbcontext;

        public EmprendedorService(SamaraMarketContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<List<Emprendedor>> GetEmprendedorAsync()
        {
            return await _dbcontext.Emprendedores.Include(e => e.Administrador).ToListAsync();
        }

        public async Task<Emprendedor> GetEmprendedorByIdAsync(int id)
        {
            return await _dbcontext.Emprendedores.Include(e => e.Administrador).FirstOrDefaultAsync(e => e.IdEmprendedor == id);
        }

        public async Task<Emprendedor> SaveEmprendedorAsync(Emprendedor emprendedor)
        {
            _dbcontext.Emprendedores.Add(emprendedor);
            await _dbcontext.SaveChangesAsync();
            return emprendedor;
        }

        public async Task<Emprendedor> EditarEmprendedorAsync(Emprendedor emprendedor)
        {
            _dbcontext.Emprendedores.Update(emprendedor);
            await _dbcontext.SaveChangesAsync();
            return emprendedor;
        }

        public async Task<Emprendedor> EliminarEmprendedorAsync(Emprendedor emprendedor)
        {
            _dbcontext.Emprendedores.Remove(emprendedor);
            await _dbcontext.SaveChangesAsync();
            return emprendedor;
        }
    }
}