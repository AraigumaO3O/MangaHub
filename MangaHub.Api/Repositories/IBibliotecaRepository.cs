using MangaHub.Api.Entities;

namespace MangaHub.Api.Repositories;

public interface IBibliotecaRepository
{
    Task<List<EntradaBiblioteca>> ObtenerPorUsuarioAsync(int usuarioId);
    Task<EntradaBiblioteca?> ObtenerAsync(int usuarioId, int mangaId);
    Task<EntradaBiblioteca> CrearAsync(EntradaBiblioteca entrada);
    Task ActualizarAsync(EntradaBiblioteca entrada);
    Task EliminarAsync(EntradaBiblioteca entrada);
}
