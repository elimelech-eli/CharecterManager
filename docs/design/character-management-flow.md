# Character Management Flow

This document designs the in-play character management experience. It focuses on manual updates to character state during play: health, spirit, supply, momentum, debilities, vows, progress tracks, and bonds.

MVP principle: the app supports fast, validated state changes. It does not automate full move resolution.

# Experience Priorities

1. Fast updates during play.
2. Minimal clicks.
3. Low cognitive load.
4. Immediate visibility of character state.
5. Clear prevention of rules-invalid changes.

# Shared Interaction Pattern

Most state updates should follow this lightweight pattern:

```text
See current state
  |
  v
Choose quick adjustment
  |
  v
App validates and previews derived effects
  |
  v
User confirms or inline change saves
  |
  v
State updates and optional note is recorded
```

Use inline controls for simple changes. Use a compact confirmation panel only when a change has derived effects, warnings, or destructive consequences.

# Health

## Most Common User Actions

- Decrease health after harm.
- Increase health after recovery.
- Add a note explaining harm or healing.
- See whether Wounded blocks recovery.

## Primary Workflow

```text
Open Character Overview
  |
  v
Use Health decrement/increment control
  |
  v
If increase and Wounded is marked, show blocked recovery warning
  |
  v
Save valid value and keep focus near Health
```

## Quick Actions

- `-1`
- `+1`
- Set exact value
- Add note

## Error Prevention

- Health cannot go below 0 or above 5.
- If Wounded is marked, block health increase and explain why.
- If health is 0 and user decreases again, show consequence guidance rather than silently applying an impossible value.

## Undo Strategy

- Support undo of the last health change.
- Undo should restore previous health value and remove the associated note if the note was created as part of that change.

## Mobile Constraints (future)

- Health control must remain thumb-friendly.
- Avoid tiny plus/minus targets.
- Keep blocked-state message directly below the meter.

## HTML Example

```html
<section aria-labelledby="health-title">
  <h2 id="health-title">Health</h2>
  <output aria-label="Current health">4</output>
  <button type="button">Decrease Health</button>
  <button type="button">Increase Health</button>
  <label>Change note <input name="health-note" /></label>
</section>
```

# Spirit

## Most Common User Actions

- Decrease spirit after stress.
- Increase spirit after recovery or fellowship.
- See whether Shaken blocks recovery.

## Primary Workflow

```text
Open Overview or Conditions
  |
  v
Adjust Spirit
  |
  v
Validate against 0 to 5 and Shaken lock
  |
  v
Save change
```

## Quick Actions

- `-1`
- `+1`
- Set exact value
- Add stress/recovery note

## Error Prevention

- Spirit cannot go below 0 or above 5.
- If Shaken is marked, block spirit increase.
- Use status text in addition to color for blocked recovery.

## Undo Strategy

- Last spirit change can be undone.
- If Shaken was marked in the same grouped update, undo should restore both values together when supported.

## Mobile Constraints (future)

- Keep health, spirit, and supply in a vertical stack on narrow screens.
- Avoid horizontal meter groups that require sideways scrolling.

## HTML Example

```html
<section aria-labelledby="spirit-title">
  <h2 id="spirit-title">Spirit</h2>
  <output aria-label="Current spirit">3</output>
  <p>Recovery is blocked while Shaken is marked.</p>
  <button type="button">Decrease Spirit</button>
  <button type="button">Increase Spirit</button>
</section>
```

# Supply

## Most Common User Actions

- Decrease supply during journeys or hardship.
- Increase supply after resupply.
- See whether Unprepared blocks recovery.

## Primary Workflow

```text
Open Overview
  |
  v
Adjust Supply
  |
  v
If Unprepared is marked, block increase
  |
  v
Save character-level supply for MVP
```

## Quick Actions

- `-1`
- `+1`
- Set exact value
- Add note

## Error Prevention

- Supply cannot go below 0 or above 5.
- If Unprepared is marked, block supply increase.
- When supply reaches 0, show a quiet warning that further consequences may apply by rules.

## Undo Strategy

- Undo last supply change.
- Preserve optional note in history until the user confirms removal if history exists.

## Mobile Constraints (future)

- Future campaign/shared supply must clearly indicate whether supply is personal or shared.
- On mobile, place shared scope label directly beside the value.

## HTML Example

```html
<section aria-labelledby="supply-title">
  <h2 id="supply-title">Supply</h2>
  <output aria-label="Current supply">2</output>
  <p>Character supply for MVP. Shared campaign supply can be added later.</p>
  <button type="button">Decrease Supply</button>
  <button type="button">Increase Supply</button>
</section>
```

# Momentum

## Most Common User Actions

- Increase or decrease current momentum.
- Set exact momentum.
- Burn/reset momentum when supported.
- See derived max and reset.

## Primary Workflow

```text
Open Overview or Conditions
  |
  v
Adjust Momentum
  |
  v
Validate current value against derived max and minimum
  |
  v
If debilities changed, recalculate max/reset immediately
  |
  v
Save momentum value
```

## Quick Actions

- `-1`
- `+1`
- Set exact value
- Reset to derived reset
- Add note

## Error Prevention

- Current momentum cannot exceed derived max.
- Current momentum cannot go below -6 without setback guidance.
- Max is reduced by marked debilities.
- Reset changes based on marked debility count.

## Undo Strategy

- Undo last momentum change.
- If the change came from marking a debility, grouped undo should restore both the debility state and derived momentum values.

## Mobile Constraints (future)

- Momentum needs more horizontal space because it has current, max, reset, and minimum.
- Use stacked labels rather than cramming values into one line.

## HTML Example

```html
<section aria-labelledby="momentum-title">
  <h2 id="momentum-title">Momentum</h2>
  <output aria-label="Current momentum">2</output>
  <dl>
    <dt>Max</dt><dd>10</dd>
    <dt>Reset</dt><dd>2</dd>
  </dl>
  <button type="button">Decrease Momentum</button>
  <button type="button">Increase Momentum</button>
  <button type="button">Reset Momentum</button>
</section>
```

# Debilities

## Most Common User Actions

- Mark Wounded, Shaken, Unprepared, or Encumbered.
- Mark a bane or burden after a major consequence.
- Clear temporary conditions.
- See how marked debilities affect momentum.

## Primary Workflow

```text
Open Conditions
  |
  v
Toggle Debility
  |
  v
Preview derived momentum max/reset and meter locks
  |
  v
Require note for permanent or quest-linked debilities
  |
  v
Save debility state
```

## Quick Actions

- Mark debility.
- Clear debility.
- Add hardship note.
- Link burden to vow when supported.

## Error Prevention

- Clearing permanent banes should require explicit confirmation and note.
- Cursed or Tormented should prompt for a linked vow when product rules support it.
- Marking Wounded, Shaken, or Unprepared should immediately explain recovery locks.

## Undo Strategy

- Undo last debility toggle.
- Restore derived momentum and any affected lock messages.

## Mobile Constraints (future)

- Use grouped lists, not a dense grid.
- Keep category labels visible: Conditions, Banes, Burdens.

## HTML Example

```html
<section aria-labelledby="debilities-title">
  <h2 id="debilities-title">Debilities</h2>
  <fieldset>
    <legend>Conditions</legend>
    <label><input type="checkbox" name="wounded" /> Wounded</label>
    <label><input type="checkbox" name="shaken" /> Shaken</label>
    <label><input type="checkbox" name="unprepared" /> Unprepared</label>
    <label><input type="checkbox" name="encumbered" /> Encumbered</label>
  </fieldset>
</section>
```

# Vows

## Most Common User Actions

- Add a vow during play.
- Mark progress.
- Edit notes.
- Fulfill or forsake a vow.

## Primary Workflow

```text
Open Vows & Progress
  |
  v
Select vow or Add Vow
  |
  v
Update title, rank, notes, progress, or status
  |
  v
Validate required title/rank and progress range
  |
  v
Save
```

## Quick Actions

- Mark tick.
- Mark progress.
- Add note.
- Change status.

## Error Prevention

- Vow title required.
- Rank required.
- Progress cannot exceed 40 ticks.
- Fulfill/Forsake should not be hidden behind tiny controls.

## Undo Strategy

- Undo last progress mark.
- Status changes should be reversible with a clear "Restore active" option where supported.

## Mobile Constraints (future)

- Vow cards should stack vertically.
- Progress controls must remain large enough to avoid mis-taps.

## HTML Example

```html
<article aria-labelledby="vow-title">
  <h2 id="vow-title">Recover the lost banner</h2>
  <p>Dangerous, active</p>
  <progress value="8" max="40">8 ticks</progress>
  <button type="button">Mark Tick</button>
  <button type="button">Mark Progress</button>
  <button type="button">Fulfill or Forsake</button>
</article>
```

# Progress Tracks

## Most Common User Actions

- Mark one tick.
- Mark progress based on rank.
- Adjust progress manually.
- Complete or retire a track.

## Primary Workflow

```text
Open Progress Track
  |
  v
Choose quick mark action
  |
  v
Update ten-box visual and progress score
  |
  v
Save track state
```

## Quick Actions

- Add tick.
- Add progress.
- Remove tick.
- Set exact ticks.
- Add note.

## Error Prevention

- Ticks must stay between 0 and 40.
- Display full boxes separately from partial ticks.
- Require a note for manual decreases if progress undermining is being recorded.

## Undo Strategy

- Undo last tick/progress change.
- Group multiple ticks from one action as one undoable change when possible.

## Mobile Constraints (future)

- Ten boxes can wrap into two rows of five.
- Do not rely on hover for tick details.

## HTML Example

```html
<section aria-labelledby="progress-title">
  <h2 id="progress-title">Journey to Ravencliff</h2>
  <p>Formidable journey</p>
  <progress value="14" max="40">14 ticks</progress>
  <button type="button">Add Tick</button>
  <button type="button">Add Progress</button>
</section>
```

# Bonds

## Most Common User Actions

- Add a bond.
- Edit bond notes.
- Mark bonds progress.
- Review relationship context.

## Primary Workflow

```text
Open Bonds
  |
  v
Add or edit bond
  |
  v
Validate bond name
  |
  v
Optionally mark bonds progress
  |
  v
Save
```

## Quick Actions

- Add Bond.
- Edit note.
- Mark bonds tick/progress.
- Archive or change status when supported.

## Error Prevention

- Bond name required.
- Duplicate names produce a warning.
- Bonds progress cannot exceed 40 ticks.

## Undo Strategy

- Undo last bond progress change.
- Newly added bond can be removed immediately with undo.

## Mobile Constraints (future)

- Separate bonds progress from bond cards.
- Keep Add Bond near the top.

## HTML Example

```html
<section aria-labelledby="bonds-management-title">
  <h2 id="bonds-management-title">Bonds</h2>
  <progress value="12" max="40">12 ticks</progress>
  <article>
    <h3>Ravencliff Village</h3>
    <p>Community bond.</p>
    <button type="button">Edit Bond</button>
  </article>
</section>
```

# Quick Update Pattern

The product docs recommend Quick Update as a repeated play tool, but full move automation is out of scope. The MVP quick update should be a compact state-change panel, not a wizard.

## Suggested Structure

```text
+------------------------------------------------------+
| Quick Update                                         |
|------------------------------------------------------|
| Affected area: Health / Spirit / Supply / Momentum   |
| Change: -1 / +1 / set exact                          |
| Optional note: ______________________________         |
| Derived warnings or locks appear here                |
| [Cancel] [Apply Update]                              |
+------------------------------------------------------+
```

## Semantic HTML Example

```html
<section aria-labelledby="quick-update-title">
  <h2 id="quick-update-title">Quick Update</h2>
  <label>Affected Area
    <select name="affected-area">
      <option>Health</option>
      <option>Spirit</option>
      <option>Supply</option>
      <option>Momentum</option>
    </select>
  </label>
  <label>Note <input name="update-note" /></label>
  <button type="button">Apply Update</button>
</section>
```

# Undo Strategy

MVP undo should focus on the most recent state change.

Rules:

- Show Undo Last Change after a successful update.
- Group related changes from one action, such as marking a debility and recalculating momentum.
- Do not silently undo unrelated edits.
- If local persistence fails, keep the attempted change visible as unsaved and explain the state.

Future change history can expand this into a timeline, but MVP should keep it simple.

# Error Prevention Summary

- Validate ranges before saving.
- Explain blocked recovery at the control that caused it.
- Show derived momentum effects when debilities change.
- Require notes for overrides, permanent conditions, and progress decreases.
- Use labels/icons as well as color for warnings and danger.

# UX Risks

- If every update opens a modal, play will feel slow.
- If every update is instant with no feedback, users may miss derived effects.
- Ten-box progress can be visually confusing without strong tick/full-box distinction.
- Undo needs clear grouping or users may distrust it.

# Product Questions

- Is Undo Last Change an MVP requirement or a strong design recommendation?
- Should Quick Update include move names as optional labels, or stay purely state-area based until move metadata exists?
- Should generic journey/fight tracks be included in MVP, or only vows and bonds?
- How strict should clearing permanent banes be in the first release?

# Recommended Next Design Tasks

- Design progress-track visual states: empty, partial ticks, full boxes, complete, forsaken.
- Define quick update microcopy and validation messages.
- Design low-friction state note capture.
- Design save/unsaved/undo feedback patterns.

# Recommended Implementation Order For Lisa

1. Static Overview state panels.
2. Meter controls for health, spirit, and supply.
3. Momentum control with derived max/reset display.
4. Debility toggles and blocked recovery messages.
5. Vow/progress track cards with manual marking.
6. Bonds progress and bond records.
7. Quick Update panel.
8. Undo Last Change feedback.
