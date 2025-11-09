using System.Security.Cryptography;
using System.Text;
using ComuniApp.Api.Data;
using ComuniApp.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class RegistroController : ControllerBase
{
    private readonly AppDbContext _context;

    public RegistroController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult> Register([FromBody] RegisterRequest request)
    {
        if (!new[] { "voluntario", "solicitante" }.Contains(request.TipoUsuario.ToLower()))
            return BadRequest("Tipo de usuario inválido.");

        if (await _context.Usuarios.AnyAsync(u => u.Email == request.Email))
            return BadRequest("El email ya está registrado.");

        var usuario = new Usuario
        {
            Nombre = request.Nombre,
            Email = request.Email,
            Contraseña = HashPassword(request.Contraseña),
            TipoUsuario = request.TipoUsuario.ToLower(),
            FechaRegistro = DateTime.UtcNow
        };

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync(); // Guarda usuario y obtiene Id

        int? solicitanteId = null;
        int? voluntarioId = null;

        if (usuario.TipoUsuario == "solicitante")
        {
            var solicitante = new Solicitante
            {
                UsuarioId = usuario.Id,
                Organizacion = "",
                Descripcion = ""
            };
            _context.Solicitantes.Add(solicitante);
            await _context.SaveChangesAsync();
            solicitanteId = solicitante.Id;
        }
        else if (usuario.TipoUsuario == "voluntario")
        {
            var voluntario = new Voluntario
            {
                UsuarioId = usuario.Id,
                Habilidades = "",
                Disponibilidad = "",
                Experiencia = ""
            };
            _context.Voluntarios.Add(voluntario);
            await _context.SaveChangesAsync();
            voluntarioId = voluntario.Id;
        }

        return Ok(new
        {
            usuario.Id,
            usuario.Email,
            usuario.Nombre,
            usuario.TipoUsuario,
            solicitanteId,
            voluntarioId
        });
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }
}
