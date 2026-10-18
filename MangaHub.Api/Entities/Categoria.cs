namespace MangaHub.Api.Entities;

public class Categoria
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }

    public List<Manga> Mangas { get; set; } = new();
}
