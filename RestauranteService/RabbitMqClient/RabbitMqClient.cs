using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RestauranteService.Dtos;

namespace RestauranteService.RabbitMqClient;

public class RabbitMqClient : IRabbitMqClient, IDisposable
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<RabbitMqClient> _logger;
    private readonly object _lock = new();
    private IConnection? _connection;
    private IModel? _channel;
    private bool _disposed;

    public RabbitMqClient(IConfiguration configuration, ILogger<RabbitMqClient> logger)
    {
        _configuration = configuration;
        _logger = logger;
        TentaInicializarRabbitMq();
    }

    private void TentaInicializarRabbitMq()
    {
        try
        {
            var host = _configuration["RabbitMQHost"] ?? "localhost";
            var portString = _configuration["RabbitMQPort"];
            var port = int.TryParse(portString, out var parsedPort) ? parsedPort : 5672;

            var factory = new ConnectionFactory
            {
                HostName = host,
                Port = port
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);

            _logger.LogInformation("Conexão com RabbitMQ estabelecida com sucesso no host {Host}:{Port}", host, port);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Não foi possível conectar ao RabbitMQ durante a inicialização.");
        }
    }

    public void PublicaRestaurante(RestauranteReadDto restauranteReadDto)
    {
        ArgumentNullException.ThrowIfNull(restauranteReadDto);

        lock (_lock)
        {
            if (_channel is null || !_channel.IsOpen)
            {
                _logger.LogWarning("Canal RabbitMQ fechado ou não inicializado. Tentando restabelecer conexão...");
                TentaInicializarRabbitMq();
            }

            if (_channel is null || !_channel.IsOpen)
            {
                _logger.LogError("Falha ao publicar evento no RabbitMQ: conexão indisponível.");
                return;
            }

            var mensagem = JsonSerializer.Serialize(restauranteReadDto);
            var body = Encoding.UTF8.GetBytes(mensagem);

            _channel.BasicPublish(
                exchange: "trigger",
                routingKey: string.Empty,
                basicProperties: null,
                body: body);

            _logger.LogInformation("Evento Restaurante publicado com sucesso no RabbitMQ para o Id {RestauranteId}", restauranteReadDto.Id);
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            try
            {
                if (_channel?.IsOpen == true)
                {
                    _channel.Close();
                    _channel.Dispose();
                }

                if (_connection?.IsOpen == true)
                {
                    _connection.Close();
                    _connection.Dispose();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao descartar recursos de conexão do RabbitMQ.");
            }
        }

        _disposed = true;
    }
}

