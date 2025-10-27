using Microsoft.EntityFrameworkCore;
using ComuniApp.Api.Models;

namespace ComuniApp.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Voluntario> Voluntarios { get; set; }
        public DbSet<Solicitante> Solicitantes { get; set; }
        public DbSet<Solicitud> Solicitudes { get; set; }
        public DbSet<Participacion> Participaciones { get; set; }
    }
}
