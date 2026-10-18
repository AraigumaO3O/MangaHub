using MangaHub.Api.Data;
using MangaHub.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace MangaHub.Api.Repositories;

public class CatalogoRepository : ICatalogoRepository
{
    private readonly AppDbContext _context;

    public CatalogoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Autor>> ObtenerAutoresAsync() =>
        await _context.Autores.AsNoTracking().OrderBy(a => a.Nombre).ToListAsync();

    public async Task<List<Categoria>> ObtenerCategoriasAsync() =>
        await _context.Categorias.AsNoTracking().OrderBy(c => c.Nombre).ToListAsync();

    public async Task<bool> ExisteAutorAsync(int id) =>
        await _context.Autores.AnyAsync(a => a.Id == id);

    public async Task<bool> ExisteCategoriaAsync(int id) =>
        await _context.Categorias.AnyAsync(c => c.Id == id);
}
