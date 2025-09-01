// Models.cs

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
    // Mantido como string para casar com "YYYY-MM-DD" do front
    public string Data { get; set; } = default!;
    public string Dose { get; set; } = default!;
    public string Fabricante { get; set; } = default!;
}

//Resposta que o FRONT espera nas requisicoes
public record CartaoVacinacaoResponse(PessoaDto pessoa, List<VacinaDto> vacinas);

public record PessoaDto(string id, string cpf, string nome, int idade, string sexo);
public record VacinaDto(string id, string nome, string data, string dose, string fabricante);
