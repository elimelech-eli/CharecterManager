namespace CharacterManager.Core.Validation;

public sealed record ValidationResult(IReadOnlyList<ValidationIssue> Errors, IReadOnlyList<ValidationIssue> Warnings)
{
    public bool IsValid => Errors.Count == 0;

    public static ValidationResult FromIssues(IEnumerable<ValidationIssue> issues)
    {
        var materialized = issues.ToArray();
        return new ValidationResult(
            materialized.Where(issue => issue.Severity == ValidationSeverity.Error).ToArray(),
            materialized.Where(issue => issue.Severity == ValidationSeverity.Warning).ToArray());
    }
}

public sealed record ValidationIssue(
    string Code,
    string Message,
    string Field,
    string Step,
    ValidationSeverity Severity);

public enum ValidationSeverity
{
    Error,
    Warning
}
