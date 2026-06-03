# Core Screens

These are MVP screen designs for the Ironsworn Character Creator & Manager. They describe structure, hierarchy, and UX intent only. HTML examples are semantic structure references; they are not production React, CSS, or JavaScript.

Shared screen principles:

- Desktop-first, readable, and calm.
- Show character state before secondary detail.
- Prefer inline editing for play updates.
- Avoid giant tables, nested modals, and full rulebook replacement.
- Use the design system: dark surfaces, restrained purple hierarchy, rare gold emphasis, compact readable controls.

# 1. Character Library

## Screen Purpose

Provide the saved-character home for opening existing characters, resuming drafts, and starting character creation.

## User Goals

- Open a saved character.
- Resume an incomplete draft.
- Create a new character.
- Understand when no local characters exist.

## Key Actions

- New Character
- Open Character
- Resume Draft
- Delete or archive character when supported

## Layout Description

The library uses the global app shell. The main content area starts with a compact header and primary action, followed by character cards. Cards show name, concept, completion state, last updated date, and a short state summary. Drafts should be visibly labeled but not visually alarming.

## Components Used

- Global sidebar
- Page header
- Primary button
- Character cards
- Draft badge
- Empty state panel
- Subtle search/filter controls when character count grows

## Empty States

If no characters exist, show one focused empty state with:

- A short welcome line.
- New Character primary action.
- A note that the app creates standard Ironsworn core characters.

## Error States

- Character failed to load: show the character card with an error badge and a retry/open file location action when available.
- Invalid saved character: label as Needs Review and prevent silent corruption.
- Persistence unavailable: show a top notice with local-save status.

## Notes

Do not lead with campaign or cloud account concepts in MVP.

## ASCII Wireframe

```text
+----------------------------------------------------------------+
| Sidebar          | Character Library              [New]        |
|------------------|---------------------------------------------|
| Dashboard        | Search or filter, only if needed            |
| Characters *     |                                             |
| Settings         | +----------------+ +----------------+       |
|                  | | Character Card | | Draft Card     |       |
| Active Character | | name, concept  | | incomplete     |       |
| none             | | meters summary | | missing steps  |       |
|                  | +----------------+ +----------------+       |
+----------------------------------------------------------------+
```

## Semantic HTML Example

```html
<main aria-labelledby="character-library-title">
  <header>
    <p>Characters</p>
    <h1 id="character-library-title">Character Library</h1>
    <button type="button">New Character</button>
  </header>

  <section aria-label="Saved characters">
    <article>
      <header>
        <h2>Kara Iron-Eyes</h2>
        <p>Complete character</p>
      </header>
      <p>Exiled scout sworn to recover a lost banner.</p>
      <dl>
        <dt>Health</dt><dd>5</dd>
        <dt>Spirit</dt><dd>4</dd>
        <dt>Supply</dt><dd>3</dd>
      </dl>
      <button type="button">Open Character</button>
    </article>
  </section>
</main>
```

# 2. Create Character Wizard

## Screen Purpose

Guide a new or experienced player through a valid standard Ironsworn starting character.

## User Goals

- Understand the current step.
- Complete required fields without needing to memorize the setup process.
- Save a draft.
- Review and finalize a valid character.

## Key Actions

- Continue
- Back
- Save Draft
- Review Character
- Create Character

## Layout Description

The wizard uses a two-column layout. The left side shows the step list and completion status. The main panel contains one focused step at a time. A compact guidance panel explains what the step is for without copying full rules text. The footer holds Back, Save Draft, and Continue/Create actions.

## Components Used

- Wizard stepper
- Form panels
- Validation messages
- Helper text
- Review summary
- Draft status badge

## Empty States

Not applicable for the wizard as a whole. Individual catalog-backed steps should handle missing asset data with a placeholder state.

## Error States

- Missing required field.
- Invalid stat distribution.
- Fewer or more than three starting assets.
- Missing vow title or rank.
- Asset catalog unavailable.

## Notes

The wizard should support draft save. Finalization should require standard MVP validation.

## ASCII Wireframe

```text
+----------------------------------------------------------------+
| Sidebar          | Create Character              Draft saved    |
|------------------|---------------------------------------------|
| Characters       | +-------------+ +-------------------------+ |
|                  | | Stepper     | | Step content            | |
|                  | | 1 Concept   | | heading                 | |
|                  | | 2 Stats     | | fields                  | |
|                  | | 3 Assets    | | guidance                | |
|                  | | 4 Bonds     | | validation              | |
|                  | | 5 Vows      | +-------------------------+ |
|                  | | 6 Review    | [Back] [Save Draft] [Next] |
+----------------------------------------------------------------+
```

## Semantic HTML Example

```html
<main aria-labelledby="create-character-title">
  <header>
    <p>Character Creation</p>
    <h1 id="create-character-title">Create Character</h1>
    <p>Draft saved locally</p>
  </header>

  <nav aria-label="Creation steps">
    <ol>
      <li aria-current="step">Concept</li>
      <li>Stats</li>
      <li>Assets</li>
      <li>Bonds</li>
      <li>Starting Vow</li>
      <li>Review</li>
    </ol>
  </nav>

  <form aria-label="Current creation step">
    <section>
      <h2>Character Concept</h2>
      <label>Name <input name="character-name" required /></label>
      <label>Concept <textarea name="concept"></textarea></label>
    </section>
    <footer>
      <button type="button">Back</button>
      <button type="button">Save Draft</button>
      <button type="submit">Continue</button>
    </footer>
  </form>
</main>
```

# 3. Character Overview

## Screen Purpose

Serve as the primary in-play dashboard for a single character.

## User Goals

- Read current state immediately.
- Adjust health, spirit, supply, and momentum quickly.
- See active debilities and derived momentum values.
- See active vows and key progress.
- Access deeper tabs without losing context.

## Key Actions

- Increment or decrement meters.
- Set momentum or burn/reset momentum when supported.
- Mark debility.
- Mark progress on active vow or track.
- Add quick note.

## Layout Description

The overview starts with a character header: name, concept, status, and save state. Below it, a state strip shows stats and core meters. The main content uses a two-column layout: the left column prioritizes active play controls, while the right column summarizes assets, bonds, vows, and recent notes.

## Components Used

- Character header
- Meter controls
- Momentum control
- Stat summary
- Debility badges
- Progress track preview
- Summary cards
- Quick note field

## Empty States

- No active vows: show Add Vow prompt unless character is a draft.
- No bonds: show Add Bond prompt.
- No notes: show quiet placeholder.

## Error States

- Meter update violates min/max range.
- Health increase blocked by Wounded.
- Spirit increase blocked by Shaken.
- Supply increase blocked by Unprepared.
- Momentum exceeds derived max.

## Notes

This is the screen most likely to be open during play. Keep clicks low and state visible.

## ASCII Wireframe

```text
+----------------------------------------------------------------+
| Sidebar          | Kara Iron-Eyes                 Saved         |
|------------------|---------------------------------------------|
| Overview *       | Stats: Edge Heart Iron Shadow Wits          |
| Assets           | +-----------------------------------------+ |
| Bonds            | | Health | Spirit | Supply | Momentum      | |
| Vows & Progress  | +-----------------------------------------+ |
| Conditions       | +----------------------+ +----------------+ |
|                  | | Active Vows          | | Assets Summary | |
|                  | | progress previews    | | Bonds Summary  | |
|                  | | quick mark controls  | | Notes          | |
|                  | +----------------------+ +----------------+ |
+----------------------------------------------------------------+
```

## Semantic HTML Example

```html
<main aria-labelledby="character-overview-title">
  <header>
    <p>Active Character</p>
    <h1 id="character-overview-title">Kara Iron-Eyes</h1>
    <p>Exiled scout sworn to recover a lost banner.</p>
  </header>

  <section aria-label="Core state">
    <article><h2>Health</h2><output>5</output><button type="button">Decrease</button><button type="button">Increase</button></article>
    <article><h2>Spirit</h2><output>4</output><button type="button">Decrease</button><button type="button">Increase</button></article>
    <article><h2>Supply</h2><output>3</output><button type="button">Decrease</button><button type="button">Increase</button></article>
    <article><h2>Momentum</h2><output>2</output><p>Max 10, reset 2</p></article>
  </section>

  <section aria-labelledby="active-vows-title">
    <h2 id="active-vows-title">Active Vows</h2>
    <article>
      <h3>Recover the lost banner</h3>
      <progress value="8" max="40">8 ticks</progress>
      <button type="button">Mark Progress</button>
    </article>
  </section>
</main>
```

# 4. Assets Screen

## Screen Purpose

Show selected assets, selected abilities, companion details, and upgrade readiness.

## User Goals

- Review what their character can do.
- See selected ability states.
- Track companion asset details if applicable.
- Understand future upgrades without adding automation.

## Key Actions

- View asset details.
- Add notes.
- Mark selected ability or upgrade when advancement is supported.
- Edit companion name or state when applicable.

## Layout Description

Assets display as card-like records because source assets are card-based. Each asset card includes type, name, summary/reference, selected ability state, and notes. Full official text should not be assumed until licensing is settled.

## Components Used

- Asset cards
- Type filters
- Ability checklist
- Companion sub-panel
- Source/reference label

## Empty States

- Draft has not selected assets yet: link back to creation wizard step.
- Catalog unavailable: show selected asset IDs/names if saved, plus a catalog status message.

## Error States

- Duplicate asset selected.
- Required companion name missing.
- Asset definition unavailable.

## Notes

Avoid rare-item styling. These are capability cards, not loot.

## ASCII Wireframe

```text
+----------------------------------------------------------------+
| Character Nav    | Assets                                      |
|------------------|---------------------------------------------|
| Overview         | [All] [Companion] [Path] [Combat] [Ritual]  |
| Assets *         | +----------------+ +----------------+       |
| Bonds            | | Asset Card     | | Asset Card     |       |
| Vows & Progress  | | type, summary  | | abilities      |       |
| Conditions       | | ability states | | notes          |       |
|                  | +----------------+ +----------------+       |
+----------------------------------------------------------------+
```

## Semantic HTML Example

```html
<main aria-labelledby="assets-title">
  <header>
    <h1 id="assets-title">Assets</h1>
    <p>Three starting assets define the character's capabilities.</p>
  </header>

  <section aria-label="Selected assets">
    <article>
      <header>
        <p>Path</p>
        <h2>Wayfinder</h2>
      </header>
      <p>Summary or source reference appears here.</p>
      <ul>
        <li><label><input type="checkbox" checked /> Starting ability selected</label></li>
        <li><label><input type="checkbox" /> Upgrade slot</label></li>
      </ul>
    </article>
  </section>
</main>
```

# 5. Bonds Screen

## Screen Purpose

Manage relationships with people, communities, and factions.

## User Goals

- View starting and acquired bonds.
- Add a bond.
- Edit bond notes.
- Track bonds progress.

## Key Actions

- Add Bond
- Edit Bond
- Mark Bonds Progress
- Archive or mark changed bond status when supported

## Layout Description

The screen splits the global bonds progress track from individual bond records. The track stays at the top because it is mechanically important. Bond records appear below as compact cards.

## Components Used

- Progress track
- Bond cards
- Add/edit form
- Status badges
- Notes fields

## Empty States

- No bonds: explain that starting characters may have up to three background bonds, and allow adding one.

## Error States

- Bond name missing.
- Bonds progress exceeds track capacity.
- Duplicate bond warning if same name appears.

## Notes

Do not over-structure bonds. Relationship notes are intentionally narrative.

## ASCII Wireframe

```text
+----------------------------------------------------------------+
| Character Nav    | Bonds                         [Add Bond]    |
|------------------|---------------------------------------------|
| Overview         | +-----------------------------------------+ |
| Assets           | | Bonds Progress Track                    | |
| Bonds *          | +-----------------------------------------+ |
| Vows & Progress  | +----------------+ +----------------+       |
| Conditions       | | Bond Card      | | Bond Card      |       |
|                  | | name, place    | | notes/status   |       |
|                  | +----------------+ +----------------+       |
+----------------------------------------------------------------+
```

## Semantic HTML Example

```html
<main aria-labelledby="bonds-title">
  <header>
    <h1 id="bonds-title">Bonds</h1>
    <button type="button">Add Bond</button>
  </header>

  <section aria-labelledby="bonds-progress-title">
    <h2 id="bonds-progress-title">Bonds Progress</h2>
    <progress value="12" max="40">12 ticks</progress>
  </section>

  <section aria-label="Bond records">
    <article>
      <h2>Ravencliff Village</h2>
      <p>Community that sheltered Kara during winter.</p>
      <button type="button">Edit</button>
    </article>
  </section>
</main>
```

# 6. Vows & Progress Screen

## Screen Purpose

Manage vows and other progress tracks used during play.

## User Goals

- View active vows.
- Mark progress quickly.
- Add new vows or generic tracks.
- Complete, forsake, or retire tracks.
- Read rank and status clearly.

## Key Actions

- Add Vow
- Add Progress Track
- Mark Tick
- Mark Progress
- Complete/Forsake Vow
- Edit Notes

## Layout Description

The screen leads with active vows because vows are core to Ironsworn. Secondary progress tracks appear below or in a right column. Each track shows rank, status, ten-box progress, and the next available action.

## Components Used

- Progress cards
- Ten-box progress visualization
- Rank badge
- Status badge
- Quick mark controls
- Add track action

## Empty States

- No active vows: prompt to add a vow. For finalized standard characters this should be an error-like warning because at least one active vow is expected.
- No generic tracks: quiet empty state; these are optional.

## Error States

- Vow title missing.
- Rank missing.
- Progress exceeds 40 ticks.
- Completing a vow without resolving progress should require confirmation when supported.

## Notes

Progress rolls are not automated in MVP. The UI should support manual status updates and notes.

## ASCII Wireframe

```text
+----------------------------------------------------------------+
| Character Nav    | Vows & Progress              [Add Vow]      |
|------------------|---------------------------------------------|
| Overview         | Active Vows                                 |
| Assets           | +-----------------------------------------+ |
| Bonds            | | Vow title, rank, status, progress       | |
| Vows & Progress* | | [tick] [progress] [complete] [forsake]  | |
| Conditions       | +-----------------------------------------+ |
|                  | Other Tracks                               |
|                  | +----------------+ +----------------+       |
+----------------------------------------------------------------+
```

## Semantic HTML Example

```html
<main aria-labelledby="vows-progress-title">
  <header>
    <h1 id="vows-progress-title">Vows &amp; Progress</h1>
    <button type="button">Add Vow</button>
  </header>

  <section aria-labelledby="active-vows-title">
    <h2 id="active-vows-title">Active Vows</h2>
    <article>
      <header>
        <h3>Recover the lost banner</h3>
        <p>Dangerous, active</p>
      </header>
      <progress value="8" max="40">8 ticks</progress>
      <button type="button">Mark Tick</button>
      <button type="button">Mark Progress</button>
    </article>
  </section>
</main>
```

# 7. Character Conditions Screen

## Screen Purpose

Manage health, spirit, supply, momentum, debilities, and derived constraints.

## User Goals

- See all condition meters together.
- Mark or clear debilities.
- Understand why momentum max/reset changed.
- Prevent illegal meter increases.

## Key Actions

- Adjust health, spirit, supply.
- Adjust momentum.
- Mark/clear debility.
- Add note for hardship or override.

## Layout Description

The top row shows health, spirit, supply, and momentum. Debilities are grouped by Conditions, Banes, and Burdens below. A derived momentum summary explains current max and reset from marked debilities.

## Components Used

- Meter controls
- Momentum control
- Debility groups
- Lock/warning messages
- Notes field

## Empty States

- No debilities marked: show all debility options unmarked with a quiet "none marked" summary.

## Error States

- Cannot increase locked meter.
- Clearing permanent/quest-locked debility requires explicit note.
- Momentum exceeds derived max after marking debility.

## Notes

This screen can be slightly more detailed than Overview, but common updates must still be fast.

## ASCII Wireframe

```text
+----------------------------------------------------------------+
| Character Nav    | Conditions                                  |
|------------------|---------------------------------------------|
| Overview         | Health | Spirit | Supply | Momentum         |
| Assets           | Derived: Max 9, Reset 1                     |
| Bonds            | +----------------+ +----------------+       |
| Vows & Progress  | | Conditions     | | Banes          |       |
| Conditions *     | | wounded etc.   | | maimed etc.    |       |
|                  | +----------------+ +----------------+       |
|                  | +----------------+                          |
|                  | | Burdens        |                          |
|                  | +----------------+                          |
+----------------------------------------------------------------+
```

## Semantic HTML Example

```html
<main aria-labelledby="conditions-title">
  <header>
    <h1 id="conditions-title">Conditions</h1>
    <p>Marked debilities reduce max momentum and may lock recovery.</p>
  </header>

  <section aria-label="Meters">
    <article><h2>Health</h2><output>5</output></article>
    <article><h2>Spirit</h2><output>4</output></article>
    <article><h2>Supply</h2><output>3</output></article>
    <article><h2>Momentum</h2><output>2</output><p>Max 9, reset 1</p></article>
  </section>

  <section aria-labelledby="debilities-title">
    <h2 id="debilities-title">Debilities</h2>
    <fieldset>
      <legend>Conditions</legend>
      <label><input type="checkbox" /> Wounded</label>
      <label><input type="checkbox" /> Shaken</label>
      <label><input type="checkbox" /> Unprepared</label>
      <label><input type="checkbox" /> Encumbered</label>
    </fieldset>
  </section>
</main>
```

# 8. Settings Screen

## Screen Purpose

Provide quiet access to app preferences, local data information, rules content status, and attribution.

## User Goals

- Confirm local save behavior.
- Review content/ruleset version when available.
- See licensing/attribution notes when available.
- Prepare for future import/export settings.

## Key Actions

- View local data status.
- View ruleset/content status.
- Open attribution/license information.
- Future: import/export data.

## Layout Description

Settings uses stacked panels. MVP should keep it short: App, Local Data, Rules Content, Attribution. Avoid complex nested settings.

## Components Used

- Panels
- Definition lists
- Secondary buttons
- Status badges

## Empty States

- Content version unknown: show "Not configured yet" rather than an error.

## Error States

- Local data path unavailable.
- Rules content missing.
- Attribution requirements incomplete.

## Notes

Settings should not become the place users must visit before creating a character.

## ASCII Wireframe

```text
+----------------------------------------------------------------+
| Sidebar          | Settings                                    |
|------------------|---------------------------------------------|
| Dashboard        | +-----------------------------------------+ |
| Characters       | | App                                     | |
| Settings *       | +-----------------------------------------+ |
|                  | +-----------------------------------------+ |
|                  | | Local Data                              | |
|                  | +-----------------------------------------+ |
|                  | +-----------------------------------------+ |
|                  | | Rules Content and Attribution           | |
|                  | +-----------------------------------------+ |
+----------------------------------------------------------------+
```

## Semantic HTML Example

```html
<main aria-labelledby="settings-title">
  <header>
    <h1 id="settings-title">Settings</h1>
  </header>

  <section aria-labelledby="local-data-title">
    <h2 id="local-data-title">Local Data</h2>
    <dl>
      <dt>Storage</dt>
      <dd>Local character persistence</dd>
      <dt>Status</dt>
      <dd>Available</dd>
    </dl>
  </section>

  <section aria-labelledby="attribution-title">
    <h2 id="attribution-title">Attribution</h2>
    <p>Source and license notes should appear here when finalized.</p>
  </section>
</main>
```

# UX Risks

- Overview could become cluttered if every domain entity gets equal visual weight.
- New-player guidance could become too verbose and slow down experienced users.
- Asset selection may feel weak if licensing prevents full asset text.
- Progress-track controls must be obvious enough that users do not miscount ticks.

# Product Questions

- What minimum asset catalog content is approved for MVP?
- Are both starting vows required for finalized standard characters?
- Should drafts appear in the same library grid as complete characters or a separate section?
- What attribution must be visible in-app before export exists?

# Recommended Next Design Tasks

- Define copy patterns for validation and guidance.
- Create visual examples for progress tracks and meter controls.
- Design printable sheet structure after licensing guidance.
- Design quick update surfaces for common play outcomes.

# Recommended Implementation Order For Lisa

1. Global app shell and sidebar.
2. Character Library with empty state.
3. Create Character wizard skeleton and stepper.
4. Character Overview with static fixture data.
5. Conditions controls and derived momentum display.
6. Vows & Progress track components.
7. Assets and Bonds detail screens.
8. Settings placeholder with attribution section.
