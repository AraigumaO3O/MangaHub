using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using MangaHub.Api.Dtos;
using MangaHub.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace MangaHub.Api.Controllers;

[ApiController]
[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class AutoresController : ControllerBase
{
    private readonly ICatalogoRepository _catalogo;

    public AutoresController(ICatalogoRepository catalogo)
    {
        _catalogo = catalogo;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AutorDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AutorDto>>> GetAll()
    {
        var autores = await _catalogo.ObtenerAutoresAsync();
        return Ok(autores.Select(a => new AutorDto(a.Id, a.Nombre, a.Nacionalidad)));
    }
}
