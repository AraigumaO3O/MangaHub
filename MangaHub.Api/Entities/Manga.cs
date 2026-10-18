namespace MangaHub.Api.Entities;

public class Manga
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Resumen { get; set; }

    public int AutorId { get; set; }
    public Autor? Autor { get; set; }

    public int CategoriaId { get; set; }
    public Categoria? Categoria { get; set; }
}
