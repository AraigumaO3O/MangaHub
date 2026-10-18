using MangaHub.Api.Entities;

namespace MangaHub.Api.Repositories;

public interface IMangaRepository
{
    Task<List<Manga>> ObtenerTodosAsync();
    Task<Manga?> ObtenerPorIdAsync(int id);
    Task<Manga> CrearAsync(Manga manga);
    Task ActualizarAsync(Manga manga);
    Task EliminarAsync(Manga manga);
}
