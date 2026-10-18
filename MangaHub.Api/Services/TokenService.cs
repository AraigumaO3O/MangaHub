using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MangaHub.Api.Dtos;
using MangaHub.Api.Entities;
using Microsoft.IdentityModel.Tokens;

namespace MangaHub.Api.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _config;

    public TokenService(IConfiguration config)
    {
        _config = config;
    }

    public TokenDto Crear(Usuario usuario)
    {
        var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var credenciales = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);
        var expira = DateTime.UtcNow.AddHours(2);

        var claims = new[]
        {
            new Claim("sub", usuario.Id.ToString()),
            new Claim("email", usuario.Email),
            new Claim("role", usuario.Rol)
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: expira,
            signingCredentials: credenciales);

        return new TokenDto(new JwtSecurityTokenHandler().WriteToken(token), expira, usuario.Rol);
    }
}
