namespace RestauranteService.Models;

public class Restaurante
{
    public int Id { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public string Endereco { get; private set; } = string.Empty;
    public string Site { get; private set; } = string.Empty;

    // Construtor sem parâmetros para uso exclusivo do EF Core
    protected Restaurante() { }

    public Restaurante(string nome, string endereco, string site, int id = 0)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nome);
        ArgumentException.ThrowIfNullOrWhiteSpace(endereco);
        ArgumentException.ThrowIfNullOrWhiteSpace(site);

        Id = id;
        Nome = nome;
        Endereco = endereco;
        Site = site;
    }

    public void AtualizarDados(string nome, string endereco, string site)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nome);
        ArgumentException.ThrowIfNullOrWhiteSpace(endereco);
        ArgumentException.ThrowIfNullOrWhiteSpace(site);

        Nome = nome;
        Endereco = endereco;
        Site = site;
    }
}
