using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RestauranteService.Data;
using RestauranteService.Dtos;
using RestauranteService.Exceptions;
using RestauranteService.ItemServiceHttpClient;
using RestauranteService.Models;
using RestauranteService.RabbitMqClient;

namespace RestauranteService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RestauranteController : ControllerBase
{
    private readonly IRestauranteRepository _repository;
    private readonly IMapper _mapper;
    private readonly IItemServiceHttpClient _itemServiceHttpClient;
    private readonly IRabbitMqClient _rabbitMqClient;

    public RestauranteController(
        IRestauranteRepository repository,
        IMapper mapper,
        IItemServiceHttpClient itemServiceHttpClient,
        IRabbitMqClient rabbitMqClient)
    {
        _repository = repository;
        _mapper = mapper;
        _itemServiceHttpClient = itemServiceHttpClient;
        _rabbitMqClient = rabbitMqClient;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<RestauranteReadDto>>> GetAllRestaurantes(
        [FromQuery] PaginationParams paginationParams,
        CancellationToken cancellationToken)
    {
        var pagedRestaurantes = await _repository.GetRestaurantesPaginadosAsync(paginationParams, cancellationToken);
        var dtos = _mapper.Map<IReadOnlyList<RestauranteReadDto>>(pagedRestaurantes.Items);

        return Ok(new PagedResult<RestauranteReadDto>(
            dtos,
            pagedRestaurantes.PageNumber,
            pagedRestaurantes.PageSize,
            pagedRestaurantes.TotalItems));
    }

    [HttpGet("{id}", Name = "GetRestauranteById")]
    public async Task<ActionResult<RestauranteReadDto>> GetRestauranteById(int id, CancellationToken cancellationToken)
    {
        var restaurante = await _repository.GetRestauranteByIdAsync(id, cancellationToken);
        if (restaurante is null)
        {
            throw new NotFoundException($"Restaurante com Id {id} não foi encontrado.");
        }

        return Ok(_mapper.Map<RestauranteReadDto>(restaurante));
    }

    [HttpPost]
    public async Task<ActionResult<RestauranteReadDto>> CreateRestaurante(RestauranteCreateDto restauranteCreateDto, CancellationToken cancellationToken)
    {
        var restaurante = _mapper.Map<Restaurante>(restauranteCreateDto);
        await _repository.CreateRestauranteAsync(restaurante, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        var restauranteReadDto = _mapper.Map<RestauranteReadDto>(restaurante);

        //_itemServiceHttpClient.EnviaRestauranteParaItemService(restauranteReadDto);

        _rabbitMqClient.PublicaRestaurante(restauranteReadDto);

        return CreatedAtRoute(nameof(GetRestauranteById), new { restauranteReadDto.Id },
            restauranteReadDto);
    }
}
