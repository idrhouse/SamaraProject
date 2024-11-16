namespace SamaraProject1.Models
{
    public class ProductoEmprendedor
    {
        public int IdProducto { get; set; }
        public Producto? Producto { get; set; }

        public int IdEmprendedor { get; set; }
        public Emprendedor? Emprendedor { get; set; }
    }

}