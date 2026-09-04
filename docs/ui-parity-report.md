# UI Parity Report

Generated: 2026-09-04 14:42:26  
Scope: 274 component(s)

## Check 1 — Compiled-utility coverage

| Component | Token missing from compiled CSS |
|-----------|----------------------------------|
| CodeViewer.razor | `language-markup` |
| CodeViewer.razor | `language-html` |
| CodeViewer.razor | `language-xml` |
| CodeViewer.razor | `language-svg` |
| CodeViewer.razor | `language-mathml` |
| CodeViewer.razor | `language-ssml` |
| CodeViewer.razor | `language-atom` |
| CodeViewer.razor | `language-rss` |
| CodeViewer.razor | `language-css` |
| CodeViewer.razor | `language-clike` |
| CodeViewer.razor | `language-js` |
| CodeViewer.razor | `language-aspnet` |
| CodeViewer.razor | `language-bash` |
| CodeViewer.razor | `language-shell` |
| CodeViewer.razor | `language-csharp` |
| CodeViewer.razor | `language-css-extras` |
| CodeViewer.razor | `language-csv` |
| CodeViewer.razor | `language-diff` |
| CodeViewer.razor | `language-graphql` |
| CodeViewer.razor | `language-handlebars` |
| CodeViewer.razor | `language-json` |
| CodeViewer.razor | `language-json5` |
| CodeViewer.razor | `language-less` |
| CodeViewer.razor | `language-markdown` |
| CodeViewer.razor | `language-markup-templating` |
| CodeViewer.razor | `language-mongodb` |
| CodeViewer.razor | `language-plsql` |
| CodeViewer.razor | `language-powershell` |
| CodeViewer.razor | `language-python` |
| CodeViewer.razor | `language-razor` |
| CodeViewer.razor | `language-sql` |
| CodeViewer.razor | `language-sass` |
| CodeViewer.razor | `language-typescript` |
| CodeViewer.razor | `language-xml-doc` |
| CodeViewer.razor | `language-yaml` |
| InputGroupAddon.razor | `inline-start` |
| InputGroupAddon.razor | `inline-end` |
| InputGroupAddon.razor | `block-start` |
| InputGroupAddon.razor | `block-end` |
| InputGroupButton.razor | `icon-xs` |
| InputGroupButton.razor | `icon-sm` |

## Check 2 — Runtime-composed class detection

No runtime-composed class names detected.

## Check 3 — Blok class-string drift

| Component | In Blok | In ours |
|-----------|---------|---------|
| button.tsx (Blok) ↔ 1 Razor file(s) | `text-primary-foreground` | (missing) |
| calendar.tsx (Blok) ↔ 1 Razor file(s) | `border-input` | (missing) |
| calendar.tsx (Blok) ↔ 1 Razor file(s) | `bg-popover` | (missing) |
| calendar.tsx (Blok) ↔ 1 Razor file(s) | `border-collapse` | (missing) |
| calendar.tsx (Blok) ↔ 1 Razor file(s) | `hover:rounded-md` | (missing) |
| calendar.tsx (Blok) ↔ 1 Razor file(s) | `text-inverse-text` | (missing) |
| context-menu.tsx (Blok) ↔ 15 Razor file(s) | `focus:text-accent-foreground` | (missing) |
| date-picker.tsx (Blok) ↔ 1 Razor file(s) | `border-input` | (missing) |
| date-picker.tsx (Blok) ↔ 1 Razor file(s) | `rounded-md` | (missing) |
| date-picker.tsx (Blok) ↔ 1 Razor file(s) | `text-base` | (missing) |
| date-picker.tsx (Blok) ↔ 1 Razor file(s) | `dark:aria-invalid:ring-destructive/40` | (missing) |
| date-picker.tsx (Blok) ↔ 1 Razor file(s) | `dark:bg-input/30` | (missing) |
| editable.tsx (Blok) ↔ 10 Razor file(s) | `hover:bg-transparent` | (missing) |
| editable.tsx (Blok) ↔ 10 Razor file(s) | `bg-white` | (missing) |
| input.tsx (Blok) ↔ 11 Razor file(s) | `focus:ring-1` | (missing) |
| kbd.tsx (Blok) ↔ 1 Razor file(s) | `dark:[[data-slot=tooltip-content]_&]:bg-background/10` | (missing) |
| popover.tsx (Blok) ↔ 3 Razor file(s) | `bg-popover` | (missing) |
| popover.tsx (Blok) ↔ 3 Razor file(s) | `text-popover-foreground` | (missing) |
| popover.tsx (Blok) ↔ 3 Razor file(s) | `rounded-md` | (missing) |
| popover.tsx (Blok) ↔ 3 Razor file(s) | `shadow-md` | (missing) |
| scroll-area.tsx (Blok) ↔ 1 Razor file(s) | `border-l` | (missing) |
| scroll-area.tsx (Blok) ↔ 1 Razor file(s) | `border-l-transparent` | (missing) |
| scroll-area.tsx (Blok) ↔ 1 Razor file(s) | `border-t` | (missing) |
| scroll-area.tsx (Blok) ↔ 1 Razor file(s) | `border-t-transparent` | (missing) |
| scroll-area.tsx (Blok) ↔ 1 Razor file(s) | `bg-border` | (missing) |
| search-input.tsx (Blok) ↔ 5 Razor file(s) | `focus:border-0` | (missing) |
| stack-navigation.tsx (Blok) ↔ 1 Razor file(s) | `shadow-none` | (missing) |
| time-picker.tsx (Blok) ↔ 1 Razor file(s) | `text-base` | (missing) |
| time-picker.tsx (Blok) ↔ 1 Razor file(s) | `dark:aria-invalid:ring-destructive/40` | (missing) |
| time-picker.tsx (Blok) ↔ 1 Razor file(s) | `dark:bg-input/30` | (missing) |
| time-picker.tsx (Blok) ↔ 1 Razor file(s) | `dark:hover:bg-input/50` | (missing) |
| time-picker.tsx (Blok) ↔ 1 Razor file(s) | `shadow-xs` | (missing) |
| tooltip.tsx (Blok) ↔ 3 Razor file(s) | `text-inverse-text` | (missing) |

## Check 4 — Surface background without paired text token

Theme-aware surface backgrounds (`bg-background`, `bg-card`, `bg-popover`, `bg-muted`, `bg-accent`, `bg-primary`, `bg-secondary`, `bg-destructive`) flip colour in dark mode. Without an explicit `text-*` token in the same class string the foreground relies on cascade — which silently breaks for fixed-positioned or portal-rendered content. Pair the surface bg with its matching text token (e.g. `bg-background text-foreground`, `bg-card text-card-foreground`, `bg-primary text-white`).

No unpaired surface backgrounds found.

## Check 5 — Fixed-shade background with flipping text token

Hardcoded fixed-shade backgrounds (`bg-gray-700`, `bg-black`, `bg-{color}-{500-900}`) do not flip with dark mode. Pairing them with a flipping text token (`text-foreground`, `text-inverse-text`, `text-*-fg`) means the text changes colour between modes while the surface stays put — producing invisible text in one mode. Use a literal text colour instead (`text-white`, `text-{shade}-50/100/200`), or suppress with `parity-no-text-pair` if the element has no rendered text.

No fixed-shade / flipping-text mismatches found.

## Check 6 — Token light/dark symmetry

Tokens defined as `var(--color-blackAlpha-N)` (or `whiteAlpha-N`) in `colors.css` are alpha-based and tied to one theme. They MUST have a corresponding override in `globals.css` `.dark { }` block that flips them to the opposite alpha (or to a non-alpha colour) — otherwise they render as nearly invisible against the opposite-theme page background. Skeleton's `bg-neutral-50` was the trigger.

No token-symmetry issues found.

