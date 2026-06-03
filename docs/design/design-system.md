# Brand Identity

## Visual Theme

The Ironsworn Character Creator & Manager should feel like a premium desktop tool discovered inside an ancient oathbound archive. The visual language is dark, calm, tactile, and restrained: blackened vellum, forged steel, deep royal purple cloth, muted gold leaf, candlelit margins, and weathered cartographic lines.

This is not a decorative fantasy skin. It is a professional character management application with mythic atmosphere. The interface should support repeated reading, editing, comparison, and decision-making without visual fatigue.

## Emotional Goals

- Calm confidence: users should feel grounded while managing complex character information.
- Mythic weight: choices, vows, progress, and character history should feel meaningful.
- Premium restraint: the interface should feel crafted, not loud.
- Readable depth: dark surfaces should be layered without becoming murky.
- Ancient utility: details may evoke journals, ledgers, maps, and forged objects, but never at the expense of clarity.

## Visual Keywords

- Ancient
- Premium
- Forged
- Candlelit
- Mythic
- Measured
- Scholarly
- Weathered
- Tactile
- Quiet
- Readable
- Durable

## Visual Anti-Patterns

- Neon glows, saturated cyberpunk gradients, and electric blues.
- Bright high-fantasy palettes with emerald, ruby, and sapphire dominance.
- MMORPG frames, item rarity colors, ornate inventory slots, or oversized metal filigree.
- Anime-inspired color, proportion, icon, or illustration treatments.
- Excessive parchment textures that reduce text contrast.
- Skulls, spikes, blood splatter, demonic motifs, or grimdark shock styling.
- Cartoonish icons, bubbly buttons, soft toy-like shadows, or playful fantasy decoration.
- Dense ornamental borders around every element. Use ornament as seasoning, not stew.

# Color System

## Palette Philosophy

The palette is built around near-black backgrounds, desaturated royal purples, antique gold accents, and warm bone text. Purple carries identity and hierarchy. Gold marks emphasis and premium moments. Black creates calm depth. Status colors are muted and grounded so they do not break the dark fantasy tone.

Use gold sparingly. If everything is gold, nothing is precious.

## Backgrounds

| Token | HEX | Purpose | Usage Guidelines | Accessibility Concerns |
| --- | --- | --- | --- | --- |
| background-primary | `#0B090F` | Main application background. | Use for the root app shell and the deepest visible areas. It should feel like a blackened archive, not pure black. | Pair with `text-primary`, `text-secondary`, or strong border values. Avoid placing low-contrast purple text directly on this color. |
| background-secondary | `#14111B` | Secondary page or workspace background. | Use for large content regions, sidebar backgrounds, and calm grouping areas. | Maintains strong contrast with primary text. Use `border-subtle` to separate from `background-primary`. |
| surface | `#1C1824` | Default component and content surface. | Use for panels, cards, form fields, lists, and quiet interactive areas. | Text remains readable with all text tokens. Avoid using `purple-100` as body text here. |
| surface-elevated | `#262030` | Raised or emphasized surface. | Use for popovers, selected panels, modal interiors, menus, and active content containers. | Use `border-normal` or a soft shadow so elevated surfaces do not blur into the background. |

## Primary: Purple

| Token | HEX | Purpose | Usage Guidelines | Accessibility Concerns |
| --- | --- | --- | --- | --- |
| purple-100 | `#E7DCF8` | Pale purple highlight. | Use for subtle active text, small accents, and selected-icon highlights on dark surfaces. | Good for text on dark backgrounds. Do not use as a large background behind light text. |
| purple-200 | `#C8B2EA` | Soft royal purple. | Use for secondary accents, focus outlines, and understated badges. | Use with dark text only when placed on light or gold surfaces; otherwise prefer as border or icon color. |
| purple-300 | `#9E78D3` | Core visible purple. | Use for primary controls, selection indicators, and meaningful navigation emphasis. | White or bone text works well. Confirm contrast when used with `text-secondary`. |
| purple-400 | `#6F45A8` | Deep royal purple. | Use for hover states, active fills, and larger brand fields where saturation must stay restrained. | Avoid small text in this color on dark backgrounds; use it as a fill or border. |
| purple-500 | `#3D255F` | Shadow purple. | Use for pressed states, deep backgrounds, and subtle purple-tinted panels. | Requires light text. Do not use for text on dark backgrounds. |

## Accent: Gold

| Token | HEX | Purpose | Usage Guidelines | Accessibility Concerns |
| --- | --- | --- | --- | --- |
| gold-100 | `#F4E7BA` | Pale gold highlight. | Use for small premium highlights, fine dividers, and rare emphasis. | Readable on dark backgrounds. Avoid using on light surfaces. |
| gold-200 | `#D7B96A` | Primary antique gold. | Use for accent buttons, important icons, active markers, and key decorative rules. | Strong on dark surfaces. For body text, reserve for short labels only. |
| gold-300 | `#A98235` | Aged gold. | Use for borders, inactive gold accents, and hover states where a quieter tone is needed. | Avoid as small text on dark backgrounds unless contrast is verified. |
| gold-400 | `#6F5224` | Tarnished gold shadow. | Use for pressed gold states, deep ornamental lines, and muted separators. | Not suitable for text on dark backgrounds. Use as a structural color only. |

## Text

| Token | HEX | Purpose | Usage Guidelines | Accessibility Concerns |
| --- | --- | --- | --- | --- |
| text-primary | `#F2EDE2` | Main readable text. | Use for primary body text, headings, labels, values, and form input text. | Excellent contrast on all dark backgrounds and surfaces. |
| text-secondary | `#C8BFAE` | Supporting text. | Use for descriptions, secondary values, inactive tab labels, and helper text. | Meets contrast on dark surfaces for normal text. Do not reduce opacity further. |
| text-muted | `#8E8474` | Low-emphasis text. | Use for timestamps, placeholders, disabled labels, and quiet metadata. | Use at 14px or larger where possible. Avoid for critical information. |

## Status

| Token | HEX | Purpose | Usage Guidelines | Accessibility Concerns |
| --- | --- | --- | --- | --- |
| success | `#7EA66A` | Positive or completed status. | Use for success states, completed progress, and confirmation indicators. | Do not rely on green alone; pair with iconography or text. |
| warning | `#C49A4A` | Caution or pending status. | Use for unresolved requirements, risky choices, and pending changes. | Close to gold; pair warnings with labels or icons so accent and status are distinguishable. |
| danger | `#B85F56` | Destructive or error status. | Use for errors, destructive actions, validation failures, and irreversible state. | Pair with clear labels and do not use as the only signal. |
| info | `#6F93B8` | Informational status. | Use for neutral notices, guidance, and system information. | Keep saturation muted. Ensure text placed over info fills uses `#0B090F` or `text-primary` based on contrast. |

## Borders

| Token | HEX | Purpose | Usage Guidelines | Accessibility Concerns |
| --- | --- | --- | --- | --- |
| border-subtle | `#2E2936` | Quiet separation. | Use for card edges, dividers, table rows, and default input borders. | Visible enough on dark surfaces without creating clutter. |
| border-normal | `#4A4058` | Standard structural border. | Use for interactive components, focused group boundaries, selected panels, and menus. | Use around low-contrast surfaces to preserve shape recognition. |
| border-strong | `#8E6F3A` | Premium or active border. | Use sparingly for active selections, high-emphasis panels, and important separators. | Gold-like border should not be the only indication of state. Pair with fill, icon, or text style. |

# Typography

## Font Families

- Primary font family: `Inter`
- Heading font family: `Cormorant Garamond`
- Body fallback fonts: `Inter, Segoe UI, Roboto, Helvetica Neue, Arial, sans-serif`
- Heading fallback fonts: `Cormorant Garamond, Georgia, Times New Roman, serif`

Use the serif heading face to create ancient journal character without sacrificing body readability. Body text must remain modern, crisp, and calm.

## Type Scale

| Style | Size | Weight | Line Height | Usage |
| --- | ---: | ---: | ---: | --- |
| H1 | 40px | 600 | 48px | Top-level document or major area title. Use sparingly. |
| H2 | 32px | 600 | 40px | Major section heading. |
| H3 | 24px | 600 | 32px | Panel or content group heading. |
| H4 | 20px | 600 | 28px | Compact group heading, modal heading, or sidebar section title. |
| Body Large | 18px | 400 | 28px | Important readable copy and high-value descriptions. |
| Body | 16px | 400 | 24px | Default interface text and content. |
| Small | 14px | 400 | 20px | Helper text, dense metadata, compact controls. |
| Caption | 12px | 500 | 16px | Labels, eyebrow metadata, table hints, and compact tags. |

## Typography Rules

- Headings should use `Cormorant Garamond` with controlled weight. Avoid overly thin or swashy styles.
- Body text should use `Inter` for sustained reading.
- Letter spacing should remain `0` for normal text.
- All caps may be used only for short captions or labels at 12px with moderate weight.
- Avoid glowing text, embossed text, and image-text overlays in application UI.

# Spacing System

## Spacing Scale

| Token | Value | Usage |
| --- | ---: | --- |
| spacing-2xs | 2px | Hairline adjustments and tight icon alignment. |
| spacing-xs | 4px | Micro gaps inside compact controls. |
| spacing-sm | 8px | Standard small gaps, icon-to-label spacing. |
| spacing-md | 12px | Compact padding and dense component spacing. |
| spacing-lg | 16px | Default component padding and grouped content spacing. |
| spacing-xl | 24px | Section spacing and larger component separation. |
| spacing-2xl | 32px | Major layout spacing. |
| spacing-3xl | 48px | Large structural separation. |
| spacing-4xl | 64px | Rare page-level spacing. |

## Grid System

- Use an 8px base grid for layout, spacing, and component dimensions.
- Use 4px increments only for fine internal alignment.
- Align text, controls, and key content edges to the same vertical rhythm.
- Avoid ornamental offsets unless they preserve reading order and alignment clarity.

## Container Widths

- compact-container: `480px`
- reading-container: `720px`
- content-container: `960px`
- wide-container: `1200px`
- app-max-container: `1440px`

Desktop layouts should feel spacious but not theatrical. Long-form reading areas should stay around `720px` for comfort.

## Content Density Rules

- Prefer medium density for general app surfaces.
- Dense areas may use 14px text and 8px vertical spacing only when scanning or comparison is the primary task.
- Avoid oversized padding in operational areas.
- Preserve enough breathing room around important choices, warnings, and destructive actions.

# Elevation System

## Shadow Philosophy

Elevation should feel like candlelit depth, not floating glass. Shadows are soft, low-saturation, and dark. Do not use bright glow effects as elevation.

## Flat Surfaces

- Background and default surfaces sit flat.
- Use `border-subtle` for separation.
- Use when content belongs to the same hierarchy level.

Suggested shadow: none.

## Raised Surfaces

- Use for cards, active panels, popovers, and menus.
- Combine `surface-elevated`, `border-normal`, and a soft ambient shadow.

Suggested shadow: `0 12px 32px rgba(0, 0, 0, 0.28)`.

## Modal Surfaces

- Use for the highest temporary visual layer.
- Modal backgrounds should use `surface-elevated`.
- Add a stronger border and deeper shadow, but keep the form dignified.

Suggested shadow: `0 24px 80px rgba(0, 0, 0, 0.48)`.

## Elevation Usage Rules

- Do not stack multiple raised surfaces inside each other.
- Avoid large blurred glows.
- Use elevation to clarify hierarchy, not decorate.
- Keep shadows consistent across the product.

# Component Style Language

## Buttons

- Border radius: `6px`.
- Primary buttons use `purple-300` fill, `text-primary` text, and a subtle `purple-200` top edge or border.
- Accent buttons use `gold-200` fill with dark text only for rare high-emphasis actions.
- Secondary buttons use transparent or `surface` fill with `border-normal`.
- Destructive buttons use `danger` with restrained contrast; avoid aggressive red fills unless the action is critical.
- Button text should be concise, medium weight, and never ornamental.
- Focus styling should use a clear `purple-200` outline with enough offset to remain visible.

## Cards

- Border radius: `8px` maximum.
- Default fill: `surface`.
- Elevated fill: `surface-elevated`.
- Borders should be `border-subtle` by default.
- Use a fine gold or purple accent line only for selected or important cards.
- Cards should feel like ledger panels, not collectible item frames.

## Panels

- Panels are larger structural surfaces.
- Use `background-secondary` or `surface`.
- Panel headers may use a subtle bottom border.
- Avoid thick ornamental frames.
- Use consistent inner padding: `spacing-lg` for standard panels and `spacing-xl` for relaxed panels.

## Inputs

- Fill: `#16121E` or `surface`.
- Border: `border-normal`.
- Text: `text-primary`.
- Placeholder: `text-muted`.
- Focus border: `purple-200`.
- Error border: `danger`.
- Input corners should be `6px`.
- Avoid parchment fills, glowing edges, or heavy inset shadows.

## Dropdowns

- Trigger styling should match inputs.
- Menu surface: `surface-elevated`.
- Menu border: `border-normal`.
- Highlighted option: `purple-500` fill with `text-primary`.
- Selected option may use a small gold or purple marker.
- Avoid large decorative chevrons.

## Checkboxes

- Size: `16px` or `18px`.
- Default border: `border-normal`.
- Checked fill: `purple-300`.
- Checkmark: `text-primary` or `background-primary`, whichever provides stronger clarity.
- Focus outline must remain visible outside the control.

## Radio Buttons

- Size: `16px` or `18px`.
- Default border: `border-normal`.
- Selected ring: `purple-300`.
- Inner dot: `gold-200` or `purple-100`.
- Use muted labels for disabled states.

## Tabs

- Default text: `text-secondary`.
- Active text: `text-primary`.
- Active indicator: `gold-200` or `purple-300`, not both at once.
- Tab backgrounds should remain calm; use borders or underline indicators before filled pills.
- Avoid bulky tab shapes that resemble game menu plaques.

## Modals

- Surface: `surface-elevated`.
- Border: `border-normal` with optional `border-strong` top accent for important modals.
- Radius: `8px`.
- Backdrop: near-black with 60 percent opacity.
- Modal typography should remain practical and readable.
- Do not add decorative illustration by default.

## Tooltips

- Surface: `#211B2B`.
- Text: `text-primary`.
- Border: `border-subtle`.
- Radius: `4px`.
- Shadow: subtle raised shadow.
- Tooltips should be compact, plain, and readable.

## Sidebars

- Background: `background-secondary`.
- Dividers: `border-subtle`.
- Active item: `purple-500` fill, `border-strong` or `gold-200` left accent.
- Inactive items: `text-secondary`.
- Sidebar styling should feel like a bound index or archive navigation, not a fantasy game menu.

# Iconography

## Icon Style

Icons should be simple, line-based, and readable at small sizes. Use a consistent stroke style with slightly softened corners. Icons may reference journals, maps, vows, shields, paths, stars, and tools, but should remain symbolic rather than illustrative.

## Icon Size System

| Token | Size | Usage |
| --- | ---: | --- |
| icon-xs | 12px | Captions, metadata, compact badges. |
| icon-sm | 16px | Default inline icon size. |
| icon-md | 20px | Buttons, tabs, form controls. |
| icon-lg | 24px | Section headers and prominent controls. |
| icon-xl | 32px | Rare empty states or large visual anchors. |

## Icon Usage Rules

- Use one icon style throughout the app.
- Prefer 1.75px or 2px stroke weight.
- Icons should inherit text color unless they communicate a specific state.
- Do not use full-color fantasy illustrations as icons.
- Avoid decorative icon clutter. If an icon does not improve scanning or recognition, leave it out.

# Motion & Animation

## Animation Philosophy

Motion should be quiet, intentional, and functional. It should suggest weight and polish, not magic effects. The interface may feel like pages, drawers, and panels settling into place, but should never sparkle, pulse, bounce, or flash.

## Durations

| Token | Duration | Usage |
| --- | ---: | --- |
| motion-fast | 120ms | Hover, press, focus, small color changes. |
| motion-base | 180ms | Menu open, tooltip fade, small panel transitions. |
| motion-slow | 240ms | Modal entry, larger panel changes. |
| motion-deliberate | 320ms | Rare major transitions. |

## Easing Guidelines

- Standard easing: `cubic-bezier(0.2, 0, 0, 1)`.
- Exit easing: `cubic-bezier(0.4, 0, 1, 1)`.
- Avoid bounce, elastic, springy, or overshooting effects.
- Respect reduced-motion preferences by removing non-essential movement.

# Accessibility Standards

## Contrast Goals

- Body text should meet WCAG AA contrast at minimum.
- Important text, labels, and controls should target WCAG AAA where practical.
- Focus indicators must be visible against all dark surfaces.
- Gold accents must not replace readable text contrast.

## Keyboard Usage Expectations

- Every interactive element must have a visible focus state.
- Focus styling should be consistent across buttons, inputs, tabs, menus, and modal controls.
- Focus rings should not rely only on color; use outline thickness, offset, or border change.

## Color Blindness Considerations

- Status states must pair color with labels, icons, or shape.
- Success, warning, danger, and info colors should not be the only signifier.
- Purple and gold accents should be supported by placement, weight, or border style.

## Text Scaling Expectations

- Text must remain usable at 200 percent scaling.
- Components should expand vertically rather than clipping text.
- Avoid fixed-height text containers for body content.
- Minimum body text size should be 16px for general reading.
- Caption text should not carry critical information alone.

# Design Tokens

The following token names are final visual-language tokens and can later be converted into CSS variables or another token format.

```css
:root {
  --color-background-primary: #0B090F;
  --color-background-secondary: #14111B;
  --color-surface: #1C1824;
  --color-surface-elevated: #262030;

  --color-purple-100: #E7DCF8;
  --color-purple-200: #C8B2EA;
  --color-purple-300: #9E78D3;
  --color-purple-400: #6F45A8;
  --color-purple-500: #3D255F;

  --color-gold-100: #F4E7BA;
  --color-gold-200: #D7B96A;
  --color-gold-300: #A98235;
  --color-gold-400: #6F5224;

  --color-text-primary: #F2EDE2;
  --color-text-secondary: #C8BFAE;
  --color-text-muted: #8E8474;

  --color-success: #7EA66A;
  --color-warning: #C49A4A;
  --color-danger: #B85F56;
  --color-info: #6F93B8;

  --color-border-subtle: #2E2936;
  --color-border-normal: #4A4058;
  --color-border-strong: #8E6F3A;

  --font-heading: "Cormorant Garamond", Georgia, "Times New Roman", serif;
  --font-body: "Inter", "Segoe UI", Roboto, "Helvetica Neue", Arial, sans-serif;

  --font-size-h1: 40px;
  --font-size-h2: 32px;
  --font-size-h3: 24px;
  --font-size-h4: 20px;
  --font-size-body-lg: 18px;
  --font-size-body: 16px;
  --font-size-small: 14px;
  --font-size-caption: 12px;

  --line-height-h1: 48px;
  --line-height-h2: 40px;
  --line-height-h3: 32px;
  --line-height-h4: 28px;
  --line-height-body-lg: 28px;
  --line-height-body: 24px;
  --line-height-small: 20px;
  --line-height-caption: 16px;

  --font-weight-regular: 400;
  --font-weight-medium: 500;
  --font-weight-semibold: 600;

  --spacing-2xs: 2px;
  --spacing-xs: 4px;
  --spacing-sm: 8px;
  --spacing-md: 12px;
  --spacing-lg: 16px;
  --spacing-xl: 24px;
  --spacing-2xl: 32px;
  --spacing-3xl: 48px;
  --spacing-4xl: 64px;

  --radius-sm: 4px;
  --radius-md: 6px;
  --radius-lg: 8px;

  --shadow-raised: 0 12px 32px rgba(0, 0, 0, 0.28);
  --shadow-modal: 0 24px 80px rgba(0, 0, 0, 0.48);

  --icon-xs: 12px;
  --icon-sm: 16px;
  --icon-md: 20px;
  --icon-lg: 24px;
  --icon-xl: 32px;

  --motion-fast: 120ms;
  --motion-base: 180ms;
  --motion-slow: 240ms;
  --motion-deliberate: 320ms;
  --ease-standard: cubic-bezier(0.2, 0, 0, 1);
  --ease-exit: cubic-bezier(0.4, 0, 1, 1);

  --container-compact: 480px;
  --container-reading: 720px;
  --container-content: 960px;
  --container-wide: 1200px;
  --container-app-max: 1440px;
}
```
