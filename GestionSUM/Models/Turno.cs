using System.ComponentModel.DataAnnotations;

namespace GestionSUM.Models
{
    public class Turno
    {
        public int Id { get; set; }

        [Required]
        public string MomentoDelDia { get; set; } = string.Empty;

        [Required]
        public TimeSpan HoraInicio { get; set; }

        [Required]
        public TimeSpan HoraFin { get; set; }
    }
}
