namespace MangaHub.Api.Entities;

public class EntradaBiblioteca
{
    public int UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }

    public int MangaId { get; set; }
    public Manga? Manga { get; set; }

    public EstadoLectura Estado { get; set; } = EstadoLectura.Pendiente;
    public int CapituloActual { get; set; }
}
