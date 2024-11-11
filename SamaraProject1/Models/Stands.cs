using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SamaraProject1.Models
{
    public class Stands
    {
        [Key]
        public int IdStand { get; set; }

        [Required(ErrorMessage = "El número de stand es requerido")]
        [Display(Name = "Número de Stand")]
        public int Numero_Stand { get; set; }

        [Display(Name = "Descripción del Stand")]
        [StringLength(500)]
        public string? Descripcion_Stand { get; set; }

        [Display(Name = "Imagen del Stand")]
        [StringLength(500)]
        public string? ImagenUrl { get; set; }

        [Required(ErrorMessage = "El emprendedor es requerido")]
        [Display(Name = "Emprendedor")]
        public int IdEmprendedor { get; set; }

        [ForeignKey("IdEmprendedor")]
        public virtual Emprendedor? Emprendedor { get; set; }
    }
}