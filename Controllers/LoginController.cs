using System.Security.Cryptography;
using System.Text;
using ComuniApp.Api.Data;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase
{
    private readonly AppDbContext _context;

    public LoginController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult> Login([FromBody] LoginRequest request)
    {
        string hashedPassword = HashPassword(request.Contraseña);

        var usuario = await _context.Usuarios
        .Include(u => u.Solicitante)
        .FirstOrDefaultAsync(u => u.Nombre == request.Usuario && u.Contraseña == hashedPassword);


        if (usuario == null)
            return Unauthorized("Usuario o contraseña incorrectos.");

        int? solicitanteId = null;
        if (usuario.TipoUsuario == "solicitante")
        {
            solicitanteId = (await _context.Solicitantes.FirstOrDefaultAsync(s => s.UsuarioId == usuario.Id))?.Id;
        }

        return Ok(new
        {
            usuario.Id,
            usuario.Email,
            usuario.Nombre,
            usuario.TipoUsuario,
            solicitanteId
        });

    }


    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }
}
