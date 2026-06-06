using System.Text.Json.Serialization;
using CharacterManager.Core.Characters;
using CharacterManager.Core.Rolling;
using CharacterManager.Core.Rules;
using CharacterManager.Core.Validation;
using CharacterManager.Infrastructure.Characters;
using CharacterManager.Infrastructure.Rules;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

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
builder.Services.AddSingleton<CharacterValidator>();
builder.Services.AddSingleton<ConditionMeterChangeValidator>();
builder.Services.AddSingleton<ICharacterRepository>(_ =>
{
    var configuredRoot = builder.Configuration["CharacterStorageRoot"]
        ?? Environment.GetEnvironmentVariable("CHARACTER_MANAGER_STORAGE_ROOT");
    var storageRoot = string.IsNullOrWhiteSpace(configuredRoot)
        ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CharacterManager", "data")
        : configuredRoot;

    return new LocalJsonCharacterRepository(storageRoot);
});

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

var v1 = app.MapGroup("/api/v1");

v1.MapGet("/rulesets", async (IRulesetCatalog catalog, CancellationToken cancellationToken) =>
{
    var rulesets = await catalog.GetRulesetsAsync(cancellationToken);
    return Results.Ok(new RulesetListDto(rulesets.Select(MapRulesetSummary).ToArray()));
});

v1.MapGet("/rulesets/{rulesetId}", async (string rulesetId, IRulesetCatalog catalog, CancellationToken cancellationToken) =>
{
    var ruleset = await catalog.GetRulesetAsync(rulesetId, cancellationToken);
    return ruleset is null
        ? Results.NotFound(Error("ruleset.notFound", "Ruleset was not found.", "rulesetId"))
        : Results.Ok(MapRuleset(ruleset));
});

v1.MapGet("/characters", async (ICharacterRepository repository, CharacterValidator validator, IRulesetCatalog catalog, CancellationToken cancellationToken) =>
{
    var entries = await repository.ListAsync(cancellationToken);
    var summaries = new List<CharacterSummaryDto>();
    foreach (var entry in entries)
    {
        Ruleset? ruleset = entry.Character is null ? null : await catalog.GetRulesetAsync(entry.Character.RulesetId, cancellationToken);
        var validation = entry.Character is not null && ruleset is not null
            ? validator.Validate(entry.Character, ruleset, entry.Character.Status == CharacterStatus.Finalized)
            : null;
        summaries.Add(MapSummary(entry, validation));
    }

    return Results.Ok(new CharacterListDto(summaries));
});

v1.MapPost("/characters/drafts", async (CreateDraftRequest request, ICharacterRepository repository, IRulesetCatalog catalog, CancellationToken cancellationToken) =>
{
    var rulesetId = string.IsNullOrWhiteSpace(request.RulesetId) ? "ironsworn" : request.RulesetId;
    var ruleset = await catalog.GetRulesetAsync(rulesetId, cancellationToken);
    if (ruleset is null)
    {
        return Results.NotFound(Error("ruleset.notFound", "Ruleset was not found.", "rulesetId"));
    }

    var now = DateTimeOffset.UtcNow;
    var stats = ruleset.Stats.ToDictionary(stat => stat.Id, _ => 0);
    var character = Character.CreateDraft(ruleset.Id, ruleset.RulesetVersion, ruleset.ContentVersion, now, stats);
    character = NormalizeCharacter(character, ruleset);
    await repository.SaveAsync(character, cancellationToken);

    return Results.Created($"/api/v1/characters/{character.Id:D}", MapCharacterDto(character));
});

v1.MapGet("/characters/{characterId:guid}", async (Guid characterId, ICharacterRepository repository, CancellationToken cancellationToken) =>
{
    var loadResult = await repository.GetAsync(characterId, cancellationToken);
    if (loadResult.Character is null)
    {
        return Results.NotFound(Error(loadResult.Error?.Code ?? "character.notFound", loadResult.Error?.Message ?? "Character was not found.", loadResult.Error?.Target ?? "characterId"));
    }

    if (loadResult.Error is not null)
    {
        return Results.Json(Error(loadResult.Error.Code, loadResult.Error.Message, loadResult.Error.Target), statusCode: StatusCodes.Status422UnprocessableEntity);
    }

    return Results.Ok(MapCharacterDto(loadResult.Character));
});

v1.MapPut("/characters/{characterId:guid}", async (Guid characterId, CharacterDto dto, ICharacterRepository repository, IRulesetCatalog catalog, ConditionMeterChangeValidator meterChangeValidator, CancellationToken cancellationToken) =>
{
    if (characterId != dto.Id)
    {
        return Results.BadRequest(Error("character.idMismatch", "Route id and payload id must match.", "characterId"));
    }

    var ruleset = await catalog.GetRulesetAsync(dto.RulesetId, cancellationToken);
    if (ruleset is null)
    {
        return Results.NotFound(Error("ruleset.notFound", "Ruleset was not found.", "rulesetId"));
    }

    var previous = await repository.GetAsync(characterId, cancellationToken);
    if (previous.Character is not null)
    {
        var meterValidation = meterChangeValidator.ValidateIncreaseLocks(previous.Character, MapCharacterFromDto(dto));
        if (!meterValidation.IsValid)
        {
            return Results.BadRequest(MapValidation(meterValidation));
        }
    }

    var character = NormalizeCharacter(MapCharacterFromDto(dto) with { UpdatedAt = DateTimeOffset.UtcNow }, ruleset);
    await repository.SaveAsync(character, cancellationToken);
    return Results.Ok(MapCharacterDto(character));
});

v1.MapPost("/characters/{characterId:guid}/validate", async (Guid characterId, ICharacterRepository repository, IRulesetCatalog catalog, CharacterValidator validator, CancellationToken cancellationToken) =>
{
    var loadResult = await repository.GetAsync(characterId, cancellationToken);
    if (loadResult.Character is null)
    {
        return Results.NotFound(Error(loadResult.Error?.Code ?? "character.notFound", loadResult.Error?.Message ?? "Character was not found.", loadResult.Error?.Target ?? "characterId"));
    }

    var ruleset = await catalog.GetRulesetAsync(loadResult.Character.RulesetId, cancellationToken);
    if (ruleset is null)
    {
        return Results.Ok(MapValidation(ValidationResult.FromIssues([new ValidationIssue("ruleset.notFound", "Ruleset was not found.", "rulesetId", "concept", ValidationSeverity.Error)])));
    }

    return Results.Ok(MapValidation(validator.Validate(loadResult.Character, ruleset, loadResult.Character.Status == CharacterStatus.Finalized)));
});

v1.MapPost("/characters/{characterId:guid}/finalize", async (Guid characterId, FinalizeDraftRequest request, ICharacterRepository repository, IRulesetCatalog catalog, CharacterValidator validator, CancellationToken cancellationToken) =>
{
    var loadResult = await repository.GetAsync(characterId, cancellationToken);
    if (loadResult.Character is null)
    {
        return Results.NotFound(Error(loadResult.Error?.Code ?? "character.notFound", loadResult.Error?.Message ?? "Character was not found.", loadResult.Error?.Target ?? "characterId"));
    }

    var ruleset = await catalog.GetRulesetAsync(loadResult.Character.RulesetId, cancellationToken);
    if (ruleset is null)
    {
        return Results.NotFound(Error("ruleset.notFound", "Ruleset was not found.", "rulesetId"));
    }

    var character = NormalizeCharacter(loadResult.Character, ruleset);
    var validation = validator.Validate(character, ruleset, requireFinalizedFields: true);
    if (!validation.IsValid)
    {
        return Results.BadRequest(MapValidation(validation));
    }

    character = character with { Status = CharacterStatus.Finalized, UpdatedAt = DateTimeOffset.UtcNow };
    await repository.SaveAsync(character, cancellationToken);
    return Results.Ok(MapCharacterDto(character));
});

v1.MapDelete("/characters/{characterId:guid}", async (Guid characterId, ICharacterRepository repository, CancellationToken cancellationToken) =>
{
    if (!await repository.ExistsAsync(characterId, cancellationToken))
    {
        return Results.NotFound(Error("character.notFound", "Character was not found.", "characterId"));
    }

    await repository.DeleteAsync(characterId, cancellationToken);
    return Results.NoContent();
});

app.Run();

static Character NormalizeCharacter(Character character, Ruleset ruleset)
{
    var lockedHealth = GetLockedMeter(character, "health", "wounded", character.ConditionMeters.Health);
    var lockedSpirit = GetLockedMeter(character, "spirit", "shaken", character.ConditionMeters.Spirit);
    var lockedSupply = GetLockedMeter(character, "supply", "unprepared", character.ConditionMeters.Supply);
    var meters = new ConditionMeters(lockedHealth, lockedSpirit, lockedSupply);
    return character with
    {
        ConditionMeters = meters,
        Momentum = MomentumService.Derive(character.Momentum, character.Debilities, ruleset.Momentum)
    };
}

static MeterState GetLockedMeter(Character character, string meterId, string debilityId, MeterState meter)
{
    var isLocked = character.Debilities.Any(debility => debility.Marked && debility.DefinitionId == debilityId);
    return meter with
    {
        IsLocked = isLocked,
        LockedByDebilityId = isLocked ? debilityId : null
    };
}

static RulesetSummaryDto MapRulesetSummary(Ruleset ruleset) => new(
    ruleset.Id,
    ruleset.Name,
    ruleset.RulesetVersion,
    ruleset.ContentVersion,
    ruleset.LicensingStatus);

static RulesetDto MapRuleset(Ruleset ruleset) => new(
    ruleset.Id,
    ruleset.Name,
    ruleset.RulesetVersion,
    ruleset.ContentVersion,
    ruleset.LicensingStatus,
    ruleset.Stats,
    ruleset.StartingCharacter,
    ruleset.Meters,
    ruleset.Momentum,
    ruleset.Ranks,
    ruleset.Debilities,
    ruleset.Moves,
    ruleset.Assets);

static CharacterSummaryDto MapSummary(CharacterListEntry entry, ValidationResult? validation)
{
    if (entry.Character is null)
    {
        return new CharacterSummaryDto(
            Id: entry.Id?.ToString("D") ?? string.Empty,
            Name: null,
            DisplayName: "Invalid character record",
            Concept: null,
            RulesetId: null,
            Status: "invalid",
            IsValid: false,
            NeedsReview: true,
            LastUpdatedAt: entry.LastUpdatedAt,
            StateSummary: new CharacterStateSummaryDto(0, 0, 0, 0, 0, 0),
            Error: entry.ErrorCode is null ? null : new CharacterRecordErrorDto(entry.ErrorCode, entry.ErrorMessage ?? "Character record could not be loaded."));
    }

    var character = entry.Character;
    return new CharacterSummaryDto(
        Id: character.Id.ToString("D"),
        Name: string.IsNullOrWhiteSpace(character.Name) ? null : character.Name,
        DisplayName: string.IsNullOrWhiteSpace(character.Name) ? "Untitled draft" : character.Name,
        Concept: character.Concept,
        RulesetId: character.RulesetId,
        Status: ToContractStatus(character.Status),
        IsValid: validation?.IsValid ?? entry.IsLoadable,
        NeedsReview: entry.NeedsReview || validation is { IsValid: false },
        LastUpdatedAt: character.UpdatedAt,
        StateSummary: new CharacterStateSummaryDto(
            character.ConditionMeters.Health.Current,
            character.ConditionMeters.Spirit.Current,
            character.ConditionMeters.Supply.Current,
            character.Momentum.Current,
            character.Vows.Count(vow => vow.Status == "active"),
            character.Debilities.Count(debility => debility.Marked)),
        Error: entry.ErrorCode is null ? null : new CharacterRecordErrorDto(entry.ErrorCode, entry.ErrorMessage ?? "Character record needs review."));
}

static CharacterDto MapCharacterDto(Character character) => new(
    character.Id,
    character.SchemaVersion,
    character.RulesetId,
    character.RulesetVersion,
    character.ContentVersion,
    ToContractStatus(character.Status),
    character.Name,
    character.Concept,
    character.BackgroundNotes,
    character.EquipmentNotes,
    character.Stats,
    character.ConditionMeters,
    character.Momentum,
    character.Debilities,
    character.Assets,
    character.Bonds,
    character.Vows,
    character.ProgressTracks,
    character.Experience,
    character.CreatedAt,
    character.UpdatedAt);

static Character MapCharacterFromDto(CharacterDto dto) => new(
    dto.Id,
    dto.SchemaVersion,
    dto.RulesetId,
    dto.RulesetVersion,
    dto.ContentVersion,
    FromContractStatus(dto.Status),
    dto.Name,
    dto.Concept,
    dto.BackgroundNotes,
    dto.EquipmentNotes,
    dto.Stats,
    dto.ConditionMeters,
    dto.Momentum,
    dto.Debilities,
    dto.Assets,
    dto.Bonds,
    dto.Vows,
    dto.ProgressTracks,
    dto.Experience,
    dto.CreatedAt,
    dto.UpdatedAt);

static ValidationResultDto MapValidation(ValidationResult validation) => new(
    validation.IsValid,
    validation.Errors.Select(MapIssue).ToArray(),
    validation.Warnings.Select(MapIssue).ToArray());

static ValidationIssueDto MapIssue(ValidationIssue issue) => new(
    issue.Code,
    issue.Message,
    issue.Field,
    issue.Step,
    issue.Severity == ValidationSeverity.Error ? "error" : "warning");

static ErrorEnvelopeDto Error(string code, string message, string? target) => new(new ErrorDto(code, message, target, []));

static string ToContractStatus(CharacterStatus status) => status switch
{
    CharacterStatus.Finalized => "finalized",
    _ => "draft"
};

static CharacterStatus FromContractStatus(string status) => string.Equals(status, "finalized", StringComparison.OrdinalIgnoreCase)
    ? CharacterStatus.Finalized
    : CharacterStatus.Draft;

internal sealed record ActionRollRequest(int Modifier);

internal sealed record CreateDraftRequest(string? RulesetId);

internal sealed record FinalizeDraftRequest(bool StandardRules);

internal sealed record RulesetListDto(IReadOnlyList<RulesetSummaryDto> Items);

internal sealed record RulesetSummaryDto(string Id, string Name, string RulesetVersion, string ContentVersion, string LicensingStatus);

internal sealed record RulesetDto(
    string Id,
    string Name,
    string RulesetVersion,
    string ContentVersion,
    string LicensingStatus,
    IReadOnlyList<StatDefinition> Stats,
    StartingCharacterDefinition StartingCharacter,
    IReadOnlyList<MeterDefinition> Meters,
    MomentumRules Momentum,
    IReadOnlyList<string> Ranks,
    IReadOnlyList<DebilityDefinition> Debilities,
    IReadOnlyList<MoveDefinition> Moves,
    IReadOnlyList<AssetDefinition> Assets);

internal sealed record CharacterListDto(IReadOnlyList<CharacterSummaryDto> Items);

internal sealed record CharacterSummaryDto(
    string Id,
    string? Name,
    string DisplayName,
    string? Concept,
    string? RulesetId,
    string Status,
    bool IsValid,
    bool NeedsReview,
    DateTimeOffset? LastUpdatedAt,
    CharacterStateSummaryDto StateSummary,
    CharacterRecordErrorDto? Error);

internal sealed record CharacterStateSummaryDto(
    int Health,
    int Spirit,
    int Supply,
    int Momentum,
    int ActiveVowCount,
    int MarkedDebilityCount);

internal sealed record CharacterRecordErrorDto(string Code, string Message);

internal sealed record CharacterDto(
    Guid Id,
    int SchemaVersion,
    string RulesetId,
    string RulesetVersion,
    string ContentVersion,
    string Status,
    string Name,
    string Concept,
    string BackgroundNotes,
    string EquipmentNotes,
    IReadOnlyDictionary<string, int> Stats,
    ConditionMeters ConditionMeters,
    Momentum Momentum,
    IReadOnlyList<DebilityState> Debilities,
    IReadOnlyList<SelectedAsset> Assets,
    IReadOnlyList<Bond> Bonds,
    IReadOnlyList<Vow> Vows,
    IReadOnlyList<ProgressTrack> ProgressTracks,
    Experience Experience,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

internal sealed record ValidationResultDto(bool IsValid, IReadOnlyList<ValidationIssueDto> Errors, IReadOnlyList<ValidationIssueDto> Warnings);

internal sealed record ValidationIssueDto(string Code, string Message, string Field, string Step, string Severity);

internal sealed record ErrorEnvelopeDto(ErrorDto Error);

internal sealed record ErrorDto(string Code, string Message, string? Target, IReadOnlyList<object> Details);

public partial class Program;
