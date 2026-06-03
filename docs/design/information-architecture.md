# Information Architecture

This structure is based on the MVP product docs for a desktop Ironsworn Character Creator & Manager. The app should help users create valid characters, reopen saved characters, and update character state quickly during play. It should not become a full rulebook, oracle engine, campaign manager, or automated move resolver in MVP.

The information architecture uses a simple desktop shell:

- Global navigation for major app areas.
- Character-scoped navigation once a character is open.
- Inline editing for common play updates.
- Wizard structure only for character creation.

# Top-Level Areas

## Dashboard

Purpose: Provide a calm starting point when the app opens.

User goals:

- Resume the most recently used character.
- Start a new character.
- See incomplete drafts that need attention.
- Confirm that saved data is available locally.

Entry points:

- App launch.
- Back navigation from a character.
- Empty state when no characters exist.

Rationale: New players need an obvious path into creation. Experienced players need one-click resume without digging through menus.

## Characters

Purpose: List saved characters and drafts.

User goals:

- Open an existing character.
- Identify incomplete character drafts.
- Create a new character.
- Delete or archive a character when supported.

Entry points:

- Global sidebar.
- Dashboard resume/list links.
- Post-creation completion.

Rationale: Local character persistence is MVP scope. The library is the safest primary home because the product is character-first, not campaign-first.

## Character Details

Purpose: Provide the in-play workspace for a selected character.

User goals:

- Read core character state at a glance.
- Update health, spirit, supply, and momentum quickly.
- Mark debilities and see derived momentum limits.
- Review assets, vows, bonds, and progress tracks.
- Add notes that explain state changes.

Entry points:

- Character Library item.
- Dashboard resume action.
- Completion of character creation.

Rationale: The manager must prioritize fast updates during play and immediate visibility of character state.

## Character Creation

Purpose: Guide users through a rules-valid starting character.

User goals:

- Create a character without missing required Ironsworn fields.
- Assign stats using the `3, 2, 2, 1, 1` array.
- Select three starting assets.
- Record background bonds.
- Create starting vows.
- Save an incomplete draft when needed.

Entry points:

- Dashboard primary action.
- Character Library primary action.
- Empty library state.

Rationale: New players need step-by-step support. Validation should prevent crisp rules errors while allowing drafts.

## Assets

Purpose: Show selected character assets and selected abilities.

User goals:

- Review asset names, types, selected abilities, and notes.
- See companion-specific fields when applicable.
- Understand upgrade slots without needing the full rulebook in MVP.

Entry points:

- Character navigation.
- Character Overview asset summary.
- Creation wizard asset step.

Rationale: Assets are card-like and central to character identity, but MVP licensing questions mean the UI should support summaries and references rather than assume full official card text.

## Progress

Purpose: Manage vows, bonds, and generic progress tracks.

User goals:

- Mark ticks and filled boxes.
- Read progress score at a glance.
- Track rank, status, and notes.
- Complete, forsake, or retire a track when appropriate.

Entry points:

- Character navigation.
- Vows & Progress screen.
- Quick update pattern from the Overview.

Rationale: Progress is a core Ironsworn mechanic. It needs a clear ten-box visual structure and low-friction update controls.

## Settings

Purpose: Hold app-level preferences and future content/storage configuration.

User goals:

- Review app version and ruleset/content version when available.
- Manage local data preferences when available.
- See source/license attribution requirements when available.

Entry points:

- Global sidebar.

Rationale: Settings should stay quiet in MVP. It should not distract from character creation or play.

# Navigation Hierarchy

```text
Application
|-- Dashboard
|-- Characters
|   |-- Character Library
|   `-- Create Character
|       |-- Character Concept
|       |-- Stats
|       |-- Assets
|       |-- Bonds
|       |-- Starting Vow
|       |-- Review
|       `-- Create Character
|-- Active Character
|   |-- Overview
|   |-- Assets
|   |-- Bonds
|   |-- Vows
|   |-- Progress
|   `-- Conditions
`-- Settings
```

## Hierarchy Rationale

- Dashboard and Characters are global because they are not tied to one active character.
- Create Character sits under Characters because it creates a character record and can save drafts.
- Active Character becomes its own character-scoped area once a character is open.
- Overview is first because play speed requires immediate access to stats, meters, momentum, debilities, and active vows.
- Assets, Bonds, Vows, Progress, and Conditions are separate enough to avoid a cluttered mega-screen, but the Overview should show summaries and quick actions for each.
- Settings is global and intentionally shallow for MVP.

# Content Grouping Rules

- Put high-frequency play state on the Overview: health, spirit, supply, momentum, debilities, active vows, and key progress.
- Put lower-frequency review/detail content behind character tabs: full asset list, full bond list, full vow/progress management.
- Keep creation guidance inside the wizard, not in the in-play manager.
- Avoid burying critical state behind modals.
- Use inline editing for meter and progress updates where possible.
- Use confirmation only for destructive, irreversible, or rules-sensitive changes.

# MVP vs Future Structure

## MVP

- Character Library
- Create Character Wizard
- Active Character Overview
- Assets
- Bonds
- Vows & Progress
- Conditions
- Settings

## Future-ready but not exposed as primary MVP areas

- Campaigns
- Worlds and truths
- Move reference
- Oracle tools
- Roll engine
- Cloud sync
- Supplement content packs

Future areas should fit into the hierarchy without forcing MVP users to think about campaigns, accounts, or content packs on day one.
