using GestionSUM.Services;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace GestionSUM.Models
{
    public class Reserva
    {
        public int Id { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }

        [Required]
        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }

        public bool Cancelada { get; set; } = false;
        public string? MotivoCancelacion { get; set; }
        public DateTime? FechaCancelacion { get; set; }

        [Required]
        public int TurnoId { get; set; }
        public Turno Turno { get; set; }
    }

}
