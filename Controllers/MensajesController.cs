using ComuniApp.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class MensajesController : ControllerBase
{
    private readonly AppDbContext _context;

    public MensajesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("chat-info/{solicitudId}")]
    public async Task<IActionResult> ObtenerChatInfo(int solicitudId)
    {
        var solicitud = await _context.Solicitudes
            .Include(s => s.Participaciones)
                .ThenInclude(p => p.Voluntario)   // 👈 incluye voluntario
            .Include(s => s.Solicitante)          // 👈 incluye solicitante
            .FirstOrDefaultAsync(s => s.Id == solicitudId);

        if (solicitud == null)
            return NotFound("Solicitud no encontrada");

        var participacion = solicitud.Participaciones.FirstOrDefault();
        if (participacion == null)
            return BadRequest("La solicitud no tiene un voluntario asignado.");

        var info = new ChatInfoDto
        {
            SolicitudId = solicitud.Id,
            SolicitanteUsuarioId = solicitud.Solicitante.UsuarioId,   // 👈 usuario_id
            VoluntarioUsuarioId = participacion.Voluntario.UsuarioId  // 👈 usuario_id
        };

        return Ok(info);
    }


    [HttpGet("historial/{solicitudId}")]
    public async Task<IActionResult> ObtenerHistorial(int solicitudId)
    {
        var mensajes = await _context.Mensajes
            .Where(m => m.SolicitudId == solicitudId)
            .OrderBy(m => m.FechaEnvio)
            .Select(m => new
            {
                m.Id,
                m.SolicitudId,
                m.EmisorId,
                m.ReceptorId,
                m.Contenido,
                m.FechaEnvio
            })
            .ToListAsync();

        return Ok(mensajes);
    }


}