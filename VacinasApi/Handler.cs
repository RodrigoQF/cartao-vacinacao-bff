// Handlers.cs
using MediatR;

public class CadastrarClienteHandler : IRequestHandler<CadastrarClienteCommand, CartaoVacinacaoResponse>
{
    private readonly ICartaoRepository _repo;
    public CadastrarClienteHandler(ICartaoRepository repo) => _repo = repo;

    public Task<CartaoVacinacaoResponse> Handle(CadastrarClienteCommand request, CancellationToken ct)
    {
        var pessoa = _repo.GetOrCreatePessoa(request.cpf, request.nome, request.idade, request.sexo);
        var vacinas = _repo.GetVacinas(request.cpf);
        return Task.FromResult(Mapping.ToCartao(pessoa, vacinas));
    }
}

public class LoginHandler : IRequestHandler<LoginQuery, CartaoVacinacaoResponse>
{
    private readonly ICartaoRepository _repo;
    public LoginHandler(ICartaoRepository repo) => _repo = repo;

    public Task<CartaoVacinacaoResponse> Handle(LoginQuery request, CancellationToken ct)
    {
        if (!_repo.TryGetPessoa(request.cpf, out var pessoa))
            throw new NotFoundException("CPF não cadastrado.");
        var vacinas = _repo.GetVacinas(request.cpf);
        return Task.FromResult(Mapping.ToCartao(pessoa, vacinas));
    }
}

public class AdicionarVacinaHandler : IRequestHandler<AdicionarVacinaCommand, CartaoVacinacaoResponse>
{
    private readonly ICartaoRepository _repo;
    public AdicionarVacinaHandler(ICartaoRepository repo) => _repo = repo;

    public Task<CartaoVacinacaoResponse> Handle(AdicionarVacinaCommand request, CancellationToken ct)
    {
        if (!_repo.TryGetPessoa(request.cpf, out var pessoa))
            throw new NotFoundException("CPF não cadastrado.");
        _repo.AddVacina(request.cpf, request.nome, request.data, request.dose, request.fabricante);
        var vacinas = _repo.GetVacinas(request.cpf);
        return Task.FromResult(Mapping.ToCartao(pessoa!, vacinas));
    }
}

public class AtualizarVacinaHandler : IRequestHandler<AtualizarVacinaCommand, CartaoVacinacaoResponse>
{
    private readonly ICartaoRepository _repo;
    public AtualizarVacinaHandler(ICartaoRepository repo) => _repo = repo;

    public Task<CartaoVacinacaoResponse> Handle(AtualizarVacinaCommand request, CancellationToken ct)
    {
        if (!_repo.TryGetPessoa(request.cpf, out var pessoa))
            throw new NotFoundException("CPF não cadastrado.");
        var updated = _repo.UpdateVacina(request.cpf, request.id, request.nome, request.data, request.dose, request.fabricante);
        if (updated is null) throw new NotFoundException("Vacina não encontrada.");
        var vacinas = _repo.GetVacinas(request.cpf);
        return Task.FromResult(Mapping.ToCartao(pessoa!, vacinas));
    }
}

public class DeletarVacinaHandler : IRequestHandler<DeletarVacinaCommand, CartaoVacinacaoResponse>
{
    private readonly ICartaoRepository _repo;
    public DeletarVacinaHandler(ICartaoRepository repo) => _repo = repo;

    public Task<CartaoVacinacaoResponse> Handle(DeletarVacinaCommand request, CancellationToken ct)
    {
        if (!_repo.TryGetPessoa(request.cpf, out var pessoa))
            throw new NotFoundException("CPF não cadastrado.");
        _repo.DeleteVacina(request.cpf, request.id);
        var vacinas = _repo.GetVacinas(request.cpf);
        return Task.FromResult(Mapping.ToCartao(pessoa!, vacinas));
    }
}
