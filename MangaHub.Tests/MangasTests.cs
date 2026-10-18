using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace MangaHub.Tests;

public class MangasTests : IClassFixture<MangaHubFactory>
{
    private readonly MangaHubFactory _factory;

    public MangasTests(MangaHubFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task ListarMangas_SinToken_Devuelve401()
    {
        var client = _factory.CreateClient();

        var respuesta = await client.GetAsync("/api/v1/Mangas");

        Assert.Equal(HttpStatusCode.Unauthorized, respuesta.StatusCode);
    }

    [Fact]
    public async Task ListarMangas_ComoLector_Devuelve200ConLosDatosIniciales()
    {
        var lector = await _factory.ClienteLectorAsync();

        var respuesta = await lector.GetAsync("/api/v1/Mangas");

        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
        var mangas = await respuesta.Content.ReadFromJsonAsync<List<MangaResponse>>();
        Assert.True(mangas!.Count >= 3);
    }

    [Fact]
    public async Task ObtenerMangaInexistente_Devuelve404()
    {
        var lector = await _factory.ClienteLectorAsync();

        var respuesta = await lector.GetAsync("/api/v1/Mangas/9999");

        Assert.Equal(HttpStatusCode.NotFound, respuesta.StatusCode);
    }

    [Fact]
    public async Task CrearManga_ComoLector_Devuelve403()
    {
        var lector = await _factory.ClienteLectorAsync();

        var respuesta = await lector.PostAsJsonAsync("/api/v1/Mangas",
            new { nombre = "Berserk", resumen = "Prueba", autorId = 3, categoriaId = 2 });

        Assert.Equal(HttpStatusCode.Forbidden, respuesta.StatusCode);
    }

    [Fact]
    public async Task CrearManga_ConDatosInvalidos_Devuelve400()
    {
        var admin = await _factory.ClienteAdminAsync();

        var respuesta = await admin.PostAsJsonAsync("/api/v1/Mangas",
            new { nombre = "", resumen = "Prueba", autorId = 99, categoriaId = 1 });

        Assert.Equal(HttpStatusCode.BadRequest, respuesta.StatusCode);
    }

    [Fact]
    public async Task CrudCompleto_ComoAdmin_FuncionaDeInicioAFin()
    {
        var admin = await _factory.ClienteAdminAsync();

        // Crear (201)
        var creado = await admin.PostAsJsonAsync("/api/v1/Mangas",
            new { nombre = "Berserk", resumen = "Un mercenario", autorId = 3, categoriaId = 2 });
        Assert.Equal(HttpStatusCode.Created, creado.StatusCode);
        var manga = await creado.Content.ReadFromJsonAsync<MangaResponse>();
        Assert.Equal("Berserk", manga!.Nombre);

        // Leer (200)
        var leido = await admin.GetAsync($"/api/v1/Mangas/{manga.Id}");
        Assert.Equal(HttpStatusCode.OK, leido.StatusCode);

        // Actualizar (204) y comprobar el cambio
        var editado = await admin.PutAsJsonAsync($"/api/v1/Mangas/{manga.Id}",
            new { nombre = "Berserk (editado)", resumen = "Un mercenario", autorId = 3, categoriaId = 2 });
        Assert.Equal(HttpStatusCode.NoContent, editado.StatusCode);

        var despuesDeEditar = await admin.GetFromJsonAsync<MangaResponse>($"/api/v1/Mangas/{manga.Id}");
        Assert.Equal("Berserk (editado)", despuesDeEditar!.Nombre);

        // Eliminar (204) y comprobar que ya no existe (404)
        var borrado = await admin.DeleteAsync($"/api/v1/Mangas/{manga.Id}");
        Assert.Equal(HttpStatusCode.NoContent, borrado.StatusCode);

        var yaNoExiste = await admin.GetAsync($"/api/v1/Mangas/{manga.Id}");
        Assert.Equal(HttpStatusCode.NotFound, yaNoExiste.StatusCode);
    }
}
