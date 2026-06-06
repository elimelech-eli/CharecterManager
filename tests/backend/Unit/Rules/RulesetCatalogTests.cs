using Xunit;

namespace CharacterManager.Backend.Unit.Tests.Rules;

public sealed class RulesetCatalogTests
{
    [Fact]
    public async Task IronswornCatalogContainsMvpMetadata()
    {
        var ruleset = await TestFixtures.GetRulesetAsync();

        Assert.Equal("ironsworn", ruleset.Id);
        Assert.Equal("0.1.0", ruleset.RulesetVersion);
        Assert.Equal("0.2.0", ruleset.ContentVersion);
        Assert.Equal("summaryApproved", ruleset.LicensingStatus);
        Assert.Equal(["edge", "heart", "iron", "shadow", "wits"], ruleset.Stats.Select(stat => stat.Id).ToArray());
        Assert.Equal([3, 2, 2, 1, 1], ruleset.StartingCharacter.StatArray);
        Assert.Equal(2, ruleset.StartingCharacter.RequiredActiveVowCount);
        Assert.Contains(ruleset.Debilities, debility => debility.Id == "wounded" && debility.BlocksMeterIncrease == "health");
        Assert.Contains(ruleset.Debilities, debility => debility.Id == "shaken" && debility.BlocksMeterIncrease == "spirit");
        Assert.Contains(ruleset.Debilities, debility => debility.Id == "unprepared" && debility.BlocksMeterIncrease == "supply");
        Assert.Contains(ruleset.Debilities, debility => debility.Id == "encumbered" && debility.Category == "condition");
        Assert.Contains(ruleset.Moves, move => move.Id == "face-danger");
        Assert.Contains(ruleset.Moves, move => move.Id == "swear-an-iron-vow");
        Assert.Contains(ruleset.Moves, move => move.Id == "fulfill-your-vow");
        Assert.Contains(ruleset.Assets, asset => asset.Id == "alchemist" && asset.Type == "path");
        Assert.Contains(ruleset.Assets, asset => asset.Id == "cave-lion" && asset.Type == "companion" && asset.RequiresCompanionName);
        Assert.Contains(ruleset.Assets, asset => asset.Id == "archer" && asset.Type == "combat-talent");
        Assert.Contains(ruleset.Assets, asset => asset.Id == "awakening" && asset.Type == "ritual");
        Assert.DoesNotContain(ruleset.Assets, asset => asset.Id.Contains("placeholder", StringComparison.OrdinalIgnoreCase));
        Assert.All(ruleset.Assets, asset => Assert.Equal("summaryApproved", asset.LicensingStatus));
        Assert.All(ruleset.Assets, asset => Assert.Equal(3, asset.Abilities.Count));
        Assert.True(ruleset.Assets.Count >= 70);
    }

    [Fact]
    public async Task CatalogIdsAreUnique()
    {
        var ruleset = await TestFixtures.GetRulesetAsync();

        AssertUnique(ruleset.Stats.Select(stat => stat.Id));
        AssertUnique(ruleset.Meters.Select(meter => meter.Id));
        AssertUnique(ruleset.Debilities.Select(debility => debility.Id));
        AssertUnique(ruleset.Moves.Select(move => move.Id));
        AssertUnique(ruleset.Assets.Select(asset => asset.Id));
    }

    private static void AssertUnique(IEnumerable<string> ids)
    {
        var materialized = ids.ToArray();
        Assert.Equal(materialized.Length, materialized.Distinct(StringComparer.OrdinalIgnoreCase).Count());
    }
}
