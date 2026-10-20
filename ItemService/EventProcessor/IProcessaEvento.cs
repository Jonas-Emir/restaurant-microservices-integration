namespace ItemService.EventProcessor;

public interface IProcessaEvento
{
    Task ProcessaAsync(string mensagem, CancellationToken cancellationToken = default);

    [Obsolete("Utilize ProcessaAsync para evitar sync-over-async")]
    void Processa(string mensagem);
}

