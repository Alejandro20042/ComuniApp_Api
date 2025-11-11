namespace ComuniApp.Api.DTOs
{
    public class ChatMessageDto
    {
        public int UsuarioId { get; set; }
        public string? UsuarioNombre { get; set; }
        public string? Mensaje { get; set; }
        public System.DateTime Timestamp { get; set; }
    }
}