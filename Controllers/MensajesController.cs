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
            .FirstOrDefaultAsync(s => s.Id == solicitudId);

        if (solicitud == null)
            return NotFound("Solicitud no encontrada");

        var participacion = solicitud.Participaciones.FirstOrDefault();
        if (participacion == null)
            return BadRequest("La solicitud no tiene un voluntario asignado.");

        var info = new ChatInfoDto
        {
            solicitudId = solicitud.Id,
            solicitanteId = solicitud.SolicitanteId,
            voluntarioId = participacion.VoluntarioId
        };

        return Ok(info);
    }

}