namespace ItemService.Models;

public class Restaurante
{
    public int Id { get; private set; }
    public int IdExterno { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public string Endereco { get; private set; } = string.Empty;
    public string Site { get; private set; } = string.Empty;
    public ICollection<Item> Itens { get; private set; } = [];

    // Construtor sem parâmetros para uso exclusivo do EF Core
    protected Restaurante() { }

    public Restaurante(int idExterno, string nome, string endereco, string site, int id = 0)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nome);
        ArgumentException.ThrowIfNullOrWhiteSpace(endereco);
        ArgumentException.ThrowIfNullOrWhiteSpace(site);

        Id = id;
        IdExterno = idExterno;
        Nome = nome;
        Endereco = endereco;
        Site = site;
    }

    public void AdicionarItem(Item item)
    {
        ArgumentNullException.ThrowIfNull(item);
        Itens.Add(item);
    }
}