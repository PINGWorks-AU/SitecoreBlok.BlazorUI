# UI Parity Report

Generated: 2026-09-02 17:04:13  
Scope: 1 component(s)

## Check 1 — Compiled-utility coverage

No missing utilities.

## Check 2 — Runtime-composed class detection

No runtime-composed class names detected.

## Check 3 — Blok class-string drift

| Component | In Blok | In ours |
|-----------|---------|---------|
| calendar.tsx (Blok) ↔ 1 Razor file(s) | `bg-popover` | (missing) |
| calendar.tsx (Blok) ↔ 1 Razor file(s) | `border-collapse` | (missing) |
| calendar.tsx (Blok) ↔ 1 Razor file(s) | `hover:rounded-md` | (missing) |
| calendar.tsx (Blok) ↔ 1 Razor file(s) | `text-inverse-text` | (missing) |

## Check 4 — Surface background without paired text token

Theme-aware surface backgrounds (`bg-background`, `bg-card`, `bg-popover`, `bg-muted`, `bg-accent`, `bg-primary`, `bg-secondary`, `bg-destructive`) flip colour in dark mode. Without an explicit `text-*` token in the same class string the foreground relies on cascade — which silently breaks for fixed-positioned or portal-rendered content. Pair the surface bg with its matching text token (e.g. `bg-background text-foreground`, `bg-card text-card-foreground`, `bg-primary text-white`).

No unpaired surface backgrounds found.

## Check 5 — Fixed-shade background with flipping text token

Hardcoded fixed-shade backgrounds (`bg-gray-700`, `bg-black`, `bg-{color}-{500-900}`) do not flip with dark mode. Pairing them with a flipping text token (`text-foreground`, `text-inverse-text`, `text-*-fg`) means the text changes colour between modes while the surface stays put — producing invisible text in one mode. Use a literal text colour instead (`text-white`, `text-{shade}-50/100/200`), or suppress with `parity-no-text-pair` if the element has no rendered text.

No fixed-shade / flipping-text mismatches found.

## Check 6 — Token light/dark symmetry

Tokens defined as `var(--color-blackAlpha-N)` (or `whiteAlpha-N`) in `colors.css` are alpha-based and tied to one theme. They MUST have a corresponding override in `globals.css` `.dark { }` block that flips them to the opposite alpha (or to a non-alpha colour) — otherwise they render as nearly invisible against the opposite-theme page background. Skeleton's `bg-neutral-50` was the trigger.

No token-symmetry issues found.

