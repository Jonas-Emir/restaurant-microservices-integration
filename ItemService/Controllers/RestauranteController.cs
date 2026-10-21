using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ItemService.Data;
using ItemService.Dtos;

namespace ItemService.Controllers;

[Route("api/item/[controller]")]
[ApiController]
public class RestauranteController : ControllerBase
{
    private readonly IItemRepository _repository;
    private readonly IMapper _mapper;

    public RestauranteController(IItemRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RestauranteReadDto>>> GetRestaurantes(CancellationToken cancellationToken)
    {
        var restaurantes = await _repository.GetAllRestaurantesAsync(cancellationToken);

        return Ok(_mapper.Map<IEnumerable<RestauranteReadDto>>(restaurantes));
    }

    [HttpPost]
    public ActionResult RecebeRestauranteDoRestauranteService(RestauranteReadDto dto)
    {
        Console.WriteLine(dto.Id);
        return Ok();
    }
}
