public class SolicitudUpdateDto
{
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string? Ubicacion { get; set; }
    public string Estado { get; set; } = "pendiente";
}
