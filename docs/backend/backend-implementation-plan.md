# Backend Implementation Plan: Ironsworn Character Creator MVP

## Implementation Principles

- Build from the existing `CharacterManager.Core`, `CharacterManager.Api`, and `CharacterManager.Infrastructure` projects.
- Keep production behavior local-first and offline.
- Put rules and validation in Core, not React components.
- Expose DTO contracts through versioned API routes.
- Treat ruleset catalog data as versioned content.
- Add tests for every crisp rule.
- Do not ingest or expose full Ironsworn official text until licensing/attribution is approved.

## Recommended Implementation Order

1. Expand Core domain model.
2. Add ruleset catalog definitions and seeded Ironsworn metadata.
3. Add validation services.
4. Add character creation commands and draft/finalized validation.
5. Add local persistence abstraction and implementation.
6. Add character API endpoints.
7. Add state update endpoints for meters, momentum, debilities, vows, bonds, and tracks.
8. Add sheet projection endpoint/DTO.
9. Add migration/version handling.
10. Harden error handling, logging, and tests.

## Work Items

### B1. Define MVP Domain Model

Scope:

- Expand `Character` beyond current name/stats/assets/progress shape.
- Add value objects for stats, meters, momentum, debilities, selected assets, vows, bonds, and progress tracks.
- Add draft/finalized status, schema version, ruleset version, created/updated timestamps.

Dependencies:

- Product domain model.
- Rules summary.

Acceptance criteria:

- Character aggregate can represent every MVP field from product docs.
- Character aggregate distinguishes catalog definition references from mutable character state.
- Draft characters can be incomplete.
- Finalized characters have the fields required for standard Ironsworn creation.
- No infrastructure dependencies exist in Core domain types.

Testing notes:

- Unit tests construct minimal draft and complete finalized character fixtures.
- Unit tests verify defaults for starting meters and momentum.

Risks/open questions:

- TODO: Decide whether finalized characters require both starting vow types or one active vow.
- TODO: Decide whether custom/variant finalized characters are allowed in MVP.

### B2. Build Ruleset Catalog Model

Scope:

- Extend ruleset definitions with starting defaults, meter definitions, debilities, ranks, progress track types, and asset metadata.
- Preserve current `IRulesetCatalog` boundary.
- Replace or supplement the in-memory seed with versioned JSON/content loader when practical.

Dependencies:

- B1 domain vocabulary.
- Licensing decision for content detail level.

Acceptance criteria:

- `/rulesets` or `/api/v1/rulesets` can list Ironsworn core.
- Ruleset detail includes stat definitions, starting stat array, meter defaults, momentum rules, debilities, ranks, and asset metadata placeholders or approved summaries.
- Catalog includes `rulesetVersion` and `contentVersion`.
- Catalog loader validates required ids and duplicate ids at startup.
- Catalog does not contain unapproved full move or asset text.

Testing notes:

- Unit tests verify catalog contains Edge, Heart, Iron, Shadow, Wits.
- Unit tests verify debility definitions map to correct meter locks and momentum effects.
- Unit tests verify duplicate ids fail catalog validation.

Risks/open questions:

- TODO: Product/legal must approve how much asset and move text can be stored/displayed.
- Asset picker usefulness is limited if only metadata placeholders are available.

### B3. Add Validation Services

Scope:

- Implement validators for stats, assets, vows, bonds, meters, momentum, progress tracks, and complete character creation.
- Return structured validation results with error code, field path, message, severity, and optional step.

Dependencies:

- B1 domain model.
- B2 catalog definitions.

Acceptance criteria:

- Standard stats must exactly match `3,2,2,1,1`.
- Health, spirit, and supply remain within `0..5`.
- Wounded blocks health increase, Shaken blocks spirit increase, Unprepared blocks supply increase.
- Momentum max/reset derives from marked debilities.
- Progress tracks remain within `0..40` ticks.
- Finalized standard character requires name, ruleset, valid stats, exactly three assets, valid required asset fields, and at least one active vow.
- Validation can return warnings for duplicate bonds and narrative asset prerequisites.

Testing notes:

- Parameterized tests cover valid and invalid stat arrays.
- Tests cover each meter lock.
- Tests cover zero, one, and multiple marked debilities.
- Tests cover progress lower/upper bounds.

Risks/open questions:

- TODO: Decide whether clearing permanent banes is blocked or allowed with explicit override in MVP.
- TODO: Decide how asset prerequisites are encoded when they are narrative.

### B4. Create Character Application Commands

Scope:

- Add command/service methods for create draft, update draft, validate draft, finalize character.
- Ensure finalization reruns backend validation regardless of frontend state.

Dependencies:

- B1, B2, B3.

Acceptance criteria:

- UI can save an incomplete draft.
- UI can request validation for a draft and receive grouped errors.
- UI can finalize only when backend validation passes.
- Finalized character receives stable id, timestamps, schema version, and ruleset/content version.
- Invalid finalization returns `400` with validation response; no partial finalized record is written.

Testing notes:

- Unit tests for draft creation with missing name.
- Unit tests for finalization failure.
- Unit tests for successful finalization.

Risks/open questions:

- Draft storage may create records with little identifying data; UI needs a display fallback such as "Untitled draft".

### B5. Implement Local Character Persistence

Scope:

- Add `ICharacterRepository`.
- Implement local storage in Infrastructure.
- Choose local JSON or SQLite behind the repository boundary.
- Include schema version and basic migration hooks.

Dependencies:

- B1 persistence shape.
- Desktop launch strategy for app data path.

Acceptance criteria:

- Characters can be created, listed, loaded, updated, and deleted.
- Data survives backend restart and app restart.
- Writes are atomic enough to avoid corrupting existing records on failed save.
- Invalid persisted data is detected and reported without crashing list/load.
- Repository does not leak filesystem details to API consumers.

Testing notes:

- Integration tests use a temp data directory.
- Tests verify save/load roundtrip.
- Tests verify invalid/corrupt record appears as load error or needs-review status.

Risks/open questions:

- TODO: Choose JSON vs SQLite.
- TODO: Decide local data path passed by Electron and development fallback path.

### B6. Add Versioned Character API Endpoints

Scope:

- Create `/api/v1` route group.
- Add character CRUD endpoints using DTOs from `docs/integration/contracts.md`.
- Keep existing `/health`, `/rulesets`, and `/rolls/action` either as compatibility endpoints or move them behind versioned routes with frontend coordination.

Dependencies:

- B4 commands.
- B5 persistence.
- Integration contracts.

Acceptance criteria:

- `GET /api/v1/characters` returns summary list including drafts and invalid records where possible.
- `POST /api/v1/characters/drafts` creates a draft.
- `GET /api/v1/characters/{id}` returns full character DTO.
- `PUT /api/v1/characters/{id}` updates draft or character state.
- `POST /api/v1/characters/{id}/validate` returns validation result.
- `POST /api/v1/characters/{id}/finalize` finalizes valid draft.
- `DELETE /api/v1/characters/{id}` deletes a character.
- Validation and not-found responses use a consistent error envelope.

Testing notes:

- API integration tests cover happy paths and validation failures.
- Contract snapshot tests can guard DTO shape once frontend begins integration.

Risks/open questions:

- Endpoint granularity could drift if every UI field gets its own endpoint. Start with document update plus targeted state update commands where validation needs context.

### B7. Add In-Play State Update Commands

Scope:

- Add backend commands/endpoints for common state updates: meters, momentum, debility toggle, vow update, bond update, progress mark.
- Return updated character or updated projection plus validation/derived effects.

Dependencies:

- B3 validators.
- B6 API baseline.

Acceptance criteria:

- Health/spirit/supply updates enforce range and locks.
- Momentum updates enforce min/max.
- Debility toggles recalculate derived momentum and meter locks.
- Vows and tracks enforce rank/status/tick bounds.
- Bonds enforce required names.
- Responses include updated derived momentum values.
- Optional note/source fields can be persisted for the update if change log is included in MVP; otherwise DTO leaves room for future history.

Testing notes:

- Unit tests for each command.
- API tests for lock violations and derived momentum responses.

Risks/open questions:

- TODO: Confirm whether Undo Last Change/change history is MVP or deferred. If deferred, avoid event-sourcing complexity.

### B8. Add Sheet Projection

Scope:

- Provide a backend DTO optimized for read-only sheet rendering and export.
- Include only approved source/reference text.

Dependencies:

- B6 character read.
- Content licensing policy.

Acceptance criteria:

- Sheet projection includes name, concept, stats, meters, momentum, debilities, assets, vows, bonds, progress tracks, equipment notes, and source/content version metadata.
- Projection warns if character is incomplete.
- Projection does not include unapproved full official text.
- UI can render printable sheet without re-deriving backend rules.

Testing notes:

- Projection tests verify derived momentum and progress score.
- Tests verify incomplete draft warning appears.

Risks/open questions:

- TODO: Attribution requirements for printable/exported views must be finalized before public release.

### B9. Add Migration And Version Handling

Scope:

- Add schema version constants.
- Add persistence migration pipeline.
- Validate ruleset/content version compatibility on load.

Dependencies:

- B5 persistence.

Acceptance criteria:

- Persisted characters include `schemaVersion`.
- Load path rejects unsupported future versions with a clear error.
- Load path can migrate known older versions.
- Ruleset/content version mismatch is surfaced as a warning or needs-review status.

Testing notes:

- Tests cover current version load.
- Tests cover unsupported version response.

Risks/open questions:

- Multiple rulesets are deferred, but version fields should exist now to avoid painful migrations.

### B10. Backend Test And Quality Pass

Scope:

- Add focused unit and API tests.
- Document manual smoke checks.
- Review contracts with frontend developer.

Dependencies:

- All backend implementation work items.

Acceptance criteria:

- Core validation tests cover every crisp MVP rule.
- API tests cover character lifecycle.
- Persistence tests cover restart-style save/load.
- Error responses match integration docs.
- No production endpoint exposes full unapproved Ironsworn text.

Testing notes:

- Use .NET test project under `tests`.
- Prefer pure Core tests for rules.
- Use temp directories for Infrastructure tests.

Risks/open questions:

- If tests are added late, DTO and domain shape will be harder to adjust safely.

## Cross-Cutting Testing Notes

- Validate every state mutation both in Core unit tests and at least once through API integration.
- Use deterministic fixtures for complete character, incomplete draft, and invalid persisted character.
- Keep roll engine tests separate from character creator tests; full move automation is deferred.
- Include negative tests for licensing/content assumptions by ensuring full text fields are absent or clearly gated.

## Backend Risks And Open Questions

- TODO: Licensing/attribution for full Ironsworn move and asset text.
- TODO: Local storage engine choice.
- TODO: Finalized vow requirements.
- TODO: MVP status of undo/change history.
- TODO: Exact Electron-to-backend app data path handoff.
- TODO: Whether generic journey/fight tracks are MVP or deferred. Product docs lean toward generic tracks being useful for play; implementation can include generic character-attached progress tracks without combat automation.
