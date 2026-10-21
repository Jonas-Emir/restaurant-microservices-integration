using System.ComponentModel.DataAnnotations;

namespace RestauranteService.Dtos;

public record RestauranteCreateDto(
    [Required] string Nome,
    [Required] string Endereco,
    [Required] string Site);