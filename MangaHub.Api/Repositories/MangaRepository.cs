using MangaHub.Api.Data;
using MangaHub.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace MangaHub.Api.Repositories;

public class MangaRepository : IMangaRepository
{
    private readonly AppDbContext _context;

    public MangaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Manga>> ObtenerTodosAsync()
    {
        return await _context.Mangas
            .AsNoTracking()
            .Include(m => m.Autor)
            .Include(m => m.Categoria)
            .OrderBy(m => m.Nombre)
            .ToListAsync();
    }

    public async Task<Manga?> ObtenerPorIdAsync(int id)
    {
        return await _context.Mangas
            .Include(m => m.Autor)
            .Include(m => m.Categoria)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<Manga> CrearAsync(Manga manga)
    {
        _context.Mangas.Add(manga);
        await _context.SaveChangesAsync();


        await _context.Entry(manga).Reference(m => m.Autor).LoadAsync();
        await _context.Entry(manga).Reference(m => m.Categoria).LoadAsync();
        return manga;
    }

    public async Task ActualizarAsync(Manga manga)
    {
        _context.Mangas.Update(manga);
        await _context.SaveChangesAsync();
    }

    public async Task EliminarAsync(Manga manga)
    {
        _context.Mangas.Remove(manga);
        await _context.SaveChangesAsync();
    }
}
