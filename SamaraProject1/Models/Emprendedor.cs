namespace SamaraProject1.Models
{
    public partial class Emprendedor
    {
        public int IdEmprendedor { get; set; }  // id bigint

        public string NombreEmprendedor { get; set; }  // nombre nvarchar(255)

        public string Apellidos { get; set; }  // apellidos nvarchar(255)

        public string NombreNegocio { get; set; }  // nombre_negocio nvarchar(255)

        public string? DescripcionNegocio { get; set; }  // descripcion_negocio nvarchar(255)

        public string? Telefono { get; set; }  // telefono nvarchar(50)

        public string? Correo { get; set; }  // correo nvarchar(255)

        public int IdAdministrador { get; set; }  // id_administrador (Foreign Key)
      
        public string? ImagenUrl { get; set; }

        // Relación con la clase Administrador (Foreign Key)
        public virtual Administrador? Administrador { get; set; }
        public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
        public virtual ICollection<Stands> Stands { get; set; } = new List<Stands>();
    }

}
