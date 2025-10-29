namespace ComuniApp.Api.Dtos
{
    public class SolicitudDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string? Ubicacion { get; set; }
        public string Estado { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
        public string? SolicitanteNombre { get; set; }
        public string? Organizacion { get; set; }
    }
}
