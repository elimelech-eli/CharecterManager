# Product Vision: Ironsworn Character Creator and Manager

## Source Status

Reviewed local source files in `D:\GIT\CharacterManager\docs`:

| Source | Used for |
| --- | --- |
| `Ironsworn-Rulebook.pdf` | Character creation, status tracks, momentum, bonds, vows, assets, debilities, moves, progress tracks. |
| `Ironsworn-Assets.pdf` | Asset categories, asset upgrade model, companion considerations, licensing notes. |
| `Ironsworn-Playkit.pdf` | Character sheet structure, move reference, progress worksheet structure. |
| `Ironsworn-Truths-Workbook.pdf` | Future world/campaign setup context. |
| `Ironsworn-Rulebook-Spreads.pdf` | Available but not separately analyzed; appears redundant with rulebook content. |

TODO: Confirm whether these PDFs are the intended canonical sources for MVP, and whether the app may store full move and asset text under the Ironsworn Creative Commons license or must store only summaries and references.

## App Purpose

CharacterManager is a desktop app for creating, viewing, saving, and updating Ironsworn characters during play. It should reduce paper-sheet friction without replacing the Ironsworn rulebook, asset cards, playkit, or oracle materials.

The app should help players:

- Build a rules-valid starting character.
- Track character state during solo, co-op, or guided play.
- Maintain vows, bonds, assets, debilities, and progress tracks over multiple sessions.
- View a readable character sheet that can be exported or printed.

## Target Users

| User | Needs |
| --- | --- |
| New solo player | Guided setup, validation, reminders about required starting values. |
| Experienced Ironsworn player | Fast editing, low-friction play tracking, flexible notes. |
| Co-op player | Shared supply awareness, character export, campaign context. |
| Guided group player or GM | Consistent character records and progress visibility. |
| Developer/designer using this project | Clear domain rules, flows, validation, and MVP boundaries. |

## Core Value Proposition

The product turns Ironsworn's lightweight but stateful character sheet into a durable desktop companion: easy to create, easy to update during play, and clear enough to support the fiction without burying the player in rules text.

## What "Character Creator" Means

For MVP, the character creator is a guided workflow that captures all required starting character data and prevents common setup mistakes.

It includes:

- Character name and optional concept/background notes.
- Five stats assigned using the starting array `3, 2, 2, 1, 1`.
- Starting health, spirit, supply, momentum, max momentum, and momentum reset values.
- Selection of three starting assets.
- Up to three background bonds.
- A background vow and an inciting-incident vow.
- Optional narrative equipment notes.

It does not need to teach the full game or include every source-book explanation.

## What "Character Manager" Means

For MVP, the character manager is the in-play workspace for an existing character.

It includes:

- Editing current health, spirit, supply, and momentum.
- Automatically displaying derived max momentum and reset based on debilities.
- Marking and clearing debilities.
- Managing vows, bonds, and other progress tracks.
- Viewing selected assets and asset upgrades.
- Viewing or exporting a character sheet.
- Capturing enough notes to explain why state changed.

## MVP Scope

| Area | MVP scope |
| --- | --- |
| Ruleset | Ironsworn core only. |
| Character creation | Guided, validated starting character wizard. |
| Assets | Select three starting assets; track acquired abilities. |
| State meters | Health, spirit, supply, momentum, max momentum, momentum reset. |
| Debilities | Conditions, banes, and burdens with derived momentum effects. |
| Vows | Create, rank, mark progress, complete/forsake status. |
| Bonds | Record background bonds and add progress ticks. |
| Progress tracks | Ten-box tracks supporting ranks and tick/progress increments. |
| Persistence | Save and reopen characters locally. |
| Export/view | Printable or shareable character sheet view. |

## Explicitly Out Of Scope For MVP

- Full rules replacement, full rulebook text, or extensive copied rules passages.
- Full oracle engine.
- Automated move resolution engine.
- Combat tracker with initiative automation.
- Campaign/world builder beyond optional campaign/world association.
- Multiplayer synchronization.
- Cloud account management.
- Starforged, Sundered Isles, Delve, or third-party supplements.
- Custom ruleset authoring.
- Character art generation.
- Inventory simulation beyond simple equipment notes and supply.
- Backend integrations that are not necessary for local character persistence.

## Product Principles

- Fiction first, mechanics supportive: state changes should leave room for narrative explanation.
- Validate where rules are crisp; allow notes where the rules depend on fiction.
- Keep the rulebook authoritative; the app should reference, summarize, and structure.
- Optimize for play speed after creation.
- Preserve extensibility for later rulesets without overbuilding MVP.
