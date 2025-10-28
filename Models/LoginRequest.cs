// Models/LoginRequest.cs
public class LoginRequest
{
    public string Usuario { get; set; } = null!;  // Aquí se envía el email
    public string Contraseña { get; set; } = null!;
}
