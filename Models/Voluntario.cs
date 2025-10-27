namespace ComuniApp.Api.Models
{
    public class Voluntario
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;
        public string? Habilidades { get; set; }
        public string? Disponibilidad { get; set; }
        public string? Experiencia { get; set; }
    }
}
