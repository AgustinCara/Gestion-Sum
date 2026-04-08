using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;


namespace GestionSUM.Models
{

    public class Usuario : IdentityUser<int>
    {
        public string NombreCompleto { get; set; }
        public string Departamento { get; set; }

        public RolUsuario Rol { get; set; }

    }

}
