using System.ComponentModel.DataAnnotations;

namespace ItemService.Dtos;

public record ItemCreateDto(
    [Required] string Nome,
    [Required] decimal Preco);