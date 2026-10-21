namespace ItemService.EventProcessor;

public interface IProcessaEvento
{
    Task ProcessaAsync(string mensagem, CancellationToken cancellationToken = default);
}

