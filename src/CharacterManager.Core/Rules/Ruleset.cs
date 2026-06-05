namespace CharacterManager.Core.Rules;

public sealed record Ruleset(
    string Id,
    string Name,
    string RulesetVersion,
    string ContentVersion,
    string LicensingStatus,
    IReadOnlyList<StatDefinition> Stats,
    StartingCharacterDefinition StartingCharacter,
    IReadOnlyList<MeterDefinition> Meters,
    MomentumRules Momentum,
    IReadOnlyList<DebilityDefinition> Debilities,
    IReadOnlyList<string> Ranks,
    IReadOnlyList<string> ProgressTrackTypes,
    IReadOnlyList<MoveDefinition> Moves,
    IReadOnlyList<AssetDefinition> Assets);

public sealed record StartingCharacterDefinition(
    IReadOnlyList<int> StatArray,
    int Health,
    int Spirit,
    int Supply,
    int Momentum,
    int MomentumMax,
    int MomentumReset,
    int RequiredAssetCount,
    int MaxStartingBonds,
    int RequiredActiveVowCount);

public sealed record MeterDefinition(string Id, string Name, int Minimum, int Maximum);

public sealed record MomentumRules(
    int Minimum,
    int BaseMax,
    IReadOnlyList<MomentumResetRule> ResetByMarkedDebilityCount);

public sealed record MomentumResetRule(int MarkedCount, int Reset);

public sealed record DebilityDefinition(
    string Id,
    string Name,
    string Category,
    string? BlocksMeterIncrease,
    bool IsPermanentByDefault,
    bool RequiresLinkedVow,
    string? SourceReference);
