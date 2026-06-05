namespace CharacterManager.Core.Characters;

public sealed record Character(
    Guid Id,
    int SchemaVersion,
    string RulesetId,
    string RulesetVersion,
    string ContentVersion,
    CharacterStatus Status,
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
    DateTimeOffset UpdatedAt)
{
    public const int CurrentSchemaVersion = 1;

    public static Character CreateDraft(
        string rulesetId,
        string rulesetVersion,
        string contentVersion,
        DateTimeOffset now,
        IReadOnlyDictionary<string, int>? stats = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(rulesetId);

        return new Character(
            Id: Guid.NewGuid(),
            SchemaVersion: CurrentSchemaVersion,
            RulesetId: rulesetId.Trim(),
            RulesetVersion: rulesetVersion,
            ContentVersion: contentVersion,
            Status: CharacterStatus.Draft,
            Name: string.Empty,
            Concept: string.Empty,
            BackgroundNotes: string.Empty,
            EquipmentNotes: string.Empty,
            Stats: stats ?? new Dictionary<string, int>(),
            ConditionMeters: ConditionMeters.Starting(),
            Momentum: Momentum.Starting(),
            Debilities: [],
            Assets: [],
            Bonds: [],
            Vows: [],
            ProgressTracks: [],
            Experience: new Experience(0, 0),
            CreatedAt: now,
            UpdatedAt: now);
    }
}

public enum CharacterStatus
{
    Draft,
    Finalized
}

public sealed record ConditionMeters(MeterState Health, MeterState Spirit, MeterState Supply)
{
    public static ConditionMeters Starting() => new(
        new MeterState(5, 0, 5, false, null),
        new MeterState(5, 0, 5, false, null),
        new MeterState(5, 0, 5, false, null));

    public int GetCurrent(string meterId) => meterId switch
    {
        "health" => Health.Current,
        "spirit" => Spirit.Current,
        "supply" => Supply.Current,
        _ => throw new ArgumentOutOfRangeException(nameof(meterId), meterId, "Unknown condition meter.")
    };
}

public sealed record MeterState(int Current, int Min, int Max, bool IsLocked, string? LockedByDebilityId);

public sealed record Momentum(int Current, int Max, int Reset, int Minimum, int MarkedDebilityCount)
{
    public static Momentum Starting() => new(Current: 2, Max: 10, Reset: 2, Minimum: -6, MarkedDebilityCount: 0);
}

public sealed record DebilityState(
    string DefinitionId,
    bool Marked,
    string Notes,
    string? LinkedVowId,
    DateTimeOffset? MarkedAt,
    DateTimeOffset? ClearedAt);

public sealed record SelectedAsset(
    string DefinitionId,
    IReadOnlyList<string> SelectedAbilityIds,
    string? CompanionName,
    int? CompanionHealth,
    string Notes,
    string? SourceReference);

public sealed record Vow(
    string Id,
    string Title,
    string Description,
    string Rank,
    string Status,
    string ProgressTrackId,
    bool IsBackgroundVow,
    bool IsIncitingIncident,
    int ExperienceAwarded,
    string Notes,
    DateTimeOffset CreatedAt,
    DateTimeOffset? CompletedAt);

public sealed record Bond(
    string Id,
    string Name,
    string Type,
    string Description,
    string Location,
    string Status,
    string Notes,
    bool CreatedFromBackground,
    DateTimeOffset CreatedAt);

public sealed record Experience(int Earned, int Spent)
{
    public int Available => Earned - Spent;
}
