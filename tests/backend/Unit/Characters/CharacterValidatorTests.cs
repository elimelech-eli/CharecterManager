using CharacterManager.Core.Characters;
using CharacterManager.Core.Validation;
using Xunit;

namespace CharacterManager.Backend.Unit.Tests.Characters;

public sealed class CharacterValidatorTests
{
    [Fact]
    public async Task DraftCanBeIncompleteWithoutFinalizationErrors()
    {
        var ruleset = await TestFixtures.GetRulesetAsync();
        var draft = Character.CreateDraft(ruleset.Id, ruleset.RulesetVersion, ruleset.ContentVersion, DateTimeOffset.UtcNow);

        var result = new CharacterValidator().Validate(draft, ruleset, requireFinalizedFields: false);

        Assert.True(result.IsValid);
    }

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
    public async Task FinalizedStandardCharacterRequiresAllStats()
    {
        var ruleset = await TestFixtures.GetRulesetAsync();
        var character = TestFixtures.CompleteCharacter(ruleset) with
        {
            Stats = new Dictionary<string, int> { ["edge"] = 3 }
        };

        var result = new CharacterValidator().Validate(character, ruleset, requireFinalizedFields: true);

        Assert.Contains(result.Errors, issue => issue.Code == "stats.missing");
    }

    [Fact]
    public async Task UnknownStatsAndDebilitiesAreErrors()
    {
        var ruleset = await TestFixtures.GetRulesetAsync();
        var character = TestFixtures.CompleteCharacter(ruleset) with
        {
            Stats = new Dictionary<string, int>
            {
                ["edge"] = 3,
                ["heart"] = 2,
                ["iron"] = 2,
                ["shadow"] = 1,
                ["wits"] = 1,
                ["wyrd"] = 3
            },
            Debilities = [new DebilityState("unknown", true, "", null, DateTimeOffset.UtcNow, null)]
        };

        var result = new CharacterValidator().Validate(character, ruleset, requireFinalizedFields: true);

        Assert.Contains(result.Errors, issue => issue.Code == "stats.unknown");
        Assert.Contains(result.Errors, issue => issue.Code == "debility.unknown");
    }

    [Fact]
    public async Task MetersMustStayWithinZeroToFive()
    {
        var ruleset = await TestFixtures.GetRulesetAsync();
        var character = TestFixtures.CompleteCharacter(ruleset) with
        {
            ConditionMeters = ConditionMeters.Starting() with
            {
                Health = new MeterState(6, 0, 5, false, null),
                Spirit = new MeterState(-1, 0, 5, false, null)
            }
        };

        var result = new CharacterValidator().Validate(character, ruleset, requireFinalizedFields: true);

        Assert.Contains(result.Errors, issue => issue.Code == "meter.outOfRange" && issue.Field == "conditionMeters.health.current");
        Assert.Contains(result.Errors, issue => issue.Code == "meter.outOfRange" && issue.Field == "conditionMeters.spirit.current");
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
    public async Task MomentumOutsideDerivedRangeIsInvalid()
    {
        var ruleset = await TestFixtures.GetRulesetAsync();
        var character = TestFixtures.CompleteCharacter(ruleset) with
        {
            Momentum = Momentum.Starting() with { Current = 11 }
        };

        var result = new CharacterValidator().Validate(character, ruleset, requireFinalizedFields: true);

        Assert.Contains(result.Errors, issue => issue.Code == "momentum.outOfRange");
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
    public async Task FinalizedCharacterRequiresNameThreeAssetsAndStartingVows()
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
        Assert.Contains(result.Errors, issue => issue.Code == "vow.backgroundRequired");
        Assert.Contains(result.Errors, issue => issue.Code == "vow.incitingIncidentRequired");
    }

    [Fact]
    public async Task FinalizedCharacterRequiresBothBackgroundAndIncitingVows()
    {
        var ruleset = await TestFixtures.GetRulesetAsync();
        var character = TestFixtures.CompleteCharacter(ruleset) with
        {
            Vows =
            [
                new Vow("background-vow-id", "Reclaim Frostmark Hall", "", "extreme", "active", "background-vow-track", true, false, 0, "", DateTimeOffset.UtcNow, null)
            ],
            ProgressTracks =
            [
                TestFixtures.ProgressTrack("background-vow-track", "Reclaim Frostmark Hall", "extreme")
            ]
        };

        var result = new CharacterValidator().Validate(character, ruleset, requireFinalizedFields: true);

        Assert.Contains(result.Errors, issue => issue.Code == "vow.activeRequired" && issue.Field == "vows" && issue.Step == "vows");
        Assert.Contains(result.Errors, issue => issue.Code == "vow.incitingIncidentRequired" && issue.Field == "vows" && issue.Step == "vows");
        Assert.DoesNotContain(result.Errors, issue => issue.Code == "vow.backgroundRequired");
    }

    [Fact]
    public async Task DuplicateAssetsAndMissingCompanionNameAreInvalid()
    {
        var ruleset = await TestFixtures.GetRulesetAsync();
        var character = TestFixtures.CompleteCharacter(ruleset) with
        {
            Assets =
            [
                new SelectedAsset("cave-lion", [], null, null, "", null),
                new SelectedAsset("alchemist", [], null, null, "", null),
                new SelectedAsset("alchemist", [], null, null, "", null)
            ]
        };

        var result = new CharacterValidator().Validate(character, ruleset, requireFinalizedFields: true);

        Assert.Contains(result.Errors, issue => issue.Code == "assets.companionNameRequired");
        Assert.Contains(result.Errors, issue => issue.Code == "assets.duplicate");
    }

    [Fact]
    public async Task VowAndBondValidationUsesStructuredFields()
    {
        var ruleset = await TestFixtures.GetRulesetAsync();
        var now = DateTimeOffset.UtcNow;
        var character = TestFixtures.CompleteCharacter(ruleset) with
        {
            Vows = [new Vow("vow-id", "", "", "unknown", "active", "", false, true, 0, "", now, null)],
            Bonds =
            [
                new Bond("1", "", "", "", "", "active", "", true, now),
                new Bond("2", "Ravencliff", "", "", "", "active", "", true, now),
                new Bond("3", "Ravencliff", "", "", "", "active", "", true, now),
                new Bond("4", "Stoneford", "", "", "", "active", "", true, now)
            ]
        };

        var result = new CharacterValidator().Validate(character, ruleset, requireFinalizedFields: true);

        Assert.Contains(result.Errors, issue => issue.Code == "vow.titleRequired" && issue.Field == "vows[0].title");
        Assert.Contains(result.Errors, issue => issue.Code == "vow.progressTrackRequired" && issue.Field == "vows[0].progressTrackId");
        Assert.Contains(result.Errors, issue => issue.Code == "vow.rankUnknown" && issue.Field == "vows[0].rank");
        Assert.Contains(result.Errors, issue => issue.Code == "bond.nameRequired" && issue.Field == "bonds[0].name");
        Assert.Contains(result.Errors, issue => issue.Code == "bond.tooManyStarting");
        Assert.Contains(result.Warnings, issue => issue.Code == "bond.duplicateName");
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
