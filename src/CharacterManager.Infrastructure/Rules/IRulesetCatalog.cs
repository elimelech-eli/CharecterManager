using CharacterManager.Core.Rules;

namespace CharacterManager.Infrastructure.Rules;

public interface IRulesetCatalog
{
    Task<IReadOnlyList<Ruleset>> GetRulesetsAsync(CancellationToken cancellationToken);
}
