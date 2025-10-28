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

        // Buscar por email
        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Nombre == request.Usuario && u.Contraseña == hashedPassword);

        if (usuario == null)
            return Unauthorized("Usuario o contraseña incorrectos.");

        return Ok(new { usuario.Email, usuario.TipoUsuario });
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }
}
