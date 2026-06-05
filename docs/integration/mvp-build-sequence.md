# MVP Build Sequence: Ironsworn Character Creator

## Purpose

This plan coordinates one C#/.NET backend developer and one React/Electron frontend developer through an implementation-ready MVP. It follows the product roadmap while making integration checkpoints explicit.

The goal is a local desktop app that can create, validate, save, reopen, update, and print/view an Ironsworn core character without shipping unresolved copyrighted text.

## Team Ownership

- Backend developer owns Core domain, validation, local persistence, ruleset catalog, API endpoints, migrations, and backend tests.
- Frontend developer owns Electron/React shell, API client, wizard, character screens, validation display, sheet view, and UI tests/manual QA.
- Both developers jointly own DTO contracts, integration fixtures, and end-to-end smoke checks.

## Milestone 0: Contract And Fixture Alignment

Goal:

- Agree on the first DTO shapes and shared test fixtures before heavy implementation.

Backend tasks:

- Review `docs/integration/contracts.md`.
- Create a complete character fixture and incomplete draft fixture in backend tests or sample data.
- Confirm interim route names if `/api/v1` cannot be implemented immediately.

Frontend tasks:

- Create TypeScript DTO types from contracts.
- Create mock fixtures matching backend fixtures.
- Prepare API client skeleton.

Integration checkpoint:

- Both developers can load the same complete character shape from fixtures.

Definition of done:

- DTO names, enum values, validation field paths, and error format are agreed.
- Fixture includes stats, meters, momentum, debilities, assets, vows, bonds, and progress tracks.

Risks/TODOs:

- TODO: Confirm local backend base URL handoff from Electron to React.

## Milestone 1: Ruleset Catalog And Static Character Sheet

Goal:

- Establish rules metadata and render a static/read-only character state.

Backend tasks:

- Expand ruleset catalog model.
- Seed Ironsworn core metadata: stats, starting defaults, meters, momentum rules, debilities, ranks, asset placeholders/approved summaries.
- Add ruleset endpoints.
- Add sheet projection shape from fixture or persisted character.

Frontend tasks:

- Build app shell and navigation.
- Load ruleset metadata.
- Build Character Overview read-only fixture.
- Build Sheet View read-only fixture.

Integration checkpoint:

- Frontend renders backend-provided ruleset metadata and a read-only character fixture.

Definition of done:

- UI displays stats, meters, momentum, debilities, assets, vows, bonds, and progress.
- Catalog includes version metadata.
- No full unapproved Ironsworn text is exposed.

Risks/TODOs:

- TODO: Licensing/attribution decision for move and asset text.

## Milestone 2: Character Creation Wizard

Goal:

- Create and validate a standard Ironsworn starting character draft.

Backend tasks:

- Add draft create/update/validate/finalize commands.
- Implement creation validation: name, stats, starting meters, assets, bonds, vows, debilities.
- Return validation errors with `field` and `step`.

Frontend tasks:

- Build wizard shell and stepper.
- Implement Concept, Stats, Assets, Bonds, Starting Vow, Review.
- Wire Save Draft, Validate, and Finalize.
- Display field/step/review validation.

Integration checkpoint:

- End-to-end creation flow creates a valid finalized character and opens Overview.

Definition of done:

- Valid stats require `3,2,2,1,1`.
- Exactly three starting assets required for finalized standard character.
- At least one active vow required.
- Drafts can save while incomplete.
- Review lists all blocking validation errors and links back to steps.

Risks/TODOs:

- TODO: Decide whether finalized characters require both starting vow types.
- TODO: Decide asset catalog content strategy for MVP.

## Milestone 3: Local Persistence

Goal:

- Save, reopen, list, update, and delete characters locally.

Backend tasks:

- Implement `ICharacterRepository`.
- Choose and implement local JSON or SQLite storage.
- Add atomic writes and invalid-data detection.
- Add list/load/save/delete API endpoints.
- Add schema version handling.

Frontend tasks:

- Build Character Library from backend list.
- Show drafts, complete characters, invalid/needs-review records.
- Open saved character from library.
- Add delete confirmation if endpoint exists.

Integration checkpoint:

- Create a character, restart backend/app, reopen the character from Character Library.

Definition of done:

- Character data survives restart.
- Invalid persisted data is surfaced without crashing.
- Draft and finalized statuses display correctly.
- API and UI both expose local save failures clearly.

Risks/TODOs:

- TODO: Choose local storage engine.
- TODO: Confirm app data path in packaged Electron.

## Milestone 4: In-Play Character State Management

Goal:

- Support fast manual updates during play.

Backend tasks:

- Add mutation commands/endpoints for meters, momentum, debilities, vows, bonds, and progress tracks.
- Enforce meter locks and progress bounds.
- Return updated derived momentum.

Frontend tasks:

- Build Character Overview update controls.
- Build Conditions screen.
- Build Vows & Progress screen.
- Build Bonds screen.
- Build Assets screen.
- Display backend validation next to controls.

Integration checkpoint:

- User can adjust health/spirit/supply/momentum, mark debilities, mark progress, add/edit vows and bonds, then reopen saved state.

Definition of done:

- Health, spirit, and supply stay within `0..5`.
- Wounded, Shaken, and Unprepared block relevant recovery.
- Debilities recalculate max/reset momentum.
- Progress tracks support `0..40` ticks and progress score display.
- State updates persist and reload.

Risks/TODOs:

- TODO: Decide whether Undo Last Change/change log is MVP.
- TODO: Confirm generic journey/fight progress tracks for MVP.

## Milestone 5: Printable Sheet, Attribution, And Polish

Goal:

- Make the MVP usable, trustworthy, and shareable.

Backend tasks:

- Finalize sheet projection endpoint.
- Include rules/content version and incomplete-character warnings.
- Add attribution fields/placeholders as product/legal defines.

Frontend tasks:

- Build print-friendly Sheet View.
- Add print action.
- Add Settings local data/rules content/attribution sections.
- Perform accessibility and visual QA pass.

Integration checkpoint:

- Complete character can be opened in Sheet View and printed/exported through the browser/Electron print path.

Definition of done:

- Sheet includes core fields and selected options.
- Long vows/assets/notes wrap without overlap.
- Draft/incomplete character warning appears.
- Attribution/legal note appears where required or is clearly marked pending for internal builds.
- Primary flows are keyboard reachable and validation is not color-only.

Risks/TODOs:

- TODO: Confirm exact attribution text and export policy.
- TODO: Confirm whether official playkit layout can be visually mimicked.

## Parallelization Plan

### Backend can start immediately

- Domain model expansion.
- Ruleset catalog shape.
- Validation services.
- Persistence abstraction.
- DTO contract draft.

### Frontend can start immediately

- App shell and routes.
- API client skeleton.
- Mock DTO fixtures.
- Wizard and sheet component skeletons using fixtures.
- Validation display components using mocked validation results.

### Work that should wait for contract checkpoint

- Final API wiring for wizard finalize.
- State mutation controls.
- Sheet projection wiring.
- Persisted invalid-record display.

## Integration Checkpoints

1. Ruleset catalog loads in UI.
2. Character fixture renders in Overview and Sheet View.
3. Draft can be saved from wizard.
4. Backend validation errors display in the correct wizard step.
5. Valid draft finalizes and opens Overview.
6. Character persists across restart.
7. Meter/debility/progress updates persist and reload.
8. Sheet View prints without layout failures.

## Cross-Milestone Definition Of Done

- No production code exposes unapproved full Ironsworn text.
- Backend validation exists for every crisp MVP rule.
- Frontend displays backend validation at the right field/step/control.
- Character records include schema version, ruleset version, and content version.
- Local persistence works offline.
- Drafts and finalized characters are visually distinct.
- All TODO decisions that block release are either resolved or explicitly deferred from MVP.

## Handoff Checklist

- Backend provides current OpenAPI-like route list or endpoint summary.
- Backend provides sample valid character, invalid draft, and validation error payloads.
- Frontend provides screen-level smoke checklist.
- Both developers confirm DTO field paths used by validation errors.
- Product confirms unresolved MVP behavior decisions or accepts TODO deferrals.
- Product/legal confirms attribution and content-text policy before public release.

## Developer Readiness Checklist

- [ ] Architecture docs exist for backend, UI, and integration.
- [ ] Backend work items have implementation order and acceptance criteria.
- [ ] Frontend work items have implementation order and acceptance criteria.
- [ ] DTOs and error envelopes are documented.
- [ ] MVP milestones include integration checkpoints.
- [ ] Licensing/content TODOs are flagged and not invented.
- [ ] Local persistence choice is explicitly unresolved and isolated behind an abstraction.
- [ ] Vow requirement ambiguity is explicitly flagged.
- [ ] Storage of supply on Character for MVP is documented with future campaign migration path.
- [ ] Ruleset catalog versioning is documented.
- [ ] Recommended next implementation task is clear: contract/fixture alignment, then ruleset catalog plus app shell in parallel.
