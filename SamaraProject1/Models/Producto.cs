using System.ComponentModel.DataAnnotations;

namespace SamaraProject1.Models
{
    public class Producto
    {
        public int IdProducto { get; set; }

        [Required(ErrorMessage = "El nombre del producto es obligatorio.")]
        [MaxLength(100)]
        public string? Nombre_Producto { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [MaxLength(500)]
        public string? Descripcion { get; set; }
        public string? ImagenUrl { get; set; }

        [Required(ErrorMessage = "Seleccione un tipo de producto.")]
        public int? IdTipoProducto { get; set; }
        public TipoProducto? TipoProducto { get; set; }

        public virtual ICollection<ProductoEmprendedor>? ProductoEmprendedores { get; set; }
    }
}