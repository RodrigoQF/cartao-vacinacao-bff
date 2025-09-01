public record Pessoa
{
    public string Id { get; init; } = default!;
    public string Cpf { get; init; } = default!;
    public string Nome { get; set; } = default!;
    public int Idade { get; set; }
    public string Sexo { get; set; } = default!;
}

public record Vacina
{
    public string Id { get; init; } = default!;
    public string Nome { get; set; } = default!;
    public string Data { get; set; } = default!;   
    public string Dose { get; set; } = default!;
    public string Fabricante { get; set; } = default!;
}

public record CadastroClienteReq(string cpf, int idade, string sexo, string nome);
public record LoginReq(string cpf);
public record AdicionarVacinaReq(string cpf, string nome, string data, string dose, string fabricante);
public record AtualizarVacinaReq(string cpf, string id, string nome, string data, string dose, string fabricante);
public record DeletarVacinaReq(string cpf, string id);
