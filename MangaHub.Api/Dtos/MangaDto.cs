namespace MangaHub.Api.Dtos;

/// <summary>Datos de un manga que devuelve la API.</summary>
public record MangaDto(
    int Id,
    string Nombre,
    string? Resumen,
    int AutorId,
    string AutorNombre,
    int CategoriaId,
    string CategoriaNombre);
