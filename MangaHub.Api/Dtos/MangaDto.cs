namespace MangaHub.Api.Dtos;


public record MangaDto(
    int Id,
    string Nombre,
    string? Resumen,
    int AutorId,
    string AutorNombre,
    int CategoriaId,
    string CategoriaNombre);
