using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ComuniApp.Api.Models
{
    [Table("solicitantes")]
    public class Solicitante
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("usuario_id")]
        public int UsuarioId { get; set; }
        [Column("organizacion")]
        public string? Organizacion { get; set; }
        [Column("descripcion")]
        public string? Descripcion { get; set; }
        public Usuario Usuario { get; set; } = null!;
    }
}
