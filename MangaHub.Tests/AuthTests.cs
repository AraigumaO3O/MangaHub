using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace MangaHub.Tests;

public class AuthTests : IClassFixture<MangaHubFactory>
{
    private readonly MangaHubFactory _factory;

    public AuthTests(MangaHubFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Login_ConCredencialesValidas_Devuelve200YToken()
    {
        var client = _factory.CreateClient();

        var respuesta = await client.PostAsJsonAsync("/api/v1/Auth/login",
            new { email = MangaHubFactory.AdminEmail, password = MangaHubFactory.AdminPassword });

        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
        var token = await respuesta.Content.ReadFromJsonAsync<TokenResponse>();
        Assert.False(string.IsNullOrWhiteSpace(token!.Token));
        Assert.Equal("Admin", token.Rol);
    }

    [Fact]
    public async Task Login_ConPasswordIncorrecta_Devuelve401()
    {
        var client = _factory.CreateClient();

        var respuesta = await client.PostAsJsonAsync("/api/v1/Auth/login",
            new { email = MangaHubFactory.AdminEmail, password = "incorrecta" });

        Assert.Equal(HttpStatusCode.Unauthorized, respuesta.StatusCode);
    }

    [Fact]
    public async Task Register_UsuarioNuevo_Devuelve201ConRolLector()
    {
        var client = _factory.CreateClient();
        var email = $"nuevo-{Guid.NewGuid():N}@test.com";

        var respuesta = await client.PostAsJsonAsync("/api/v1/Auth/register",
            new { email, password = "Lector123" });

        Assert.Equal(HttpStatusCode.Created, respuesta.StatusCode);
        var usuario = await respuesta.Content.ReadFromJsonAsync<UsuarioResponse>();
        Assert.Equal("Lector", usuario!.Rol);
    }

    [Fact]
    public async Task Register_EmailRepetido_Devuelve409()
    {
        var client = _factory.CreateClient();
        var email = $"repetido-{Guid.NewGuid():N}@test.com";

        var primera = await client.PostAsJsonAsync("/api/v1/Auth/register", new { email, password = "Lector123" });
        var segunda = await client.PostAsJsonAsync("/api/v1/Auth/register", new { email, password = "Lector123" });

        Assert.Equal(HttpStatusCode.Created, primera.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, segunda.StatusCode);
    }

    [Fact]
    public async Task Register_ConDatosInvalidos_Devuelve400()
    {
        var client = _factory.CreateClient();

        var respuesta = await client.PostAsJsonAsync("/api/v1/Auth/register",
            new { email = "esto-no-es-un-email", password = "123" });

        Assert.Equal(HttpStatusCode.BadRequest, respuesta.StatusCode);
    }
}
