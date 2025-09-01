// Requests.cs
using MediatR;

// Clientes
public record CadastrarClienteCommand(string cpf, int idade, string sexo, string nome)
    : IRequest<CartaoVacinacaoResponse>;

public record LoginQuery(string cpf)
    : IRequest<CartaoVacinacaoResponse>;

// Vacinas
public record AdicionarVacinaCommand(string cpf, string nome, string data, string dose, string fabricante)
    : IRequest<CartaoVacinacaoResponse>;

public record AtualizarVacinaCommand(string cpf, string id, string nome, string data, string dose, string fabricante)
    : IRequest<CartaoVacinacaoResponse>;

public record DeletarVacinaCommand(string cpf, string id)
    : IRequest<CartaoVacinacaoResponse>;
