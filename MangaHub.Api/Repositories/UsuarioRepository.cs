using MangaHub.Api.Data;
using MangaHub.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace MangaHub.Api.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly AppDbContext _context;

    public UsuarioRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Usuario?> ObtenerPorEmailAsync(string email) =>
        await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);

    public async Task<bool> ExisteEmailAsync(string email) =>
        await _context.Usuarios.AnyAsync(u => u.Email == email);

    public async Task<Usuario> CrearAsync(Usuario usuario)
    {
        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();
        return usuario;
    }
}
