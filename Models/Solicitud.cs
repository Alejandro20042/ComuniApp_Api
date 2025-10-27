namespace ComuniApp.Api.Models
{
    public class Solicitud
    {
        public int Id { get; set; }
        public int SolicitanteId { get; set; }
        public Solicitante Solicitante { get; set; } = null!;
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string? Ubicacion { get; set; }
        public string Estado { get; set; } = "pendiente"; // pendiente, en progreso, completada
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    }
}
