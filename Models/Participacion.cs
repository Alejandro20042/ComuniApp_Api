using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ComuniApp.Api.Models
{
    [Table("participaciones")]
    public class Participacion
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("voluntario_id")]
        public int VoluntarioId { get; set; }
        [Column("solicitud_id")]
        public int SolicitudId { get; set; }
        [Column("estado")]
        public string Estado { get; set; } = "activo"; // activo, finalizado, cancelado
        [Column("fecha_participacion")]
        public DateTime FechaParticipacion { get; set; } = DateTime.UtcNow;
        public Voluntario Voluntario { get; set; } = null!;
        public Solicitud Solicitud { get; set; } = null!;

    }
}
