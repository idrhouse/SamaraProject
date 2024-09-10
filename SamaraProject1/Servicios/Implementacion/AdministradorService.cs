using Microsoft.EntityFrameworkCore;
using SamaraProject1.Models;
using SamaraProject1.Servicios.Contrato;

namespace SamaraProject1.Servicios.Implementacion
{
    public class AdministradorService : IAdministradorService
    {
        private readonly SamaraMarketContext _dbcontext;
        public AdministradorService(SamaraMarketContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

   
        public async Task<Administrador> GetAdministrador(string correo, string clave)
        {
            Administrador administrador_encontrado = await _dbcontext.Administrador.Where(x => x.Correo == correo && x.Clave == clave).FirstOrDefaultAsync();
            return administrador_encontrado;
        }

        public async Task<Administrador> SaveAdministrador(Administrador modelo)
        {
            _dbcontext.Administrador.Add(modelo);
            await _dbcontext.SaveChangesAsync();
            return modelo;
        }
        public Task<Administrador> EditarAdministrador(Administrador modelo)
        {
            throw new NotImplementedException();
        }

        public Task<Administrador> EliminarAdministrador(Administrador modelo)
        {
            throw new NotImplementedException();
        }

    }
}


