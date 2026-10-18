using System.ComponentModel.DataAnnotations;

namespace MangaHub.Api.Dtos;

/// <summary>Datos que se envían para crear o actualizar un manga.</summary>
public class MangaRequestDto
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(150, ErrorMessage = "El nombre no puede superar los 150 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(2000, ErrorMessage = "El resumen no puede superar los 2000 caracteres.")]
    public string? Resumen { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Debes indicar un autor válido.")]
    public int AutorId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Debes indicar una categoría válida.")]
    public int CategoriaId { get; set; }
}
