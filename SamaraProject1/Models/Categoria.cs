using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace SamaraProject1.Models
{
    public class Categoria
    {
        public int IdCategoria { get; set; } // Clave primaria

        [Required(ErrorMessage = "El nombre de la categoría es obligatorio.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre de la categoría debe tener al menos 3 caracteres.")]
        public string NombreCategoria { get; set; }

        public virtual ICollection<Emprendedor> Emprendedores { get; set; } = new List<Emprendedor>();
    }
}
