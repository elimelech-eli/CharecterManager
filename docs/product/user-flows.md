# User Flows

These flows describe product behavior for an Ironsworn character creator and manager. They assume MVP does not automate full move resolution, but does validate character sheet state.

## Flow 1: Creating A New Character

| Field | Details |
| --- | --- |
| User goal | Start a valid Ironsworn character that can be saved and used in play. |
| Steps | Open New Character; select Ironsworn ruleset; enter name; optionally add concept/background; assign stats; confirm starting meters; choose assets; add bonds; create vows; review sheet; save. |
| Required data | Name, ruleset id, five stats, health, spirit, supply, momentum, max momentum, momentum reset, three assets, at least one vow. |
| Validation rules | Name required; stats must use `3, 2, 2, 1, 1`; health/spirit/supply start at `+5`; momentum starts at `+2`; max momentum starts at `+10`; reset starts at `+2`; debilities unmarked at creation. |
| Edge cases | User wants to save draft before complete; user uses a custom starting setup; user has fewer than three assets for a deliberate variant; source catalog missing full assets. |

MVP recommendation: allow draft save, but clearly label incomplete characters.

## Flow 2: Choosing Stats

| Field | Details |
| --- | --- |
| User goal | Assign starting stat values according to Ironsworn rules. |
| Steps | Show five stats; show available value chips `3, 2, 2, 1, 1`; user assigns one value to each stat; app previews summary; user confirms. |
| Required data | Edge, Heart, Iron, Shadow, Wits numeric values. |
| Validation rules | Each stat required; values must exactly match the starting array; no stat below `1` or above `3` for standard character creation. |
| Edge cases | User changes earlier assignment; user wants custom/variant stats; user imports a character with non-standard stats. |

Design note: Make the distribution visually obvious so users do not need to mentally count duplicates.

## Flow 3: Choosing Assets

| Field | Details |
| --- | --- |
| User goal | Select the three assets that define the character's starting capabilities. |
| Steps | Browse/filter asset catalog by type; inspect summarized card; select asset; enter required companion name if needed; choose starting ability when asset allows choice; repeat until three selected; confirm. |
| Required data | Asset ids, asset type, selected abilities, companion name/health when applicable, prerequisite notes if applicable. |
| Validation rules | Standard creation requires exactly three assets; duplicate assets not allowed unless rules source explicitly permits; required asset fields must be complete. |
| Edge cases | Asset catalog unavailable; asset has prerequisite; companion asset needs a name; legal decision prevents storing full asset text. |

TODO: Confirm full asset catalog ingestion strategy from `Ironsworn-Assets.pdf`.

## Flow 4: Setting Bonds

| Field | Details |
| --- | --- |
| User goal | Record initial relationships and bond progress. |
| Steps | Open Bonds step; add person/community name; optional relationship notes; repeat up to three; app marks corresponding bond progress ticks; confirm. |
| Required data | Bond name; optional type, description, location/community. |
| Validation rules | Starting character allows `0` to `3` background bonds; each bond name must be non-empty; bond progress cannot exceed track capacity. |
| Edge cases | User starts with no bonds; user wants more than three for a variant; same community/person appears twice; bond later changes or is tested. |

## Flow 5: Creating Vows

| Field | Details |
| --- | --- |
| User goal | Establish the character's long-term purpose and immediate inciting quest. |
| Steps | Add background vow; select rank; add notes; add inciting-incident vow; select rank; confirm active progress tracks. |
| Required data | Vow title/summary, rank, status, progress value, notes. |
| Validation rules | Vow title required; rank must be one of the five standard ranks; progress starts empty; status starts active. |
| Edge cases | User wants to start without an inciting incident; user creates multiple starting vows; user imports a partially progressed vow; vow is shared with allies. |

MVP recommendation: Require at least one active vow and strongly prompt for both starting vow types.

## Flow 6: Managing Health, Spirit, And Supply

| Field | Details |
| --- | --- |
| User goal | Update physical, mental, and resource state during play. |
| Steps | Open play manager; choose meter; increment/decrement or type value; optionally select reason/move; app applies caps and condition locks; save change log entry. |
| Required data | Meter type, new value or delta, optional reason, timestamp. |
| Validation rules | Values range `0` to `+5`; wounded blocks health increase; shaken blocks spirit increase; unprepared blocks supply increase; supply may be shared in future campaign mode. |
| Edge cases | Damage below zero spills into momentum or other consequence; user intentionally overrides condition lock; shared supply differs across party members. |

## Flow 7: Changing Momentum

| Field | Details |
| --- | --- |
| User goal | Track advantage and setbacks accurately. |
| Steps | Open momentum control; apply delta or set value; app enforces max/min; if below minimum, prompt for setback handling; if burning momentum, reset to calculated reset value. |
| Required data | Current momentum, max momentum, reset value, marked debilities. |
| Validation rules | Current momentum cannot exceed calculated max; minimum is `-6`; max is `10 - markedDebilityCount`; reset is `+2`, `+1`, or `0` based on debilities. |
| Edge cases | User imports illegal momentum; user burns momentum; debility change lowers max below current momentum; negative momentum die cancellation is roll-specific and should be handled by roll UI later. |

## Flow 8: Adding Debilities

| Field | Details |
| --- | --- |
| User goal | Record major hardships and automatically update momentum limits. |
| Steps | Open debilities panel; mark debility; add narrative note if relevant; app recalculates max/reset; app applies meter restrictions for wounded/shaken/unprepared; save. |
| Required data | Debility id, category, marked state, optional note, source move/context. |
| Validation rules | Debility can be marked/unmarked unless permanent or quest-locked; every marked debility reduces max momentum; reset changes at one and two-plus marked debilities. |
| Edge cases | Maimed/corrupted should not casually clear; cursed/tormented requires linked vow; encumbered is fiction-driven; asset may count as a debility. |

## Flow 9: Managing Progress Tracks

| Field | Details |
| --- | --- |
| User goal | Track progress toward vows, journeys, fights, and bonds. |
| Steps | Create or open track; select type and rank; mark ticks/progress; view filled boxes and progress score; optionally resolve/complete track. |
| Required data | Track name, type, rank when applicable, ticks/progress, status, shared/character scope. |
| Validation rules | Ten boxes; four ticks per box; total ticks max `40`; rank must be standard when required; progress score counts only full boxes. |
| Edge cases | Shared track with allies; progress undermined by a move; user resolves before track is full; bond track uses ticks differently from vow/journey/fight context. |

## Flow 10: Updating Character State During Play

| Field | Details |
| --- | --- |
| User goal | Quickly adjust sheet state after a move outcome or narrative event. |
| Steps | Open character; select Quick Update; choose affected area or move; adjust meters, debilities, progress, assets, bonds, or vows; add note; confirm. |
| Required data | Character id, changed fields, optional source move, optional note. |
| Validation rules | All changed fields must remain within domain constraints; derived values recalculate immediately; user overrides must be marked as overrides. |
| Edge cases | Multiple state changes from one move; Pay the Price can affect any state; companion suffers harm; move creates new vow or burden; app has no move automation yet. |

Design note: Quick Update should be optimized for repeated use and should not trap users in a wizard.

## Flow 11: Exporting Or Viewing A Character Sheet

| Field | Details |
| --- | --- |
| User goal | See, print, or share a readable character sheet. |
| Steps | Open character; choose Sheet View; review layout; choose export format; app generates print/share file; user saves or opens result. |
| Required data | Full character record, selected assets, progress tracks, notes to include/exclude. |
| Validation rules | Export should not expose incomplete required fields without warning; long text should wrap; source/license attribution should appear where required. |
| Edge cases | Character has more vows/assets than fit one page; full asset text cannot be exported for licensing reasons; user wants player-safe summary without notes. |

MVP export recommendation: start with printable HTML or PDF-style view before adding richer file formats.
