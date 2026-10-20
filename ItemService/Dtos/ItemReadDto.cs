namespace ItemService.Dtos;

public record ItemReadDto(
    int Id,
    string Nome,
    decimal Preco,
    int IdRestaurante);