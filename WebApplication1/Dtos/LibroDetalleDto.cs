namespace WebApplication1.Dtos;

public record LibroDetalleDto(
    int ISBN,
    string Titulo,
    string Sipnosis,
    string NumeroPaginas,
    string Editorial,
    List<string> Autores
);
