# UI Implementation Plan: Ironsworn Character Creator MVP

## Implementation Principles

- Reuse the existing `apps/desktop` React/Vite/Electron structure and design-system primitives.
- Build actual app screens, not a landing page.
- Keep backend contracts behind a typed API layer.
- Treat backend validation as authoritative.
- Keep creation guidance concise and field-adjacent.
- Optimize the active character screens for repeated use during play.

## Recommended Implementation Order

1. App shell and routing.
2. API client and DTO types.
3. Ruleset/catalog loading.
4. Character Library.
5. Wizard skeleton and draft save.
6. Wizard steps and validation display.
7. Character Overview with backend data.
8. Conditions and progress mutation controls.
9. Assets/Bonds/Vows detail screens.
10. Sheet View and print styles.
11. Settings/status/attribution.
12. Accessibility, loading, and error-state pass.

## Work Items

### U1. Establish App Shell And Routing

Scope:

- Replace the design-system demo as the primary screen with an app shell.
- Add route/view structure for Dashboard, Characters, Create Character, Character Layout, and Settings.
- Preserve design-system primitives for reuse.

Dependencies:

- Existing design-system components.
- Design docs for information architecture.

Acceptance criteria:

- App opens to Dashboard or Character Library.
- Global sidebar includes Dashboard, Characters, Settings.
- Character-scoped nav appears only when a character is active.
- Navigation works without page reload.
- Current view is visually and semantically indicated.

Testing notes:

- Component smoke tests or manual browser checks for route navigation.
- Verify keyboard focus moves sensibly after navigation.

UX risks/open questions:

- If Dashboard and Character Library duplicate too much, MVP can start with Character Library as the first screen and add Dashboard later.

### U2. Add Typed API Client

Scope:

- Create frontend API client modules for health, rulesets, characters, validation, and sheet projection.
- Add TypeScript DTO types matching integration contracts.
- Centralize error envelope parsing.

Dependencies:

- Backend `/api/v1` contract or interim compatibility endpoints.
- Electron/preload configuration for backend base URL if needed.

Acceptance criteria:

- Components do not call raw `fetch` directly.
- API errors are normalized into typed errors.
- Validation errors preserve `field`, `step`, `code`, `message`, and `severity`.
- Backend base URL can be configured for dev and packaged Electron.

Testing notes:

- Unit tests for response/error parsing if test harness exists.
- Manual test against `/health` and `/rulesets`.

UX risks/open questions:

- TODO: Confirm how Electron exposes the local backend URL to the renderer.

### U3. Load Ruleset Catalog

Scope:

- Fetch Ironsworn ruleset metadata.
- Provide catalog data to wizard and character screens.
- Handle catalog loading/error states.

Dependencies:

- Backend ruleset endpoint.

Acceptance criteria:

- Stats step uses catalog stat definitions and starting array.
- Assets step uses catalog asset metadata or a clear placeholder/unavailable state.
- Debilities and ranks come from catalog data.
- Catalog unavailable blocks final creation but allows draft save.
- UI displays ruleset/content version in Settings or debug/status area.

Testing notes:

- Manual check with backend running.
- Mock catalog unavailable state.

UX risks/open questions:

- Asset selection is weaker if full catalog content is deferred. Use ids, names, types, short summaries, and source references when approved.

### U4. Build Character Library

Scope:

- Render saved characters and drafts.
- Add empty state and New Character action.
- Display invalid/needs-review records from backend if present.

Dependencies:

- Character list endpoint.

Acceptance criteria:

- Empty library clearly offers New Character.
- Complete characters and drafts are visually distinct.
- Character cards show name or fallback title, concept, status, last updated, and core state summary when available.
- Opening a character routes to Overview.
- Delete action is available only if backend endpoint exists and should require confirmation.

Testing notes:

- Manual checks for empty, draft, complete, invalid, and load-error states.

UX risks/open questions:

- TODO: Decide whether drafts appear mixed with complete characters or in a separate Drafts section.

### U5. Build Wizard Shell And Draft Lifecycle

Scope:

- Implement wizard stepper, footer actions, and shared draft state.
- Wire Save Draft to backend.
- Support Back/Continue and Review navigation.

Dependencies:

- Character draft endpoints.
- Ruleset catalog.

Acceptance criteria:

- Wizard includes Concept, Stats, Assets, Bonds, Starting Vow, Review.
- Save Draft is available from every step.
- Draft save status is visible.
- Navigating back preserves entered values.
- Review can link back to steps with errors.
- Failed draft save keeps user input in memory and shows retry/status.

Testing notes:

- Manual check save from every step.
- Refresh/reopen draft if persistence endpoint exists.

UX risks/open questions:

- Too much validation during drafting can make drafts feel broken. Use warnings until finalization unless a step cannot continue.

### U6. Implement Concept Step

Scope:

- Fields: name, concept, background notes, equipment notes.
- Attach backend validation to name on Review/finalize.

Dependencies:

- Wizard shell.

Acceptance criteria:

- Name trims whitespace for validation display.
- Draft can save without name.
- Finalization shows field-level error if name is missing.
- Long notes remain usable without breaking layout.

Testing notes:

- Manual input tests for blank, whitespace, long notes.

UX risks/open questions:

- Equipment must remain narrative notes, not a structured inventory in MVP.

### U7. Implement Stats Step

Scope:

- Assign starting values `3,2,2,1,1` to Edge, Heart, Iron, Shadow, Wits.
- Show remaining values clearly.

Dependencies:

- Ruleset catalog.
- Validation response mapping.

Acceptance criteria:

- User can assign exactly five stat values.
- UI prevents or clearly flags invalid duplicate/missing distribution.
- Backend validation errors appear near the stat control and in the stepper.
- Step supports returning and editing without losing assignments.

Testing notes:

- Manual cases: valid `3,2,2,1,1`, two 3s, missing value, blank stat.

UX risks/open questions:

- TODO: Decide whether custom/variant stats are hidden, deferred, or exposed as a warning override.

### U8. Implement Assets Step

Scope:

- Browse/filter catalog by type.
- Select exactly three starting assets for finalized standard characters.
- Capture selected ability ids and companion fields if catalog marks them required.

Dependencies:

- Ruleset asset catalog content.
- Backend asset validation.

Acceptance criteria:

- Selected count is always visible.
- Duplicate selection is blocked or flagged according to catalog rules.
- Required companion name/state fields appear when needed.
- Catalog-unavailable state supports draft save but blocks final creation.
- Full official text is not displayed unless approved.

Testing notes:

- Manual cases: fewer than three, more than three, duplicate, companion missing name.

UX risks/open questions:

- TODO: Product/legal must decide approved asset text scope.

### U9. Implement Bonds Step

Scope:

- Add, edit, remove up to three starting background bonds.
- Capture name, relationship/type, location, notes.

Dependencies:

- Backend bond validation.

Acceptance criteria:

- Zero bonds is allowed.
- More than three starting bonds is blocked or flagged for standard finalization.
- Empty bond row with no name cannot finalize.
- Duplicate bond names produce warning, not hard block.

Testing notes:

- Manual cases: zero bonds, three bonds, four bonds, blank bond, duplicate names.

UX risks/open questions:

- Relationship notes should stay lightweight; avoid forcing NPC/campaign structure.

### U10. Implement Starting Vow Step

Scope:

- Capture background vow and inciting-incident vow fields.
- Require at least one active vow for finalized MVP.
- Strongly prompt for both vow types.

Dependencies:

- Backend vow/progress validation.
- Product decision on both vows.

Acceptance criteria:

- Vow title and rank are required for entered vows.
- Rank selector includes troublesome, dangerous, formidable, extreme, epic.
- Progress starts empty.
- Missing required vow data appears inline and on Review.
- At least one valid active vow is required to finalize.

Testing notes:

- Manual cases: no vows, one valid vow, two valid vows, missing rank, missing title.

UX risks/open questions:

- TODO: Decide whether both starting vow types are mandatory or one is acceptable with warning.

### U11. Implement Review And Finalize

Scope:

- Show complete character summary and validation checklist.
- Call backend validation/finalization.
- Route to Character Overview on success.

Dependencies:

- All wizard steps.
- Backend validate/finalize endpoints.

Acceptance criteria:

- Review lists all blocking errors grouped by step.
- Each error can route user back to the relevant step.
- Create Character is disabled or fails gracefully until backend validation passes.
- Successful finalize opens Character Overview.
- Failed finalize does not lose draft data.

Testing notes:

- Manual end-to-end valid character creation.
- Manual invalid review and correction loop.

UX risks/open questions:

- Review should not become a wall of rules text. Keep it checklist-oriented.

### U12. Build Character Overview

Scope:

- Render character header, stats, meters, momentum, debilities summary, active vows, assets/bonds summaries, and quick update entry.

Dependencies:

- Character detail endpoint.
- State update endpoints for controls.

Acceptance criteria:

- Overview shows all high-frequency play state at a glance.
- Meter and momentum summaries match backend DTO values.
- Active vows and key progress are visible.
- Draft or invalid character state is clearly labeled.
- Updates refresh the displayed derived state.

Testing notes:

- Manual check complete character.
- Manual check draft/incomplete character.

UX risks/open questions:

- Overview can become cluttered. Put detail editing in nested screens and keep Overview focused.

### U13. Build Conditions Controls

Scope:

- Health, spirit, supply, momentum controls.
- Debility groups and lock messages.

Dependencies:

- Backend mutation endpoints and validation errors.

Acceptance criteria:

- Health/spirit/supply cannot display values outside `0..5`.
- Blocked recovery errors appear next to the relevant meter.
- Debility toggles update derived momentum max/reset.
- Momentum cannot exceed derived max or go below minimum without backend validation error.
- Permanent/quest-linked debility clearing can require note when backend supports it.

Testing notes:

- Manual cases for Wounded/Shaken/Unprepared locks.
- Manual cases for one and multiple debilities.

UX risks/open questions:

- If every debility update opens a modal, play slows down. Use inline preview/confirmation for derived effects.

### U14. Build Vows, Bonds, And Progress Screens

Scope:

- Vow cards, bond cards, progress track controls, add/edit forms.

Dependencies:

- Backend update endpoints.

Acceptance criteria:

- Progress controls support ticks from `0..40`.
- Progress score/full boxes are visually clear.
- Vow title/rank/status validation displays inline.
- Bond names validate inline.
- Generic character-attached tracks can be displayed if backend includes them.

Testing notes:

- Manual cases for adding/removing ticks, max progress, missing vow rank/title.

UX risks/open questions:

- TODO: Confirm whether generic journey/fight tracks are MVP or deferred.

### U15. Build Assets Screen

Scope:

- Display selected assets, ability states, companion sub-state, notes, and source references.

Dependencies:

- Ruleset asset catalog.
- Character detail endpoint.

Acceptance criteria:

- Selected assets render even if full text is unavailable.
- Missing asset definitions show a catalog warning instead of crashing.
- Companion fields render when present.
- Source/reference labels are visible where available.

Testing notes:

- Manual check selected assets with and without catalog definitions.

UX risks/open questions:

- TODO: Full asset text display is blocked by licensing/attribution decision.

### U16. Build Sheet View And Print Path

Scope:

- Render backend sheet projection.
- Add print-friendly CSS and print action.

Dependencies:

- Backend sheet projection endpoint.
- Attribution policy.

Acceptance criteria:

- Sheet includes name, concept, stats, meters, momentum, debilities, assets, vows, bonds, progress, notes.
- Draft/incomplete sheet shows warning.
- Long text wraps without overlap.
- Print preview is usable at common desktop paper sizes.
- Attribution placeholder or finalized attribution appears.
- No unapproved full official text appears.

Testing notes:

- Browser/Electron print preview check.
- Manual long-vow/long-notes overflow check.

UX risks/open questions:

- TODO: Confirm attribution requirements and whether official sheet layout can be mimicked.

### U17. Settings And Status

Scope:

- Show app version, backend health, local data status, ruleset/content version, and attribution/licensing note.

Dependencies:

- Health/ruleset endpoints.

Acceptance criteria:

- Settings does not block character creation.
- Backend unavailable state is understandable.
- Rules content version is visible.
- Attribution/legal status is visible as pending or finalized.

Testing notes:

- Manual backend available/unavailable states.

UX risks/open questions:

- Avoid turning Settings into required setup for MVP.

### U18. Accessibility And Visual QA Pass

Scope:

- Keyboard navigation.
- Focus states.
- Screen reader labels.
- Text overflow.
- High-frequency control sizing.
- Error readability.

Dependencies:

- All screens.

Acceptance criteria:

- Primary flows are keyboard reachable.
- Inputs have labels.
- Error/warning states are not color-only.
- Buttons and controls do not resize unpredictably.
- Text does not overlap on desktop and narrow responsive widths.
- Progress tracks remain understandable without hover.

Testing notes:

- Manual keyboard pass.
- Browser screenshots for main screens.
- Check long text and validation messages.

UX risks/open questions:

- The design language is dark and atmospheric; ensure contrast remains practical and readable.

## Frontend Testing Notes

- Add component tests where project test tooling exists; otherwise begin with manual smoke checklist documented in PR.
- Mock backend responses for core UI states if test framework is added.
- Use contract fixtures shared with backend where possible.
- Verify creation flow end-to-end against running backend before declaring MVP flow complete.

## UX Risks And Open Questions

- TODO: Licensing and attribution for full text and exports.
- TODO: Whether both starting vows are mandatory.
- TODO: Draft grouping in library.
- TODO: Custom/variant character support.
- TODO: Generic journey/fight tracks in MVP.
- TODO: Undo Last Change scope.
