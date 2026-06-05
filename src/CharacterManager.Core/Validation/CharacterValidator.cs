using CharacterManager.Core.Characters;
using CharacterManager.Core.Rules;

namespace CharacterManager.Core.Validation;

public sealed class CharacterValidator
{
    public ValidationResult Validate(Character character, Ruleset ruleset, bool requireFinalizedFields)
    {
        var issues = new List<ValidationIssue>();

        ValidateSchema(character, issues);
        ValidateRuleset(character, ruleset, issues);
        ValidateStats(character, ruleset, requireFinalizedFields, issues);
        ValidateMeters(character, ruleset, issues);
        ValidateDebilities(character, ruleset, issues);
        ValidateMomentum(character, ruleset, issues);
        ValidateAssets(character, ruleset, requireFinalizedFields, issues);
        ValidateProgressTracks(character, ruleset, issues);
        ValidateVows(character, ruleset, requireFinalizedFields, issues);
        ValidateBonds(character, ruleset, issues);

        if (requireFinalizedFields && string.IsNullOrWhiteSpace(character.Name))
        {
            issues.Add(Error("character.nameRequired", "Name is required before finalizing.", "name", "concept"));
        }

        return ValidationResult.FromIssues(issues);
    }

    private static void ValidateSchema(Character character, List<ValidationIssue> issues)
    {
        if (character.SchemaVersion != Character.CurrentSchemaVersion)
        {
            issues.Add(Error("character.unsupportedSchemaVersion", "Character schema version is not supported.", "schemaVersion", "review"));
        }
    }

    private static void ValidateRuleset(Character character, Ruleset ruleset, List<ValidationIssue> issues)
    {
        if (string.IsNullOrWhiteSpace(character.RulesetId))
        {
            issues.Add(Error("character.rulesetRequired", "Ruleset is required.", "rulesetId", "concept"));
            return;
        }

        if (!StringComparer.OrdinalIgnoreCase.Equals(character.RulesetId, ruleset.Id))
        {
            issues.Add(Error("character.rulesetUnknown", "Character ruleset does not match a known ruleset.", "rulesetId", "concept"));
        }
    }

    private static void ValidateStats(Character character, Ruleset ruleset, bool requireFinalizedFields, List<ValidationIssue> issues)
    {
        var statIds = ruleset.Stats.Select(stat => stat.Id).ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var stat in character.Stats)
        {
            if (!statIds.Contains(stat.Key))
            {
                issues.Add(Error("stats.unknown", "Stat is not defined by the ruleset.", $"stats.{stat.Key}", "stats"));
                continue;
            }

            var definition = ruleset.Stats.First(candidate => StringComparer.OrdinalIgnoreCase.Equals(candidate.Id, stat.Key));
            if (stat.Value < definition.Minimum || stat.Value > definition.Maximum)
            {
                issues.Add(Error("stats.outOfRange", $"Stat must be between {definition.Minimum} and {definition.Maximum}.", $"stats.{stat.Key}", "stats"));
            }
        }

        if (!requireFinalizedFields)
        {
            return;
        }

        foreach (var statId in statIds)
        {
            if (!character.Stats.ContainsKey(statId))
            {
                issues.Add(Error("stats.missing", "All five stats are required before finalizing.", "stats", "stats"));
                break;
            }
        }

        var actual = character.Stats
            .Where(stat => statIds.Contains(stat.Key))
            .Select(stat => stat.Value)
            .OrderByDescending(value => value)
            .ToArray();
        var expected = ruleset.StartingCharacter.StatArray.OrderByDescending(value => value).ToArray();

        if (actual.Length != expected.Length || !actual.SequenceEqual(expected))
        {
            issues.Add(Error("stats.invalidDistribution", "Use exactly one 3, two 2s, and two 1s.", "stats", "stats"));
        }
    }

    private static void ValidateMeters(Character character, Ruleset ruleset, List<ValidationIssue> issues)
    {
        ValidateMeter("health", character.ConditionMeters.Health, ruleset, issues);
        ValidateMeter("spirit", character.ConditionMeters.Spirit, ruleset, issues);
        ValidateMeter("supply", character.ConditionMeters.Supply, ruleset, issues);

        ValidateMeterLock(character, "health", "wounded", issues);
        ValidateMeterLock(character, "spirit", "shaken", issues);
        ValidateMeterLock(character, "supply", "unprepared", issues);
    }

    private static void ValidateMeter(string meterId, MeterState meter, Ruleset ruleset, List<ValidationIssue> issues)
    {
        var definition = ruleset.Meters.FirstOrDefault(candidate => candidate.Id == meterId);
        if (definition is null)
        {
            return;
        }

        if (meter.Current < definition.Minimum || meter.Current > definition.Maximum)
        {
            issues.Add(Error("meter.outOfRange", $"{definition.Name} must be between {definition.Minimum} and {definition.Maximum}.", $"conditionMeters.{meterId}.current", "conditions"));
        }
    }

    private static void ValidateMeterLock(Character character, string meterId, string debilityId, List<ValidationIssue> issues)
    {
        var debilityMarked = character.Debilities.Any(debility => debility.Marked && debility.DefinitionId == debilityId);
        if (!debilityMarked)
        {
            return;
        }

        var meter = meterId switch
        {
            "health" => character.ConditionMeters.Health,
            "spirit" => character.ConditionMeters.Spirit,
            "supply" => character.ConditionMeters.Supply,
            _ => null
        };

        if (meter is { Current: > 0 } && !meter.IsLocked)
        {
            issues.Add(Warning($"meter.{meterId}.increaseBlocked", $"{debilityId} blocks {meterId} increases until cleared.", $"conditionMeters.{meterId}", "conditions"));
        }
    }

    private static void ValidateDebilities(Character character, Ruleset ruleset, List<ValidationIssue> issues)
    {
        var knownIds = ruleset.Debilities.Select(debility => debility.Id).ToHashSet(StringComparer.OrdinalIgnoreCase);
        foreach (var debility in character.Debilities)
        {
            if (!knownIds.Contains(debility.DefinitionId))
            {
                issues.Add(Error("debility.unknown", "Debility is not defined by the ruleset.", $"debilities.{debility.DefinitionId}", "conditions"));
            }
        }
    }

    private static void ValidateMomentum(Character character, Ruleset ruleset, List<ValidationIssue> issues)
    {
        var derived = MomentumService.Derive(character.Momentum, character.Debilities, ruleset.Momentum);
        if (character.Momentum.Current < derived.Minimum || character.Momentum.Current > derived.Max)
        {
            issues.Add(Error("momentum.outOfRange", $"Momentum must be between {derived.Minimum} and {derived.Max}.", "momentum.current", "conditions"));
        }

        if (character.Momentum.Max != derived.Max || character.Momentum.Reset != derived.Reset || character.Momentum.MarkedDebilityCount != derived.MarkedDebilityCount)
        {
            issues.Add(Warning("momentum.derivedMismatch", "Momentum max/reset should be recalculated from marked debilities.", "momentum", "conditions"));
        }
    }

    private static void ValidateAssets(Character character, Ruleset ruleset, bool requireFinalizedFields, List<ValidationIssue> issues)
    {
        if (requireFinalizedFields && character.Assets.Count != ruleset.StartingCharacter.RequiredAssetCount)
        {
            issues.Add(Error("assets.invalidCount", $"Select exactly {ruleset.StartingCharacter.RequiredAssetCount} assets before finalizing.", "assets", "assets"));
        }

        var definitions = ruleset.Assets.ToDictionary(asset => asset.Id, StringComparer.OrdinalIgnoreCase);
        foreach (var asset in character.Assets)
        {
            if (!definitions.TryGetValue(asset.DefinitionId, out var definition))
            {
                issues.Add(Error("assets.unknown", "Selected asset is not in the ruleset catalog.", $"assets.{asset.DefinitionId}", "assets"));
                continue;
            }

            if (definition.RequiresCompanionName && string.IsNullOrWhiteSpace(asset.CompanionName))
            {
                issues.Add(Error("assets.companionNameRequired", "Companion name is required for this asset.", $"assets.{asset.DefinitionId}.companionName", "assets"));
            }
        }

        foreach (var duplicate in character.Assets.GroupBy(asset => asset.DefinitionId, StringComparer.OrdinalIgnoreCase).Where(group => group.Count() > 1))
        {
            var definition = ruleset.Assets.FirstOrDefault(asset => StringComparer.OrdinalIgnoreCase.Equals(asset.Id, duplicate.Key));
            if (definition is not null && !definition.PermitsDuplicates)
            {
                issues.Add(Error("assets.duplicate", "This asset cannot be selected more than once.", $"assets.{duplicate.Key}", "assets"));
            }
        }
    }

    private static void ValidateProgressTracks(Character character, Ruleset ruleset, List<ValidationIssue> issues)
    {
        for (var index = 0; index < character.ProgressTracks.Count; index++)
        {
            var track = character.ProgressTracks[index];
            if (track.Ticks is < 0 or > ProgressTrack.DefaultMaxTicks || track.MaxTicks != ProgressTrack.DefaultMaxTicks)
            {
                issues.Add(Error("progressTrack.ticksOutOfRange", "Progress track ticks must be between 0 and 40.", $"progressTracks[{index}].ticks", "progress"));
            }

            if (!string.IsNullOrWhiteSpace(track.Rank) && !ruleset.Ranks.Contains(track.Rank, StringComparer.OrdinalIgnoreCase))
            {
                issues.Add(Error("progressTrack.rankUnknown", "Progress track rank is not defined by the ruleset.", $"progressTracks[{index}].rank", "progress"));
            }
        }
    }

    private static void ValidateVows(Character character, Ruleset ruleset, bool requireFinalizedFields, List<ValidationIssue> issues)
    {
        var activeVows = character.Vows.Where(vow => vow.Status == "active").ToArray();
        if (requireFinalizedFields && activeVows.Length < ruleset.StartingCharacter.RequiredActiveVowCount)
        {
            issues.Add(Error("vow.activeRequired", "At least one active vow is required before finalizing.", "vows", "vows"));
        }

        for (var index = 0; index < character.Vows.Count; index++)
        {
            var vow = character.Vows[index];
            if (vow.Status == "active" && string.IsNullOrWhiteSpace(vow.Title))
            {
                issues.Add(Error("vow.titleRequired", "Active vows require a title.", $"vows[{index}].title", "vows"));
            }

            if (vow.Status == "active" && string.IsNullOrWhiteSpace(vow.ProgressTrackId))
            {
                issues.Add(Error("vow.progressTrackRequired", "Active vows require a progress track.", $"vows[{index}].progressTrackId", "vows"));
            }

            if (!string.IsNullOrWhiteSpace(vow.Rank) && !ruleset.Ranks.Contains(vow.Rank, StringComparer.OrdinalIgnoreCase))
            {
                issues.Add(Error("vow.rankUnknown", "Vow rank is not defined by the ruleset.", $"vows[{index}].rank", "vows"));
            }
        }
    }

    private static void ValidateBonds(Character character, Ruleset ruleset, List<ValidationIssue> issues)
    {
        var startingBondCount = character.Bonds.Count(bond => bond.CreatedFromBackground);
        if (startingBondCount > ruleset.StartingCharacter.MaxStartingBonds)
        {
            issues.Add(Error("bond.tooManyStarting", $"Starting background bonds are limited to {ruleset.StartingCharacter.MaxStartingBonds}.", "bonds", "bonds"));
        }

        for (var index = 0; index < character.Bonds.Count; index++)
        {
            if (string.IsNullOrWhiteSpace(character.Bonds[index].Name))
            {
                issues.Add(Error("bond.nameRequired", "Bond name is required.", $"bonds[{index}].name", "bonds"));
            }
        }

        foreach (var duplicate in character.Bonds.Where(bond => !string.IsNullOrWhiteSpace(bond.Name)).GroupBy(bond => bond.Name.Trim(), StringComparer.OrdinalIgnoreCase).Where(group => group.Count() > 1))
        {
            issues.Add(Warning("bond.duplicateName", "A bond with this name already exists.", "bonds", "bonds"));
        }
    }

    private static ValidationIssue Error(string code, string message, string field, string step) => new(code, message, field, step, ValidationSeverity.Error);

    private static ValidationIssue Warning(string code, string message, string field, string step) => new(code, message, field, step, ValidationSeverity.Warning);
}
