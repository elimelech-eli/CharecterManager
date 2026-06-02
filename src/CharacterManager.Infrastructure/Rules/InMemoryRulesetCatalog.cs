using CharacterManager.Core.Rules;

namespace CharacterManager.Infrastructure.Rules;

public sealed class InMemoryRulesetCatalog : IRulesetCatalog
{
    private static readonly Ruleset IronswornStarter = new(
        Id: "ironsworn",
        Name: "Ironsworn",
        Version: "0.1.0",
        Stats:
        [
            new("edge", "Edge", 1, 3),
            new("heart", "Heart", 1, 3),
            new("iron", "Iron", 1, 3),
            new("shadow", "Shadow", 1, 3),
            new("wits", "Wits", 1, 3)
        ],
        Moves:
        [
            new("face-danger", "Face Danger", "When you attempt something risky or react to an imminent threat.", null),
            new("secure-advantage", "Secure an Advantage", "When you assess, prepare, or gain leverage.", null)
        ],
        Assets: []);

    public Task<IReadOnlyList<Ruleset>> GetRulesetsAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<Ruleset> rulesets = [IronswornStarter];
        return Task.FromResult(rulesets);
    }
}
