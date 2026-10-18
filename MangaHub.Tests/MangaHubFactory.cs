using MangaHub.Api.Data;
using MangaHub.Api.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace MangaHub.Tests;

/// <summary>
/// Levanta la API completa en memoria, pero con una base de datos InMemory
/// para no tocar la base real (MangaHubDb) durante las pruebas.
/// </summary>
public class MangaHubFactory : WebApplicationFactory<Program>
{
    public const string AdminEmail = "admin.test@mangahub.com";
    public const string AdminPassword = "Admin123!";

    private readonly string _dbName = "MangaHubTests-" + Guid.NewGuid();

    static MangaHubFactory()
    {
        // Configuración solo para las pruebas (afecta únicamente a este proceso)
        Environment.SetEnvironmentVariable("Jwt__Key", "llave-solo-para-pruebas-de-integracion-1234567890");
        Environment.SetEnvironmentVariable("Jwt__Issuer", "MangaHubTests");
        Environment.SetEnvironmentVariable("Jwt__Audience", "MangaHubTestsUsers");
        Environment.SetEnvironmentVariable("ConnectionStrings__Default", "Server=(local);Database=NoSeUsaEnPruebas;Trusted_Connection=True");
        Environment.SetEnvironmentVariable("Seed__AdminEmail", null);
        Environment.SetEnvironmentVariable("Seed__AdminPassword", null);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Quitamos el DbContext que apunta a SQL Server y ponemos uno en memoria
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase(_dbName));
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);

        using var scope = host.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        db.Database.EnsureCreated(); // crea la BD en memoria con los datos iniciales (seed)

        db.Usuarios.Add(new Usuario
        {
            Email = AdminEmail,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(AdminPassword),
            Rol = "Admin"
        });
        db.SaveChanges();

        return host;
    }
}
