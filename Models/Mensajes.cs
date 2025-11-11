namespace ComuniApp_Api.Models
{
    public class Mensaje
    {
        public int Id { get; set; }

        public int EmisorId { get; set; }
        public Usuario Emisor { get; set; }   // relación con tabla usuarios

        public int ReceptorId { get; set; }
        public Usuario Receptor { get; set; } // relación con tabla usuarios

        public string Contenido { get; set; }

        public DateTime FechaEnvio { get; set; } = DateTime.UtcNow;
    }
}
