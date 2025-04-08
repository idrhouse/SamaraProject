using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SamaraProject1.Models;

public partial class Administrador
{
    public int IdAdministrador { get; set; }

    public string? NombreAdministrador { get; set; }

    public string? Apellido { get; set; }

    public string? Correo { get; set; }

    public string? Clave { get; set; }

    [NotMapped]
    [Compare("Clave", ErrorMessage = "Las contraseñas no coinciden.")]
    public string? ConfirmarClave { get; set; }

    public string? TokenRecuperacion { get; set; }

    public DateTime? TokenExpiracion { get; set; }
}

