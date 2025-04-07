using System;
using System.ComponentModel.DataAnnotations;

namespace SamaraProject1.Models
{
    public class Testimonio
    {
        public int IdTestimonio { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "Formato de correo electrónico inválido.")]
        public string Correo { get; set; }

        [Required(ErrorMessage = "El contenido del testimonio es obligatorio.")]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "El testimonio debe tener entre 10 y 1000 caracteres.")]
        public string Contenido { get; set; }

        [StringLength(100, ErrorMessage = "El producto favorito no puede exceder los 100 caracteres.")]
        public string? ProductoFavorito { get; set; }

        [Range(1, 5, ErrorMessage = "La calificación debe estar entre 1 y 5.")]
        public int Calificacion { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        public bool Aprobado { get; set; } = false;
    }
}