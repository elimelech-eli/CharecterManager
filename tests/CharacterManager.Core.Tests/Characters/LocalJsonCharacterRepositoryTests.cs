using CharacterManager.Core.Characters;
using CharacterManager.Infrastructure.Characters;
using Xunit;

namespace CharacterManager.Core.Tests.Characters;

public sealed class LocalJsonCharacterRepositoryTests
{
    [Fact]
    public async Task SavesAndLoadsCharacterRoundtrip()
    {
        using var temp = new TempDirectory();
        var ruleset = await TestFixtures.GetRulesetAsync();
        var character = TestFixtures.CompleteCharacter(ruleset);
        var repository = new LocalJsonCharacterRepository(temp.Path);

        await repository.SaveAsync(character, CancellationToken.None);
        var loaded = await repository.GetAsync(character.Id, CancellationToken.None);

        Assert.Null(loaded.Error);
        Assert.Equal(character.Id, loaded.Character?.Id);
        Assert.Equal(character.Name, loaded.Character?.Name);
        Assert.Equal(character.RulesetVersion, loaded.Character?.RulesetVersion);
    }

    [Fact]
    public async Task ListReportsInvalidJsonWithoutThrowing()
    {
        using var temp = new TempDirectory();
        var charactersPath = System.IO.Path.Combine(temp.Path, "characters");
        Directory.CreateDirectory(charactersPath);
        File.WriteAllText(System.IO.Path.Combine(charactersPath, $"{Guid.NewGuid():N}.json"), "{ invalid json");
        var repository = new LocalJsonCharacterRepository(temp.Path);

        var entries = await repository.ListAsync(CancellationToken.None);

        var entry = Assert.Single(entries);
        Assert.False(entry.IsLoadable);
        Assert.True(entry.NeedsReview);
        Assert.Equal("character.invalidJson", entry.ErrorCode);
    }

    [Fact]
    public async Task DeleteRemovesPersistedCharacter()
    {
        using var temp = new TempDirectory();
        var ruleset = await TestFixtures.GetRulesetAsync();
        var character = TestFixtures.CompleteCharacter(ruleset);
        var repository = new LocalJsonCharacterRepository(temp.Path);

        await repository.SaveAsync(character, CancellationToken.None);
        await repository.DeleteAsync(character.Id, CancellationToken.None);

        Assert.False(await repository.ExistsAsync(character.Id, CancellationToken.None));
        var loaded = await repository.GetAsync(character.Id, CancellationToken.None);
        Assert.Equal("character.notFound", loaded.Error?.Code);
    }
}

internal sealed class TempDirectory : IDisposable
{
    public string Path { get; } = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"cm-tests-{Guid.NewGuid():N}");

    public TempDirectory()
    {
        Directory.CreateDirectory(Path);
    }

    public void Dispose()
    {
        if (Directory.Exists(Path))
        {
            Directory.Delete(Path, recursive: true);
        }
    }
}
