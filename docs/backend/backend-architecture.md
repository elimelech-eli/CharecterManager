# Backend Architecture: Ironsworn Character Creator MVP

## Purpose

This document converts the product model into a backend architecture for the Ironsworn character creator and manager MVP. It fits the current repository shape:

- `src/CharacterManager.Api`: local ASP.NET backend launched by Electron.
- `src/CharacterManager.Core`: domain entities, rules, validation, and pure application behavior.
- `src/CharacterManager.Infrastructure`: persistence and ruleset catalog adapters.

The MVP backend should be a local-first service. It should validate and persist character state, expose stable DTO contracts to the React/Electron UI, and keep rules content versioned without becoming a full rulebook or move automation engine.

## Backend Responsibilities

- Own character aggregate rules and state transitions.
- Expose local HTTP endpoints for character list/create/read/update/delete, validation, ruleset catalog reads, and export-ready sheet projections.
- Store and load characters locally with schema/version metadata.
- Provide an Ironsworn core ruleset catalog with stats, meters, debilities, ranks, progress track types, and asset metadata approved for MVP.
- Enforce crisp rules: stat array, meter bounds, progress bounds, starting asset count, required vows, and derived momentum.
- Return structured validation errors that the UI can attach to fields and wizard steps.
- Keep catalog definitions separate from mutable character state.
- Preserve future extension points for campaigns, shared supply, content packs, cloud sync, and additional rulesets.

The backend should not:

- Implement full move resolution for MVP.
- Store or expose full Ironsworn move/asset text until licensing/attribution is explicitly approved.
- Require cloud storage or user accounts.
- Model campaign/world/truths as required MVP entities.

## Domain Boundaries

### Character Aggregate

`Character` is the MVP write aggregate. It owns:

- Identity and metadata.
- Character concept/background/equipment notes.
- Stats.
- Selected assets and selected asset ability state.
- Health, spirit, supply, momentum, and debilities.
- Vows, bonds, and progress tracks.
- Draft/finalized status.
- Schema and ruleset version.

All user edits to character state should flow through character-scoped commands or services so validation and derived values stay consistent.

### Ruleset Catalog

`Ruleset` is read-only content metadata. It owns:

- Stat definitions.
- Meter definitions.
- Momentum defaults and derivation rules.
- Debility definitions.
- Progress ranks and track definitions.
- Asset definitions or placeholders, depending on approved content scope.
- Move summaries/state-impact metadata only if approved and needed.

Catalog data should be versioned independently from app binaries with `rulesetId`, `rulesetVersion`, and `contentVersion`.

### Persistence Boundary

Persistence should use an interface owned by Core or an application layer, implemented in Infrastructure. MVP should favor local JSON files or SQLite behind an abstraction. The exact storage choice is still open in product docs.

TODO: Choose local JSON vs SQLite before implementation. JSON is simpler for one-character documents and easy export. SQLite is stronger for queries/history. Either way, do not bind Core to a storage engine.

### API Boundary

The API should expose DTOs, not Core records directly. DTOs define the frontend/backend contract and can evolve with explicit versions.

## Core Entities And Value Objects

### Character

Fields:

- `id`
- `name`
- `rulesetId`
- `rulesetVersion`
- `schemaVersion`
- `status`: `draft` or `finalized`
- `concept`
- `backgroundNotes`
- `equipmentNotes`
- `stats`
- `assets`
- `conditionMeters`
- `momentum`
- `debilities`
- `vows`
- `bonds`
- `progressTracks`
- `experience`
- `createdAt`
- `updatedAt`

Rules:

- Name required for finalized characters.
- Ruleset required.
- Standard finalized creation requires stat values `3,2,2,1,1`.
- Standard finalized creation requires exactly three starting assets.
- Standard finalized creation requires at least one active vow.
- Starting health, spirit, and supply default to `5`.
- Starting momentum defaults to current `2`, max `10`, reset `2`.
- Starting debilities are unmarked unless imported or variant state is explicitly allowed.

TODO: Decide whether finalized standard characters must include both a background vow and inciting-incident vow, or whether one active vow is sufficient with a warning.

### StatBlock

Value object keyed by stat id:

- `edge`
- `heart`
- `iron`
- `shadow`
- `wits`

Rules:

- Values must be integers.
- Standard creation values must exactly match the configured starting array.
- MVP range is `1` to `3`.

### ConditionMeters

Value object:

- `health`
- `spirit`
- `supply`

Rules:

- Each meter range is `0` to `5`.
- `wounded` blocks health increases.
- `shaken` blocks spirit increases.
- `unprepared` blocks supply increases.
- Supply is stored on Character in MVP but must include enough contract clarity to migrate to campaign/shared supply later.

### Momentum

Value object:

- `current`
- `max`
- `reset`
- `minimum`

Rules:

- Minimum is `-6`.
- Derived max is `10 - markedDebilityCount`.
- Reset is `2` with no marked debilities, `1` with one marked debility, and `0` with two or more marked debilities.
- Current cannot exceed derived max.
- If debilities lower max below current, current should clamp to max or return a validation result requiring user confirmation. MVP recommendation: clamp only through explicit command preview; never silently mutate on read.

### Debility

Definition fields:

- `id`
- `name`
- `category`: condition, bane, burden
- `blocksMeterIncrease`
- `isPermanentByDefault`
- `requiresLinkedVow`
- `sourceReference`

Character state fields:

- `definitionId`
- `marked`
- `notes`
- `linkedVowId`
- `markedAt`
- `clearedAt`

Rules:

- Every marked debility contributes to derived momentum.
- Clearing permanent banes requires explicit override and note.
- Cursed/tormented should support an optional linked vow.

### SelectedAsset

Fields:

- `definitionId`
- `selectedAbilityIds`
- `companionName`
- `companionHealth`
- `notes`
- `sourceReference`

Rules:

- Standard finalized creation requires exactly three assets.
- Duplicate assets are invalid unless the catalog explicitly permits duplicates.
- Companion assets can require companion name and state.
- Asset prerequisites should be warnings with override notes in MVP because many prerequisites are narrative.

TODO: Product/legal must decide whether full asset card text can be stored and displayed. Until then, store ids, names, types, short summaries, selected ability metadata, and source references only.

### Vow

Fields:

- `id`
- `title`
- `description`
- `rank`
- `status`: active, fulfilled, forsaken
- `progressTrackId`
- `isBackgroundVow`
- `isIncitingIncident`
- `experienceAwarded`
- `notes`
- `createdAt`
- `completedAt`

Rules:

- Title and rank required for active/finalized vows.
- Rank must be one of troublesome, dangerous, formidable, extreme, epic.
- Active vow must reference a progress track.
- Progress cannot exceed track max.

### Bond

Fields:

- `id`
- `name`
- `type`
- `description`
- `location`
- `status`
- `notes`
- `createdFromBackground`
- `createdAt`

Rules:

- Name required.
- Starting background bonds limited to `0` to `3`.
- Duplicate names should warn, not hard fail.
- Bonds progress uses a progress track.

### ProgressTrack

Fields:

- `id`
- `name`
- `type`: vow, bonds, journey, fight, generic
- `rank`
- `ticks`
- `maxTicks`
- `status`
- `sharedScope`
- `notes`

Rules:

- `maxTicks` is `40`.
- `ticks` range is `0` to `40`.
- Progress score is full boxes only: `ticks / 4`, rounded down.
- Rank required for vows, journeys, fights, and other ranked tracks.
- MVP should support generic character-attached tracks; campaign/shared tracks are deferred.

## Validation Services

Recommended Core services:

- `CharacterCreationValidator`: validates draft/finalized creation requirements.
- `StatAssignmentValidator`: validates stat distribution against ruleset metadata.
- `AssetSelectionValidator`: validates count, duplicate selection, required companion fields, and prerequisite warnings.
- `VowValidator`: validates title, rank, status, and progress link.
- `BondValidator`: validates required names and starting limits.
- `ConditionMeterValidator`: validates range and recovery locks.
- `MomentumService`: computes derived max/reset and validates current value.
- `ProgressTrackValidator`: validates ticks, rank, type, and status.
- `CharacterMutationService`: coordinates multi-field changes and returns validation errors plus derived state.

Validation should return structured results, not throw for user-correctable input. Exceptions should be reserved for programming errors and infrastructure failures.

## Ruleset Catalog Design

The existing `IRulesetCatalog` and `InMemoryRulesetCatalog` are a useful starting point, but MVP needs more structured content:

- `RulesetDefinition`
  - id, name, version, contentVersion
  - stat definitions
  - starting character defaults
  - meter definitions
  - momentum rules
  - debility definitions
  - rank definitions
  - progress track definitions
  - asset definitions
  - move metadata, if used

Recommended MVP implementation:

1. Keep catalog access behind `IRulesetCatalog`.
2. Move seeded Ironsworn metadata to versioned JSON embedded in Infrastructure or loaded from an app data content folder.
3. Validate catalog data at API startup.
4. Return catalog DTOs to the UI through `/api/v1/rulesets/{id}` and lightweight summaries through `/api/v1/rulesets`.
5. Include content licensing status flags per content type:
   - `metadataOnly`
   - `summaryApproved`
   - `fullTextApproved`

TODO: Do not import full rulebook, move, or asset text until licensing and attribution requirements are documented.

## Persistence Strategy

MVP requirements:

- Offline by default.
- Save/reopen local characters.
- Detect invalid persisted data.
- Include schema version and ruleset/content version.
- Allow future import/export and cloud sync.

Recommended abstraction:

- `ICharacterRepository`
  - `ListAsync`
  - `GetAsync`
  - `SaveAsync`
  - `DeleteAsync`
  - `ExistsAsync`

Recommended storage model:

- One persisted character document per character, with a manifest/index for list performance.
- Store DTO-shaped persistence records separately from API DTOs if schema migration becomes non-trivial.
- Use atomic write semantics: write temp file, validate, then replace.
- Keep backups or last-known-good copies if using JSON files.

Recommended data location:

- Resolve from Electron/user app data path, passed to the backend through environment variable or command-line argument.
- Fall back to a documented development path only for local dev.

Deferred:

- MongoDB Atlas.
- Accounts/auth.
- Multi-device sync.
- Conflict resolution.

## Proposed Project And Module Ownership

### `CharacterManager.Core`

Owned by the backend developer.

Contains:

- Domain records/entities/value objects.
- Validation services.
- Derived momentum/progress calculations.
- Interfaces for repositories/catalogs if no separate Application project is created.
- No ASP.NET, filesystem, or JSON-specific persistence code.

### `CharacterManager.Api`

Owned by the backend developer, with DTO contract review from frontend developer.

Contains:

- Minimal API endpoints.
- Request/response DTOs.
- Dependency injection.
- API version route group.
- Error/validation response mapping.
- CORS/local Electron hosting settings.

### `CharacterManager.Infrastructure`

Owned by the backend developer.

Contains:

- Local character repository implementation.
- Ruleset catalog implementation.
- Catalog seed loading and validation.
- File path/app data adapters.

### `apps/desktop`

Owned by the React/Electron frontend developer.

Consumes API contracts only. It should not duplicate backend domain rules except for lightweight immediate UI hints. Final validation comes from backend.

## MVP vs Deferred

### MVP

- Ironsworn core ruleset metadata.
- Draft and finalized character states.
- Character CRUD.
- Standard creation validation.
- Local persistence.
- Derived momentum.
- Health/spirit/supply validation.
- Debility toggles.
- Vows, bonds, and character-attached progress tracks.
- Asset selection metadata and selected ability state.
- Sheet projection endpoint or DTO shape for printable UI.

### Deferred

- Full move engine.
- Roll automation beyond the existing action roller.
- Oracle engine.
- Campaign/world/truths setup.
- Shared campaign supply.
- Cloud sync and MongoDB Atlas.
- Multiple rulesets and supplement packs.
- Custom/homebrew ruleset authoring.
- Full asset/move text until licensing is resolved.
- Rich export formats beyond printable HTML/PDF-style UI.
