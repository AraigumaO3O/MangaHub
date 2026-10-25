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
public class CategoriasController : ControllerBase
{
    private readonly ICatalogoRepository _catalogo;

    public CategoriasController(ICatalogoRepository catalogo)
    {
        _catalogo = catalogo;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CategoriaDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CategoriaDto>>> GetAll()
    {
        var categorias = await _catalogo.ObtenerCategoriasAsync();
        return Ok(categorias.Select(c => new CategoriaDto(c.Id, c.Nombre, c.Descripcion)));
    }
}
