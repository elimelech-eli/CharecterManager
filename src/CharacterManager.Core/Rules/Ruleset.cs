namespace CharacterManager.Core.Rules;

public sealed record Ruleset(
    string Id,
    string Name,
    string Version,
    IReadOnlyList<StatDefinition> Stats,
    IReadOnlyList<MoveDefinition> Moves,
    IReadOnlyList<AssetDefinition> Assets);
