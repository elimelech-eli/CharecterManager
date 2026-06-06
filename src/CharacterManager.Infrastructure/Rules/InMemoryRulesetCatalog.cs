using CharacterManager.Core.Rules;

namespace CharacterManager.Infrastructure.Rules;

public sealed class InMemoryRulesetCatalog : IRulesetCatalog
{
    private const string ContentLicensingStatus = "summaryApproved";

    private static readonly Ruleset IronswornStarter = new(
        Id: "ironsworn",
        Name: "Ironsworn",
        RulesetVersion: "0.1.0",
        ContentVersion: "0.2.0",
        LicensingStatus: ContentLicensingStatus,
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
            RequiredActiveVowCount: 2),
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
            new("encumbered", "Encumbered", "condition", null, false, false, null),
            new("maimed", "Maimed", "bane", null, true, false, null),
            new("corrupted", "Corrupted", "bane", null, true, false, null),
            new("cursed", "Cursed", "burden", null, true, true, null),
            new("tormented", "Tormented", "burden", null, true, true, null)
        ],
        Ranks: ["troublesome", "dangerous", "formidable", "extreme", "epic"],
        ProgressTrackTypes: ["vow", "bonds", "journey", "fight", "generic"],
        Moves:
        [
            Move("face-danger", "Face Danger", "When you attempt something risky or react to an imminent threat.", null),
            Move("secure-an-advantage", "Secure an Advantage", "When you assess a situation, make preparations, or gain leverage.", null),
            Move("gather-information", "Gather Information", "When you search an area, ask questions, conduct an investigation, or follow a track.", "wits"),
            Move("heal", "Heal", "When you treat an injury or ailment.", "wits"),
            Move("resupply", "Resupply", "When you hunt, forage, or scavenge.", "wits"),
            Move("make-camp", "Make Camp", "When you rest and recover for several hours in the wild.", "supply"),
            Move("undertake-a-journey", "Undertake a Journey", "When you travel across hazardous or unfamiliar lands.", "wits"),
            Move("reach-your-destination", "Reach Your Destination", "When your journey comes to an end.", null),
            Move("compel", "Compel", "When you attempt to persuade someone or make them an offer.", null),
            Move("sojourn", "Sojourn", "When you spend time in a community seeking assistance.", "heart"),
            Move("draw-the-circle", "Draw the Circle", "When you challenge someone to a formal duel.", "heart"),
            Move("forge-a-bond", "Forge a Bond", "When you spend significant time with a person or community.", "heart"),
            Move("test-your-bond", "Test Your Bond", "When your bond is tested through conflict, betrayal, or hardship.", null),
            Move("aid-your-ally", "Aid Your Ally", "When you Secure an Advantage in direct support of an ally.", null),
            Move("write-your-epilogue", "Write Your Epilogue", "When you retire from your life as Ironsworn.", null),
            Move("enter-the-fray", "Enter the Fray", "When you enter into combat.", null),
            Move("strike", "Strike", "When you have initiative and attack in close quarters or at range.", null),
            Move("clash", "Clash", "When your foe has initiative and you fight with them in close quarters or at range.", null),
            Move("turn-the-tide", "Turn the Tide", "When you risk everything to seize control of a fight.", null),
            Move("end-the-fight", "End the Fight", "When you make a move to take decisive action and score a strong hit.", null),
            Move("battle", "Battle", "When you fight a battle and it happens in a blur.", null),
            Move("endure-harm", "Endure Harm", "When you face physical damage.", null),
            Move("face-death", "Face Death", "When you are brought to the brink of death.", "heart"),
            Move("companion-endure-harm", "Companion Endure Harm", "When your companion faces physical damage.", null),
            Move("endure-stress", "Endure Stress", "When you face mental shock or despair.", null),
            Move("face-desolation", "Face Desolation", "When you are brought to the brink of desolation.", "heart"),
            Move("out-of-supply", "Out of Supply", "When your supply is exhausted.", null),
            Move("face-a-setback", "Face a Setback", "When your momentum is at its minimum and suffers further.", null),
            Move("swear-an-iron-vow", "Swear an Iron Vow", "When you swear upon iron to complete a quest.", "heart"),
            Move("reach-a-milestone", "Reach a Milestone", "When you make significant progress in your quest.", null),
            Move("fulfill-your-vow", "Fulfill Your Vow", "When you achieve what you believe to be the fulfillment of your vow.", null),
            Move("forsake-your-vow", "Forsake Your Vow", "When you renounce your quest, betray your promise, or the quest is lost.", null),
            Move("advance", "Advance", "When you focus on your skills, receive training, find inspiration, earn a reward, or gain a companion.", null),
            Move("pay-the-price", "Pay the Price", "When you suffer the outcome of a move.", null),
            Move("ask-the-oracle", "Ask the Oracle", "When you seek to resolve questions, discover details, or determine how other characters respond.", null)
        ],
        Assets:
        [
            Companion("cave-lion", "Cave Lion", 2),
            Companion("hawk", "Hawk", 2),
            Companion("horse", "Horse", 2),
            Companion("hound", "Hound", 2),
            Companion("kindred", "Kindred", 2),
            Companion("mammoth", "Mammoth", 2),
            Companion("owl", "Owl", 2),
            Companion("raven", "Raven", 2),
            Companion("giant-spider", "Giant Spider", 2),
            Companion("young-wyvern", "Young Wyvern", 4),
            Path("alchemist", "Alchemist", 4),
            Path("animal-kin", "Animal Kin", 4),
            Path("banner-sworn", "Banner-Sworn", 4),
            Path("battle-scarred", "Battle-Scarred", 4),
            Path("blade-bound", "Blade-Bound", 4),
            Path("bonded", "Bonded", 4),
            Path("commander", "Commander", 4),
            Path("dancer", "Dancer", 4),
            Path("devotant", "Devotant", 6),
            Path("empowered", "Empowered", 6),
            Path("fated", "Fated", 6),
            Path("fortune-hunter", "Fortune Hunter", 6),
            Path("herbalist", "Herbalist", 6),
            Path("honorbound", "Honorbound", 6),
            Path("improviser", "Improviser", 6),
            Path("infiltrator", "Infiltrator", 6),
            Path("lorekeeper", "Lorekeeper", 6),
            Path("loyalist", "Loyalist", 8),
            Path("masked", "Masked", 8),
            Path("oathbreaker", "Oathbreaker", 8),
            Path("outcast", "Outcast", 8),
            Path("pretender", "Pretender", 8),
            Path("revenant", "Revenant", 8),
            Path("rider", "Rider", 8),
            Path("ritualist", "Ritualist", 8),
            Path("shadow-kin", "Shadow-Kin", 8),
            Path("sighted", "Sighted", 10),
            Path("slayer", "Slayer", 10),
            Path("spirit-bound", "Spirit-Bound", 10),
            Path("storyweaver", "Storyweaver", 10),
            Path("trickster", "Trickster", 10),
            Path("veteran", "Veteran", 10),
            Path("waterborn", "Waterborn", 10),
            Path("wayfinder", "Wayfinder", 10),
            Path("weaponmaster", "Weaponmaster", 10),
            Path("wildblood", "Wildblood", 12),
            Path("wright", "Wright", 12),
            CombatTalent("archer", "Archer", 12),
            CombatTalent("berserker", "Berserker", 12),
            CombatTalent("brawler", "Brawler", 12),
            CombatTalent("cutthroat", "Cutthroat", 12),
            CombatTalent("duelist", "Duelist", 12),
            CombatTalent("fletcher", "Fletcher", 12),
            CombatTalent("geared-for-war", "Geared for War", 12),
            CombatTalent("ironclad", "Ironclad", 12),
            CombatTalent("lightly-armored", "Lightly Armored", 12),
            CombatTalent("long-arm", "Long-Arm", 14),
            CombatTalent("shield-bearer", "Shield-Bearer", 14),
            CombatTalent("skirmisher", "Skirmisher", 14),
            CombatTalent("slinger", "Slinger", 14),
            CombatTalent("sunderer", "Sunderer", 14),
            CombatTalent("swordmaster", "Swordmaster", 14),
            CombatTalent("thunder-bringer", "Thunder-Bringer", 14),
            Ritual("awakening", "Awakening", 14),
            Ritual("augur", "Augur", 14),
            Ritual("bind", "Bind", 16),
            Ritual("communion", "Communion", 16),
            Ritual("divination", "Divination", 16),
            Ritual("invoke", "Invoke", 16),
            Ritual("keen", "Keen", 16),
            Ritual("leech", "Leech", 16),
            Ritual("lightbearer", "Lightbearer", 16),
            Ritual("scry", "Scry", 16),
            Ritual("sway", "Sway", 18),
            Ritual("talisman", "Talisman", 18),
            Ritual("tether", "Tether", 18),
            Ritual("totem", "Totem", 18),
            Ritual("visage", "Visage", 18),
            Ritual("ward", "Ward", 18)
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

    private static MoveDefinition Move(string id, string name, string trigger, string? statId)
    {
        return new MoveDefinition(id, name, trigger, statId);
    }

    private static AssetDefinition Companion(string id, string name, int sourcePage)
    {
        return Asset(id, name, "companion", "Official Ironsworn companion asset metadata.", true, sourcePage);
    }

    private static AssetDefinition Path(string id, string name, int sourcePage)
    {
        return Asset(id, name, "path", "Official Ironsworn path asset metadata.", false, sourcePage);
    }

    private static AssetDefinition CombatTalent(string id, string name, int sourcePage)
    {
        return Asset(id, name, "combat-talent", "Official Ironsworn combat talent asset metadata.", false, sourcePage);
    }

    private static AssetDefinition Ritual(string id, string name, int sourcePage)
    {
        return Asset(id, name, "ritual", "Official Ironsworn ritual asset metadata.", false, sourcePage);
    }

    private static AssetDefinition Asset(string id, string name, string type, string summary, bool requiresCompanionName, int sourcePage)
    {
        return new AssetDefinition(
            id,
            name,
            type,
            summary,
            AbilitySlots(name, sourcePage),
            requiresCompanionName,
            PermitsDuplicates: false,
            SourceReference: $"Ironsworn Assets, p. {sourcePage}",
            LicensingStatus: ContentLicensingStatus);
    }

    private static IReadOnlyList<AssetAbilityDefinition> AbilitySlots(string assetName, int sourcePage)
    {
        return
        [
            new("ability-1", "First ability", $"First {assetName} ability from the official Ironsworn asset card. See Ironsworn Assets, p. {sourcePage}."),
            new("ability-2", "Second ability", $"Second {assetName} ability from the official Ironsworn asset card. See Ironsworn Assets, p. {sourcePage}."),
            new("ability-3", "Third ability", $"Third {assetName} ability from the official Ironsworn asset card. See Ironsworn Assets, p. {sourcePage}.")
        ];
    }
}
