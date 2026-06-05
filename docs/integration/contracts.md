# Integration Contracts: Ironsworn Character Creator MVP

## Purpose

This document defines the frontend/backend boundary for the MVP. The backend is a local ASP.NET service launched by Electron. The frontend is React/Electron and should communicate with the backend through versioned DTO contracts, not by duplicating Core domain types.

All shapes below are implementation-ready contract targets. Names can be adjusted during implementation, but any change should be reviewed by both the backend and frontend developer.

## Boundary Expectations

- Base route: `/api/v1`.
- Payload format: JSON.
- IDs: strings containing GUIDs for mutable records unless otherwise noted.
- Dates: ISO 8601 UTC strings.
- Durable state authority: backend.
- UI may keep draft form state locally, but persisted character state must roundtrip through backend.
- Ruleset catalog definitions are read-only from the UI perspective.
- DTOs include versions for schema and rules/content compatibility.

## API Surface

### Health

`GET /health`

Response:

```json
{
  "service": "CharacterManager.Api",
  "version": "0.1.0"
}
```

### Rulesets

`GET /api/v1/rulesets`

Response:

```json
{
  "items": [
    {
      "id": "ironsworn",
      "name": "Ironsworn",
      "rulesetVersion": "0.1.0",
      "contentVersion": "0.1.0",
      "licensingStatus": "metadataOnly"
    }
  ]
}
```

`GET /api/v1/rulesets/{rulesetId}`

Response: `RulesetDto`.

### Characters

`GET /api/v1/characters`

Returns summaries for complete characters, drafts, and loadable invalid records.

`POST /api/v1/characters/drafts`

Creates a draft character.

`GET /api/v1/characters/{characterId}`

Returns full `CharacterDto`.

`PUT /api/v1/characters/{characterId}`

Replaces or patches the editable character draft/state. MVP recommendation: start with full document update for wizard drafts, then add targeted mutation endpoints for play updates.

`POST /api/v1/characters/{characterId}/validate`

Runs backend validation and returns `ValidationResultDto`.

`POST /api/v1/characters/{characterId}/finalize`

Finalizes a valid draft and returns `CharacterDto`.

`DELETE /api/v1/characters/{characterId}`

Deletes a character.

### Targeted State Updates

These endpoints reduce accidental full-document conflicts during play:

- `POST /api/v1/characters/{characterId}/meters/{meterId}`
- `POST /api/v1/characters/{characterId}/momentum`
- `POST /api/v1/characters/{characterId}/debilities/{debilityId}`
- `POST /api/v1/characters/{characterId}/vows`
- `PUT /api/v1/characters/{characterId}/vows/{vowId}`
- `POST /api/v1/characters/{characterId}/bonds`
- `PUT /api/v1/characters/{characterId}/bonds/{bondId}`
- `POST /api/v1/characters/{characterId}/progress-tracks`
- `PUT /api/v1/characters/{characterId}/progress-tracks/{trackId}`

All mutation responses should return either the updated `CharacterDto` or a smaller `CharacterMutationResultDto` containing updated character version, changed fields, derived state, and validation.

### Sheet Projection

`GET /api/v1/characters/{characterId}/sheet`

Returns `CharacterSheetDto`, optimized for read-only display/print/export.

## DTOs

### RulesetDto

```json
{
  "id": "ironsworn",
  "name": "Ironsworn",
  "rulesetVersion": "0.1.0",
  "contentVersion": "0.1.0",
  "licensingStatus": "metadataOnly",
  "stats": [
    {
      "id": "edge",
      "name": "Edge",
      "min": 1,
      "max": 3,
      "summary": "Agility, speed, ranged precision.",
      "sourceReference": null
    }
  ],
  "startingCharacter": {
    "statArray": [3, 2, 2, 1, 1],
    "health": 5,
    "spirit": 5,
    "supply": 5,
    "momentum": 2,
    "momentumMax": 10,
    "momentumReset": 2,
    "requiredAssetCount": 3,
    "maxStartingBonds": 3,
    "requiredActiveVowCount": 1
  },
  "meters": [
    { "id": "health", "name": "Health", "min": 0, "max": 5 },
    { "id": "spirit", "name": "Spirit", "min": 0, "max": 5 },
    { "id": "supply", "name": "Supply", "min": 0, "max": 5 }
  ],
  "momentum": {
    "minimum": -6,
    "baseMax": 10,
    "resetByMarkedDebilityCount": [
      { "markedCount": 0, "reset": 2 },
      { "markedCount": 1, "reset": 1 },
      { "markedCount": 2, "reset": 0 }
    ]
  },
  "ranks": [
    "troublesome",
    "dangerous",
    "formidable",
    "extreme",
    "epic"
  ],
  "debilities": [
    {
      "id": "wounded",
      "name": "Wounded",
      "category": "condition",
      "blocksMeterIncrease": "health",
      "isPermanentByDefault": false,
      "requiresLinkedVow": false,
      "sourceReference": null
    }
  ],
  "assets": [
    {
      "id": "asset-id",
      "name": "Asset Name",
      "type": "path",
      "summary": "Approved short summary or placeholder.",
      "abilities": [],
      "requiresCompanionName": false,
      "permitsDuplicates": false,
      "sourceReference": null,
      "licensingStatus": "metadataOnly"
    }
  ]
}
```

TODO: Confirm legal/product policy before `licensingStatus` ever becomes `fullTextApproved`.

### CharacterSummaryDto

```json
{
  "id": "8a82fe1e-4c3b-48d1-b5f7-c8b8c6f1d900",
  "name": "Kara Iron-Eyes",
  "displayName": "Kara Iron-Eyes",
  "concept": "Exiled scout",
  "rulesetId": "ironsworn",
  "status": "finalized",
  "isValid": true,
  "needsReview": false,
  "lastUpdatedAt": "2026-06-05T12:00:00Z",
  "stateSummary": {
    "health": 5,
    "spirit": 4,
    "supply": 3,
    "momentum": 2,
    "activeVowCount": 2,
    "markedDebilityCount": 0
  }
}
```

### CharacterDto

```json
{
  "id": "8a82fe1e-4c3b-48d1-b5f7-c8b8c6f1d900",
  "schemaVersion": 1,
  "rulesetId": "ironsworn",
  "rulesetVersion": "0.1.0",
  "contentVersion": "0.1.0",
  "status": "draft",
  "name": "Kara Iron-Eyes",
  "concept": "Exiled scout",
  "backgroundNotes": "",
  "equipmentNotes": "",
  "stats": {
    "edge": 3,
    "heart": 2,
    "iron": 2,
    "shadow": 1,
    "wits": 1
  },
  "conditionMeters": {
    "health": { "current": 5, "min": 0, "max": 5, "isLocked": false, "lockedByDebilityId": null },
    "spirit": { "current": 5, "min": 0, "max": 5, "isLocked": false, "lockedByDebilityId": null },
    "supply": { "current": 5, "min": 0, "max": 5, "isLocked": false, "lockedByDebilityId": null }
  },
  "momentum": {
    "current": 2,
    "max": 10,
    "reset": 2,
    "minimum": -6,
    "markedDebilityCount": 0
  },
  "debilities": [],
  "assets": [],
  "bonds": [],
  "vows": [],
  "progressTracks": [],
  "experience": {
    "earned": 0,
    "spent": 0,
    "available": 0
  },
  "createdAt": "2026-06-05T12:00:00Z",
  "updatedAt": "2026-06-05T12:00:00Z"
}
```

### DebilityStateDto

```json
{
  "definitionId": "wounded",
  "marked": true,
  "notes": "Marked after Endure Harm.",
  "linkedVowId": null,
  "markedAt": "2026-06-05T12:00:00Z",
  "clearedAt": null
}
```

### SelectedAssetDto

```json
{
  "definitionId": "asset-id",
  "selectedAbilityIds": ["ability-1"],
  "companionName": null,
  "companionHealth": null,
  "notes": "",
  "sourceReference": null
}
```

### BondDto

```json
{
  "id": "bond-id",
  "name": "Ravencliff Village",
  "type": "community",
  "description": "Sheltered Kara during winter.",
  "location": "Ravencliff",
  "status": "active",
  "notes": "",
  "createdFromBackground": true,
  "createdAt": "2026-06-05T12:00:00Z"
}
```

### VowDto

```json
{
  "id": "vow-id",
  "title": "Recover the lost banner",
  "description": "",
  "rank": "dangerous",
  "status": "active",
  "progressTrackId": "track-id",
  "isBackgroundVow": false,
  "isIncitingIncident": true,
  "experienceAwarded": 0,
  "notes": "",
  "createdAt": "2026-06-05T12:00:00Z",
  "completedAt": null
}
```

### ProgressTrackDto

```json
{
  "id": "track-id",
  "name": "Recover the lost banner",
  "type": "vow",
  "rank": "dangerous",
  "ticks": 8,
  "maxTicks": 40,
  "progressScore": 2,
  "status": "active",
  "sharedScope": "character",
  "notes": ""
}
```

## Request Shapes

### Create Draft

`POST /api/v1/characters/drafts`

```json
{
  "rulesetId": "ironsworn"
}
```

Response: `201 Created` with `CharacterDto`.

### Update Meter

`POST /api/v1/characters/{characterId}/meters/{meterId}`

```json
{
  "operation": "delta",
  "delta": -1,
  "value": null,
  "note": "Harm from a clash."
}
```

Rules:

- `operation` is `delta` or `set`.
- `delta` required for `delta`.
- `value` required for `set`.

### Update Momentum

```json
{
  "operation": "delta",
  "delta": 1,
  "value": null,
  "resetToDerivedReset": false,
  "note": "Secure an Advantage outcome."
}
```

### Toggle Debility

```json
{
  "marked": true,
  "note": "Marked after Endure Harm.",
  "overridePermanentClear": false,
  "linkedVowId": null
}
```

### Mark Progress

```json
{
  "operation": "addTicks",
  "ticks": 1,
  "note": "Reached a milestone."
}
```

Allowed operations:

- `addTicks`
- `removeTicks`
- `setTicks`

### Finalize Draft

`POST /api/v1/characters/{characterId}/finalize`

```json
{
  "standardRules": true
}
```

Response:

- `200 OK` with `CharacterDto` if valid.
- `400 Bad Request` with `ValidationResultDto` if invalid.

## Error And Validation Format

### ValidationResultDto

```json
{
  "isValid": false,
  "errors": [
    {
      "code": "stats.invalidDistribution",
      "message": "Use exactly one 3, two 2s, and two 1s.",
      "field": "stats",
      "step": "stats",
      "severity": "error"
    }
  ],
  "warnings": [
    {
      "code": "bond.duplicateName",
      "message": "A bond with this name already exists.",
      "field": "bonds[1].name",
      "step": "bonds",
      "severity": "warning"
    }
  ]
}
```

### ErrorEnvelopeDto

```json
{
  "error": {
    "code": "character.notFound",
    "message": "Character was not found.",
    "target": "characterId",
    "details": []
  }
}
```

Recommended HTTP status mapping:

- `400`: validation failure or malformed request.
- `404`: record not found.
- `409`: version/conflict issue.
- `422`: semantically valid JSON that cannot be applied because of domain state, if the team wants to distinguish from `400`.
- `500`: unexpected backend failure.
- `503`: local storage/backend dependency unavailable.

## CharacterMutationResultDto

```json
{
  "characterId": "8a82fe1e-4c3b-48d1-b5f7-c8b8c6f1d900",
  "updatedAt": "2026-06-05T12:05:00Z",
  "changedFields": ["conditionMeters.health"],
  "character": {},
  "validation": {
    "isValid": true,
    "errors": [],
    "warnings": []
  }
}
```

MVP can return the full updated `CharacterDto` in `character` for simplicity.

## Versioning Expectations

- API routes are versioned as `/api/v1`.
- Character payloads include `schemaVersion`.
- Rules content includes `rulesetVersion` and `contentVersion`.
- Frontend should not assume unknown enum values are impossible; render fallback labels.
- Backend should reject unsupported future schema versions with a clear error.
- Additive fields are allowed within `v1`.
- Breaking DTO changes require `/api/v2` or coordinated release before implementation dependencies exist.

## Contract Review Checklist

- Backend and frontend agree on field paths used in validation errors.
- Frontend can render drafts and finalized characters from the same `CharacterDto`.
- Asset catalog can represent metadata-only content.
- Sheet projection does not require frontend to recalculate derived momentum.
- Error envelope covers validation, not found, persistence unavailable, and backend unavailable states.
