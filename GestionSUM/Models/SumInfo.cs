using GestionSUM.Services;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionSUM.Models
{
    public class SumInfo
    {
        public int Id { get; set; }

        public string InformacionGeneral { get; set; }
        public string Capacidad { get; set; }
        public string Equipamiento { get; set; }
        public string Reglas { get; set; }

        public DateTime UltimaActualizacion { get; set; }
    }
}
