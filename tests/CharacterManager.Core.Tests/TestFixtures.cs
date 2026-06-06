using CharacterManager.Core.Characters;
using CharacterManager.Core.Rules;
using CharacterManager.Infrastructure.Rules;

namespace CharacterManager.Core.Tests;

internal static class TestFixtures
{
    public static async Task<Ruleset> GetRulesetAsync()
    {
        var catalog = new InMemoryRulesetCatalog();
        var ruleset = await catalog.GetRulesetAsync("ironsworn", CancellationToken.None);
        return ruleset ?? throw new InvalidOperationException("Ironsworn test ruleset was not found.");
    }

    public static Character CompleteCharacter(Ruleset ruleset)
    {
        var now = DateTimeOffset.UtcNow;
        var backgroundTrack = ProgressTrack("background-vow-track", "Reclaim Frostmark Hall", "extreme");
        var incitingTrack = ProgressTrack("inciting-vow-track", "Recover the lost banner", "dangerous");
        var character = Character.CreateDraft(
            ruleset.Id,
            ruleset.RulesetVersion,
            ruleset.ContentVersion,
            now,
            new Dictionary<string, int>
            {
                ["edge"] = 3,
                ["heart"] = 2,
                ["iron"] = 2,
                ["shadow"] = 1,
                ["wits"] = 1
            });

        return character with
        {
            Name = "Kara Iron-Eyes",
            Concept = "Exiled scout",
            Assets =
            [
                new SelectedAsset("alchemist", ["ability-1"], null, null, "", null),
                new SelectedAsset("cave-lion", ["ability-1"], "Ash", 5, "", null),
                new SelectedAsset("archer", ["ability-1"], null, null, "", null)
            ],
            Vows =
            [
                new Vow("background-vow-id", "Reclaim Frostmark Hall", "", "extreme", "active", backgroundTrack.Id, true, false, 0, "", now, null),
                new Vow("inciting-vow-id", "Recover the lost banner", "", "dangerous", "active", incitingTrack.Id, false, true, 0, "", now, null)
            ],
            ProgressTracks = [backgroundTrack, incitingTrack]
        };
    }

    public static ProgressTrack ProgressTrack(string id, string name = "Recover the lost banner", string rank = "dangerous") => new(
        Id: id,
        Name: name,
        Type: "vow",
        Rank: rank,
        Ticks: 8,
        MaxTicks: CharacterManager.Core.Characters.ProgressTrack.DefaultMaxTicks,
        Status: "active",
        SharedScope: "character",
        Notes: "");
}
