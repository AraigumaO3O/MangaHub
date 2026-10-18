using MangaHub.Api.Dtos;
using MangaHub.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace MangaHub.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class AutoresController : ControllerBase
{
    private readonly ICatalogoRepository _catalogo;

    public AutoresController(ICatalogoRepository catalogo)
    {
        _catalogo = catalogo;
    }

    /// <summary>Lista los autores.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AutorDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AutorDto>>> GetAll()
    {
        var autores = await _catalogo.ObtenerAutoresAsync();
        return Ok(autores.Select(a => new AutorDto(a.Id, a.Nombre, a.Nacionalidad)));
    }
}
