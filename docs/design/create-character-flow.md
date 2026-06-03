# Create Character Flow

This flow designs the complete MVP character creation journey. It is based on the product requirements: standard Ironsworn core characters, guided setup, validation for crisp rules, draft saving, three starting assets, starting bonds, and at least one active vow.

The wizard is not a full Ironsworn tutorial. It should help a new player complete the sheet without missing required setup.

# User Journey

1. User chooses New Character from Dashboard or Character Library.
2. User enters concept and name.
3. User assigns the starting stat array `3, 2, 2, 1, 1`.
4. User reviews default meters: health 5, spirit 5, supply 5, momentum 2, max 10, reset 2.
5. User selects three starting assets.
6. User adds zero to three background bonds.
7. User creates starting vow information.
8. User reviews completeness and validation.
9. User creates the character or saves a draft.

Journey principles:

- One major decision per step.
- Always show where the user is in the wizard.
- Let users save drafts without meeting final validation.
- Final creation requires standard MVP validation.
- Guidance should be practical, brief, and field-adjacent.

# Wizard Steps

```text
1. Character Concept
2. Stats
3. Assets
4. Bonds
5. Starting Vow
6. Review
7. Create Character
```

# Step 1: Character Concept

## Purpose

Capture the identity of the character before mechanical choices.

## Fields

- Character name: required for final creation.
- Concept: optional short description.
- Background notes: optional.
- Equipment notes: optional narrative notes, not an inventory simulator.

## Validation

- Name is required before final creation.
- Name should not be only whitespace.
- Long notes should be allowed but visually contained.

## User Guidance

- Ask for a plain, usable character idea.
- Encourage a simple phrase for concept.
- Make it clear that equipment is narrative unless tied to assets or supply.

## Common Mistakes

- User writes a full backstory before naming the character.
- User treats equipment as a structured inventory.
- User leaves name blank and expects final creation to work.

## Error Handling

- Show inline error under name.
- Allow Save Draft even if name is missing, but label draft clearly.

## Navigation Rules

- Continue is allowed with missing optional fields.
- If name is missing, Continue may still proceed for drafting, but Review must flag it before final creation.

## Wireframe

```text
+---------------------------------------------------------------+
| Create Character                               Draft saved     |
|---------------------------------------------------------------|
| Steps         | Character Concept                             |
| 1 Concept *   | Name [________________]                       |
| 2 Stats       | Concept [____________________________]        |
| 3 Assets      | Background notes [____________________]       |
| 4 Bonds       | Equipment notes [_____________________]       |
| 5 Vow         |                                               |
| 6 Review      | [Back] [Save Draft] [Continue]               |
+---------------------------------------------------------------+
```

## Semantic HTML Example

```html
<form aria-labelledby="concept-step-title">
  <h2 id="concept-step-title">Character Concept</h2>
  <label>Name <input name="name" required /></label>
  <label>Concept <input name="concept" /></label>
  <label>Background Notes <textarea name="background-notes"></textarea></label>
  <label>Equipment Notes <textarea name="equipment-notes"></textarea></label>
  <button type="button">Save Draft</button>
  <button type="submit">Continue</button>
</form>
```

# Step 2: Stats

## Purpose

Help the user assign Edge, Heart, Iron, Shadow, and Wits using the exact standard starting array.

## Fields

- Edge value.
- Heart value.
- Iron value.
- Shadow value.
- Wits value.

## Validation

- All five stats are required for final creation.
- Values must exactly match `3, 2, 2, 1, 1`.
- No standard stat value may be lower than 1 or higher than 3.

## User Guidance

- Show the available value chips separately from the stat list.
- Explain each stat with a short product summary.
- Show remaining values so the user does not count manually.

## Common Mistakes

- Assigning two 3s.
- Forgetting the second 2 or second 1.
- Confusing Iron with generic health.

## Error Handling

- Show a missing/duplicate value message near the stat controls.
- Highlight unassigned stats.
- Do not wait until final Review to explain stat distribution errors.

## Navigation Rules

- User can go Back without losing assignments.
- Continue should be blocked until all stats are assigned in standard mode.
- Save Draft is always available.

## Wireframe

```text
+---------------------------------------------------------------+
| Steps         | Stats                                         |
| 1 Concept     | Available values: [3] [2] [2] [1] [1]         |
| 2 Stats *     | Edge    [ ]  speed, ranged, agility           |
| 3 Assets      | Heart   [ ]  courage, loyalty, empathy        |
| 4 Bonds       | Iron    [ ]  strength, endurance              |
| 5 Vow         | Shadow  [ ]  stealth, deception               |
| 6 Review      | Wits    [ ]  knowledge, observation           |
|               | [Back] [Save Draft] [Continue]               |
+---------------------------------------------------------------+
```

## Semantic HTML Example

```html
<form aria-labelledby="stats-step-title">
  <h2 id="stats-step-title">Stats</h2>
  <p>Assign the starting values 3, 2, 2, 1, and 1.</p>
  <fieldset>
    <legend>Assign stat values</legend>
    <label>Edge <select name="edge"><option></option><option>3</option><option>2</option><option>1</option></select></label>
    <label>Heart <select name="heart"><option></option><option>3</option><option>2</option><option>1</option></select></label>
    <label>Iron <select name="iron"><option></option><option>3</option><option>2</option><option>1</option></select></label>
    <label>Shadow <select name="shadow"><option></option><option>3</option><option>2</option><option>1</option></select></label>
    <label>Wits <select name="wits"><option></option><option>3</option><option>2</option><option>1</option></select></label>
  </fieldset>
  <button type="submit">Continue</button>
</form>
```

# Step 3: Assets

## Purpose

Let the user choose three starting assets and capture required asset-specific fields.

## Fields

- Asset selection: exactly three for standard final creation.
- Asset type filter: companion, path, combat talent, ritual.
- Selected ability state.
- Companion name where required.
- Asset notes where useful.

## Validation

- Exactly three assets required for finalized standard characters.
- Duplicate assets are not allowed by default.
- Required companion names must be present.
- Asset prerequisites should warn and allow explicit note where rules are narrative.

## User Guidance

- Use short summaries and source references until full asset text is approved.
- Explain that assets define capabilities and future upgrades.
- Show selected count: `2 of 3 selected`.

## Common Mistakes

- Selecting fewer than three.
- Selecting the same asset twice.
- Missing companion name.
- Expecting the app to include full official card text before licensing approval.

## Error Handling

- Show selected count error.
- Mark incomplete selected asset cards.
- If catalog is missing, allow draft save and show catalog-unavailable state.

## Navigation Rules

- Continue is blocked until standard asset requirements are met.
- Save Draft remains available.
- User can return to this step from Review.

## Wireframe

```text
+---------------------------------------------------------------+
| Steps         | Assets                          2 of 3 selected |
| 1 Concept     | [All] [Companion] [Path] [Combat] [Ritual]     |
| 2 Stats       | +--------------+ +--------------+              |
| 3 Assets *    | | Asset card   | | Asset card   |              |
| 4 Bonds       | | summary      | | selected     |              |
| 5 Vow         | | [Select]     | | ability      |              |
| 6 Review      | +--------------+ +--------------+              |
|               | [Back] [Save Draft] [Continue]                |
+---------------------------------------------------------------+
```

## Semantic HTML Example

```html
<form aria-labelledby="assets-step-title">
  <h2 id="assets-step-title">Assets</h2>
  <p>Choose three starting assets.</p>
  <section aria-label="Asset choices">
    <article>
      <h3>Wayfinder</h3>
      <p>Path asset summary or source reference.</p>
      <label><input type="checkbox" name="asset" value="wayfinder" /> Select asset</label>
    </article>
  </section>
  <p aria-live="polite">2 of 3 selected</p>
  <button type="submit">Continue</button>
</form>
```

# Step 4: Bonds

## Purpose

Capture starting background bonds and introduce the relationship/progress concept gently.

## Fields

- Bond name: required per bond.
- Type or relationship: optional.
- Location/community: optional.
- Notes: optional.

## Validation

- Starting characters may have 0 to 3 background bonds.
- Each entered bond requires a non-empty name.
- Duplicate names should warn, not hard block.

## User Guidance

- Explain that bonds are meaningful relationships with people or communities.
- Make zero bonds a valid choice.
- Show how many starting bond slots are used.

## Common Mistakes

- User thinks three bonds are mandatory.
- User writes a long NPC profile when a name and note are enough.
- User enters duplicate community names accidentally.

## Error Handling

- Empty bond name blocks continuing if that bond row exists.
- Duplicate bond warning asks user to confirm or rename.

## Navigation Rules

- Continue is allowed with zero bonds.
- Maximum three starting bonds in standard creation.
- Additional bonds can be added after creation through the Bonds screen.

## Wireframe

```text
+---------------------------------------------------------------+
| Steps         | Bonds                            1 of 3 used   |
| 1 Concept     | +-------------------------------------------+ |
| 2 Stats       | | Bond name [________________]              | |
| 3 Assets      | | Relationship [______________]             | |
| 4 Bonds *     | | Notes [____________________]              | |
| 5 Vow         | +-------------------------------------------+ |
| 6 Review      | [Add Bond]                                  |
|               | [Back] [Save Draft] [Continue]              |
+---------------------------------------------------------------+
```

## Semantic HTML Example

```html
<form aria-labelledby="bonds-step-title">
  <h2 id="bonds-step-title">Bonds</h2>
  <p>Add up to three background bonds, or continue with none.</p>
  <fieldset>
    <legend>Background bond</legend>
    <label>Name <input name="bond-name" /></label>
    <label>Relationship <input name="bond-relationship" /></label>
    <label>Notes <textarea name="bond-notes"></textarea></label>
  </fieldset>
  <button type="button">Add Bond</button>
  <button type="submit">Continue</button>
</form>
```

# Step 5: Starting Vow

## Purpose

Establish the character's quest structure: long-term background purpose and immediate playable situation.

## Fields

- Background vow title.
- Background vow rank.
- Background vow notes.
- Inciting-incident vow title.
- Inciting-incident vow rank.
- Inciting-incident vow notes.

## Validation

- At least one active vow is required for finalized MVP character creation.
- Vow title is required for each entered vow.
- Rank is required for each entered vow.
- Rank must be one of: troublesome, dangerous, formidable, extreme, epic.
- Progress starts empty.

## User Guidance

- Strongly prompt for both a background vow and inciting-incident vow.
- Use brief descriptions: background vow is long-term purpose; inciting vow is what starts play.
- Do not prescribe exact vow text.

## Common Mistakes

- User writes a vague title with no action.
- User forgets rank.
- User assumes progress starts partly filled.

## Error Handling

- Missing title or rank appears inline.
- If only one vow exists, Review shows warning if product chooses to strongly prefer both.
- Final creation follows product decision: at least one active vow required, both recommended unless mandated.

## Navigation Rules

- Continue requires at least one valid vow in standard mode.
- Save Draft remains available with incomplete vow data.

## Wireframe

```text
+---------------------------------------------------------------+
| Steps         | Starting Vow                                  |
| 1 Concept     | Background Vow                               |
| 2 Stats       | Title [________________] Rank [________]     |
| 3 Assets      | Notes [______________________________]       |
| 4 Bonds       | Inciting Vow                                 |
| 5 Vow *       | Title [________________] Rank [________]     |
| 6 Review      | Notes [______________________________]       |
|               | [Back] [Save Draft] [Continue]              |
+---------------------------------------------------------------+
```

## Semantic HTML Example

```html
<form aria-labelledby="vow-step-title">
  <h2 id="vow-step-title">Starting Vow</h2>
  <fieldset>
    <legend>Background vow</legend>
    <label>Title <input name="background-vow-title" /></label>
    <label>Rank <select name="background-vow-rank"><option>Extreme</option><option>Epic</option></select></label>
    <label>Notes <textarea name="background-vow-notes"></textarea></label>
  </fieldset>
  <fieldset>
    <legend>Inciting-incident vow</legend>
    <label>Title <input name="inciting-vow-title" /></label>
    <label>Rank <select name="inciting-vow-rank"><option>Troublesome</option><option>Dangerous</option><option>Formidable</option></select></label>
  </fieldset>
  <button type="submit">Continue</button>
</form>
```

# Step 6: Review

## Purpose

Let the user confirm character completeness before final creation.

## Fields

Review-only summaries:

- Concept and name.
- Stats.
- Starting meters and momentum defaults.
- Assets.
- Bonds.
- Vows.
- Validation checklist.

## Validation

- Name required.
- Stats distribution valid.
- Starting meters initialized.
- Exactly three assets selected.
- Asset-required fields complete.
- At least one active vow valid.
- Debilities unmarked for standard creation.

## User Guidance

- Show a checklist of complete and incomplete sections.
- Link each problem back to the relevant step.
- Avoid scary language unless data would be invalid.

## Common Mistakes

- User assumes draft is a playable character.
- User misses asset companion name.
- User forgets vow rank.

## Error Handling

- Disable final Create Character while required errors remain.
- Allow Save Draft.
- Present all validation issues together, grouped by step.

## Navigation Rules

- User can jump back to any step from the checklist.
- Create Character becomes available only when validation passes.

## Wireframe

```text
+---------------------------------------------------------------+
| Steps         | Review                                        |
| 1 Concept     | +----------------+ +----------------+         |
| 2 Stats       | | Character      | | Validation     |         |
| 3 Assets      | | summary        | | checklist      |         |
| 4 Bonds       | +----------------+ +----------------+         |
| 5 Vow         | Assets, Bonds, Vows summaries                 |
| 6 Review *    | [Back] [Save Draft] [Create Character]        |
+---------------------------------------------------------------+
```

## Semantic HTML Example

```html
<main aria-labelledby="review-step-title">
  <h2 id="review-step-title">Review Character</h2>
  <section aria-labelledby="validation-title">
    <h3 id="validation-title">Validation</h3>
    <ul>
      <li>Stats use 3, 2, 2, 1, 1</li>
      <li>Three assets selected</li>
      <li>At least one active vow exists</li>
    </ul>
  </section>
  <button type="button">Back</button>
  <button type="button">Save Draft</button>
  <button type="button">Create Character</button>
</main>
```

# Step 7: Create Character

## Purpose

Confirm successful creation and route the user into character management.

## Fields

No new fields.

## Validation

- Final validation must run before creation.
- If validation fails, keep user on Review with grouped errors.

## User Guidance

- Confirm the character has been created.
- Offer one primary next action: Open Character.
- Secondary action: return to Character Library.

## Common Mistakes

- User expects creation to export or print automatically.
- User expects move automation immediately.

## Error Handling

- Save failed: show retry and keep draft data.
- Character created but failed to open: show success plus library link.

## Navigation Rules

- Success routes to Character Overview.
- Draft remains only if save fails or user explicitly saved without finalizing.

## Wireframe

```text
+---------------------------------------------------------------+
| Create Character                                              |
|---------------------------------------------------------------|
| Character created                                             |
| Kara Iron-Eyes is ready for play.                             |
|                                                               |
| [Open Character] [Back to Library]                            |
+---------------------------------------------------------------+
```

## Semantic HTML Example

```html
<main aria-labelledby="created-title">
  <h1 id="created-title">Character Created</h1>
  <p>Kara Iron-Eyes is ready for play.</p>
  <a href="/characters/kara/overview">Open Character</a>
  <a href="/characters">Back to Library</a>
</main>
```

# Complete Flow Diagram

```text
New Character
  |
  v
Concept -> Stats -> Assets -> Bonds -> Starting Vow -> Review
   |         |        |         |          |              |
   +---------+--------+---------+----------+--------------+
             Save Draft available from every step
                              |
                              v
                      Create Character
                              |
                              v
                    Character Overview
```

# UX Risks

- Asset catalog uncertainty may weaken the Assets step.
- New players may need enough vow guidance to write useful vows without copied rule text.
- Strict validation could frustrate variant play if custom setup is not clearly deferred.

# Product Questions

- Should both starting vows be mandatory for finalized standard characters?
- Should custom/variant character creation be hidden entirely in MVP or exposed as a warning/override?
- What asset summaries and references are approved for display?

# Recommended Next Design Tasks

- Write validation microcopy for each wizard step.
- Create progress-track visual rules for Review and Vows.
- Define the draft-state badge and incomplete-character library treatment.

# Recommended Implementation Order For Lisa

1. Wizard shell with stepper, Back, Save Draft, Continue.
2. Concept step.
3. Stats assignment with distribution validation.
4. Assets step with selected count and placeholder catalog state.
5. Bonds step.
6. Starting Vow step.
7. Review checklist and final creation handoff.
