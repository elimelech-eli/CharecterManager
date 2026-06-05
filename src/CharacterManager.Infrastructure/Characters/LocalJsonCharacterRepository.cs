using System.Text.Json;
using System.Text.Json.Serialization;
using CharacterManager.Core.Characters;

namespace CharacterManager.Infrastructure.Characters;

public sealed class LocalJsonCharacterRepository : ICharacterRepository
{
    private const string CharactersDirectoryName = "characters";
    private readonly string _charactersDirectory;
    private readonly JsonSerializerOptions _jsonOptions;

    public LocalJsonCharacterRepository(string storageRoot)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(storageRoot);

        _charactersDirectory = Path.Combine(storageRoot, CharactersDirectoryName);
        _jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            WriteIndented = true
        };
        _jsonOptions.Converters.Add(new JsonStringEnumConverter());
    }

    public async Task<IReadOnlyList<CharacterListEntry>> ListAsync(CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(_charactersDirectory);

        var entries = new List<CharacterListEntry>();
        foreach (var path in Directory.EnumerateFiles(_charactersDirectory, "*.json"))
        {
            var loadResult = await LoadFromPathAsync(path, cancellationToken);
            if (loadResult.Character is not null)
            {
                entries.Add(new CharacterListEntry(
                    Id: loadResult.Character.Id,
                    Character: loadResult.Character,
                    IsLoadable: loadResult.Error is null,
                    NeedsReview: loadResult.Error is not null,
                    ErrorCode: loadResult.Error?.Code,
                    ErrorMessage: loadResult.Error?.Message,
                    LastUpdatedAt: loadResult.Character.UpdatedAt));
                continue;
            }

            entries.Add(new CharacterListEntry(
                Id: TryParseIdFromFileName(path),
                Character: null,
                IsLoadable: false,
                NeedsReview: true,
                ErrorCode: loadResult.Error?.Code ?? "character.loadFailed",
                ErrorMessage: loadResult.Error?.Message ?? "Character record could not be loaded.",
                LastUpdatedAt: File.GetLastWriteTimeUtc(path)));
        }

        return entries
            .OrderByDescending(entry => entry.LastUpdatedAt ?? DateTimeOffset.MinValue)
            .ToArray();
    }

    public async Task<CharacterLoadResult> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var path = GetCharacterPath(id);
        if (!File.Exists(path))
        {
            return new CharacterLoadResult(null, new PersistenceError("character.notFound", "Character was not found.", "characterId"));
        }

        return await LoadFromPathAsync(path, cancellationToken);
    }

    public async Task SaveAsync(Character character, CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(_charactersDirectory);

        var path = GetCharacterPath(character.Id);
        var tempPath = Path.Combine(_charactersDirectory, $"{character.Id:N}.{Guid.NewGuid():N}.tmp");
        await using (var stream = File.Create(tempPath))
        {
            await JsonSerializer.SerializeAsync(stream, character, _jsonOptions, cancellationToken);
            await stream.FlushAsync(cancellationToken);
        }

        File.Move(tempPath, path, overwrite: true);
    }

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var path = GetCharacterPath(id);
        if (File.Exists(path))
        {
            File.Delete(path);
        }

        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        return Task.FromResult(File.Exists(GetCharacterPath(id)));
    }

    private async Task<CharacterLoadResult> LoadFromPathAsync(string path, CancellationToken cancellationToken)
    {
        try
        {
            await using var stream = File.OpenRead(path);
            var character = await JsonSerializer.DeserializeAsync<Character>(stream, _jsonOptions, cancellationToken);
            if (character is null)
            {
                return new CharacterLoadResult(null, new PersistenceError("character.emptyRecord", "Character record is empty.", Path.GetFileName(path)));
            }

            if (character.SchemaVersion != Character.CurrentSchemaVersion)
            {
                return new CharacterLoadResult(character, new PersistenceError("character.unsupportedSchemaVersion", "Character schema version is not supported.", "schemaVersion"));
            }

            return new CharacterLoadResult(character, null);
        }
        catch (JsonException exception)
        {
            return new CharacterLoadResult(null, new PersistenceError("character.invalidJson", $"Character record is invalid JSON: {exception.Message}", Path.GetFileName(path)));
        }
        catch (IOException exception)
        {
            return new CharacterLoadResult(null, new PersistenceError("character.readFailed", $"Character record could not be read: {exception.Message}", Path.GetFileName(path)));
        }
        catch (NotSupportedException exception)
        {
            return new CharacterLoadResult(null, new PersistenceError("character.unsupportedRecord", $"Character record shape is unsupported: {exception.Message}", Path.GetFileName(path)));
        }
    }

    private string GetCharacterPath(Guid id) => Path.Combine(_charactersDirectory, $"{id:N}.json");

    private static Guid? TryParseIdFromFileName(string path)
    {
        var fileName = Path.GetFileNameWithoutExtension(path);
        return Guid.TryParseExact(fileName, "N", out var id) || Guid.TryParse(fileName, out id) ? id : null;
    }
}
