using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComuniApp.Api.Models
{
    [Table("solicitudes")]
    public class Solicitud
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("solicitante_id")]
        public int SolicitanteId { get; set; }
        [Column("titulo")]
        public string Titulo { get; set; } = string.Empty;
        [Column("descripcion")]
        public string Descripcion { get; set; } = string.Empty;
        [Column("ubicacion")]
        public string? Ubicacion { get; set; }
        [Column("estado")]
        public string Estado { get; set; } = "pendiente"; 
        [Column("fecha_creacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public Solicitante Solicitante { get; set; } = null!;
        public ICollection<Participacion> Participaciones { get; set; } = new List<Participacion>();
    }
}
