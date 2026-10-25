using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace MangaHub.Tests;

public record TokenResponse(string Token, DateTime ExpiraUtc, string Rol);
public record UsuarioResponse(int Id, string Email, string Rol);
public record MangaResponse(int Id, string Nombre, string? Resumen, int AutorId, string AutorNombre, int CategoriaId, string CategoriaNombre);
public record EntradaResponse(int MangaId, string MangaNombre, string Estado, int CapituloActual);

public static class TestHelpers
{
    public static async Task<string> LoginAsync(this HttpClient client, string email, string password)
    {
        var respuesta = await client.PostAsJsonAsync("/api/v1/Auth/login", new { email, password });
        respuesta.EnsureSuccessStatusCode();

        var token = await respuesta.Content.ReadFromJsonAsync<TokenResponse>();
        return token!.Token;
    }

    public static async Task<HttpClient> ClienteAdminAsync(this MangaHubFactory factory)
    {
        var client = factory.CreateClient();
        var token = await client.LoginAsync(MangaHubFactory.AdminEmail, MangaHubFactory.AdminPassword);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    public static async Task<HttpClient> ClienteLectorAsync(this MangaHubFactory factory)
    {
        var client = factory.CreateClient();
        var email = $"lector-{Guid.NewGuid():N}@test.com";
        const string password = "Lector123";

        var registro = await client.PostAsJsonAsync("/api/v1/Auth/register", new { email, password });
        registro.EnsureSuccessStatusCode();

        var token = await client.LoginAsync(email, password);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }
}
