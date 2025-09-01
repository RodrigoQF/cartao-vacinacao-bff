// Repository.cs
using System.Collections.Concurrent;

public interface ICartaoRepository
{
    Pessoa GetOrCreatePessoa(string cpf, string nome, int idade, string sexo);
    bool TryGetPessoa(string cpf, out Pessoa pessoa);
    List<Vacina> GetVacinas(string cpf);
    Vacina AddVacina(string cpf, string nome, string data, string dose, string fabricante);
    Vacina? UpdateVacina(string cpf, string id, string nome, string data, string dose, string fabricante);
    bool DeleteVacina(string cpf, string id);
}

public class InMemoryCartaoRepository : ICartaoRepository
{
    private readonly ConcurrentDictionary<string, Pessoa> _pessoas =
        new(StringComparer.OrdinalIgnoreCase);

    private readonly ConcurrentDictionary<string, List<Vacina>> _vacinasPorCpf =
        new(StringComparer.OrdinalIgnoreCase);

    private static string NewId(string prefix) => prefix + Guid.NewGuid().ToString("N")[..8];

    public Pessoa GetOrCreatePessoa(string cpf, string nome, int idade, string sexo)
    {
        var pessoa = _pessoas.GetOrAdd(cpf, _ =>
        {
            var p = new Pessoa
            {
                Id = NewId("p"),
                Cpf = cpf,
                Nome = nome,
                Idade = idade,
                Sexo = sexo
            };
            _vacinasPorCpf.TryAdd(cpf, new List<Vacina>());
            return p;
        });

        // Se já existia, atualiza os dados cadastrais
        pessoa.Nome = nome;
        pessoa.Idade = idade;
        pessoa.Sexo = sexo;

        return pessoa;
    }

    public bool TryGetPessoa(string cpf, out Pessoa pessoa) => _pessoas.TryGetValue(cpf, out pessoa!);

    public List<Vacina> GetVacinas(string cpf)
    {
        if (_vacinasPorCpf.TryGetValue(cpf, out var list)) return list;
        var nova = new List<Vacina>();
        _vacinasPorCpf[cpf] = nova;
        return nova;
    }

    public Vacina AddVacina(string cpf, string nome, string data, string dose, string fabricante)
    {
        var list = GetVacinas(cpf);
        var vacina = new Vacina
        {
            Id = NewId("v"),
            Nome = nome,
            Data = data,
            Dose = dose,
            Fabricante = fabricante
        };
        list.Add(vacina);
        return vacina;
    }

    public Vacina? UpdateVacina(string cpf, string id, string nome, string data, string dose, string fabricante)
    {
        var list = GetVacinas(cpf);
        var v = list.FirstOrDefault(x => string.Equals(x.Id, id, StringComparison.OrdinalIgnoreCase));
        if (v is null) return null;
        v.Nome = nome;
        v.Data = data;
        v.Dose = dose;
        v.Fabricante = fabricante;
        return v;
    }

    public bool DeleteVacina(string cpf, string id)
    {
        var list = GetVacinas(cpf);
        var removed = list.RemoveAll(v => string.Equals(v.Id, id, StringComparison.OrdinalIgnoreCase));
        return removed > 0;
    }
}

// Helpers de mapeamento para DTO esperado pelo front
public static class Mapping
{
    public static CartaoVacinacaoResponse ToCartao(Pessoa p, List<Vacina> vacinas)
    {
        var pessoa = new PessoaDto(p.Id, p.Cpf, p.Nome, p.Idade, p.Sexo);
        var vacinasDto = vacinas.Select(v => new VacinaDto(v.Id, v.Nome, v.Data, v.Dose, v.Fabricante)).ToList();
        return new CartaoVacinacaoResponse(pessoa, vacinasDto);
    }
}

// Exceção simples para 404
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}
