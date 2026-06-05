namespace CharacterManager.Core.Characters;

public interface ICharacterRepository
{
    Task<IReadOnlyList<CharacterListEntry>> ListAsync(CancellationToken cancellationToken);

    Task<CharacterLoadResult> GetAsync(Guid id, CancellationToken cancellationToken);

    Task SaveAsync(Character character, CancellationToken cancellationToken);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken);

    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken);
}

public sealed record CharacterListEntry(
    Guid? Id,
    Character? Character,
    bool IsLoadable,
    bool NeedsReview,
    string? ErrorCode,
    string? ErrorMessage,
    DateTimeOffset? LastUpdatedAt);

public sealed record CharacterLoadResult(Character? Character, PersistenceError? Error)
{
    public bool IsFound => Character is not null || Error?.Code is not "character.notFound";
    public bool IsLoadable => Character is not null && Error is null;
}

public sealed record PersistenceError(string Code, string Message, string? Target);
