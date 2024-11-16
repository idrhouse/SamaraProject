using Microsoft.EntityFrameworkCore;
using SamaraProject1.Models;
using SamaraProject1.Servicios.Contrato;

namespace SamaraProject1.Servicios.Implementacion
{
    public class ProductoService : IProductoService
    {
        private readonly SamaraMarketContext _dbcontext;

        public ProductoService(SamaraMarketContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<List<Producto>> GetProductosAsync()
        {
            return await _dbcontext.Productos
                .Include(p => p.ProductoEmprendedores)
                    .ThenInclude(pe => pe.Emprendedor)
                .Include(p => p.TipoProducto)
                .ToListAsync();
        }

        public async Task<Producto> GetProductoByIdAsync(int id)
        {
            return await _dbcontext.Productos
                .Include(p => p.ProductoEmprendedores)
                    .ThenInclude(pe => pe.Emprendedor)
                .Include(p => p.TipoProducto)
                .FirstOrDefaultAsync(p => p.IdProducto == id);
        }

        public async Task<Producto> SaveProductoAsync(Producto producto)
        {
            _dbcontext.Productos.Add(producto);
            await _dbcontext.SaveChangesAsync();
            return producto;
        }

        public async Task<Producto> EditarProductoAsync(Producto producto)
        {
            _dbcontext.Productos.Update(producto);
            await _dbcontext.SaveChangesAsync();
            return producto;
        }

        public async Task<Producto> EliminarProductoAsync(Producto producto)
        {
            _dbcontext.Productos.Remove(producto);
            await _dbcontext.SaveChangesAsync();
            return producto;
        }
    }
}