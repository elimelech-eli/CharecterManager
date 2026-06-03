# Ironsworn Rules Summary For Product Development

This document summarizes character-management requirements from the local Ironsworn PDFs. It is not a rules replacement and should not be presented to users as a complete rules reference.

## Source Status And TODOs

Reviewed:

- `D:\GIT\CharacterManager\docs\Ironsworn-Rulebook.pdf`
- `D:\GIT\CharacterManager\docs\Ironsworn-Assets.pdf`
- `D:\GIT\CharacterManager\docs\Ironsworn-Playkit.pdf`
- `D:\GIT\CharacterManager\docs\Ironsworn-Truths-Workbook.pdf`

TODO: Confirm exact page references and permissible in-app wording before shipping. The PDFs include Creative Commons licensing notes, but product/legal still needs to decide whether to store complete move and asset text or only IDs, names, summaries, and source references.

## Character Creation Overview

Starting character setup requires the user to:

1. Envision the character concept.
2. Choose a name.
3. Assign stat values across Edge, Heart, Iron, Shadow, and Wits using the starting array.
4. Set starting health, spirit, supply, and momentum values.
5. Record up to three background bonds.
6. Choose three assets.
7. Note important equipment or story items.
8. Create a background vow and an inciting-incident vow.

Implementation note: Steps can be flexible in order, but the app should validate completeness before finalizing a playable character.

## Stats

| Stat | Product meaning | MVP validation |
| --- | --- | --- |
| Edge | Agility, speed, ranged precision. | Must receive one value from the starting array. |
| Heart | Courage, loyalty, empathy, sociability. | Must receive one value from the starting array. |
| Iron | Strength, endurance, close fighting. | Must receive one value from the starting array. |
| Shadow | Stealth, deception, cunning. | Must receive one value from the starting array. |
| Wits | Knowledge, expertise, observation. | Must receive one value from the starting array. |

Starting array: `3, 2, 2, 1, 1`.

Rules dependency: Most action moves add one relevant stat or ask the player to choose a stat based on fictional approach.

## Assets

Assets represent character capabilities, background, companions, combat talents, and rituals.

| Asset type | Product notes |
| --- | --- |
| Companion | May require a companion name and health track. Companion harm can remove or endanger the asset. |
| Path | Represents training, identity, or background. Often modifies moves or state gains. |
| Combat talent | Often depends on fictional equipment, such as using a certain weapon or shield. |
| Ritual | Often behaves like a special move and may have backlash or cost. |

Creation requirement: choose three starting assets.

Asset model requirements:

- Asset has name, type, source, abilities, selected ability state, requirements, and notes.
- Most assets include three abilities.
- Upgrades can be bought later with experience.
- Some assets require narrative or mechanical prerequisites.

TODO: Decide whether MVP ships with full asset catalog data from `Ironsworn-Assets.pdf`, a minimal curated subset, or metadata-only placeholders until licensing is finalized.

## Bonds

Bonds represent meaningful relationships with people or communities.

Product requirements:

- Starting character may record up to three background bonds.
- Bonds contribute to a bonds progress track.
- Bonds may affect some moves, especially community or relationship moves.
- Bonds need a label/name and optional description.

Validation:

- Starting bonds: `0` to `3`.
- Each bond should have a non-empty name.
- Bonds progress should not exceed a ten-box progress-track structure.

## Vows

Vows are core quest records.

Required starting vows:

- Background vow: long-term character-defining goal, typically high rank.
- Inciting-incident vow: immediate playable situation.

Vow fields:

- Title or oath summary.
- Rank: troublesome, dangerous, formidable, extreme, or epic.
- Progress track.
- Status: active, fulfilled, forsaken.
- Notes and source context.

Rules dependency:

- Vows use progress tracks.
- Milestones mark progress based on rank.
- Fulfillment is resolved with a progress roll, not a normal action roll.
- Fulfilled vows award experience based on rank and outcome.

## Momentum

Momentum is a numeric track used to represent advantage or setbacks.

| Field | Default | Notes |
| --- | --- | --- |
| Current momentum | `+2` | Can range from `-6` to max momentum. |
| Max momentum | `+10` | Reduced by marked debilities. |
| Momentum reset | `+2` | Reduced by marked debilities. |

Rules dependencies:

- Positive momentum may be burned under rules-specific circumstances.
- Negative momentum can cancel an action die when it matches the die value.
- Momentum is ignored on progress rolls.
- If current momentum would drop below `-6`, rules route the player to a setback workflow.

Product requirement: the app should calculate max momentum and reset from debilities, while allowing the user to record current momentum changes.

## Health

Health is a condition meter from `0` to `+5` representing physical condition.

Dependencies:

- Reduced primarily through harm.
- Increased through recovery/care moves.
- If health is `0`, further harm can spill into momentum and may trigger debilities or death-related moves.
- If wounded is marked, health cannot increase until wounded is cleared.

## Spirit

Spirit is a condition meter from `0` to `+5` representing mental/emotional condition.

Dependencies:

- Reduced through stress.
- Increased through rest, fellowship, and some move/asset outcomes.
- If spirit is `0`, further stress can trigger debilities or desolation-related moves.
- If shaken is marked, spirit cannot increase until shaken is cleared.

## Supply

Supply is a condition meter from `0` to `+5` representing shared preparedness, provisions, and general adventure resources.

Dependencies:

- Supply is shared among allies while travelling together.
- Reduced by travel outcomes, costs, hardship, and some moves.
- Increased through resupply or community support.
- If supply is `0`, Out of Supply applies and unprepared may be marked.
- If unprepared is marked, supply cannot increase until unprepared is cleared.

Product implication: MVP can store supply on the character but should expose a future campaign/shared-supply question.

## Debilities

Debilities are marked hardship states. All marked debilities affect momentum.

| Category | Debilities | Product rules |
| --- | --- | --- |
| Conditions | Wounded, Shaken, Unprepared, Encumbered | Usually temporary; some prevent increasing related meters. |
| Banes | Maimed, Corrupted | Permanent narrative/mechanical burdens. |
| Burdens | Cursed, Tormented | Quest-linked burdens. |

Derived momentum:

- Each marked debility reduces max momentum by `1`.
- One marked debility changes reset to `+1`.
- More than one marked debility changes reset to `0`.

Special restrictions:

- Wounded blocks health increases.
- Shaken blocks spirit increases.
- Unprepared blocks supply increases.

## Progress Tracks

Progress tracks measure vows, journeys, fights, and bonds.

Product requirements:

- Tracks have ten boxes.
- Each box can hold four ticks.
- A fully filled box contributes to progress score.
- Track rank affects how much progress is marked per milestone or harm.
- Progress rolls ignore momentum.

Ranks:

- Troublesome
- Dangerous
- Formidable
- Extreme
- Epic

Track types needed for character management:

- Vow progress tracks.
- Bonds track.
- Optional journey or combat tracks for active play notes.

## Moves That Affect Character State

The app does not need full move automation for MVP, but it must understand which moves commonly change stored character state.

| Move or move family | State touched | Product implication |
| --- | --- | --- |
| Face Danger | Momentum, health, spirit, supply | Offer quick manual adjustments after outcomes. |
| Secure an Advantage | Momentum, next move bonus notes | Track momentum changes; optional note for temporary add. |
| Heal | Health, wounded condition | Support clearing wounded and increasing health. |
| Resupply | Supply, unprepared condition indirectly | Support supply increase and notes. |
| Make Camp | Health, spirit, supply, companion health, momentum | Support multi-meter recovery. |
| Undertake a Journey | Supply, journey progress | Support supply decrement and journey track progress. |
| Reach Your Destination | Journey track resolution | Optional track completion status. |
| Sojourn | Health, spirit, supply, momentum, wounded, shaken, unprepared | Key recovery workflow for communities. |
| Forge a Bond | Bond list, bonds progress, momentum/spirit via outcomes/assets | Add bond and mark bond progress. |
| Test Your Bond | Bond status | Future support for bond strain/removal notes. |
| Enter the Fray | Combat progress track, momentum, initiative note | Optional combat-state support. |
| Strike / Clash / Battle | Combat progress, harm notes, momentum | Optional combat-state support. |
| Endure Harm | Health, momentum, wounded, maimed, death state | Key suffer workflow. |
| Companion Endure Harm | Companion health, companion asset status | Needed if companion assets are included. |
| Endure Stress | Spirit, momentum, shaken, corrupted, desolation state | Key suffer workflow. |
| Face Death | Cursed burden, death status, vow creation | Advanced consequence workflow. |
| Face Desolation | Tormented burden, vow creation | Advanced consequence workflow. |
| Out of Supply | Unprepared, health/spirit/momentum spillover | Key supply edge case. |
| Face a Setback | Health, spirit, supply, momentum, progress undermining | Needed for lower-bound resource handling. |
| Swear an Iron Vow | Vow creation, momentum | Required for vow management. |
| Reach a Milestone | Vow progress | Required for progress marking. |
| Fulfill Your Vow | Vow status, experience | Required for quest completion. |
| Forsake Your Vow | Vow status, spirit/momentum/debility consequences | Required for vow status changes. |
| Advance | Experience, assets, asset abilities | Required for post-creation progression. |
| Pay the Price | Any state by narrative cost | Needs freeform adjustment/logging. |

## Rule Dependencies Relevant To Character Management

- Character creation is valid only when stat distribution, starting tracks, starting assets, and starting vows are complete.
- Debilities create derived momentum values and condition-specific meter locks.
- Progress tracks are shared for some group challenges; MVP should not assume every progress track belongs to one character.
- Supply can be shared by a party; MVP storage should allow future promotion from character-level field to campaign-level field.
- Assets can modify move inputs, outputs, and available state changes; MVP should model assets as structured capabilities even if it does not automate them.
- Equipment is mostly narrative unless tied to an asset or move consequence.
- Experience is earned through vow fulfillment and spent on assets.
- Some consequences create new vows or burdens, so the manager must support adding new vows during play.
