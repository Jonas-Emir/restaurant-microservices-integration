using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RestauranteService.Data;
using RestauranteService.Dtos;
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
    private IItemServiceHttpClient _itemServiceHttpClient;
    private IRabbitMqClient _rabbitMqClient;

    public RestauranteController(
        IRestauranteRepository repository,
        IMapper mapper, IItemServiceHttpClient itemServiceHttpClient, IRabbitMqClient rabbitMqClient)
    {
        _repository = repository;
        _mapper = mapper;
        _itemServiceHttpClient = itemServiceHttpClient;
        _rabbitMqClient = rabbitMqClient;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RestauranteReadDto>>> GetAllRestaurantes(CancellationToken cancellationToken)
    {
        var restaurantes = await _repository.GetAllRestaurantesAsync(cancellationToken);

        return Ok(_mapper.Map<IEnumerable<RestauranteReadDto>>(restaurantes));
    }

    [HttpGet("{id}", Name = "GetRestauranteById")]
    public async Task<ActionResult<RestauranteReadDto>> GetRestauranteById(int id, CancellationToken cancellationToken)
    {
        var restaurante = await _repository.GetRestauranteByIdAsync(id, cancellationToken);
        if (restaurante != null)
        {
            return Ok(_mapper.Map<RestauranteReadDto>(restaurante));
        }

        return NotFound();
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
