using System.Security.Cryptography;
using System.Text;
using ComuniApp.Api.Data;
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
        await _context.SaveChangesAsync();

        return Ok(new { usuario.Email, usuario.TipoUsuario }); // solo devolvemos lo necesario
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }
}
