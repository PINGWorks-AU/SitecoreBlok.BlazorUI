# UI Parity Report

Generated: 2026-09-04 17:26:44  
Scope: 275 component(s)

## Check 1 — Compiled-utility coverage

No missing utilities.

## Check 2 — Runtime-composed class detection

No runtime-composed class names detected.

## Check 3 — Blok class-string drift

No unexpected drift.

### Accepted — 25 documented divergence(s)

Listed in `tools/parity-known-drift.json` and argued in [docs/ui-parity-audit.md](ui-parity-audit.md). These do not fail the run.

| Component | In Blok | Why |
|-----------|---------|-----|
| button.tsx (Blok) ↔ 1 Razor file(s) | `text-primary-foreground` | Removed deliberately. Set in both the Default variant base and the Default+Primary compound, which raced on source order and lost in dark mode. Text colour is set once, in the compound. |
| calendar.tsx (Blok) ↔ 1 Razor file(s) | `border-input` | Composition artefact. Moved to SelectTrigger.razor with the InBuiltDropdown port, outside the harness's Calendar file scope. |
| calendar.tsx (Blok) ↔ 1 Razor file(s) | `bg-popover` | Composition artefact. Surface now comes from SelectContent, which sets it in its own file. |
| calendar.tsx (Blok) ↔ 1 Razor file(s) | `border-collapse` | Blok renders the day grid as a <table>; we render a flat div grid. Covered by the DivergenceNote on CalendarPage. |
| calendar.tsx (Blok) ↔ 1 Razor file(s) | `hover:rounded-md` | Our rounding is per-state (rounded-md / rounded-l-md / rounded-none). A base hover rounding would round range-middle cells and break the continuous range bar. |
| calendar.tsx (Blok) ↔ 1 Razor file(s) | `text-inverse-text` | Selected and range-endpoint cells use literal text-white. The surface is a fixed bg-primary-500 that does not flip, so the token would resolve dark-on-blue in dark mode. Enforced by Check 5. |
| editable.tsx (Blok) ↔ 10 Razor file(s) | `hover:bg-transparent` | Same visual result reached differently — hover:bg-neutral-bg is conditionally omitted when IsPreviewFocusable is false. |
| editable.tsx (Blok) ↔ 10 Razor file(s) | `bg-white` | We use bg-popover instead, deliberately. bg-white does not flip, so Blok's error tooltip is white-on-dark in dark mode with text-destructive on it. |
| kbd.tsx (Blok) ↔ 1 Razor file(s) | `dark:[[data-slot=tooltip-content]_&]:bg-background/10` | Our Tooltip surface is a fixed bg-gray-700 in both modes, so the nested Kbd uses bg-white/20 with literal text-white and needs no dark variant. |
| popover.tsx (Blok) ↔ 3 Razor file(s) | `bg-popover` | Ours is deliberately headless; consumers supply the surface via ClassName. Filter's dropdown is the worked example. |
| popover.tsx (Blok) ↔ 3 Razor file(s) | `text-popover-foreground` | Headless — see bg-popover above. |
| popover.tsx (Blok) ↔ 3 Razor file(s) | `rounded-md` | Headless — see bg-popover above. |
| popover.tsx (Blok) ↔ 3 Razor file(s) | `shadow-md` | Headless — see bg-popover above. |
| scroll-area.tsx (Blok) ↔ 1 Razor file(s) | `border-l` | Blok styles a Radix custom scrollbar with a bordered track. We use the library-wide native thin-scrollbar treatment. |
| scroll-area.tsx (Blok) ↔ 1 Razor file(s) | `border-l-transparent` | Radix scrollbar track — see border-l above. |
| scroll-area.tsx (Blok) ↔ 1 Razor file(s) | `border-t` | Radix scrollbar track — see border-l above. |
| scroll-area.tsx (Blok) ↔ 1 Razor file(s) | `border-t-transparent` | Radix scrollbar track — see border-l above. |
| scroll-area.tsx (Blok) ↔ 1 Razor file(s) | `bg-border` | Present, but as a ::-webkit-scrollbar-thumb variant rather than a bare token, so the comparison cannot see it. |
| search-input.tsx (Blok) ↔ 5 Razor file(s) | `focus:border-0` | Pre-existing. The wrapper draws the focus border via a has-[] selector, so the inner input having none is already the intent; left as-is pending a SearchInput pass. |
| time-picker.tsx (Blok) ↔ 2 Razor file(s) | `border-input` | Composition artefact. Our trigger is a Button with the Outline variant, so this token lives in Button.razor, outside the harness's TimePicker file scope. Blok inlines the whole trigger class string instead of composing a Button. |
| time-picker.tsx (Blok) ↔ 2 Razor file(s) | `text-base` | Composition artefact. Our trigger is a Button with the Outline variant, so this token lives in Button.razor, outside the harness's TimePicker file scope. Blok inlines the whole trigger class string instead of composing a Button. |
| time-picker.tsx (Blok) ↔ 2 Razor file(s) | `dark:aria-invalid:ring-destructive/40` | Composition artefact. Our trigger is a Button with the Outline variant, so this token lives in Button.razor, outside the harness's TimePicker file scope. Blok inlines the whole trigger class string instead of composing a Button. |
| time-picker.tsx (Blok) ↔ 2 Razor file(s) | `dark:bg-input/30` | Composition artefact. Our trigger is a Button with the Outline variant, so this token lives in Button.razor, outside the harness's TimePicker file scope. Blok inlines the whole trigger class string instead of composing a Button. |
| time-picker.tsx (Blok) ↔ 2 Razor file(s) | `dark:hover:bg-input/50` | Composition artefact. Our trigger is a Button with the Outline variant, so this token lives in Button.razor, outside the harness's TimePicker file scope. Blok inlines the whole trigger class string instead of composing a Button. |
| tooltip.tsx (Blok) ↔ 3 Razor file(s) | `text-inverse-text` | Replaced by literal text-white. The surface is a fixed bg-gray-700 in both modes, so a flipping token renders invisible in dark. Enforced by Check 5. |

## Check 4 — Surface background without paired text token

Theme-aware surface backgrounds (`bg-background`, `bg-card`, `bg-popover`, `bg-muted`, `bg-accent`, `bg-primary`, `bg-secondary`, `bg-destructive`) flip colour in dark mode. Without an explicit `text-*` token in the same class string the foreground relies on cascade — which silently breaks for fixed-positioned or portal-rendered content. Pair the surface bg with its matching text token (e.g. `bg-background text-foreground`, `bg-card text-card-foreground`, `bg-primary text-white`).

No unpaired surface backgrounds found.

## Check 5 — Fixed-shade background with flipping text token

Hardcoded fixed-shade backgrounds (`bg-gray-700`, `bg-black`, `bg-{color}-{500-900}`) do not flip with dark mode. Pairing them with a flipping text token (`text-foreground`, `text-inverse-text`, `text-*-fg`) means the text changes colour between modes while the surface stays put — producing invisible text in one mode. Use a literal text colour instead (`text-white`, `text-{shade}-50/100/200`), or suppress with `parity-no-text-pair` if the element has no rendered text.

No fixed-shade / flipping-text mismatches found.

## Check 6 — Token light/dark symmetry

Tokens defined as `var(--color-blackAlpha-N)` (or `whiteAlpha-N`) in `colors.css` are alpha-based and tied to one theme. They MUST have a corresponding override in `globals.css` `.dark { }` block that flips them to the opposite alpha (or to a non-alpha colour) — otherwise they render as nearly invisible against the opposite-theme page background. Skeleton's `bg-neutral-50` was the trigger.

No token-symmetry issues found.

