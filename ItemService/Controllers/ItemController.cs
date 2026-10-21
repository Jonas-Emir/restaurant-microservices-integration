using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ItemService.Dtos;
using ItemService.Data;
using ItemService.Models;

namespace ItemService.Controllers;

[Route("api/item/restaurante/{restauranteId}/[controller]")]
[ApiController]
public class ItemController : ControllerBase
{
    private readonly IItemRepository _repository;
    private readonly IMapper _mapper;

    public ItemController(IItemRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ItemReadDto>>> GetItensForRestaurante(int restauranteId, CancellationToken cancellationToken)
    {
        if (!await _repository.RestauranteExisteAsync(restauranteId, cancellationToken))
        {
            return NotFound();
        }

        var itens = await _repository.GetItensDeRestauranteAsync(restauranteId, cancellationToken);

        return Ok(_mapper.Map<IEnumerable<ItemReadDto>>(itens));
    }

    [HttpGet("{ItemId}", Name = "GetItemForRestaurante")]
    public async Task<ActionResult<ItemReadDto>> GetItemForRestaurante(int restauranteId, int itemId, CancellationToken cancellationToken)
    {
        if (!await _repository.RestauranteExisteAsync(restauranteId, cancellationToken))
        {
            return NotFound();
        }

        var item = await _repository.GetItemAsync(restauranteId, itemId, cancellationToken);

        if (item == null)
        {
            return NotFound();
        }

        return Ok(_mapper.Map<ItemReadDto>(item));
    }

    [HttpPost]
    public async Task<ActionResult<ItemReadDto>> CreateItemForRestaurante(int restauranteId, ItemCreateDto itemDto, CancellationToken cancellationToken)
    {
        if (!await _repository.RestauranteExisteAsync(restauranteId, cancellationToken))
        {
            return NotFound();
        }

        var item = _mapper.Map<Item>(itemDto);

        await _repository.CreateItemAsync(restauranteId, item, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        var itemReadDto = _mapper.Map<ItemReadDto>(item);

        return CreatedAtRoute(nameof(GetItemForRestaurante),
            new { restauranteId, ItemId = itemReadDto.Id }, itemReadDto);
    }

}
