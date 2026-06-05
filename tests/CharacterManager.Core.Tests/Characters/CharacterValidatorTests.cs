using CharacterManager.Core.Characters;
using CharacterManager.Core.Validation;
using CharacterManager.Infrastructure.Rules;
using Xunit;

namespace CharacterManager.Core.Tests.Characters;

public sealed class CharacterValidatorTests
{
    [Fact]
    public async Task CompleteStandardCharacterIsValidForFinalization()
    {
        var ruleset = await TestFixtures.GetRulesetAsync();
        var character = TestFixtures.CompleteCharacter(ruleset);

        var result = new CharacterValidator().Validate(character, ruleset, requireFinalizedFields: true);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task FinalizedStandardCharacterRequiresStartingStatDistribution()
    {
        var ruleset = await TestFixtures.GetRulesetAsync();
        var character = TestFixtures.CompleteCharacter(ruleset) with
        {
            Stats = new Dictionary<string, int>
            {
                ["edge"] = 3,
                ["heart"] = 3,
                ["iron"] = 2,
                ["shadow"] = 1,
                ["wits"] = 1
            }
        };

        var result = new CharacterValidator().Validate(character, ruleset, requireFinalizedFields: true);

        Assert.Contains(result.Errors, issue => issue.Code == "stats.invalidDistribution" && issue.Field == "stats" && issue.Step == "stats");
    }

    [Fact]
    public async Task MetersMustStayWithinZeroToFive()
    {
        var ruleset = await TestFixtures.GetRulesetAsync();
        var character = TestFixtures.CompleteCharacter(ruleset) with
        {
            ConditionMeters = ConditionMeters.Starting() with
            {
                Health = new MeterState(6, 0, 5, false, null)
            }
        };

        var result = new CharacterValidator().Validate(character, ruleset, requireFinalizedFields: true);

        Assert.Contains(result.Errors, issue => issue.Code == "meter.outOfRange" && issue.Field == "conditionMeters.health.current");
    }

    [Fact]
    public async Task MarkedDebilitiesDeriveMomentumMaximumAndReset()
    {
        var ruleset = await TestFixtures.GetRulesetAsync();
        var character = TestFixtures.CompleteCharacter(ruleset) with
        {
            Debilities =
            [
                new DebilityState("wounded", true, "", null, DateTimeOffset.UtcNow, null),
                new DebilityState("shaken", true, "", null, DateTimeOffset.UtcNow, null)
            ],
            Momentum = Momentum.Starting()
        };

        var derived = MomentumService.Derive(character.Momentum, character.Debilities, ruleset.Momentum);

        Assert.Equal(8, derived.Max);
        Assert.Equal(0, derived.Reset);
        Assert.Equal(2, derived.MarkedDebilityCount);
    }

    [Fact]
    public async Task ProgressTracksMustStayWithinFortyTicks()
    {
        var ruleset = await TestFixtures.GetRulesetAsync();
        var character = TestFixtures.CompleteCharacter(ruleset) with
        {
            ProgressTracks = [TestFixtures.ProgressTrack("track-id") with { Ticks = 41 }]
        };

        var result = new CharacterValidator().Validate(character, ruleset, requireFinalizedFields: true);

        Assert.Contains(result.Errors, issue => issue.Code == "progressTrack.ticksOutOfRange");
    }

    [Fact]
    public async Task FinalizedCharacterRequiresNameThreeAssetsAndActiveVow()
    {
        var ruleset = await TestFixtures.GetRulesetAsync();
        var character = TestFixtures.CompleteCharacter(ruleset) with
        {
            Name = "",
            Assets = [],
            Vows = []
        };

        var result = new CharacterValidator().Validate(character, ruleset, requireFinalizedFields: true);

        Assert.Contains(result.Errors, issue => issue.Code == "character.nameRequired");
        Assert.Contains(result.Errors, issue => issue.Code == "assets.invalidCount");
        Assert.Contains(result.Errors, issue => issue.Code == "vow.activeRequired");
    }

    [Fact]
    public async Task DebilityBlocksMatchingMeterIncrease()
    {
        var ruleset = await TestFixtures.GetRulesetAsync();
        var previous = TestFixtures.CompleteCharacter(ruleset) with
        {
            ConditionMeters = ConditionMeters.Starting() with { Health = new MeterState(3, 0, 5, true, "wounded") },
            Debilities = [new DebilityState("wounded", true, "", null, DateTimeOffset.UtcNow, null)]
        };
        var requested = previous with
        {
            ConditionMeters = previous.ConditionMeters with { Health = previous.ConditionMeters.Health with { Current = 4 } }
        };

        var result = new ConditionMeterChangeValidator().ValidateIncreaseLocks(previous, requested);

        Assert.Contains(result.Errors, issue => issue.Code == "meter.health.increaseBlocked" && issue.Field == "conditionMeters.health.current");
    }
}
