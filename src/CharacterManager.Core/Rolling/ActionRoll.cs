namespace CharacterManager.Core.Rolling;

public sealed record ActionRoll(int ActionDie, int ChallengeDieOne, int ChallengeDieTwo, int Modifier)
{
    public int ActionScore => ActionDie + Modifier;

    public RollOutcome Outcome
    {
        get
        {
            var beatsFirst = ActionScore > ChallengeDieOne;
            var beatsSecond = ActionScore > ChallengeDieTwo;

            if (beatsFirst && beatsSecond)
            {
                return RollOutcome.StrongHit;
            }

            return beatsFirst || beatsSecond ? RollOutcome.WeakHit : RollOutcome.Miss;
        }
    }

    public bool IsMatch => ChallengeDieOne == ChallengeDieTwo;
}
