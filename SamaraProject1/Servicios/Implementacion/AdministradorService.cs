using Microsoft.EntityFrameworkCore;
using SamaraProject1.Models;
using SamaraProject1.Servicios.Contrato;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace SamaraProject1.Servicios.Implementacion
{
    public class AdministradorService : IAdministradorService
    {
        private readonly SamaraMarketContext _dbContext;
        private readonly ILogger<AdministradorService> _logger;

        public AdministradorService(SamaraMarketContext dbContext, ILogger<AdministradorService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Administrador> GetAdministrador(string correo, string clave)
        {
            return await _dbContext.Administrador
                .FirstOrDefaultAsync(x => x.Correo == correo && x.Clave == clave);
        }

        public async Task<Administrador> SaveAdministrador(Administrador modelo)
        {
            try
            {
                _dbContext.Administrador.Add(modelo);
                await _dbContext.SaveChangesAsync();
                return modelo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar el administrador: {NombreAdministrador}", modelo.NombreAdministrador);
                throw;
            }
        }

        public async Task<Administrador> EditarAdministrador(Administrador modelo)
        {
            try
            {
                var administrador = await _dbContext.Administrador.FindAsync(modelo.IdAdministrador);
                if (administrador != null)
                {
                    administrador.NombreAdministrador = modelo.NombreAdministrador;
                    administrador.Apellido = modelo.Apellido;
                    administrador.Correo = modelo.Correo;
                    if (!string.IsNullOrEmpty(modelo.Clave))
                    {
                        administrador.Clave = modelo.Clave;
                    }

                    _dbContext.Administrador.Update(administrador);
                    await _dbContext.SaveChangesAsync();
                }

                return administrador;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al editar el administrador: {IdAdministrador}", modelo.IdAdministrador);
                throw;
            }
        }

        public async Task<bool> EliminarAdministrador(int id)
        {
            try
            {
                var administrador = await _dbContext.Administrador.FindAsync(id);
                if (administrador != null)
                {
                    _dbContext.Administrador.Remove(administrador);
                    await _dbContext.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el administrador: {IdAdministrador}", id);
                throw;
            }
        }

        public async Task<Administrador> GetAdministradorPorCorreo(string correo)
        {
            return await _dbContext.Administrador
                .FirstOrDefaultAsync(a => a.Correo == correo);
        }

        public async Task<Administrador> GetAdministradorPorToken(string token)
        {
            return await _dbContext.Administrador
                .FirstOrDefaultAsync(a => a.TokenRecuperacion == token);
        }

        public async Task<bool> ActualizarAdministrador(Administrador administrador)
        {
            try
            {
                _dbContext.Administrador.Update(administrador);
                var result = await _dbContext.SaveChangesAsync();
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el administrador: {IdAdministrador}", administrador.IdAdministrador);
                throw;
            }
        }

        public async Task<List<Administrador>> GetAllAdministradores()
        {
            return await _dbContext.Administrador.ToListAsync();
        }

        public async Task<Administrador> GetAdministradorPorId(int id)
        {
            return await _dbContext.Administrador.FindAsync(id);
        }

        public async Task<bool> ExisteAdministradorConCorreo(string correo, int? idExcluir = null)
        {
            if (idExcluir.HasValue)
            {
                return await _dbContext.Administrador.AnyAsync(a => a.Correo == correo && a.IdAdministrador != idExcluir);
            }
            else
            {
                return await _dbContext.Administrador.AnyAsync(a => a.Correo == correo);
            }
        }
    }
}