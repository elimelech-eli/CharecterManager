namespace CharacterManager.Core.Rolling;

public sealed class RandomRoller : IRoller
{
    private readonly Random _random;

    public RandomRoller(Random? random = null)
    {
        _random = random ?? new Random();
    }

    public ActionRoll RollAction(int modifier)
    {
        return new ActionRoll(
            ActionDie: _random.Next(1, 7),
            ChallengeDieOne: _random.Next(1, 11),
            ChallengeDieTwo: _random.Next(1, 11),
            Modifier: modifier);
    }
}
