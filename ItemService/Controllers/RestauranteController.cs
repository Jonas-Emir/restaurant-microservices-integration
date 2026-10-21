using AutoMapper;
using ItemService.Data;
using ItemService.Dtos;
using ItemService.Extensions;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<ActionResult<PagedResult<RestauranteReadDto>>> GetRestaurantes(
        [FromQuery] PaginationParams paginationParams,
        CancellationToken cancellationToken)
    {
        var pagedRestaurantes = await _repository.GetRestaurantesPaginadosAsync(paginationParams, cancellationToken);
        Response.AddPaginationHeader(pagedRestaurantes);

        return Ok(pagedRestaurantes.Map(r => _mapper.Map<RestauranteReadDto>(r)));
    }

    [HttpPost]
    public ActionResult RecebeRestauranteDoRestauranteService(RestauranteReadDto dto)
    {
        Console.WriteLine(dto.Id);
        return Ok();
    }
}
