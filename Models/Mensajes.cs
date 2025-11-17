using System.ComponentModel.DataAnnotations.Schema;
using ComuniApp.Api.Models;

[Table("mensajes")]
public class Mensaje
{
    [Column("id")]
    public int Id { get; set; }
    [Column("solicitud_id")]
    public int SolicitudId { get; set; }
    [Column("emisor_id")]
    public int EmisorId { get; set; }
    [Column("receptor_id")]
    public int ReceptorId { get; set; }
    [Column("contenido")]
    public string Contenido { get; set; } = string.Empty;
    [Column("fecha_envio")]
    public DateTime FechaEnvio { get; set; } = DateTime.UtcNow;

    public Solicitud? Solicitud { get; set; }
    public Usuario? Emisor { get; set; }
    public Usuario? Receptor { get; set; }
}
