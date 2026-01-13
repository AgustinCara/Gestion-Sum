using System.ComponentModel.DataAnnotations;


namespace GestionSUM.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; } = string.Empty;
        public string Departamento { get; set; } = string.Empty;

        [Required]
        public RolUsuario Rol { get; set; }

    }
}
