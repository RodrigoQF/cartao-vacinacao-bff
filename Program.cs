// Program.cs
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://localhost:4000");

builder.Services.ConfigureHttpJsonOptions(opts =>
{
    opts.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

// Permitir requisicoes locais
builder.Services.AddCors(o => o.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();
app.UseCors();

// -------- Banco de dados de memoria --------
var pessoasByCpf = new Dictionary<string, Pessoa>(StringComparer.OrdinalIgnoreCase);
var vacinasByCpf  = new Dictionary<string, List<Vacina>>(StringComparer.OrdinalIgnoreCase);

string NewId(string prefix = "") => prefix + Guid.NewGuid().ToString("N")[..8];


//Mapeando json de resposta
object Cartao(Pessoa p, List<Vacina> vacinas) => new
{
    pessoa = new { id = p.Id, cpf = p.Cpf, nome = p.Nome, idade = p.Idade, sexo = p.Sexo },
    vacinas = vacinas 
};

// -------- Endpoints --------

// POST /cadastrar -> serviço de cadsatro de cliente
app.MapPost("/cadastrar", (CadastroClienteReq body) =>
{
    if (!pessoasByCpf.TryGetValue(body.cpf, out var pessoa))
    {
        pessoa = new Pessoa
        {
            Id = NewId("p"),
            Cpf = body.cpf,
            Nome = body.nome,
            Idade = body.idade,
            Sexo = body.sexo
        };
        pessoasByCpf[body.cpf] = pessoa;
        vacinasByCpf[body.cpf] = new List<Vacina>();
    }
    else
    {
        // Permite pessoa já incluida realizar uma 'atualizacao' sem tomar erro
        pessoa.Nome = body.nome;
        pessoa.Idade = body.idade;
        pessoa.Sexo = body.sexo;
    }

    return Results.Ok(Cartao(pessoa, vacinasByCpf[body.cpf]));
});

// POST /login -> Retorna sempre as vacinas / Caso cliente nao exista um 404
app.MapPost("/login", (LoginReq body) =>
{
    if (!pessoasByCpf.TryGetValue(body.cpf, out var pessoa))
        return Results.NotFound(new { message = "CPF não cadastrado." });

    var vacinas = vacinasByCpf.TryGetValue(body.cpf, out var list) ? list : new List<Vacina>();
    return Results.Ok(Cartao(pessoa, vacinas));
});

// POST /vacina/adicionar
app.MapPost("/vacina/adicionar", (AdicionarVacinaReq body) =>
{
    if (!pessoasByCpf.TryGetValue(body.cpf, out var pessoa))
        return Results.NotFound(new { message = "CPF não cadastrado." });

    var vacinas = vacinasByCpf[body.cpf];

    var vacina = new Vacina
    {
        Id = NewId("v"),
        Nome = body.nome,
        Data = body.data,
        Dose = body.dose,
        Fabricante = body.fabricante
    };
    vacinas.Add(vacina);

    return Results.Ok(Cartao(pessoa, vacinas));
});

// POST /vacina/atualizar
app.MapPost("/vacina/atualizar", (AtualizarVacinaReq body) =>
{
    if (!pessoasByCpf.TryGetValue(body.cpf, out var pessoa))
        return Results.NotFound(new { message = "CPF não cadastrado." });

    var vacinas = vacinasByCpf[body.cpf];
    var v = vacinas.FirstOrDefault(x => string.Equals(x.Id, body.id, StringComparison.OrdinalIgnoreCase));
    if (v is null) return Results.NotFound(new { message = "Vacina não encontrada." });

    v.Nome = body.nome;
    v.Data = body.data;
    v.Dose = body.dose;
    v.Fabricante = body.fabricante;

    return Results.Ok(Cartao(pessoa, vacinas));
});

// POST /vacina/deletar
app.MapPost("/vacina/deletar", (DeletarVacinaReq body) =>
{
    if (!pessoasByCpf.TryGetValue(body.cpf, out var pessoa))
        return Results.NotFound(new { message = "CPF não cadastrado." });

    var vacinas = vacinasByCpf[body.cpf];
    vacinas.RemoveAll(v => string.Equals(v.Id, body.id, StringComparison.OrdinalIgnoreCase));

    return Results.Ok(Cartao(pessoa, vacinas));
});

app.Run();
