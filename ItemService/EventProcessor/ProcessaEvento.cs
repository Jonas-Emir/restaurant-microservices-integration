using System.Text.Json;
using AutoMapper;
using ItemService.Data;
using ItemService.Dtos;
using ItemService.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ItemService.EventProcessor;

public class ProcessaEvento : IProcessaEvento
{
    private readonly IMapper _mapper;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ProcessaEvento> _logger;

    public ProcessaEvento(
        IMapper mapper,
        IServiceScopeFactory scopeFactory,
        ILogger<ProcessaEvento> logger)
    {
        _mapper = mapper;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task ProcessaAsync(string mensagem, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(mensagem);

        var restauranteReadDto = JsonSerializer.Deserialize<RestauranteReadDto>(
            mensagem,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (restauranteReadDto is null)
        {
            _logger.LogWarning("Mensagem recebida do RabbitMQ é inválida ou não pôde ser desserializada.");
            return;
        }

        using var scope = _scopeFactory.CreateScope();
        var itemRepository = scope.ServiceProvider.GetRequiredService<IItemRepository>();

        var restaurante = _mapper.Map<Restaurante>(restauranteReadDto);

        var existe = await itemRepository.ExisteRestauranteExternoAsync(restaurante.IdExterno, cancellationToken);
        if (!existe)
        {
            await itemRepository.CreateRestauranteAsync(restaurante, cancellationToken);
            await itemRepository.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Restaurante externo com IdExterno {IdExterno} ('{Nome}') cadastrado com sucesso.", restaurante.IdExterno, restaurante.Nome);
        }
        else
        {
            _logger.LogInformation("Restaurante externo com IdExterno {IdExterno} já existe. Evento ignorado por idempotência.", restaurante.IdExterno);
        }
    }

    [Obsolete("Utilize ProcessaAsync para evitar sync-over-async")]
    public void Processa(string mensagem)
    {
        ProcessaAsync(mensagem).GetAwaiter().GetResult();
    }
}
