using AutoMapper;
using ItemService.Data;
using ItemService.Dtos;
using ItemService.Exceptions;
using ItemService.Extensions;
using ItemService.Models;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<ActionResult<PagedResult<ItemReadDto>>> GetItensForRestaurante(
        int restauranteId,
        [FromQuery] PaginationParams paginationParams,
        CancellationToken cancellationToken)
    {
        if (!await _repository.RestauranteExisteAsync(restauranteId, cancellationToken))
        {
            throw new NotFoundException($"Restaurante com Id {restauranteId} não foi encontrado.");
        }

        var pagedItens = await _repository.GetItensDeRestaurantePaginadosAsync(restauranteId, paginationParams, cancellationToken);
        Response.AddPaginationHeader(pagedItens);

        return Ok(pagedItens.Map(i => _mapper.Map<ItemReadDto>(i)));
    }

    [HttpGet("{ItemId}", Name = "GetItemForRestaurante")]
    public async Task<ActionResult<ItemReadDto>> GetItemForRestaurante(int restauranteId, int itemId, CancellationToken cancellationToken)
    {
        if (!await _repository.RestauranteExisteAsync(restauranteId, cancellationToken))
        {
            throw new NotFoundException($"Restaurante com Id {restauranteId} não foi encontrado.");
        }

        var item = await _repository.GetItemAsync(restauranteId, itemId, cancellationToken);

        if (item is null)
        {
            throw new NotFoundException($"Item com Id {itemId} não foi encontrado para o restaurante {restauranteId}.");
        }

        return Ok(_mapper.Map<ItemReadDto>(item));
    }

    [HttpPost]
    public async Task<ActionResult<ItemReadDto>> CreateItemForRestaurante(int restauranteId, ItemCreateDto itemDto, CancellationToken cancellationToken)
    {
        if (!await _repository.RestauranteExisteAsync(restauranteId, cancellationToken))
        {
            throw new NotFoundException($"Restaurante com Id {restauranteId} não foi encontrado.");
        }

        var item = _mapper.Map<Item>(itemDto);

        await _repository.CreateItemAsync(restauranteId, item, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        var itemReadDto = _mapper.Map<ItemReadDto>(item);

        return CreatedAtRoute(nameof(GetItemForRestaurante),
            new { restauranteId, ItemId = itemReadDto.Id }, itemReadDto);
    }

}
