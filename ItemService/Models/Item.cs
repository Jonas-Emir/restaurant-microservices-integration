namespace ItemService.Models;

public class Item
{
    public int Id { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public decimal Preco { get; private set; }
    public int IdRestaurante { get; set; }
    public Restaurante? Restaurante { get; private set; }

    // Construtor sem parâmetros para uso exclusivo do EF Core
    protected Item() { }

    public Item(string nome, decimal preco, int idRestaurante = 0, int id = 0)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nome);
        if (preco < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(preco), "O preço do item não pode ser negativo.");
        }

        Id = id;
        Nome = nome;
        Preco = preco;
        IdRestaurante = idRestaurante;
    }

    public void AssociarAoRestaurante(int idRestaurante)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(idRestaurante);
        IdRestaurante = idRestaurante;
    }

    public void AtualizarPreco(decimal novoPreco)
    {
        if (novoPreco < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(novoPreco), "O preço do item não pode ser negativo.");
        }
        Preco = novoPreco;
    }
}