# MVP Roadmap

## Milestone 1: Rules Knowledge And Static Character Sheet

| Field | Details |
| --- | --- |
| Goal | Establish the Ironsworn character domain and display a static character sheet. |
| Deliverables | Rules summary; domain model; static character fixture; read-only sheet view; validation checklist. |
| Acceptance criteria | Sheet displays stats, meters, momentum, debilities, assets, vows, bonds, and progress; docs identify source/licensing TODOs; no production rules automation required. |
| Risks | Copyright uncertainty; asset catalog scope; existing branch/rebase hygiene. |
| Recommended owner role | Product Owner with Architect and UX Designer. |

## Milestone 2: Character Creation Wizard

| Field | Details |
| --- | --- |
| Goal | Let users create a standard Ironsworn starting character. |
| Deliverables | Wizard flow; stat assignment; starting meter defaults; asset selection placeholder or catalog; bonds and vows steps; review/finalize. |
| Acceptance criteria | User can create a valid character using `3,2,2,1,1`; three assets selected; starting vows captured; incomplete drafts are labeled. |
| Risks | Full asset text licensing; custom/variant character requests; UX complexity for new players. |
| Recommended owner role | UX Designer with Frontend Developer and Product Owner. |

## Milestone 3: Character Persistence

| Field | Details |
| --- | --- |
| Goal | Save, reopen, update, and delete characters locally. |
| Deliverables | Local storage design; character list; create/read/update/delete operations; schema versioning; import/export investigation. |
| Acceptance criteria | Character survives app restart; invalid persisted data is detected; storage implementation can be replaced or extended later. |
| Risks | Premature MongoDB dependency; schema churn; unclear desktop storage location. |
| Recommended owner role | Backend Developer with Architect. |

## Milestone 4: Character State Management During Play

| Field | Details |
| --- | --- |
| Goal | Support fast manual updates to character state while playing. |
| Deliverables | Meter controls; momentum controls; debility toggles; vow/bond/progress track management; optional quick-update log. |
| Acceptance criteria | Health/spirit/supply stay within range; debilities recalculate max/reset momentum; progress tracks support ticks and rank; users can update state without entering creation flow. |
| Risks | Over-automation; edge cases around shared supply; Pay the Price can affect many state areas. |
| Recommended owner role | Frontend Developer with Product Owner and Rules Reviewer. |

## Milestone 5: Polish / Export

| Field | Details |
| --- | --- |
| Goal | Make the MVP pleasant, trustworthy, and shareable. |
| Deliverables | Printable sheet view; export path; empty/error states; source attribution; accessibility pass; usability review. |
| Acceptance criteria | User can view and print/export a complete sheet; long vows/assets do not break layout; source/license note is present where needed; primary flows are usable without developer assistance. |
| Risks | Export layout overflow; content attribution requirements; unclear product name/branding. |
| Recommended owner role | UX Designer with Frontend Developer and Product Owner. |

## Cross-Milestone Definition Of Done

- Only summarized rules guidance is exposed unless legal/product approves full text.
- New validation rules are documented and tested.
- Character data includes schema/ruleset version.
- UX supports both creation-time guidance and play-time speed.
- Open questions are either resolved or intentionally deferred.
