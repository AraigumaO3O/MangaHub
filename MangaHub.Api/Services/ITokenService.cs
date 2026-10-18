using MangaHub.Api.Dtos;
using MangaHub.Api.Entities;

namespace MangaHub.Api.Services;

public interface ITokenService
{
    TokenDto Crear(Usuario usuario);
}
