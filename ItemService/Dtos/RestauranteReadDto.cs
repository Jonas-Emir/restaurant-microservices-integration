namespace ItemService.Dtos;

public record RestauranteReadDto(
    int Id,
    string Nome,
    string Endereco,
    string Site);