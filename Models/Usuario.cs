using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ComuniApp.Api.Models;

[Table("usuarios")]  
public class Usuario
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("nombre")]
    public string Nombre { get; set; } = null!;

    [Column("apellido")]
    public string? Apellido { get; set; }

    [Column("email")]
    public string Email { get; set; } = null!;

    [Column("contraseña")]
    public string Contraseña { get; set; } = null!;

    [Column("telefono")]
    public string? Telefono { get; set; }

    [Column("tipo_usuario")]
    public string TipoUsuario { get; set; } = null!;

    [Column("fecha_registro")]
    public DateTime FechaRegistro { get; set; }

    public Solicitante? Solicitante { get; set; }
}
