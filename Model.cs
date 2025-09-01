public record Pessoa
{
    public string Id { get; init; } = default!;
    public string Cpf { get; init; } = default!;
    public string Nome { get; set; } = default!;
    public int Idade { get; set; }
    public string Sexo { get; set; } = default!;
}
