using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace MangaHub.Tests;

public class BibliotecaTests : IClassFixture<MangaHubFactory>
{
    private readonly MangaHubFactory _factory;

    public BibliotecaTests(MangaHubFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Biblioteca_SinToken_Devuelve401()
    {
        var client = _factory.CreateClient();

        var respuesta = await client.GetAsync("/api/v1/Biblioteca");

        Assert.Equal(HttpStatusCode.Unauthorized, respuesta.StatusCode);
    }

    [Fact]
    public async Task AgregarManga_Devuelve201_YSiSeRepiteDevuelve409()
    {
        var lector = await _factory.ClienteLectorAsync();
        var cuerpo = new { mangaId = 1, estado = "Leyendo", capituloActual = 12 };

        var primera = await lector.PostAsJsonAsync("/api/v1/Biblioteca", cuerpo);
        var segunda = await lector.PostAsJsonAsync("/api/v1/Biblioteca", cuerpo);

        Assert.Equal(HttpStatusCode.Created, primera.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, segunda.StatusCode);
    }

    [Fact]
    public async Task AgregarMangaInexistente_Devuelve400()
    {
        var lector = await _factory.ClienteLectorAsync();

        var respuesta = await lector.PostAsJsonAsync("/api/v1/Biblioteca",
            new { mangaId = 9999, estado = "Pendiente", capituloActual = 0 });

        Assert.Equal(HttpStatusCode.BadRequest, respuesta.StatusCode);
    }

    [Fact]
    public async Task ActualizarEstado_Devuelve204_YElCambioSeGuarda()
    {
        var lector = await _factory.ClienteLectorAsync();
        await lector.PostAsJsonAsync("/api/v1/Biblioteca",
            new { mangaId = 1, estado = "Leyendo", capituloActual = 12 });

        var actualizado = await lector.PutAsJsonAsync("/api/v1/Biblioteca/1",
            new { estado = "Terminado", capituloActual = 100 });
        Assert.Equal(HttpStatusCode.NoContent, actualizado.StatusCode);

        var entrada = await lector.GetFromJsonAsync<EntradaResponse>("/api/v1/Biblioteca/1");
        Assert.Equal("Terminado", entrada!.Estado);
        Assert.Equal(100, entrada.CapituloActual);
    }

    [Fact]
    public async Task CadaUsuarioSoloVeSuPropiaBiblioteca()
    {
        var lectorA = await _factory.ClienteLectorAsync();
        var lectorB = await _factory.ClienteLectorAsync();

        await lectorA.PostAsJsonAsync("/api/v1/Biblioteca",
            new { mangaId = 2, estado = "Pendiente", capituloActual = 0 });

        var listaDeB = await lectorB.GetFromJsonAsync<List<EntradaResponse>>("/api/v1/Biblioteca");
        Assert.Empty(listaDeB!);

        var respuestaB = await lectorB.GetAsync("/api/v1/Biblioteca/2");
        Assert.Equal(HttpStatusCode.NotFound, respuestaB.StatusCode);
    }
}
