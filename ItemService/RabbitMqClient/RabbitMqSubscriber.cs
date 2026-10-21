using System.Text;
using ItemService.EventProcessor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ItemService.RabbitMqClient;

public class RabbitMqSubscriber : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly IProcessaEvento _processaEvento;
    private readonly ILogger<RabbitMqSubscriber> _logger;
    private IConnection? _connection;
    private IModel? _channel;
    private string? _nomeDaFila;

    public RabbitMqSubscriber(
        IConfiguration configuration,
        IProcessaEvento processaEvento,
        ILogger<RabbitMqSubscriber> logger)
    {
        _configuration = configuration;
        _processaEvento = processaEvento;
        _logger = logger;
    }

    private bool ConectarRabbitMq()
    {
        try
        {
            var host = _configuration["RabbitMQHost"] ?? "localhost";
            var portString = _configuration["RabbitMQPort"];
            var port = int.TryParse(portString, out var parsedPort) ? parsedPort : 5672;

            var factory = new ConnectionFactory
            {
                HostName = host,
                Port = port,
                DispatchConsumersAsync = true
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
            _nomeDaFila = _channel.QueueDeclare().QueueName;
            _channel.QueueBind(queue: _nomeDaFila, exchange: "trigger", routingKey: string.Empty);

            _logger.LogInformation("RabbitMqSubscriber conectado com sucesso ao broker em {Host}:{Port}, fila vinculada: {Fila}", host, port, _nomeDaFila);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Falha ao conectar ao RabbitMQ no host {Host}. Nova tentativa em 5 segundos...", _configuration["RabbitMQHost"]);
            return false;
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested && (_channel is null || !_channel.IsOpen))
        {
            if (ConectarRabbitMq())
            {
                break;
            }

            try
            {
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                return;
            }
        }

        if (_channel is null || string.IsNullOrEmpty(_nomeDaFila))
        {
            _logger.LogError("Não foi possível iniciar o consumidor do RabbitMQ: canal ou fila indisponíveis.");
            return;
        }

        var consumidor = new AsyncEventingBasicConsumer(_channel);

        consumidor.Received += async (_, ea) =>
        {
            var body = ea.Body.ToArray();
            var mensagem = Encoding.UTF8.GetString(body);

            try
            {
                _logger.LogInformation("Mensagem recebida do RabbitMQ (DeliveryTag: {DeliveryTag})", ea.DeliveryTag);
                await _processaEvento.ProcessaAsync(mensagem, stoppingToken);
                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro no processamento da mensagem (DeliveryTag: {DeliveryTag}). Rejeitando sem reenfileirar.", ea.DeliveryTag);
                _channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
            }
        };

        _channel.BasicConsume(queue: _nomeDaFila, autoAck: false, consumer: consumidor);

        try
        {
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("RabbitMqSubscriber está sendo cancelado e desligado.");
        }
    }

    public override void Dispose()
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
            _logger.LogError(ex, "Erro ao descartar conexão ou canal do RabbitMQ em RabbitMqSubscriber.");
        }

        base.Dispose();
    }
}