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
        Assert.Equal("0.1.0", ruleset.ContentVersion);
        Assert.Equal("metadataOnly", ruleset.LicensingStatus);
        Assert.Equal(["edge", "heart", "iron", "shadow", "wits"], ruleset.Stats.Select(stat => stat.Id).ToArray());
        Assert.Equal([3, 2, 2, 1, 1], ruleset.StartingCharacter.StatArray);
        Assert.Contains(ruleset.Debilities, debility => debility.Id == "wounded" && debility.BlocksMeterIncrease == "health");
        Assert.Contains(ruleset.Debilities, debility => debility.Id == "shaken" && debility.BlocksMeterIncrease == "spirit");
        Assert.Contains(ruleset.Debilities, debility => debility.Id == "unprepared" && debility.BlocksMeterIncrease == "supply");
        Assert.All(ruleset.Assets, asset => Assert.Equal("metadataOnly", asset.LicensingStatus));
    }

    [Fact]
    public async Task CatalogIdsAreUnique()
    {
        var ruleset = await TestFixtures.GetRulesetAsync();

        AssertUnique(ruleset.Stats.Select(stat => stat.Id));
        AssertUnique(ruleset.Meters.Select(meter => meter.Id));
        AssertUnique(ruleset.Debilities.Select(debility => debility.Id));
        AssertUnique(ruleset.Assets.Select(asset => asset.Id));
    }

    private static void AssertUnique(IEnumerable<string> ids)
    {
        var materialized = ids.ToArray();
        Assert.Equal(materialized.Length, materialized.Distinct(StringComparer.OrdinalIgnoreCase).Count());
    }
}
