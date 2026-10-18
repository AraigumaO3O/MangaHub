using MangaHub.Api.Entities;

namespace MangaHub.Api.Repositories;

public interface ICatalogoRepository
{
    Task<List<Autor>> ObtenerAutoresAsync();
    Task<List<Categoria>> ObtenerCategoriasAsync();
    Task<bool> ExisteAutorAsync(int id);
    Task<bool> ExisteCategoriaAsync(int id);
}
