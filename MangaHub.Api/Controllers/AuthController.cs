using Asp.Versioning;
using MangaHub.Api.Dtos;
using MangaHub.Api.Entities;
using MangaHub.Api.Repositories;
using MangaHub.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MangaHub.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly IUsuarioRepository _usuarios;
    private readonly ITokenService _tokens;

    public AuthController(IUsuarioRepository usuarios, ITokenService tokens)
    {
        _usuarios = usuarios;
        _tokens = tokens;
    }

    /// <summary>Registra un usuario nuevo con rol Lector.</summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(UsuarioDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UsuarioDto>> Register(RegistroDto dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();

        if (await _usuarios.ExisteEmailAsync(email))
        {
            return Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Email ya registrado",
                detail: "Ya existe un usuario con ese email.");
        }

        var usuario = new Usuario
        {
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Rol = "Lector" // el rol Admin nunca se puede elegir al registrarse
        };

        var creado = await _usuarios.CrearAsync(usuario);
        return StatusCode(StatusCodes.Status201Created, new UsuarioDto(creado.Id, creado.Email, creado.Rol));
    }

    /// <summary>Inicia sesión y devuelve un token JWT.</summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(TokenDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TokenDto>> Login(LoginDto dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();
        var usuario = await _usuarios.ObtenerPorEmailAsync(email);

        if (usuario is null || !BCrypt.Net.BCrypt.Verify(dto.Password, usuario.PasswordHash))
        {
            return Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                title: "Credenciales inválidas",
                detail: "El email o la contraseña son incorrectos.");
        }

        return Ok(_tokens.Crear(usuario));
    }
}
