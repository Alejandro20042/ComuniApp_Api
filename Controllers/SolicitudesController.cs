using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ComuniApp.Api.Models;
using ComuniApp.Api.Data;
using ComuniApp.Api.Dtos;

namespace ComuniApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SolicitudesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SolicitudesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SolicitudDto>>> GetSolicitudes()
        {
            var solicitudes = await _context.Solicitudes
                .Include(s => s.Solicitante)
                .ThenInclude(s => s.Usuario)
                .OrderByDescending(s => s.FechaCreacion)
                .Select(s => new SolicitudDto
                {
                    Id = s.Id,
                    Titulo = s.Titulo,
                    Descripcion = s.Descripcion,
                    Ubicacion = s.Ubicacion,
                    Estado = s.Estado,
                    FechaCreacion = s.FechaCreacion,
                    SolicitanteNombre = s.Solicitante.Usuario.Nombre,
                    Organizacion = s.Solicitante.Organizacion
                })
                .ToListAsync();

            return Ok(solicitudes);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Solicitud>> GetSolicitud(int id)
        {
            var solicitud = await _context.Solicitudes
                .Include(s => s.Solicitante)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (solicitud == null)
                return NotFound();

            return Ok(solicitud);
        }

        [HttpPost]
        public async Task<ActionResult> CrearSolicitud([FromBody] CreateSolicitudDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Titulo) || string.IsNullOrWhiteSpace(request.Descripcion))
                return BadRequest("Título y descripción son requeridos.");

            var solicitud = new Solicitud
            {
                SolicitanteId = request.SolicitanteId,
                Titulo = request.Titulo,
                Descripcion = request.Descripcion,
                Ubicacion = request.Ubicacion,
                Estado = "pendiente",
                FechaCreacion = DateTime.UtcNow
            };



            _context.Solicitudes.Add(solicitud);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSolicitud), new { id = solicitud.Id }, solicitud);
        }


        [HttpPut("{id}")]
        public async Task<ActionResult> ActualizarSolicitud(int id, [FromBody] SolicitudUpdateDto request)
        {
            var solicitud = await _context.Solicitudes.FindAsync(id);
            if (solicitud == null)
                return NotFound();

            solicitud.Titulo = request.Titulo;
            solicitud.Descripcion = request.Descripcion;
            solicitud.Ubicacion = request.Ubicacion;
            solicitud.Estado = request.Estado;

            await _context.SaveChangesAsync();
            return Ok(solicitud);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> EliminarSolicitud(int id)
        {
            var solicitud = await _context.Solicitudes.FindAsync(id);
            if (solicitud == null)
                return NotFound();

            _context.Solicitudes.Remove(solicitud);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
