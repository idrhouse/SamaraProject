using System.ComponentModel.DataAnnotations;

namespace SamaraProject1.Models
{
    public class Evento
    {
        public int IdEvento { get; set; }
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public DateTime Fecha { get; set; } = DateTime.UtcNow;
        public string? ImagenUrl { get; set; }

        [Required(ErrorMessage = "La hora de inicio es obligatoria")]
        [Display(Name = "Hora de inicio")]
        public TimeSpan HoraInicio { get; set; }

        [Required(ErrorMessage = "La hora de fin es obligatoria")]
        [Display(Name = "Hora de inicio")]
        public TimeSpan HoraFin { get; set; }
    }
}