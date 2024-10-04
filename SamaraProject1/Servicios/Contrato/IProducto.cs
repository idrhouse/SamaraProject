
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using SamaraProject1.Models;

    namespace SamaraProject1.Servicios.Contrato
    {
        public interface IProductoService
        {
            Task<List<Producto>> GetProductosAsync();
            Task<Producto> GetProductoByIdAsync(int id);
            Task<Producto> SaveProductoAsync(Producto producto);
            Task<Producto> EditarProductoAsync(Producto producto);
            Task<Producto> EliminarProductoAsync(Producto producto);
        }
    }

