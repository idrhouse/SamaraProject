namespace SamaraProject1.Models
{
    public class TipoProducto
    {
        public int IdTipoProducto { get; set; }
        public string? NombreTipo { get; set; }

        public ICollection<Producto>? Productos { get; set; }  // Relación 1 a muchos
    }

}