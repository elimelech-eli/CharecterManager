using CharacterManager.Core.Rolling;
using Xunit;

namespace CharacterManager.Backend.Unit.Tests.Rolling;

public sealed class ActionRollTests
{
    [Fact]
    public void OutcomeIsStrongHitWhenActionScoreBeatsBothChallengeDice()
    {
        var roll = new ActionRoll(ActionDie: 6, ChallengeDieOne: 4, ChallengeDieTwo: 5, Modifier: 1);

        Assert.Equal(RollOutcome.StrongHit, roll.Outcome);
    }

    [Fact]
    public void OutcomeIsWeakHitWhenActionScoreBeatsOneChallengeDie()
    {
        var roll = new ActionRoll(ActionDie: 4, ChallengeDieOne: 3, ChallengeDieTwo: 9, Modifier: 0);

        Assert.Equal(RollOutcome.WeakHit, roll.Outcome);
    }

    [Fact]
    public void OutcomeIsMissWhenActionScoreBeatsNeitherChallengeDie()
    {
        var roll = new ActionRoll(ActionDie: 2, ChallengeDieOne: 4, ChallengeDieTwo: 5, Modifier: 0);

        Assert.Equal(RollOutcome.Miss, roll.Outcome);
    }
}
