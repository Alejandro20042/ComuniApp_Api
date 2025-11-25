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
                .Include(s => s.Participaciones)
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
                    Organizacion = s.Solicitante.Organizacion,
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

        [HttpGet("voluntario/{usuarioId}")]
        public async Task<ActionResult<IEnumerable<SolicitudDto>>> GetSolicitudesVoluntario(int usuarioId)
        {
            // Busca el voluntario por usuarioId
            var voluntario = await _context.Voluntarios
                .FirstOrDefaultAsync(v => v.UsuarioId == usuarioId);

            if (voluntario == null) return BadRequest("El voluntario no existe.");

            var solicitudes = await _context.Participaciones
                .Where(p => p.VoluntarioId == voluntario.Id && p.Estado == "activo")
                .Include(p => p.Solicitud)
                    .ThenInclude(s => s.Solicitante)
                        .ThenInclude(s => s.Usuario)
                .Select(p => new SolicitudDto
                {
                    Id = p.Solicitud.Id,
                    Titulo = p.Solicitud.Titulo,
                    Descripcion = p.Solicitud.Descripcion,
                    Ubicacion = p.Solicitud.Ubicacion,
                    Estado = p.Solicitud.Estado,
                    FechaCreacion = p.Solicitud.FechaCreacion,
                    SolicitanteNombre = p.Solicitud.Solicitante.Usuario.Nombre,
                    Organizacion = p.Solicitud.Solicitante.Organizacion
                })
                .ToListAsync();

            return Ok(solicitudes);
        }

        [HttpPut("{id}/aceptar")]
        public async Task<IActionResult> AceptarSolicitud(int id, [FromBody] AceptarSolicitudDto request)
        {
            if (request == null || request.VoluntarioId <= 0)
                return BadRequest("UsuarioId es requerido y debe ser válido.");

            var solicitud = await _context.Solicitudes.FindAsync(id);
            if (solicitud == null) return NotFound("La solicitud no existe.");

            // Busca el voluntario usando UsuarioId
            var voluntario = await _context.Voluntarios
                .FirstOrDefaultAsync(v => v.UsuarioId == request.VoluntarioId);

            if (voluntario == null) return BadRequest("El voluntario no existe.");

            var yaParticipa = await _context.Participaciones
                .AnyAsync(p => p.VoluntarioId == voluntario.Id && p.SolicitudId == solicitud.Id);
            if (yaParticipa) return BadRequest("El voluntario ya participa en esta solicitud.");

            var participacion = new Participacion
            {
                VoluntarioId = voluntario.Id,
                SolicitudId = solicitud.Id,
                Estado = "activo",
                FechaParticipacion = DateTime.UtcNow
            };
            _context.Participaciones.Add(participacion);

            solicitud.Estado = "en progreso";

            try
            {
                await _context.SaveChangesAsync();
                return Ok("Solicitud aceptada correctamente.");
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, "Ocurrió un error al guardar la participación: " + ex.Message);
            }
        }

        // Endpoint para obtener solicitudes pendientes
        [HttpGet("pendientes")]
        public async Task<ActionResult<IEnumerable<SolicitudDto>>> GetSolicitudesPendientes()
        {
            var solicitudes = await _context.Solicitudes
                .Where(s => s.Estado == "pendiente" &&
                            !_context.Participaciones.Any(p => p.SolicitudId == s.Id && p.Estado == "activo"))
                .Include(s => s.Solicitante)
                    .ThenInclude(s => s.Usuario)
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

        [HttpPost("offline-send")]
        public async Task<IActionResult> EnviarOffline([FromBody] CreateSolicitudDto dto)
        {
            var solicitud = new Solicitud
            {
                SolicitanteId = dto.SolicitanteId,
                Titulo = dto.Titulo,
                Descripcion = dto.Descripcion,
                Ubicacion = dto.Ubicacion,
                Estado = "pendiente",
                FechaCreacion = DateTime.UtcNow
            };
            _context.Solicitudes.Add(solicitud);
            await _context.SaveChangesAsync();

            return Ok(solicitud);
        }

        [HttpPut("{id}/completar")]
        public async Task<IActionResult> CompletarSolicitud(int id, [FromBody] CompletarSolicitudDto request)
        {
            if (request == null || request.VoluntarioId <= 0)
                return BadRequest("VoluntarioId es requerido y debe ser válido.");

            var solicitud = await _context.Solicitudes.FindAsync(id);
            if (solicitud == null) return NotFound("La solicitud no existe.");

            // Validar que el voluntario participa en la solicitud
            var participa = await _context.Participaciones
                .AnyAsync(p => p.VoluntarioId == request.VoluntarioId && p.SolicitudId == id && p.Estado == "activo");
            if (!participa) return BadRequest("El voluntario no participa en esta solicitud.");

            solicitud.Estado = "completada";
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Solicitud marcada como completada.", solicitud });
        }

        [HttpPut("{id}/confirmar")]
        public async Task<IActionResult> ConfirmarSolicitud(int id, [FromBody] ConfirmarSolicitudDto request)
        {
            if (request == null || request.SolicitanteId <= 0)
                return BadRequest("SolicitanteId es requerido y debe ser válido.");

            var solicitud = await _context.Solicitudes.FindAsync(id);
            if (solicitud == null) return NotFound("La solicitud no existe.");

            if (solicitud.SolicitanteId != request.SolicitanteId)
                return BadRequest("Este solicitante no es dueño de la solicitud.");

            if (solicitud.Estado != "completada")
                return BadRequest("La solicitud debe estar marcada como completada por un voluntario antes de confirmarla.");

            solicitud.Estado = "finalizada";
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Solicitud confirmada y cerrada.", solicitud });
        }

    }
}
