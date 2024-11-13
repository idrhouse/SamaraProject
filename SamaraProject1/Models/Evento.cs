namespace SamaraProject1.Models
{
    public class Evento
    {
        public int IdEvento { get; set; }
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public DateTime Fecha { get; set; } = DateTime.UtcNow;
        public string? ImagenUrl { get; set; }
    }
}