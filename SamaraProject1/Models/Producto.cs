namespace SamaraProject1.Models
{
    public class Producto
    {
        public int IdProducto { get; set; }
        public string? Nombre_Producto { get; set; }
        public string? Descripcion { get; set; }
        public int IdEmprendedor { get; set; }

        public virtual Emprendedor? Emprendedor { get; set; }

    }
}