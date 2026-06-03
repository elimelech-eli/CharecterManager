# Open Questions

## Source And Copyright

| Question | Why it matters | Proposed next step |
| --- | --- | --- |
| Can the app store and display full Ironsworn move text? | Determines whether move reference can be in-app or only summarized with page/source references. | Review the Creative Commons Attribution 4.0 license notes in the PDFs and decide attribution/product policy. |
| Can the app store and display full asset card text from `Ironsworn-Assets.pdf`? | Asset selection is much more useful with full text, but content scope must be approved. | Product/legal decision before full catalog ingestion. |
| Can the app visually mimic the official playkit character sheet? | Export and sheet UI may be affected by layout rights and attribution. | Prefer an original layout unless explicit reuse is approved. |
| What attribution must appear in-app and in exports? | Required for compliant use of licensed content. | Create a license/credits requirements doc. |

## Rules Interpretation

| Question | Why it matters | Proposed next step |
| --- | --- | --- |
| Should starting background vow be required to be extreme or epic? | Rulebook guidance suggests high-rank background vow, but product may allow variants. | Default to recommended ranks with override. |
| Should the inciting-incident vow be mandatory? | Core first-session guidance expects it, but users may create drafts. | Require for finalized standard character; allow draft without it. |
| How strictly should asset prerequisites be enforced? | Some prerequisites are narrative and hard to validate. | Use warnings and override notes for MVP. |
| How should assets that count as debilities be modeled? | Affects momentum calculations and asset/debility UI. | Represent as asset flag plus derived debility count contribution. |
| How should permanent banes be cleared, if at all? | Maimed/corrupted are intended as permanent narrative changes. | Require explicit override and note if clearing. |
| Should progress undermining remove ticks, boxes, or freeform notes? | Moves can undermine progress narratively. | MVP should support manual tick adjustment and required note. |

## Content Storage

| Question | Why it matters | Proposed next step |
| --- | --- | --- |
| Store full text, summaries, or references for moves? | Drives data model, UI, search, and licensing. | Start with summaries/state-impact metadata; add full text only after approval. |
| Store full text, summaries, or references for assets? | Affects asset picker usefulness. | Start with structured metadata plus short summaries; plan full text as a content pack. |
| Should source page references be stored on every rule entity? | Helps verification and attribution. | Yes for imported catalog data. |
| Do we need a content version separate from app version? | Rules/content updates may ship independently. | Recommended for asset/move catalog. |

## Storage And Architecture

| Question | Why it matters | Proposed next step |
| --- | --- | --- |
| Should MVP use local file storage, SQLite, IndexedDB, or MongoDB? | Desktop app needs reliable offline persistence. | Favor local storage abstraction first; defer MongoDB until sync/cross-device is required. |
| Is MongoDB Atlas actually needed for MVP? | Adds account/network complexity. | Treat as post-MVP unless a clear requirement emerges. |
| Should supply live on Character or Campaign? | Ironsworn supply can be shared among allies. | Store on Character for solo MVP; design field so it can migrate to Campaign shared supply. |
| Should journey/fight tracks be stored if MVP focuses on characters? | Players often need temporary tracks during play. | Support generic progress tracks attached to character; campaign tracks later. |

## Product Scope

| Question | Why it matters | Proposed next step |
| --- | --- | --- |
| Is the app solo-only, co-op, guided, campaign-based, or all of the above? | Changes navigation, shared supply, campaign model, and export. | MVP should support solo character use and not block campaign association later. |
| Should campaign/world/truths setup be part of MVP? | Truths are important to Ironsworn but not strictly character-sheet state. | Defer to Post-MVP unless user research says otherwise. |
| Should move rolling be included in MVP? | Roll automation is useful but expands scope. | Defer full move engine; include manual state updates. |
| Should oracle support be included in MVP? | Important for solo play but outside character management. | Defer; add hooks for future. |
| Should equipment be structured inventory? | Ironsworn treats mundane gear abstractly through supply. | Keep equipment as notes unless tied to assets. |

## Future Rulesets And Supplements

| Question | Why it matters | Proposed next step |
| --- | --- | --- |
| Will the app support Delve? | Adds threats, sites, and additional assets. | Keep ruleset/content modules versioned. |
| Will the app support Starforged or Sundered Isles? | Related systems differ in character resources and assets. | Do not hardcode Ironsworn-only concepts outside ruleset module. |
| Will users create custom assets or homebrew rules? | Changes validation and catalog design. | Defer homebrew until after core Ironsworn MVP. |
| Will multiple characters share one campaign file? | Important for co-op/guided play. | Model Campaign as optional future aggregate. |

## Immediate TODOs

- TODO: Decide whether to remove or migrate the existing remote file under `docs\ product\ironsworn-rules-notes.md`; the folder name includes a leading space and does not match requested structure.
- TODO: Create a licensing/attribution note before importing full move or asset text.
- TODO: Confirm whether `Ironsworn-Rulebook-Spreads.pdf` needs independent review or is only a layout variant of `Ironsworn-Rulebook.pdf`.
- TODO: Define MVP storage choice before implementation starts.
- TODO: Decide whether MVP finalized characters require both starting vows or allow a non-standard finalized state.
