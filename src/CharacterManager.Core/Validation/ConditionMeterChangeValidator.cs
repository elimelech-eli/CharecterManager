using CharacterManager.Core.Characters;

namespace CharacterManager.Core.Validation;

public sealed class ConditionMeterChangeValidator
{
    public ValidationResult ValidateIncreaseLocks(Character previous, Character requested)
    {
        var issues = new List<ValidationIssue>();
        ValidateMeter(previous, requested, "health", "wounded", previous.ConditionMeters.Health, requested.ConditionMeters.Health, issues);
        ValidateMeter(previous, requested, "spirit", "shaken", previous.ConditionMeters.Spirit, requested.ConditionMeters.Spirit, issues);
        ValidateMeter(previous, requested, "supply", "unprepared", previous.ConditionMeters.Supply, requested.ConditionMeters.Supply, issues);
        return ValidationResult.FromIssues(issues);
    }

    private static void ValidateMeter(
        Character previous,
        Character requested,
        string meterId,
        string debilityId,
        MeterState previousMeter,
        MeterState requestedMeter,
        List<ValidationIssue> issues)
    {
        var isBlocked = previous.Debilities.Any(debility => debility.Marked && debility.DefinitionId == debilityId)
            || requested.Debilities.Any(debility => debility.Marked && debility.DefinitionId == debilityId);

        if (isBlocked && requestedMeter.Current > previousMeter.Current)
        {
            issues.Add(new ValidationIssue(
                $"meter.{meterId}.increaseBlocked",
                $"{debilityId} blocks {meterId} increases until cleared.",
                $"conditionMeters.{meterId}.current",
                "conditions",
                ValidationSeverity.Error));
        }
    }
}
