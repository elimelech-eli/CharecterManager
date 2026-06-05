using CharacterManager.Core.Rules;

namespace CharacterManager.Infrastructure.Rules;

public sealed class InMemoryRulesetCatalog : IRulesetCatalog
{
    private static readonly Ruleset IronswornStarter = new(
        Id: "ironsworn",
        Name: "Ironsworn",
        RulesetVersion: "0.1.0",
        ContentVersion: "0.1.0",
        LicensingStatus: "metadataOnly",
        Stats:
        [
            new("edge", "Edge", 1, 3, "Agility, speed, and ranged action metadata.", null),
            new("heart", "Heart", 1, 3, "Courage, social connection, and resolve metadata.", null),
            new("iron", "Iron", 1, 3, "Strength, endurance, and close action metadata.", null),
            new("shadow", "Shadow", 1, 3, "Stealth, deception, and subtle action metadata.", null),
            new("wits", "Wits", 1, 3, "Observation, expertise, and preparation metadata.", null)
        ],
        StartingCharacter: new(
            StatArray: [3, 2, 2, 1, 1],
            Health: 5,
            Spirit: 5,
            Supply: 5,
            Momentum: 2,
            MomentumMax: 10,
            MomentumReset: 2,
            RequiredAssetCount: 3,
            MaxStartingBonds: 3,
            RequiredActiveVowCount: 1),
        Meters:
        [
            new("health", "Health", 0, 5),
            new("spirit", "Spirit", 0, 5),
            new("supply", "Supply", 0, 5)
        ],
        Momentum: new(
            Minimum: -6,
            BaseMax: 10,
            ResetByMarkedDebilityCount:
            [
                new(0, 2),
                new(1, 1),
                new(2, 0)
            ]),
        Debilities:
        [
            new("wounded", "Wounded", "condition", "health", false, false, null),
            new("shaken", "Shaken", "condition", "spirit", false, false, null),
            new("unprepared", "Unprepared", "condition", "supply", false, false, null),
            new("maimed", "Maimed", "bane", null, true, false, null),
            new("corrupted", "Corrupted", "bane", null, true, false, null),
            new("cursed", "Cursed", "burden", null, true, true, null),
            new("tormented", "Tormented", "burden", null, true, true, null)
        ],
        Ranks: ["troublesome", "dangerous", "formidable", "extreme", "epic"],
        ProgressTrackTypes: ["vow", "bonds", "journey", "fight", "generic"],
        Moves:
        [
            new("face-danger", "Face Danger", "Metadata placeholder for a risky action move.", null),
            new("secure-advantage", "Secure an Advantage", "Metadata placeholder for a preparation/leverage move.", null)
        ],
        Assets:
        [
            new("path-placeholder", "Path Placeholder", "path", "Metadata-only placeholder for an Ironsworn path asset.", [], false, false, null, "metadataOnly"),
            new("companion-placeholder", "Companion Placeholder", "companion", "Metadata-only placeholder for an Ironsworn companion asset.", [], true, false, null, "metadataOnly"),
            new("combat-talent-placeholder", "Combat Talent Placeholder", "combat-talent", "Metadata-only placeholder for an Ironsworn combat talent asset.", [], false, false, null, "metadataOnly"),
            new("ritual-placeholder", "Ritual Placeholder", "ritual", "Metadata-only placeholder for an Ironsworn ritual asset.", [], false, false, null, "metadataOnly")
        ]
    );

    public Task<IReadOnlyList<Ruleset>> GetRulesetsAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<Ruleset> rulesets = [IronswornStarter];
        return Task.FromResult(rulesets);
    }

    public Task<Ruleset?> GetRulesetAsync(string rulesetId, CancellationToken cancellationToken)
    {
        var ruleset = StringComparer.OrdinalIgnoreCase.Equals(rulesetId, IronswornStarter.Id)
            ? IronswornStarter
            : null;

        return Task.FromResult(ruleset);
    }
}
