namespace CharacterManager.Core.Rules;

public sealed record AssetDefinition(
    string Id,
    string Name,
    string Type,
    string Summary,
    IReadOnlyList<AssetAbilityDefinition> Abilities,
    bool RequiresCompanionName,
    bool PermitsDuplicates,
    string? SourceReference,
    string LicensingStatus);

public sealed record AssetAbilityDefinition(string Id, string Name, string Summary);
