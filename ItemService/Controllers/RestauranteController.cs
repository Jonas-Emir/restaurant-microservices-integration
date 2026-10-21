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
    public async Task<ActionResult<PagedResult<RestauranteReadDto>>> GetRestaurantes(
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

    [HttpPost]
    public ActionResult RecebeRestauranteDoRestauranteService(RestauranteReadDto dto)
    {
        Console.WriteLine(dto.Id);
        return Ok();
    }
}
