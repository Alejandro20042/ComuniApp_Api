using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComuniApp.Api.Models
{
    [Table("voluntarios")]
    public class Voluntario
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("usuario_id")]
        public int UsuarioId { get; set; }
        [Column("habilidades")]
        public string? Habilidades { get; set; }
        [Column("disponibilidad")]
        public string? Disponibilidad { get; set; }
        [Column("experiencia")]
        public string? Experiencia { get; set; }
        public Usuario Usuario { get; set; } = null!;
    }
}
