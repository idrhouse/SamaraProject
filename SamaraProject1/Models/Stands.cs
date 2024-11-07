namespace SamaraProject1.Models
{
    public class Stands
    {
        public int IdStand { get; set; }
        public int Numero_Stand { get; set; }
        public string Descripcion_Stand { get; set; }
        public int IdEmprendedor { get; set; }
        public string? ImagenUrl { get; set; }
        public virtual Emprendedor Emprendedor { get; set; }
    }
}