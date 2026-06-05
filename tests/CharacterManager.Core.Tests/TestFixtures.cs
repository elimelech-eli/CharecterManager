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
        var track = ProgressTrack("vow-track");
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
                new Vow("vow-id", "Recover the lost banner", "", "dangerous", "active", track.Id, false, true, 0, "", now, null)
            ],
            ProgressTracks = [track]
        };
    }

    public static ProgressTrack ProgressTrack(string id) => new(
        Id: id,
        Name: "Recover the lost banner",
        Type: "vow",
        Rank: "dangerous",
        Ticks: 8,
        MaxTicks: CharacterManager.Core.Characters.ProgressTrack.DefaultMaxTicks,
        Status: "active",
        SharedScope: "character",
        Notes: "");
}
