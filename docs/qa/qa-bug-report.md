# QA Bug Report

## Test Environment
- App version/build: `0.1.0`, rebuilt from `origin/main` commit `59d46a6ad48b422f980fcad70a700fbe4f6b08db` using `npm.cmd run package:win`
- OS: Microsoft Windows NT `10.0.26200.0`
- Date tested: 2026-06-06
- EXE path: `C:\Users\Eli\OneDrive\Documents\CharacterManager\.codex-qa-main\apps\desktop\release\win-unpacked\CharacterManager.exe`
- PDFs used: `D:\GIT\CharacterManager\docs\Ironsworn-Rulebook.pdf`, `D:\GIT\CharacterManager\docs\Ironsworn-Assets.pdf`, `D:\GIT\CharacterManager\docs\Ironsworn-Playkit.pdf`, `D:\GIT\CharacterManager\docs\Ironsworn-Truths-Workbook.pdf`

## Summary
- Total bugs found: 5
- UI bugs: 2
- Backend bugs: 3
- Critical: 0
- High: 3
- Medium: 2
- Low: 0

## Bugs

## Bug: Character can be finalized without an inciting-incident vow

**Category:** Backend

**Severity:** High

**Source of truth:**  
`Ironsworn-Rulebook.pdf`, character creation / vows guidance: a starting character should have both a background vow and an inciting-incident vow.

**Steps to recreate:**
1. Launch `CharacterManager.exe`.
2. Click `New Character`.
3. Enter name `QA One Vow Accepted` and concept `Exiled Ironlander scout`.
4. Assign stats as Edge `3`, Heart `2`, Iron `2`, Shadow `1`, Wits `1`.
5. Select exactly three non-companion assets: `Alchemist`, `Archer`, and `Duelist`.
6. Leave bonds empty.
7. On `Starting Vow`, add only `Background vow`.
8. Set title `Reclaim Frostmark Hall` and rank `Extreme`.
9. Do not add an `Inciting-incident vow`.
10. Continue to `Review` and click `Finalize`.

**Expected result:**  
Finalization should fail and require both starting vows before creating a playable Ironsworn character.

**Actual result:**  
The app finalized the character and opened the active character view. The finalized character showed `ACTIVE VOWS 1` and only `Reclaim Frostmark Hall`.

**Notes for developer:**  
The packaged backend ruleset response reports `startingCharacter.requiredActiveVowCount: 1`, and the UI checklist says `At least one active vow has title and rank`. Suspected area: backend ruleset data and `CharacterValidator.ValidateVows`.

## Bug: Review screen shows a success validation message before validation has run

**Category:** UI

**Severity:** Medium

**Source of truth:**  
App design expectation: review/finalization should clearly communicate whether backend validation has run and whether required creation fields are missing.

**Steps to recreate:**
1. Launch `CharacterManager.exe`.
2. Click `New Character`.
3. Do not enter a name, stats, assets, or vows.
4. Click wizard step `Review`.
5. Observe the validation area.
6. Click `Finalize`.

**Expected result:**  
Before validation runs, the Review screen should not present the draft as clean. It should either show validation has not been run, run validation automatically, or show required-field issues.

**Actual result:**  
The Review screen displays `No backend validation issues are currently reported.` for a completely incomplete draft. After clicking `Finalize`, the app then reports errors for missing/invalid stats, missing assets, missing vow, and missing name.

**Notes for developer:**  
This appears to be a UI state problem where `validation === null` is rendered the same as a successful validation result. Suspected area: `ReviewStep` / `ValidationPanel`.

## Bug: Active character meters, momentum, vows, bonds, and debilities are read-only

**Category:** UI

**Severity:** High

**Source of truth:**  
`Ironsworn-Rulebook.pdf` and `Ironsworn-Playkit.pdf`: play requires tracking health, spirit, supply, momentum, debilities, vow progress, and bonds as moves and consequences change character state.

**Steps to recreate:**
1. Launch `CharacterManager.exe`.
2. Open a finalized character, such as `QA One Vow Accepted`.
3. Inspect `Overview`, `Vows`, and `Conditions`.
4. Try to change health, spirit, supply, momentum, vow progress, or debilities.

**Expected result:**  
A user should be able to record play-state changes: decrease/increase condition meters, adjust momentum, mark debilities, mark vow progress, and update bonds/vows.

**Actual result:**  
The active character screens expose only navigation buttons. `Overview`, `Vows`, and `Conditions` had zero inputs, selects, textareas, steppers, toggles, or save controls. Condition meters, momentum, vow progress, bonds, and debilities are display-only.

**Notes for developer:**  
Suspected area: active character screens after finalization. This blocks the core character manager use case after creation.

## Bug: Ruleset API does not expose the Ironsworn move catalog

**Category:** Backend

**Severity:** High

**Source of truth:**  
`Ironsworn-Rulebook.pdf`, moves chapter, and `Ironsworn-Playkit.pdf`: moves such as Face Danger, Secure an Advantage, Endure Harm, Swear an Iron Vow, Reach a Milestone, and Fulfill Your Vow are core play procedures.

**Steps to recreate:**
1. Launch `CharacterManager.exe`.
2. Query the packaged backend endpoint `http://127.0.0.1:53987/api/v1/rulesets/ironsworn`.
3. Inspect the JSON response.
4. In the UI, inspect the primary navigation and finalized character tabs.

**Expected result:**  
The ruleset API should expose move definitions needed by the UI, and the app should provide a move list or move resolution workflow.

**Actual result:**  
The ruleset response includes stats, starting character data, meters, momentum, ranks, debilities, and assets, but no `moves` collection. The UI has no Moves navigation or move-resolution screen.

**Notes for developer:**  
The in-memory catalog appears to define move metadata, but the API DTO mapping does not include it. Suspected area: ruleset DTO/API mapping and move UI.

## Bug: Encumbered debility is missing from the Ironsworn ruleset

**Category:** Backend

**Severity:** Medium

**Source of truth:**  
`Ironsworn-Rulebook.pdf` / character sheet debilities and `Ironsworn-Playkit.pdf`: conditions include Wounded, Shaken, Unprepared, and Encumbered.

**Steps to recreate:**
1. Launch `CharacterManager.exe`.
2. Query `http://127.0.0.1:53987/api/v1/rulesets/ironsworn`.
3. Inspect the `debilities` array.

**Expected result:**  
The ruleset debilities should include `Encumbered` along with the other Ironsworn conditions.

**Actual result:**  
The debilities returned are `Wounded`, `Shaken`, `Unprepared`, `Maimed`, `Corrupted`, `Cursed`, and `Tormented`. `Encumbered` is missing.

**Notes for developer:**  
Suspected area: `InMemoryRulesetCatalog` debility data and any condition/debility UI generated from it.

## Potential Issues / Questions

- `npm.cmd run package:win` produced an unpacked executable at `release\win-unpacked\CharacterManager.exe`, not the portable artifact described in the root README. This may be intentional after packaging changes, but the docs and QA expectation mention a portable EXE.
- Clicking `New Character` immediately creates and persists an untitled draft. Failed or abandoned creation attempts leave multiple `Untitled draft` records in the library unless the user manually deletes them.
- Asset cards intentionally show metadata-only placeholder ability text. This may be a product/licensing decision, but it limits in-app verification against `Ironsworn-Assets.pdf`.

## Areas Not Fully Tested

- Full NSIS/release installer flow and self-signed packaging.
- Corrupt local JSON character records and backend-unavailable recovery beyond visible retry messaging.
- Exhaustive asset catalog comparison against every card in `Ironsworn-Assets.pdf`.
- Exact move math and roll resolution beyond confirming the UI/API lacks a move workflow.
- Visual layout across multiple display sizes and high-DPI settings.
