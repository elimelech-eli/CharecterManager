# Navigation Model

The navigation model is desktop-first and character-first. It must help new players find the guided creation path while giving experienced players fast access to the active character during play.

# Application Navigation

## Global Sidebar

Primary destinations:

- Dashboard
- Characters
- Settings

Optional future destinations:

- Campaigns
- Worlds
- Content
- Import/Export

Rules:

- Keep MVP global navigation short.
- Do not expose unavailable future modules as disabled clutter.
- Show the active character resume action near the top when a character is open.
- Use text labels with simple line icons for recognition.
- Use the design system sidebar treatment: dark secondary background, subtle dividers, active purple fill, gold left accent.

## Global Header

Purpose:

- Show current area or active character.
- Provide high-value actions relevant to the current area.

Rules:

- Character Library: primary action is New Character.
- Active Character: primary quick actions are state update, export/view sheet when available, and save status.
- Creation Wizard: show draft status and exit/save draft affordance.

# Sidebar Navigation

The sidebar should support quick orientation without becoming a fantasy control panel.

Recommended structure:

```text
+-----------------------------+
| Ironsworn                   |
| Last save: local            |
|-----------------------------|
| Dashboard                   |
| Characters                  |
| Settings                    |
|-----------------------------|
| Active Character            |
| Kara Iron-Eyes              |
| Overview                    |
| Assets                      |
| Bonds                       |
| Vows & Progress             |
| Conditions                  |
+-----------------------------+
```

Rules:

- Show character-scoped navigation only when a character is active.
- Keep global and character navigation visually separated.
- Use active-state styling on exactly one current destination.
- Keep labels plain: `Vows & Progress`, not clever fantasy labels.
- Do not use nested flyouts for MVP.

# Character Navigation

Character navigation appears after a character is opened.

Tabs:

- Overview
- Assets
- Bonds
- Vows & Progress
- Conditions

Rules:

- Overview is the default landing page for any active character.
- Character tabs should remain visible while inside character details.
- Do not require users to return to the library to move between character sections.
- The active character name should stay visible in the shell or page header.

## Character Switcher

MVP:

- Link back to Character Library from the active character header or sidebar.
- Show active character name and draft/complete state.

Future:

- Add a compact character switcher if users manage multiple active characters frequently.
- Support campaign grouping later without forcing a campaign selection in MVP.

# Breadcrumb Rules

Breadcrumbs should be sparse.

Use breadcrumbs for:

- Deep wizard steps.
- Future campaign/world nesting.
- Import/export or settings subpages if they grow.

Avoid breadcrumbs for:

- Top-level app navigation.
- Simple character tabs.
- Modal-like quick update surfaces.

Examples:

```text
Characters / Create Character / Assets
Characters / Kara Iron-Eyes / Vows & Progress
Settings / Rules Content
```

Rules:

- Breadcrumbs should never replace the sidebar.
- Breadcrumbs should show location, not every interaction.
- Character names may appear in breadcrumbs when the user is inside character-scoped pages.

# Deep Linking Strategy

Deep links should support reopening exact app states without creating complex MVP navigation.

Recommended route concepts:

```text
/dashboard
/characters
/characters/new
/characters/new/concept
/characters/new/stats
/characters/new/assets
/characters/new/bonds
/characters/new/vows
/characters/new/review
/characters/:characterId/overview
/characters/:characterId/assets
/characters/:characterId/bonds
/characters/:characterId/vows-progress
/characters/:characterId/conditions
/settings
```

Rules:

- Deep links to a missing character should land on a clear not-found state with a return to Character Library.
- Deep links to incomplete wizard steps should load draft data if available.
- Deep links should not bypass final validation.
- Future campaign links should wrap character links without breaking existing character routes.

# Future Expansion Strategy

The MVP should leave room for future modules without showing them prematurely.

## Multiple Characters

Navigation impact:

- Character Library remains the source of truth for all characters.
- Active character navigation is scoped to one selected character.
- A future switcher can appear in the active character header.

UX rule: Do not turn the sidebar into a giant character list in MVP.

## Future Campaigns

Navigation impact:

```text
Application
|-- Campaigns
|   |-- Campaign Overview
|   |-- Shared Supply
|   |-- Shared Tracks
|   `-- Characters
`-- Characters
```

UX rule: Campaign should be optional. Solo users should never have to create a campaign to manage one character.

## Future Worlds

Navigation impact:

- Worlds can belong to Campaigns or stand alone as reference context.
- Truths setup should be a future guided flow, not an MVP sidebar destination.

UX rule: Keep worldbuilding separate from character state so play updates stay fast.

## Future Ironsworn Supplements

Navigation impact:

- Add content/ruleset selection in Settings or a future Content area.
- Keep character routes stable.
- Show ruleset/content version on character records where needed.

UX rule: Supplements should expand available content and validation, not remake the whole navigation system.

# Navigation Anti-Patterns

- Giant tables as primary navigation.
- Nested modal chains.
- Multiple sidebars fighting for attention.
- Campaign-first navigation for solo MVP.
- Hidden quick updates inside deep menus.
- Fantasy naming that obscures simple destinations.

# Recommended Implementation Order For Lisa

1. App shell with global sidebar and active-character area.
2. Character Library route and empty state.
3. Create Character wizard navigation skeleton.
4. Active Character Overview route.
5. Character tabs for Assets, Bonds, Vows & Progress, and Conditions.
6. Settings placeholder with attribution/content-version space.

# Product Questions

- Should a finalized MVP character require both a background vow and an inciting-incident vow, or only one active vow?
- Should Character Library expose draft deletion in MVP?
- Should campaign association appear as an optional field during creation, or wait until campaigns exist?

# UX Risks

- Too much rules guidance could make the app feel like a rulebook replacement.
- Too little guidance could fail new players during creation.
- Hiding state updates behind menus would slow play.
- Premature campaign navigation could confuse solo users.

# Recommended Next Design Tasks

- Design empty/error state language for character persistence.
- Define quick update patterns for common move outcomes without automating moves.
- Define printable sheet layout after licensing/official-sheet mimicry questions are settled.
