using MangaHub.Api.Entities;

namespace MangaHub.Api.Repositories;

public interface IUsuarioRepository
{
    Task<Usuario?> ObtenerPorEmailAsync(string email);
    Task<bool> ExisteEmailAsync(string email);
    Task<Usuario> CrearAsync(Usuario usuario);
}
