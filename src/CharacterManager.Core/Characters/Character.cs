namespace CharacterManager.Core.Characters;

public sealed record Character(
    Guid Id,
    string Name,
    string RulesetId,
    IReadOnlyDictionary<string, int> Stats,
    IReadOnlyList<string> AssetIds,
    IReadOnlyList<ProgressTrack> ProgressTracks)
{
    public static Character Create(string name, string rulesetId, IReadOnlyDictionary<string, int> stats)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(rulesetId);

        return new Character(
            Guid.NewGuid(),
            name.Trim(),
            rulesetId.Trim(),
            stats,
            Array.Empty<string>(),
            Array.Empty<ProgressTrack>());
    }
}
