namespace ComuniApp.Api.Models
{
    public class Solicitante
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;
        public string? Organizacion { get; set; }
        public string? Descripcion { get; set; }
    }
}
