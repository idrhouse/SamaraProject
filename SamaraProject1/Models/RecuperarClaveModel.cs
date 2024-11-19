using System.ComponentModel.DataAnnotations;

namespace SamaraProject1.Models
{
    public class RecuperarClaveModel
    {
        [Required(ErrorMessage = "El correo electrónico es requerido.")]
        [EmailAddress(ErrorMessage = "El correo electrónico no es válido.")]
        public string Correo { get; set; }
    }
}