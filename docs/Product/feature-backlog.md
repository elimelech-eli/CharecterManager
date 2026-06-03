# Feature Backlog

Each item is written so product, UX, frontend, backend, and architecture can refine it into implementation issues.

## MVP

| Feature | Description | User story | Acceptance criteria | Dependencies | Design notes | Architecture/backend notes |
| --- | --- | --- | --- | --- | --- | --- |
| Ironsworn ruleset shell | Define core Ironsworn character fields and validation. | As a player, I want the app to know the Ironsworn sheet structure so I can create a valid character. | Ruleset includes stats, meters, momentum, debilities, vows, bonds, progress tracks, and assets placeholders; validation errors are clear. | Rules summary and domain model. | Keep rule hints brief; do not make it a rules textbook. | Store ruleset metadata separately from character state. |
| Static character sheet view | Display a saved character in a readable sheet layout. | As a player, I want to view my character at a glance during play. | Sheet shows name, stats, health, spirit, supply, momentum, debilities, assets, vows, bonds, and progress. | Domain model. | Prioritize scannability and quick state reading. | Read-only projection from character aggregate. |
| Character creation wizard | Guided setup for a new Ironsworn character. | As a new player, I want step-by-step setup so I do not miss required fields. | User can complete name, stats, meters, assets, bonds, and vows; invalid stats cannot finalize; drafts can save. | Ruleset metadata, asset catalog strategy. | Use progress indicator and review step. | Wizard can write draft and finalized character states. |
| Stat assignment validation | Enforce starting stat array. | As a player, I want confidence my stats are legal. | Values must exactly match `3,2,2,1,1`; errors identify duplicate/missing values. | Ruleset metadata. | Drag/drop or chip assignment recommended. | Validation should be reusable for import. |
| Asset selection tracking | Select and display three starting assets. | As a player, I want my assets recorded with selected abilities. | Three assets selected; companion names captured; selected abilities visible. | Asset catalog content decision. | Use card-like display because source material is card-based. | Asset definitions should support abilities, prerequisites, and companion sub-state. |
| Vow and bond setup | Capture starting vows and background bonds. | As a player, I want my relationships and quests on the sheet from session one. | Supports up to three starting bonds; at least one active vow; rank required for vows. | Progress track model. | Separate background vow from inciting vow visually. | Vows and bonds both use progress concepts but have different fields. |
| Local character persistence | Save and reopen characters locally. | As a player, I want my character to persist between sessions. | Create, update, list, open, and delete local characters; app survives restart. | Storage decision. | Surface save status. | Use a storage abstraction so MongoDB/cloud can be added later. |
| In-play state manager | Edit meters, momentum, debilities, and progress. | As a player, I want fast updates after moves. | User can adjust health, spirit, supply, momentum, debilities, vows, bonds, and tracks; derived momentum recalculates. | Domain model and validation. | Favor compact controls and undo/change log. | State changes should be transactional enough to avoid partial updates. |
| Printable/exportable sheet | Generate a user-facing character sheet. | As a player, I want to print or share my character. | Export includes core fields and selected options; handles overflow gracefully. | Static sheet view. | Match printable sheet expectations without copying protected layout wholesale unless licensed. | First export can be HTML/print-to-PDF. |

## Post-MVP

| Feature | Description | User story | Acceptance criteria | Dependencies | Design notes | Architecture/backend notes |
| --- | --- | --- | --- | --- | --- | --- |
| Move-aware quick updates | Let users pick a move and see likely affected state. | As a player, I want help applying move outcomes without full automation. | Move list shows affected fields; user manually confirms changes. | Move metadata. | Keep it fast and optional. | Move definitions need state-impact metadata. |
| Full asset catalog | Import all core assets as structured definitions. | As a player, I want all official core assets available. | Catalog supports filter/search/type; prerequisites and upgrades represented. | Licensing decision. | Card preview works well. | Consider data seeding and versioned rules content. |
| Companion management | Track companion names, health, harm, and removal. | As a player with companions, I want their state on my sheet. | Companion assets expose health and notes; Companion Endure Harm can be logged. | Asset model. | Companion section should remain compact. | Companion state may be embedded under selected asset. |
| Experience and advancement | Track XP and spend it on assets/upgrades. | As a player, I want character growth supported. | XP earned/spent; add asset cost and upgrade cost represented; selected abilities update. | Asset catalog. | Show available upgrades clearly. | Need audit trail for earned/spent XP. |
| Character import/export JSON | Portable backup and sharing. | As a player, I want to move my character between machines. | Export/import roundtrip preserves all domain fields and validates version. | Persistence model. | Provide clear conflict/error messages. | Include schema version and ruleset version. |
| Campaign/world association | Group characters, shared supply, and shared tracks. | As a co-op player, I want party-level context. | Character can belong to campaign; campaign can hold shared supply/tracks/notes. | Storage model. | Campaign switcher or context header. | Do not force campaign for solo users. |

## Nice-To-Have

| Feature | Description | User story | Acceptance criteria | Dependencies | Design notes | Architecture/backend notes |
| --- | --- | --- | --- | --- | --- | --- |
| Guided vow prompts | Help users write vows from source inspiration. | As a new player, I want prompts for meaningful vows. | User can browse prompt categories and insert/edit generated structure. | Copyright/content decision. | Prompts should inspire, not prescribe. | Store prompt source references separately. |
| Change history | Log state updates with notes. | As a player, I want to understand why my sheet changed. | Each update can record reason, move, timestamp; history can be viewed. | State manager. | Timeline view should be secondary. | Event log can support undo later. |
| Undo last state change | Recover from accidental edits. | As a player, I want to quickly undo a mistake. | Last change can be reverted; grouped changes revert together. | Change history. | Keep command visible near quick updates. | Requires change events or snapshots. |
| Character sheet themes | Offer print/display variations. | As a player, I want a sheet style that fits my table. | At least compact and print-friendly themes. | Sheet view. | Avoid overdecorating operational play UI. | Themes should not change data model. |
| Source reference links | Link fields to rulebook/page references. | As a player, I want to know where a rule came from. | Key fields show source labels/page references where allowed. | Source mapping. | Use unobtrusive info affordances. | Store references as metadata, not prose blobs. |

## Future / Advanced

| Feature | Description | User story | Acceptance criteria | Dependencies | Design notes | Architecture/backend notes |
| --- | --- | --- | --- | --- | --- | --- |
| Roll and move engine | Resolve action/progress rolls and apply optional state changes. | As a player, I want the app to help run moves. | Supports action rolls, progress rolls, momentum burn, negative momentum cancellation, and manual confirmation. | Move metadata, roller. | Roll results must leave room for fiction. | Requires deterministic test coverage and rules-versioning. |
| Oracle support | Provide oracle rolls and custom tables. | As a solo player, I want prompts without leaving the app. | Core oracle tables available where licensed; custom tables supported. | Licensing/content decision. | Keep oracle separate from character sheet. | Likely a rules-content module. |
| Supplement support | Add Delve, Starforged, or other compatible rulesets. | As a fan of related games, I want this app to grow with the system. | Multiple rulesets can coexist without corrupting character data. | Extensible ruleset architecture. | Clear ruleset selector. | Requires schema versioning and per-ruleset modules. |
| Cloud sync | Sync characters across devices. | As a player, I want my character available elsewhere. | Login/sync/conflict handling defined. | Storage abstraction. | Avoid account-first experience. | Needs auth, remote persistence, conflict resolution. |
| Co-op session mode | Shared campaign dashboard for multiple characters. | As a group, we want shared supply, vows, and progress visible. | Campaign-level tracks and shared supply update consistently. | Campaign model, sync if remote. | Dashboard should support table use. | Requires multi-character aggregate and possibly realtime sync. |
