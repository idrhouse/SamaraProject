using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace SamaraProject1.Models
{
    public partial class Emprendedor
    {
        public int IdEmprendedor { get; set; }

        // Validación: Nombre no puede ser nulo y debe tener más de 3 caracteres
        [Required(ErrorMessage = "El nombre del emprendedor es obligatorio.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre del emprendedor debe tener al menos 3 caracteres.")]
        public string NombreEmprendedor { get; set; }

        // Validación: Apellidos no pueden ser nulos y deben tener más de 3 caracteres
        [Required(ErrorMessage = "Los apellidos son obligatorios.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Los apellidos deben tener al menos 3 caracteres.")]
        public string Apellidos { get; set; }

        // Validación: Nombre del negocio no puede ser nulo y debe tener más de 3 caracteres
        [Required(ErrorMessage = "El nombre del negocio es obligatorio.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre del negocio debe tener al menos 3 caracteres.")]
        public string NombreNegocio { get; set; }

        // Validación: Descripción del negocio, opcional, pero si se ingresa, debe tener más de 3 caracteres
        [StringLength(500, MinimumLength = 3, ErrorMessage = "La descripción del negocio debe tener al menos 3 caracteres.")]
        public string? DescripcionNegocio { get; set; }

        // Validación: Teléfono debe tener al menos 8 caracteres numéricos
        [StringLength(15, MinimumLength = 8, ErrorMessage = "El teléfono debe tener al menos 8 caracteres.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "El teléfono solo debe contener números.")]
        public string? Telefono { get; set; }

        // Validación: Correo debe ser único (no nulo si se ingresa) y debe cumplir con el formato estándar de correo
        [EmailAddress(ErrorMessage = "Correo electrónico no válido.")]
        public string? Correo { get; set; }

        [Display(Name = "Imagen del Emprendedor")]
        public byte[]? ImagenDatos { get; set; }

        // Fecha de creación para filtrar reportes
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public int IdCategoria { get; set; }
        public virtual Categoria? Categoria { get; set; }

        public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();

        public virtual ICollection<Stands> Stands { get; set; } = new List<Stands>();

        public virtual ICollection<ProductoEmprendedor>? ProductoEmprendedores { get; set; }
    }
}
