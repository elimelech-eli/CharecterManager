# UI Architecture: Ironsworn Character Creator MVP

## Purpose

This document defines the React/Electron UI architecture for the Ironsworn character creator and manager MVP. It aligns with:

- Product docs in `docs/Product`.
- Rules summary in `docs/knowledge`.
- MVP roadmap/open questions in `docs/planning`.
- Existing design docs in `docs/design`.
- Current frontend shape in `apps/desktop`, which is a Vite/React/Electron app with reusable design-system primitives.

The MVP UI should be character-first, local-first, and optimized for two different modes:

- Guided creation, where validation and step clarity matter.
- In-play management, where fast updates and immediate state visibility matter.

## Frontend Responsibilities

- Render the desktop shell, navigation, screens, and design-system components.
- Manage wizard draft state and call backend validation/finalization.
- Display backend validation errors beside the relevant field, section, or wizard step.
- Render character overview, assets, bonds, vows/progress, conditions, settings, and printable sheet views.
- Call local backend API through a typed service layer.
- Keep optimistic UI limited to low-risk interactions; trust backend as final authority for rules.
- Persist only transient UI state in React. Durable character state belongs to the backend.
- Surface local save status, draft status, invalid data states, and attribution placeholders.

The UI should not:

- Duplicate all backend validation rules as production authority.
- Store full official rules text unless product/legal approves it.
- Build a full move resolver or oracle engine for MVP.
- Require users to create a campaign/account before creating a character.

## Application Shell

The app should use a stable desktop shell:

- Global sidebar:
  - Dashboard
  - Characters
  - Settings
- Character-scoped navigation when a character is active:
  - Overview
  - Assets
  - Bonds
  - Vows & Progress
  - Conditions
  - Sheet View
- Main content region with route-level screen components.
- Save/status area for selected character or draft.

The existing design-system primitives should be reused for buttons, panels, fields, badges, meters, tabs, modal surfaces, and sidebar items.

## Page And View Structure

Recommended route/view model:

- `DashboardView`
  - Resume recent character.
  - Show drafts needing attention.
  - New Character entry point.
- `CharacterLibraryView`
  - Saved characters and drafts.
  - Empty state.
  - Search/filter only when needed.
- `CreateCharacterWizardView`
  - Concept step.
  - Stats step.
  - Assets step.
  - Bonds step.
  - Starting Vow step.
  - Review step.
  - Created confirmation or route to Overview.
- `CharacterLayout`
  - Character header.
  - Character-scoped nav.
  - Nested character views.
- `CharacterOverviewView`
  - Stats and core meters.
  - Momentum and debility summary.
  - Active vows/progress preview.
  - Assets/bonds summaries.
  - Quick update entry.
- `AssetsView`
  - Selected asset cards.
  - Ability state and companion sub-state.
  - Source/reference labels.
- `BondsView`
  - Bonds progress track.
  - Bond records and add/edit form.
- `VowsProgressView`
  - Active vows.
  - Other character-attached progress tracks.
  - Manual tick/progress controls.
- `ConditionsView`
  - Health/spirit/supply controls.
  - Momentum control.
  - Debility groups and lock messages.
- `SheetView`
  - Read-only printable/shareable character sheet projection.
- `SettingsView`
  - Local data status.
  - Rules content version.
  - Attribution/licensing placeholder.

## Component Boundaries

### Shell Components

- `AppShell`
- `GlobalSidebar`
- `CharacterNav`
- `PageHeader`
- `SaveStatusIndicator`

These components should know navigation and display state, not domain validation.

### Data Boundary Components

- `CharacterDataProvider` or route loader.
- `RulesetDataProvider` or query hook.
- `ApiErrorBoundary`.

These components fetch data and normalize loading/error states.

### Wizard Components

- `CreateCharacterWizard`
- `WizardStepper`
- `WizardFooter`
- `ConceptStep`
- `StatsStep`
- `AssetsStep`
- `BondsStep`
- `StartingVowStep`
- `ReviewStep`
- `ValidationChecklist`

Each step owns local form interaction but receives backend validation results in a shared shape.

### Character State Components

- `StatSummary`
- `MeterControl`
- `MomentumControl`
- `DebilityGroup`
- `ProgressTrackControl`
- `VowCard`
- `BondCard`
- `AssetCard`
- `QuickUpdatePanel`

These components should accept DTO-shaped props and call typed mutation functions. They should not reach directly into `fetch`.

### Sheet Components

- `CharacterSheetPage`
- `SheetStats`
- `SheetMeters`
- `SheetAssets`
- `SheetVows`
- `SheetBonds`
- `SheetProgressTracks`
- `PrintActions`

Sheet components should be read-only and receive a projection DTO from the backend.

## State Management Approach

MVP can use React state plus a data-fetching layer. A large global state library is not required unless complexity grows.

Recommended layers:

- Server state:
  - Use a query/mutation abstraction around local API calls.
  - Cache character list, character detail, ruleset catalog, and sheet projection.
  - Invalidate/refetch after mutations.
- UI state:
  - Route selection, active tabs, open panels, focused wizard step.
  - Unsaved local form edits before save/validate.
- Draft wizard state:
  - Keep current form values in the wizard.
  - Save draft to backend frequently enough to prevent data loss.
  - Backend validation remains source of truth for finalization.

If adding a library, prefer one of:

- TanStack Query for server state.
- Zustand for small app UI state, if needed.

Avoid Redux unless the app grows into complex cross-screen editing with offline conflict handling.

## API Service Layer

Create a typed API layer, for example:

- `src/api/client.ts`
- `src/api/characters.ts`
- `src/api/rulesets.ts`
- `src/api/contracts.ts`

Responsibilities:

- Build URLs against the local backend base URL exposed by Electron/preload config.
- Parse JSON.
- Convert error envelopes into typed UI errors.
- Export DTO TypeScript types that mirror `docs/integration/contracts.md`.

Do not scatter raw `fetch` calls across components.

## Form And Wizard Architecture

The wizard should support:

- Draft save from every step.
- Backend validation on step continuation where useful.
- Final validation on Review.
- Jumping from Review errors back to the relevant step.
- Catalog-unavailable states for assets.

Recommended form strategy:

- Use controlled components for compact step forms.
- Keep step state in a single wizard reducer or form library.
- Use field paths from backend validation responses to attach errors.
- Allow incomplete drafts.
- Block final creation until backend validation passes.

Wizard steps:

1. Concept: name, concept, background notes, equipment notes.
2. Stats: assign `3,2,2,1,1` to Edge, Heart, Iron, Shadow, Wits.
3. Assets: choose exactly three starting assets for finalized standard character.
4. Bonds: capture `0..3` starting background bonds.
5. Starting Vow: capture at least one active vow, strongly prompt for both vow types.
6. Review: display checklist, errors, warnings, and create action.

TODO: Product must decide whether both starting vows are required for finalized standard characters.

## Validation Display

Backend validation response should be mapped to:

- Field-level errors under inputs.
- Section-level warnings in panels.
- Wizard-step status in the stepper.
- Review checklist items.
- Toast/banner only for save/network/system failures.

Validation display rules:

- Error text must be specific: what failed and how to fix it.
- Warnings should be visually distinct from blocking errors.
- Color cannot be the only signal.
- Draft save should not feel like failure when required final fields are missing.
- Blocked meter increases should appear next to the meter control that caused the issue.

Examples:

- Stats step: "Use exactly one 3, two 2s, and two 1s."
- Asset step: "Select exactly three starting assets."
- Health control: "Health cannot increase while Wounded is marked."
- Momentum: "Momentum cannot exceed derived max 9."

## Character Sheet View Architecture

Sheet View should render a backend-provided projection rather than reconstructing all domain rules in React.

Responsibilities:

- Display complete character state in a printable layout.
- Warn if the character is a draft or invalid.
- Include content/ruleset version and attribution placeholder.
- Wrap long text gracefully.
- Keep official layout/text use original unless reuse is approved.

MVP export recommendation:

- Start with a print-friendly HTML view and browser/Electron print-to-PDF.
- Defer custom PDF generation until layout and attribution requirements settle.

TODO: Product/legal must decide whether exports can include full asset/move text and what attribution is required.

## Error And Empty States

Required states:

- No characters: focused empty state with New Character action.
- Draft character: visible draft badge and missing-step summary.
- Invalid saved character: Needs Review status, load details, no silent corruption.
- Backend unavailable: clear local service error with retry.
- Ruleset catalog unavailable: block final creation, allow draft save.
- Asset definition missing: show saved id/name if available and mark catalog issue.
- Persistence unavailable: display local save warning and avoid claiming data is saved.

## MVP UI Scope

MVP includes:

- Desktop shell and navigation.
- Character Library.
- Create Character wizard.
- Draft save/finalize flow.
- Character Overview.
- Assets, Bonds, Vows & Progress, Conditions views.
- Manual state editing.
- Backend validation display.
- Sheet View and print-friendly layout.
- Settings placeholder for local data and attribution.

Deferred:

- Full move reference and move automation.
- Oracle tools.
- Campaign/world/truths setup.
- Cloud account/sync UI.
- Custom ruleset authoring.
- Rich import/export UI beyond MVP print/share path.
- Full asset/move official text until approved.
