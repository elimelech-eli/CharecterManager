using CharacterManager.Core.Rolling;
using CharacterManager.Infrastructure.Rules;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .SetIsOriginAllowed(origin => origin is "http://127.0.0.1:5173" or "null")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
builder.Services.AddSingleton<IRoller, RandomRoller>();
builder.Services.AddSingleton<IRulesetCatalog, InMemoryRulesetCatalog>();

var app = builder.Build();

app.UseCors();

app.MapGet("/health", () => new
{
    service = "CharacterManager.Api",
    version = typeof(Program).Assembly.GetName().Version?.ToString() ?? "0.1.0"
});

app.MapGet("/rulesets", async (IRulesetCatalog catalog, CancellationToken cancellationToken) =>
{
    return await catalog.GetRulesetsAsync(cancellationToken);
});

app.MapPost("/rolls/action", (ActionRollRequest request, IRoller roller) =>
{
    return roller.RollAction(request.Modifier);
});

app.Run();

internal sealed record ActionRollRequest(int Modifier);
