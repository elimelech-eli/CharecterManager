namespace CharacterManager.Core.Rules;

public sealed record StatDefinition(
    string Id,
    string Name,
    int Minimum,
    int Maximum,
    string Summary,
    string? SourceReference);
