using CharacterManager.Core.Rules;

namespace CharacterManager.Core.Characters;

public static class MomentumService
{
    public static Momentum Derive(Momentum current, IReadOnlyList<DebilityState> debilities, MomentumRules rules)
    {
        var markedCount = debilities.Count(debility => debility.Marked);
        var max = rules.BaseMax - markedCount;
        var reset = rules.ResetByMarkedDebilityCount
            .OrderBy(rule => rule.MarkedCount)
            .LastOrDefault(rule => markedCount >= rule.MarkedCount)?.Reset ?? 0;

        return current with
        {
            Max = max,
            Reset = reset,
            Minimum = rules.Minimum,
            MarkedDebilityCount = markedCount
        };
    }
}
