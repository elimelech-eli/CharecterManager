# Product Domain Model

This is a product-level model, not a database schema. It describes the objects the app should understand for an Ironsworn character creator and manager.

## Entity Overview

| Entity | Purpose | Key relationships |
| --- | --- | --- |
| Character | Root player character record. | Owns stats, selected assets, vows, bonds, meters, momentum, debilities, progress tracks. |
| Stat | Core numeric character aptitude. | Belongs to character and ruleset. |
| Asset | Structured character capability. | Selected by character; defined by ruleset; may contain abilities and companion state. |
| Bond | Meaningful relationship with a person/community. | Belongs to character; contributes to bonds progress. |
| Vow | Quest promise and progress record. | Belongs to character or campaign; uses progress track; may create XP. |
| ProgressTrack | Ten-box progress structure. | Used by vows, bonds, journeys, fights, and campaign challenges. |
| Momentum | Current advantage/setback state. | Derived max/reset depends on debilities. |
| ConditionMeter | Health, spirit, or supply meter. | Belongs to character; supply may later move to campaign. |
| Debility | Marked hardship state. | Belongs to character; affects momentum and sometimes meter increases. |
| Move | Rules action that may change state. | Defined by ruleset; may reference stats, assets, meters, tracks. |
| Campaign / World | Optional play context. | May group characters, shared supply, shared tracks, truths, notes. |

## Character

| Aspect | Details |
| --- | --- |
| Purpose | Represents a playable Ironsworn character and current sheet state. |
| Important fields | `id`, `name`, `rulesetId`, `concept`, `backgroundNotes`, `stats`, `assets`, `bonds`, `vows`, `conditionMeters`, `momentum`, `debilities`, `progressTracks`, `experience`, `equipmentNotes`, `campaignId`, `createdAt`, `updatedAt`, `schemaVersion`. |
| Relationships | Owns most character state; may belong to one campaign/world; references ruleset definitions for stats, moves, and assets. |
| Validation rules | Name required; ruleset required; standard creation stats must use `3,2,2,1,1`; starting meters must initialize correctly; debilities unmarked at creation unless imported/custom. |
| Open questions | Should custom/variant characters be first-class in MVP? Should character support multiple rulesets after creation? |

## Stat

| Aspect | Details |
| --- | --- |
| Purpose | Provides numeric modifiers for moves. |
| Important fields | `id`, `name`, `description`, `value`, `min`, `max`, `sourceReference`. |
| Relationships | Belongs to character; definition belongs to ruleset. |
| Validation rules | MVP standard values are `1` to `3`; starting distribution must be exact. |
| Open questions | Should later advancement ever change stats in core Ironsworn MVP, or only through assets/advanced content? |

## Asset

| Aspect | Details |
| --- | --- |
| Purpose | Represents selected capabilities, companions, rituals, paths, and combat talents. |
| Important fields | `definitionId`, `name`, `type`, `summary`, `abilities`, `selectedAbilityIds`, `requirements`, `companionState`, `notes`, `sourceReference`. |
| Relationships | Asset definitions belong to ruleset; selected assets belong to character; companion assets may contain companion health/name. |
| Validation rules | Three selected at standard creation; no duplicates by default; required companion names must be captured; prerequisites need either validation or override note. |
| Open questions | Can app store full official asset text? How should asset prerequisites be machine-readable? |

## Bond

| Aspect | Details |
| --- | --- |
| Purpose | Records a meaningful relationship with a person, faction, or community. |
| Important fields | `id`, `name`, `type`, `description`, `location`, `status`, `notes`, `createdFromBackground`, `createdAt`. |
| Relationships | Belongs to character; may reference campaign/world entity; contributes to bonds progress track. |
| Validation rules | Name required; starting background bonds limited to three for standard creation. |
| Open questions | Should individual bonds each have status, or is only the global bonds progress track required for MVP? |

## Vow

| Aspect | Details |
| --- | --- |
| Purpose | Tracks an active, fulfilled, or forsaken quest promise. |
| Important fields | `id`, `title`, `description`, `rank`, `status`, `progressTrackId`, `isBackgroundVow`, `isIncitingIncident`, `experienceAwarded`, `notes`, `createdAt`, `completedAt`. |
| Relationships | Belongs to character or campaign; owns/references a progress track; may be linked to burdens such as cursed or tormented. |
| Validation rules | Title and rank required; rank must be standard; active vow must have progress track; progress cannot exceed track maximum. |
| Open questions | Should shared vows live on campaign with participant links, or be duplicated per character for MVP? |

## ProgressTrack

| Aspect | Details |
| --- | --- |
| Purpose | Measures progress toward vows, bonds, journeys, fights, and other challenges. |
| Important fields | `id`, `name`, `type`, `rank`, `ticks`, `maxTicks`, `status`, `sharedScope`, `notes`. |
| Relationships | Referenced by vows, bonds, active journeys/fights, or campaign. |
| Validation rules | `maxTicks` should be `40` for ten boxes with four ticks each; ticks range `0` to `40`; progress score is full boxes only. |
| Open questions | Should MVP store journey/fight tracks as generic tracks or specialized entities? |

## Momentum

| Aspect | Details |
| --- | --- |
| Purpose | Tracks advantage/setback and derived momentum limits. |
| Important fields | `current`, `max`, `reset`, `minimum`, `lastBurnedAt`, `notes`. |
| Relationships | Belongs to character; derived from debilities. |
| Validation rules | Current cannot exceed max; current cannot go below `-6` without setback handling; max is `10 - markedDebilityCount`; reset depends on debility count. |
| Open questions | Should burning momentum be a first-class action in MVP or a simple set-to-reset shortcut? |

## ConditionMeter

| Aspect | Details |
| --- | --- |
| Purpose | Represents health, spirit, and supply. |
| Important fields | `type`, `current`, `min`, `max`, `isShared`, `lockedByDebility`, `notes`. |
| Relationships | Belongs to character; supply may later belong to campaign; locks are caused by debilities. |
| Validation rules | Current range `0` to `5`; health cannot increase while wounded; spirit cannot increase while shaken; supply cannot increase while unprepared. |
| Open questions | Should supply be stored on character in MVP and migrated later, or model shared supply from day one? |

## Debility

| Aspect | Details |
| --- | --- |
| Purpose | Captures conditions, banes, and burdens that constrain the character. |
| Important fields | `id`, `name`, `category`, `marked`, `isPermanent`, `linkedVowId`, `notes`, `markedAt`, `clearedAt`. |
| Relationships | Belongs to character; affects momentum; may lock a condition meter; burdens may reference vows. |
| Validation rules | Marked debilities reduce max momentum; reset recalculates based on count; banes should require explicit override to clear; burdens should link to quest/vow when possible. |
| Open questions | How should assets that count as debilities be represented: debility records, asset flags, or both? |

## Move

| Aspect | Details |
| --- | --- |
| Purpose | Defines rules actions that may reference stats and change character state. |
| Important fields | `id`, `name`, `category`, `triggerSummary`, `statOptions`, `stateImpacts`, `isProgressMove`, `sourceReference`, `licensingStatus`. |
| Relationships | Belongs to ruleset; may reference stats, assets, meters, debilities, and tracks. |
| Validation rules | MVP can store summaries and state-impact metadata; full automation is out of scope. |
| Open questions | Can move text be stored verbatim? Which moves need MVP state-update shortcuts? |

## Campaign / World

| Aspect | Details |
| --- | --- |
| Purpose | Optional container for shared play context, especially co-op or guided games. |
| Important fields | `id`, `name`, `mode`, `truths`, `characters`, `sharedSupply`, `sharedProgressTracks`, `notes`, `createdAt`. |
| Relationships | Owns or references characters; may own shared vows, journeys, fights, and supply. |
| Validation rules | Not required for solo MVP; if used, name required. |
| Open questions | Is MVP solo-only, campaign-based, or flexible? Should Truths Workbook support be planned now or deferred? |

## Suggested Aggregate Boundaries

- `Character` should be the primary aggregate for MVP local persistence.
- `Ruleset` and `AssetDefinition` should be read-only catalog data.
- `Campaign` should remain optional until shared supply/progress is implemented.
- `Move` should begin as metadata for UI guidance, not an automation engine.
