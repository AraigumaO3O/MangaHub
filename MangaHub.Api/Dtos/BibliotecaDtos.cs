using System.ComponentModel.DataAnnotations;
using MangaHub.Api.Entities;

namespace MangaHub.Api.Dtos;


public class EntradaRequestDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Debes indicar un manga válido.")]
    public int MangaId { get; set; }

    [EnumDataType(typeof(EstadoLectura), ErrorMessage = "El estado debe ser Pendiente, Leyendo o Terminado.")]
    public EstadoLectura Estado { get; set; } = EstadoLectura.Pendiente;

    [Range(0, 100000, ErrorMessage = "El capítulo actual debe ser un número entre 0 y 100000.")]
    public int CapituloActual { get; set; }
}


public class EntradaUpdateDto
{
    [EnumDataType(typeof(EstadoLectura), ErrorMessage = "El estado debe ser Pendiente, Leyendo o Terminado.")]
    public EstadoLectura Estado { get; set; }

    [Range(0, 100000, ErrorMessage = "El capítulo actual debe ser un número entre 0 y 100000.")]
    public int CapituloActual { get; set; }
}

public record EntradaDto(int MangaId, string MangaNombre, EstadoLectura Estado, int CapituloActual);
