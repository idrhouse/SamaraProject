using System.ComponentModel.DataAnnotations;

namespace SamaraProject1.Models
{
    public class RestablecerClaveModel
    {
        [Required(ErrorMessage = "El token es requerido.")]
        public string Token { get; set; }

        [Required(ErrorMessage = "La nueva contraseña es requerida.")]
        [DataType(DataType.Password)]
        [Display(Name = "Nueva contraseña")]
        public string NuevaClave { get; set; }


        [DataType(DataType.Password)]
        [Display(Name = "Confirmar nueva contraseña")]
        [Compare("NuevaClave", ErrorMessage = "La nueva contraseña y la confirmación no coinciden.")]
        public string ConfirmarNuevaClave { get; set; }
    }
}