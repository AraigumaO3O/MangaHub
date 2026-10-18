using MangaHub.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace MangaHub.Api.Data;

public static class DbSeeder
{
    /// <summary>
    /// Crea el usuario Admin si no existe. El email y la contraseña vienen de las variables de
    /// entorno Seed__AdminEmail y Seed__AdminPassword (nunca del repositorio).
    /// </summary>
    public static async Task SeedAdminAsync(IServiceProvider services, IConfiguration config)
    {
        var email = config["Seed:AdminEmail"];
        var password = config["Seed:AdminPassword"];

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return;

        email = email.Trim().ToLowerInvariant();

        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if (await db.Usuarios.AnyAsync(u => u.Email == email))
            return;

        db.Usuarios.Add(new Usuario
        {
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            Rol = "Admin"
        });
        await db.SaveChangesAsync();
    }
}
