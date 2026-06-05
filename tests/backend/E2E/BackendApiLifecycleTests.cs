using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using Xunit;

namespace CharacterManager.Backend.E2E.Tests;

public sealed class BackendApiLifecycleTests
{
    [Fact]
    public async Task HealthAndCompatibilityRollEndpointWork()
    {
        using var temp = new TempDirectory();
        using var factory = new BackendApiFactory(temp.Path);
        using var client = factory.CreateClient();

        var health = await GetJsonAsync(client, "/health");
        var roll = await PostJsonAsync(client, "/rolls/action", new { modifier = 1 });

        Assert.Equal("CharacterManager.Api", health["service"]?.GetValue<string>());
        Assert.True(roll["actionScore"]?.GetValue<int>() >= 2);
        Assert.NotNull(roll["outcome"]?.GetValue<string>());
    }

    [Fact]
    public async Task RulesetEndpointsReturnMvpIronswornMetadata()
    {
        using var temp = new TempDirectory();
        using var factory = new BackendApiFactory(temp.Path);
        using var client = factory.CreateClient();

        var list = await GetJsonAsync(client, "/api/v1/rulesets");
        var detail = await GetJsonAsync(client, "/api/v1/rulesets/ironsworn");

        Assert.Equal("ironsworn", list["items"]?[0]?["id"]?.GetValue<string>());
        Assert.Equal("summaryApproved", detail["licensingStatus"]?.GetValue<string>());
        Assert.Equal("edge", detail["stats"]?[0]?["id"]?.GetValue<string>());
        Assert.Equal(3, detail["meters"]?.AsArray().Count);
        Assert.Contains(detail["debilities"]!.AsArray(), node => node?["id"]?.GetValue<string>() == "wounded");
        Assert.Contains(detail["assets"]!.AsArray(), asset => asset?["id"]?.GetValue<string>() == "alchemist");
        Assert.Contains(detail["assets"]!.AsArray(), asset => asset?["id"]?.GetValue<string>() == "cave-lion");
        Assert.DoesNotContain(detail["assets"]!.AsArray(), asset => asset?["id"]?.GetValue<string>()?.Contains("placeholder", StringComparison.OrdinalIgnoreCase) == true);
        Assert.All(detail["assets"]!.AsArray(), asset => Assert.Equal("summaryApproved", asset?["licensingStatus"]?.GetValue<string>()));
    }

    [Fact]
    public async Task CreateDraftPersistsJsonAndSurvivesApiFactoryRestart()
    {
        using var temp = new TempDirectory();
        string characterId;

        using (var firstFactory = new BackendApiFactory(temp.Path))
        using (var firstClient = firstFactory.CreateClient())
        {
            var draft = await CreateDraftAsync(firstClient);
            characterId = draft["id"]!.GetValue<string>();
        }

        using var secondFactory = new BackendApiFactory(temp.Path);
        using var secondClient = secondFactory.CreateClient();
        var reloaded = await GetJsonAsync(secondClient, $"/api/v1/characters/{characterId}");
        var list = await GetJsonAsync(secondClient, "/api/v1/characters");

        Assert.Equal(characterId, reloaded["id"]?.GetValue<string>());
        Assert.Equal(characterId, list["items"]?[0]?["id"]?.GetValue<string>());
        Assert.Single(Directory.GetFiles(System.IO.Path.Combine(temp.Path, "characters"), "*.json"));
    }

    [Fact]
    public async Task FinalizeIncompleteDraftReturnsStructuredValidationErrors()
    {
        using var temp = new TempDirectory();
        using var factory = new BackendApiFactory(temp.Path);
        using var client = factory.CreateClient();
        var draft = await CreateDraftAsync(client);

        var response = await client.PostAsJsonAsync($"/api/v1/characters/{draft["id"]!.GetValue<string>()}/finalize", new { standardRules = true });
        var validation = await ReadJsonAsync(response);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.False(validation["isValid"]!.GetValue<bool>());
        Assert.Contains(validation["errors"]!.AsArray(), error => HasIssue(error, "character.nameRequired", "name", "concept", "error"));
        Assert.Contains(validation["errors"]!.AsArray(), error => HasIssue(error, "stats.invalidDistribution", "stats", "stats", "error"));
        Assert.Contains(validation["errors"]!.AsArray(), error => HasIssue(error, "assets.invalidCount", "assets", "assets", "error"));
        Assert.Contains(validation["errors"]!.AsArray(), error => HasIssue(error, "vow.activeRequired", "vows", "vows", "error"));
    }

    [Fact]
    public async Task CompleteCharacterCanBeUpdatedValidatedFinalizedListedAndDeleted()
    {
        using var temp = new TempDirectory();
        using var factory = new BackendApiFactory(temp.Path);
        using var client = factory.CreateClient();
        var draft = await CreateDraftAsync(client);
        var complete = MakeCompleteCharacter(draft);
        var characterId = complete["id"]!.GetValue<string>();

        var updateResponse = await client.PutAsJsonAsync($"/api/v1/characters/{characterId}", complete);
        var updated = await ReadJsonAsync(updateResponse);
        var validation = await PostJsonAsync(client, $"/api/v1/characters/{characterId}/validate", new { });
        var finalizeResponse = await client.PostAsJsonAsync($"/api/v1/characters/{characterId}/finalize", new { standardRules = true });
        var finalized = await ReadJsonAsync(finalizeResponse);
        var list = await GetJsonAsync(client, "/api/v1/characters");
        var deleteResponse = await client.DeleteAsync($"/api/v1/characters/{characterId}");
        var getAfterDelete = await client.GetAsync($"/api/v1/characters/{characterId}");

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        Assert.Equal("Kara Iron-Eyes", updated["name"]?.GetValue<string>());
        Assert.True(validation["isValid"]!.GetValue<bool>());
        Assert.Equal(HttpStatusCode.OK, finalizeResponse.StatusCode);
        Assert.Equal("finalized", finalized["status"]?.GetValue<string>());
        Assert.Equal(characterId, list["items"]?[0]?["id"]?.GetValue<string>());
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, getAfterDelete.StatusCode);
    }

    [Fact]
    public async Task BlockedMeterIncreaseReturnsValidationResponse()
    {
        using var temp = new TempDirectory();
        using var factory = new BackendApiFactory(temp.Path);
        using var client = factory.CreateClient();
        var draft = await CreateDraftAsync(client);
        var character = MakeCompleteCharacter(draft);
        var characterId = character["id"]!.GetValue<string>();
        character["debilities"] = new JsonArray(new JsonObject
        {
            ["definitionId"] = "wounded",
            ["marked"] = true,
            ["notes"] = "",
            ["linkedVowId"] = null,
            ["markedAt"] = DateTimeOffset.UtcNow,
            ["clearedAt"] = null
        });
        character["conditionMeters"]!["health"]!["current"] = 3;

        var initialSave = await client.PutAsJsonAsync($"/api/v1/characters/{characterId}", character);
        Assert.Equal(HttpStatusCode.OK, initialSave.StatusCode);

        character["conditionMeters"]!["health"]!["current"] = 4;
        var blockedResponse = await client.PutAsJsonAsync($"/api/v1/characters/{characterId}", character);
        var validation = await ReadJsonAsync(blockedResponse);

        Assert.Equal(HttpStatusCode.BadRequest, blockedResponse.StatusCode);
        Assert.Contains(validation["errors"]!.AsArray(), error => HasIssue(error, "meter.health.increaseBlocked", "conditionMeters.health.current", "conditions", "error"));
    }

    [Fact]
    public async Task InvalidPersistedJsonIsListedSafely()
    {
        using var temp = new TempDirectory();
        var charactersPath = System.IO.Path.Combine(temp.Path, "characters");
        Directory.CreateDirectory(charactersPath);
        File.WriteAllText(System.IO.Path.Combine(charactersPath, $"{Guid.NewGuid():N}.json"), "{ invalid json");
        using var factory = new BackendApiFactory(temp.Path);
        using var client = factory.CreateClient();

        var list = await GetJsonAsync(client, "/api/v1/characters");

        Assert.Equal("invalid", list["items"]?[0]?["status"]?.GetValue<string>());
        Assert.True(list["items"]?[0]?["needsReview"]?.GetValue<bool>());
        Assert.Equal("character.invalidJson", list["items"]?[0]?["error"]?["code"]?.GetValue<string>());
    }

    [Fact]
    public async Task NotFoundAndBadRequestEndpointsReturnErrorEnvelopes()
    {
        using var temp = new TempDirectory();
        using var factory = new BackendApiFactory(temp.Path);
        using var client = factory.CreateClient();
        var draft = await CreateDraftAsync(client);
        var routeMismatchPayload = MakeCompleteCharacter(draft);

        var missingRuleset = await client.GetAsync("/api/v1/rulesets/missing");
        var missingCharacter = await client.GetAsync($"/api/v1/characters/{Guid.NewGuid():D}");
        var routeMismatch = await client.PutAsJsonAsync($"/api/v1/characters/{Guid.NewGuid():D}", routeMismatchPayload);
        var deleteMissing = await client.DeleteAsync($"/api/v1/characters/{Guid.NewGuid():D}");

        AssertErrorEnvelope(await ReadJsonAsync(missingRuleset), "ruleset.notFound", "rulesetId");
        AssertErrorEnvelope(await ReadJsonAsync(missingCharacter), "character.notFound", "characterId");
        AssertErrorEnvelope(await ReadJsonAsync(routeMismatch), "character.idMismatch", "characterId");
        AssertErrorEnvelope(await ReadJsonAsync(deleteMissing), "character.notFound", "characterId");
    }

    private static async Task<JsonObject> CreateDraftAsync(HttpClient client)
    {
        return await PostJsonAsync(client, "/api/v1/characters/drafts", new { rulesetId = "ironsworn" });
    }

    private static async Task<JsonObject> GetJsonAsync(HttpClient client, string uri)
    {
        var response = await client.GetAsync(uri);
        return await ReadJsonAsync(response);
    }

    private static async Task<JsonObject> PostJsonAsync(HttpClient client, string uri, object body)
    {
        var response = await client.PostAsJsonAsync(uri, body);
        return await ReadJsonAsync(response);
    }

    private static async Task<JsonObject> ReadJsonAsync(HttpResponseMessage response)
    {
        var json = await response.Content.ReadFromJsonAsync<JsonObject>();
        return json ?? throw new InvalidOperationException("Expected a JSON response body.");
    }

    private static JsonObject MakeCompleteCharacter(JsonObject draft)
    {
        draft["name"] = "Kara Iron-Eyes";
        draft["concept"] = "Exiled scout";
        draft["stats"] = new JsonObject
        {
            ["edge"] = 3,
            ["heart"] = 2,
            ["iron"] = 2,
            ["shadow"] = 1,
            ["wits"] = 1
        };
        draft["assets"] = new JsonArray(
            Asset("alchemist", null),
            Asset("cave-lion", "Ash"),
            Asset("archer", null));
        draft["progressTracks"] = new JsonArray(new JsonObject
        {
            ["id"] = "track-id",
            ["name"] = "Recover the lost banner",
            ["type"] = "vow",
            ["rank"] = "dangerous",
            ["ticks"] = 8,
            ["maxTicks"] = 40,
            ["status"] = "active",
            ["sharedScope"] = "character",
            ["notes"] = ""
        });
        draft["vows"] = new JsonArray(new JsonObject
        {
            ["id"] = "vow-id",
            ["title"] = "Recover the lost banner",
            ["description"] = "",
            ["rank"] = "dangerous",
            ["status"] = "active",
            ["progressTrackId"] = "track-id",
            ["isBackgroundVow"] = false,
            ["isIncitingIncident"] = true,
            ["experienceAwarded"] = 0,
            ["notes"] = "",
            ["createdAt"] = DateTimeOffset.UtcNow,
            ["completedAt"] = null
        });

        return draft;
    }

    private static JsonObject Asset(string definitionId, string? companionName) => new()
    {
        ["definitionId"] = definitionId,
        ["selectedAbilityIds"] = new JsonArray("ability-1"),
        ["companionName"] = companionName,
        ["companionHealth"] = companionName is null ? null : 5,
        ["notes"] = "",
        ["sourceReference"] = null
    };

    private static bool HasIssue(JsonNode? issue, string code, string field, string step, string severity)
    {
        return issue?["code"]?.GetValue<string>() == code
            && issue["field"]?.GetValue<string>() == field
            && issue["step"]?.GetValue<string>() == step
            && issue["severity"]?.GetValue<string>() == severity;
    }

    private static void AssertErrorEnvelope(JsonObject body, string code, string target)
    {
        Assert.Equal(code, body["error"]?["code"]?.GetValue<string>());
        Assert.Equal(target, body["error"]?["target"]?.GetValue<string>());
        Assert.NotNull(body["error"]?["message"]?.GetValue<string>());
    }
}
