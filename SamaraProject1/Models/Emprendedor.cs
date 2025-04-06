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
        [StringLength(1000, MinimumLength = 3, ErrorMessage = "La descripción del negocio debe tener al menos 3 caracteres.")]
        public string? DescripcionNegocio { get; set; }

        // Validación: Teléfono debe tener al menos 8 caracteres numéricos
        [StringLength(15, MinimumLength = 8, ErrorMessage = "El teléfono debe tener al menos 8 caracteres.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "El teléfono solo debe contener números.")]
        public string? Telefono { get; set; }

        // Validación: Cédula debe ser única y tener formato válido
        [Required(ErrorMessage = "El número de cédula es obligatorio.")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "El número de cédula debe tener entre 5 y 20 caracteres.")]
        [RegularExpression(@"^[0-9\-]+$", ErrorMessage = "La cédula solo debe contener números y guiones.")]
        [Display(Name = "Número de Cédula")]
        public string Cedula { get; set; }

        // Validación: Correo debe ser único (no nulo si se ingresa) y debe cumplir con el formato estándar de correo
        [EmailAddress(ErrorMessage = "Correo electrónico no válido.")]
        public string? Correo { get; set; }

        [Display(Name = "Imagen del Emprendedor")]
        public byte[]? ImagenDatos { get; set; }

        // Fecha de creación para filtrar reportes - Siempre en UTC
        private DateTime _fechaCreacion = DateTime.UtcNow;

        public DateTime FechaCreacion
        {
            get => _fechaCreacion;
            set => _fechaCreacion = value.Kind == DateTimeKind.Unspecified
                ? DateTime.SpecifyKind(value, DateTimeKind.Utc)
                : value.ToUniversalTime();
        }

        public int IdCategoria { get; set; }
        public virtual Categoria? Categoria { get; set; }

        public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();

        public virtual ICollection<Stands> Stands { get; set; } = new List<Stands>();

        public virtual ICollection<ProductoEmprendedor>? ProductoEmprendedores { get; set; }
    }
}

