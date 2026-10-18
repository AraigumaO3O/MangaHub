using MangaHub.Api.Data;
using MangaHub.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace MangaHub.Api.Repositories;

public class BibliotecaRepository : IBibliotecaRepository
{
    private readonly AppDbContext _context;

    public BibliotecaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<EntradaBiblioteca>> ObtenerPorUsuarioAsync(int usuarioId)
    {
        return await _context.Biblioteca
            .AsNoTracking()
            .Include(e => e.Manga)
            .Where(e => e.UsuarioId == usuarioId)
            .OrderBy(e => e.Manga!.Nombre)
            .ToListAsync();
    }

    public async Task<EntradaBiblioteca?> ObtenerAsync(int usuarioId, int mangaId)
    {
        return await _context.Biblioteca
            .Include(e => e.Manga)
            .FirstOrDefaultAsync(e => e.UsuarioId == usuarioId && e.MangaId == mangaId);
    }

    public async Task<EntradaBiblioteca> CrearAsync(EntradaBiblioteca entrada)
    {
        _context.Biblioteca.Add(entrada);
        await _context.SaveChangesAsync();
        await _context.Entry(entrada).Reference(e => e.Manga).LoadAsync();
        return entrada;
    }

    // La entrada ya viene rastreada por el DbContext, así que basta con guardar los cambios.
    public async Task ActualizarAsync(EntradaBiblioteca entrada)
    {
        await _context.SaveChangesAsync();
    }

    public async Task EliminarAsync(EntradaBiblioteca entrada)
    {
        _context.Biblioteca.Remove(entrada);
        await _context.SaveChangesAsync();
    }
}
