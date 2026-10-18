namespace MangaHub.Api.Entities;

public class Autor
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Nacionalidad { get; set; }
    public string? Biografia { get; set; }

    public List<Manga> Mangas { get; set; } = new();
}
