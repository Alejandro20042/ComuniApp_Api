namespace ComuniApp.Api.Models
{
    public class Participacion
    {
        public int Id { get; set; }
        public int VoluntarioId { get; set; }
        public Voluntario Voluntario { get; set; } = null!;
        public int SolicitudId { get; set; }
        public Solicitud Solicitud { get; set; } = null!;
        public string Estado { get; set; } = "activo"; // activo, finalizado, cancelado
        public DateTime FechaParticipacion { get; set; } = DateTime.UtcNow;
    }
}
