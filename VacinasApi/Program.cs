// Program.cs
using MediatR;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Rodar em http://localhost:4000 para bater com seu front
builder.WebHost.UseUrls("http://localhost:4000");

// JSON básico
builder.Services.ConfigureHttpJsonOptions(opts =>
{
    opts.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

// CORS liberado para dev local
builder.Services.AddCors(o => o.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

// Repo em memória
builder.Services.AddSingleton<ICartaoRepository, InMemoryCartaoRepository>();

// MediatR: registra handlers deste assembly
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

var app = builder.Build();
app.UseCors();

// Mapas — mesmos endpoints do seu front

app.MapPost("/cadastrar", async (CadastrarClienteCommand body, IMediator mediator) =>
{
    var result = await mediator.Send(body);
    return Results.Ok(result);
});

app.MapPost("/login", async (LoginQuery body, IMediator mediator) =>
{
    try
    {
        var result = await mediator.Send(body);
        return Results.Ok(result);
    }
    catch (NotFoundException ex)
    {
        return Results.NotFound(new { message = ex.Message });
    }
});

app.MapPost("/vacina/adicionar", async (AdicionarVacinaCommand body, IMediator mediator) =>
{
    try
    {
        var result = await mediator.Send(body);
        return Results.Ok(result);
    }
    catch (NotFoundException ex)
    {
        return Results.NotFound(new { message = ex.Message });
    }
});

app.MapPost("/vacina/atualizar", async (AtualizarVacinaCommand body, IMediator mediator) =>
{
    try
    {
        var result = await mediator.Send(body);
        return Results.Ok(result);
    }
    catch (NotFoundException ex)
    {
        return Results.NotFound(new { message = ex.Message });
    }
});

app.MapPost("/vacina/deletar", async (DeletarVacinaCommand body, IMediator mediator) =>
{
    try
    {
        var result = await mediator.Send(body);
        return Results.Ok(result);
    }
    catch (NotFoundException ex)
    {
        return Results.NotFound(new { message = ex.Message });
    }
});

app.Run();
