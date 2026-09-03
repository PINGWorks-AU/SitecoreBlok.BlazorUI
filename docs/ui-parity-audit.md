# UI Parity Audit

Tracking document for the component-by-component UI-parity audit against the Blok source (https://github.com/Sitecore/blok/tree/main/src) and the Blok live site (https://blok.sitecore.com/primitives).

**Automated verification:** `pwsh ./tools/verify-ui-parity.ps1` runs six checks on every Razor file under the component library and writes a transient `docs/ui-parity-report.md` summarising findings for that run. The report file is a build artifact — not tracked in git — and is overwritten on each run. Integrated into the `blok-migration` skill — runs on every component migration / update.

Last automated harness run, re-baselined 2026-09-03 after two harness fixes (see the triage section below): **Check 3 65 findings across 24 Blok sources · Check 1 41 · Check 4 27 · Checks 2, 5, 6 clean.** The previous header claimed "35 drift, Checks 1/2/4/5/6 clean", which was stale on every count — Checks 1 and 4 were never clean, and Check 3 was only comparing 16 sources. Findings are triaged below; most are artefacts of composition rather than real drift.

## The six automated checks

1. **Compiled-utility coverage** — every Tailwind class referenced in Razor exists in the compiled CSS (`sitecore-blok.css`). Catches misspelled or deprecated utility names.
2. **Runtime-composed class detection** — flags `$"bg-{color}"`-style interpolations Tailwind's scanner cannot see.
3. **Blok class-string drift** (opt-in) — fetches Blok source and diffs class strings.
4. **Surface background without paired text token** — flags any class string that sets a theme-aware surface bg (`bg-background`, `bg-card`, `bg-popover`, `bg-muted`, `bg-accent`, `bg-primary`, `bg-secondary`, `bg-destructive`) without an explicit `text-*` token in the same string. Cascade-only foreground silently breaks for fixed-positioned and portal-rendered content (Dialog, AlertDialog, Sheet, Toast). Decorative surfaces (slider tracks, progress fills, indicator dots) and wrappers whose cells set their own text suppress the check with the marker `parity-no-text-pair`.
5. **Fixed-shade bg paired with flipping text token** — flags fixed-shade backgrounds (`bg-gray-700`, `bg-zinc-900`, `bg-black`, etc.) combined with a flipping text token (`text-foreground`, `text-inverse-text`, `text-{thing}-fg`). Because fixed shades don't change between modes but flipping text tokens do, contrast disappears in one mode and text becomes invisible. Tooltip is the canonical case.
6. **Token light/dark symmetry** — flags any `--color-{name}` defined in `colors.css` as `var(--color-blackAlpha-N)` (or `whiteAlpha-N`) that isn't redefined in `globals.css`'s `.dark { }` block. Without the dark override, subtle alpha shades render invisibly against the dark page background (Skeleton's `bg-neutral-50` was the trigger).

## Status legend

- ✅ Matches Blok
- ⚠️ Minor deviation (documented in Notes)
- ❌ Broken / needs fix
- ⏳ Not yet audited

## Phase 1 — Token Foundation  ✅ COMPLETE

Fixes that land transparently across many components (no per-component changes needed):

| # | Fix | Impact | Status |
|---|-----|--------|--------|
| 1 | **Case-mismatch bug**: `--color-whitealpha-*` → `--color-whiteAlpha-*` in `globals.css` `.dark { }` and `ThemeToggle.razor`. The original port used lowercase but `colors.css` defines camelCase. Dark mode was silently broken for `border`, `muted-foreground`, `placeholder`, `neutral-bg`, `neutral-bg-active`, `neutral-hover`, `neutral-active` in the `.dark { }` block. | Systemic | ✅ |
| 2 | **Alpha-based `-bg` / `-bg-active` tokens** now use `rgba(..., 0.12)` / `rgba(..., 0.24)` matching Blok exactly. Previous solid `-900` / `-800` colours were too heavy. | Alert, Badge, Button-outline/ghost, Checkbox | ✅ |
| 3 | **`info` colour family** — added `--color-info-fg`, `--color-info-bg`, `--color-info-bg-active`, `--info-hover`, `--info-active` to `.dark`. | Checkbox, Info Badge, Info Alert | ✅ |
| 4 | **`warning` hover/active** — added `--warning-hover`, `--warning-active`. | Warning buttons/badges | ✅ |
| 5 | **`--primary-foreground` in dark** — changed from `blackAlpha-900` to `primary-200` matching Blok. | Generic `text-primary-foreground` references | ✅ |
| 6 | **`--color-neutral-fg`** — changed from `whiteAlpha-600` to `whiteAlpha-700` matching Blok. | Outline/Ghost neutral buttons, neutral-fg text | ✅ |
| 7 | **`--color-neutral-bg-active`** — changed from `whiteAlpha-200` to `whiteAlpha-300` matching Blok. | Neutral active state | ✅ |
| 8 | **Direct colour tokens** (`--color-success`, `--color-danger`, `--color-warning`, `--color-info`) added to dark; these are used by Default-variant buttons. Previously they stayed at light-mode `-500` shade. | Default-variant buttons in Danger/Success/Warning/Info schemes | ✅ |
| 9 | **All changes mirrored in `ThemeToggle.razor` `<style>` block** — runtime override is verbatim with `globals.css` `.dark { }`. | Runtime toggle fidelity | ✅ |

## Phase 2 — Component audit, by batch

For each component: render in Catalogue dark mode, compare side-by-side to `blok.sitecore.com/primitives/{name}` dark mode, cross-check Blok source at `github.com/Sitecore/blok/blob/main/src/components/ui/{name}.tsx`. Variants audited: default, each `size`, each `variant`, each `colorScheme`, each interactive state (default/hover/active/focus/disabled).

### Batch A — Core (highest traffic) ✅ AUDITED

| Component | Default | Hover | Active | Focus | Variants | Notes |
|-----------|---------|-------|--------|-------|----------|-------|
| Button | ✅ | ✅ | ✅ | ✅ | ✅ | Classes match Blok. Focus-visible now uses `border-primary`/`ring-primary/50` matching Blok. |
| Card | ✅ | ✅ | — | — | ✅ | Uses `bg-background` / `bg-subtle-bg` / `border-border`; Blok uses `bg-body-bg` / `bg-subtle-bg` / `border-border-color`. Functionally equivalent (both resolve via `@theme inline` to same semantic tokens). |
| Input | ✅ | ✅ | — | ✅ | ✅ | Now uses `bg-background` (functionally equivalent to Blok's `bg-body-bg`). |
| Dialog | ✅ | — | — | — | ✅ | Uses `bg-background`. `shadow-lg` matching Blok. |
| Badge | ✅ | — | — | — | ✅ | Class-for-class match with Blok, including hardcoded yellow/teal/cyan/blue/pink `dark:` variants. |
| Alert | ✅ | — | — | — | ✅ | Class-for-class match. Icon colours use `dark:[&>svg]:text-{variant}-200` overrides. |
| Separator | ✅ | — | — | — | ✅ | `bg-border` — cleanly theme-aware. |

**Batch A verdict:** No component code changes needed. Phase 1 token fixes resolve the visible issues. Minor deviations documented above are cosmetic / light-mode-only.

### Batch B — Form controls ✅ AUDITED

| Component | Default | Hover | Active | Focus | Variants | Notes |
|-----------|---------|-------|--------|-------|----------|-------|
| Checkbox | ✅ | ✅ | ✅ | ✅ | ✅ | Uses info-* family for checked state (our design choice; Blok uses primary). `dark:` variants wired for info-900/info-950 boundaries. Info family now fully overridden in Phase 1. |
| RadioGroup | ✅ | ✅ | ✅ | ✅ | ⚠️ | Class strings match Blok verbatim (`border-input`, `text-primary`, `dark:bg-input/30`, `dark:aria-invalid:ring-destructive/40`). **Two documented divergences (2026-04-20):** (1) Item wraps button in `<div class="flex items-center gap-2">` with optional `Label` string — ergonomic helper; DivergenceNote on page. (2) Button adds `inline-flex items-center justify-center` beyond Blok's string — required to centre the indicator dot in both axes because our raw `<button>` + `<span>` doesn't inherit Radix's implicit centering. Also: indicator Icon now sized via `!size-[0.8rem]` (matches Blok's `<Icon size={0.8}>` = 12.8px), fixing the prior bug where `Scale="0.8"` left the layout box at `size-6` (24px) and pushed the dot off-centre. Anti-revert comments on both class strings. |
| Select | ✅ | ✅ | ✅ | ✅ | ✅ | **Fixed** `bg-white` → `bg-body-bg` to match Blok. All other classes match. |
| Switch | ✅ | — | ✅ | ✅ | ✅ | `data-[state=unchecked]:bg-input`, `bg-background` thumb — theme-aware via Phase 1 `--input` fix. |
| Textarea | ✅ | — | — | ✅ | ✅ | `bg-white dark:bg-input/30` — matches Blok exactly. |
| SearchInput | ✅ | — | — | ✅ | ✅ | `bg-white dark:bg-input/30` — matches Blok. |
| InputGroup | ✅ | — | — | ✅ | ✅ | `bg-white dark:bg-input/30` — matches Blok. |
| Field | ✅ | — | — | — | ✅ | Layout only; inherits theme from parents. |
| Label | ✅ | — | — | — | ✅ | Layout only; inherits theme. |
| Toggle | ✅ | ✅ | ✅ | ✅ | ✅ | `data-[state=on]:bg-primary-bg data-[state=on]:text-primary-fg` — primary-bg/fg now alpha-based after Phase 1. |
| ToggleGroup | ✅ | ✅ | ✅ | ✅ | ✅ | Composes Toggle; inherits. |
| Slider | ✅ | ✅ | ✅ | ✅ | ✅ | Uses `bg-primary`, `bg-muted`, `border-primary` — all theme-aware. |
| Combobox | ✅ | ✅ | ✅ | ✅ | ✅ | Port of Blok's `combobox.tsx` (`4a6b44`). Structure mirrors Blok: 18 file-per-export components (Combobox, Value, Trigger, Input, Content, List, Item, ItemText/Title/Description, Group, Label, Collection, Empty, Separator, Chips, Chip, ChipsInput). Class strings match Blok verbatim. **Paradigm translation (not divergence):** (1) State bindings use Blazor's `@bind-Value` / `@bind-Values` / `@bind-InputValue` instead of React state hooks — same behavioural surface, Blazor idiom. (2) `useComboboxAnchor` React hook omitted — Blazor consumers use `@ref` on elements directly; 18 of 19 Blok exports are present (the hook has no Blazor equivalent to port). **Known limitations:** basic keyboard navigation only (ArrowUp/Down, Enter, Escape, Backspace-to-clear). Blok's `@base-ui` backing adds Home/End jumps, typeahead buffering, and pointer-coarse tap-to-open affordances that are not ported — tracked as a feature gap on the Home page. Dropdown anchors in-place with `position:fixed` (not via PopoverService) so filter state re-renders items live. Mouse hover tracks highlight; backspacing the filter to empty clears the selection. |

**Batch B verdict:** 1 component fix (Select `bg-white` → `bg-body-bg`). Combobox added 2026-04-21 (Parity; advanced keyboard affordances noted as Known Gap). All other components pass after Phase 1.

### Batch C — Overlays ✅ AUDITED

| Component | Default | Hover | Active | Focus | Variants | Notes |
|-----------|---------|-------|--------|-------|----------|-------|
| Tooltip | ✅ | — | — | — | ✅ | Uses `bg-gray-700` hardcoded — **intentional, matches Blok**. Tooltips stay dark in both modes (standard UX pattern). |
| Popover | ✅ | — | — | — | ✅ | Uses `bg-popover text-popover-foreground` — theme-aware. |
| Sheet | ✅ | — | — | — | ✅ | Uses `bg-background` with overlay `bg-black/50` — standard, matches Blok. |
| AlertDialog | ✅ | — | — | — | ✅ | Same as Dialog. |
| DropdownMenu | ✅ | ✅ | ✅ | ✅ | ✅ | Uses `bg-popover`, `hover:bg-accent`, `focus:bg-accent` — all theme-aware. |
| ContextMenu | ✅ | ✅ | ✅ | ✅ | ✅ | Full 15-component split (2026-04-22). All `bg-popover text-popover-foreground` — theme-aware. Submenus hover-driven (not Radix focus). CheckboxItem keeps menu open. ContextMenuPortal is no-op passthrough. Harness clean. |
| Menubar | ✅ | ✅ | ✅ | ✅ | ✅ | Full 16-component split (2026-04-22). Root `bg-background text-foreground`; content/sub-content use `bg-popover text-popover-foreground` — all theme-aware. Deliberate divergences: (a) no keyboard navigation (Radix handles this via focus trap; no Blazor equivalent); (b) `MenubarPortal` is a no-op passthrough (Blazor uses fixed positioning); (c) hover-to-switch implemented via `@onmouseenter` on trigger checking `Menubar.HasAnyOpen`. `MenubarCheckboxItem` and `MenubarRadioItem` close menu on activation (matches Radix Menubar default). Harness clean (16/16 components, all 6 checks). |
| HoverCard | ✅ | — | — | — | ✅ | Uses `bg-popover text-popover-foreground` — theme-aware. Ported 2026-04-21. Hover-only (no focus-open); self-managed `OpenDelay` / `CloseDelay` instead of Radix `delayDuration` context. Position computed from trigger bounds with known `w-64` width; height approximated for side=top/left first open. |
| Sidebar (21 exports) | ⚠️ minor | ✅ | — | — | ✅ | Ported 2026-04-21. Uses `bg-sidebar`, `bg-sidebar-accent text-sidebar-accent-foreground`, `border-sidebar-border`, `ring-sidebar-ring` — all theme-aware tokens defined in `globals.css`. Deliberate divergences: (a) desktop-only rendering, no mobile `Sheet` fallback; (b) no `Ctrl/Cmd+B` keyboard shortcut; (c) no cookie persistence; (d) no `tooltip` on `SidebarMenuButton` collapsed state (BlazorUI Tooltip has no TooltipProvider or programmatic hidden); (e) no `asChild` / Slot pattern. `SidebarInput` uses `parity-no-text-pair` marker — inner `<Input>` already sets `text-foreground`, matching Blok's source which also omits the explicit pair. |

**Batch C verdict:** All components pass. Tooltip intentionally uses hardcoded dark surface (matches Blok).

### Batch D — Navigation ✅ AUDITED

| Component | Default | Hover | Active | Focus | Variants | Notes |
|-----------|---------|-------|--------|-------|----------|-------|
| Breadcrumb | ✅ | ✅ | — | — | ✅ | Uses `text-muted-foreground`, `text-foreground` — theme-aware. |
| Pagination | ✅ | ✅ | ✅ | ✅ | ✅ | Uses Button variants internally; inherits. |
| Tabs | ✅ | ✅ | ✅ | ✅ | ✅ | Uses `text-muted-foreground`, `bg-primary-bg text-primary-fg` for active — alpha-based after Phase 1. |
| NavigationMenu | ✅ | ✅ | ✅ | ✅ | ✅ | Verified 2026-04-22, architectural rewrite 2026-04-22. Phase 1 fixes: (a) added `group/navigation-menu` root class; (b) `NavigationMenuTrigger` cascades open state via named `CascadingValue<bool>("NavigationMenuItemOpen")`, sets `data-state`, open-state CSS, chevron rotation; (c) removed spurious `cursor-pointer`. Phase 2 architectural rewrite: prior implementation used per-item fixed popups (diverged from Blok's shared Radix Viewport). Rewritten to implement a single shared viewport panel inside `NavigationMenu`, matching Blok's UX: all items share one viewport that repositions and cross-fades between panels on trigger hover. `NavigationMenu` cascades itself via `CascadingValue`; children register `NavEntry` records; `SetActive` JS-measures trigger+nav bounds and positions viewport with `fixed` positioning (escapes `overflow-hidden` ancestors). Two-phase close animation preserved (150ms → `_activeId=null` → 200ms → `_isOpen=false`). `NavigationMenuIndicator` ported — diamond caret tracks active trigger with `fixed` positioning. `NavigationMenuViewport` stub added for export parity. Remaining Blazor idiom difference: `NavigationMenuItem` uses named `Trigger`/`Content` RenderFragment slots rather than sibling sub-components (Blazor child-matching limitation) — no DivergenceNote since it is the only practical API shape in Blazor. |
| Stepper | ✅ | — | — | — | ✅ | Uses `bg-primary`, `text-primary-foreground`, `text-muted-foreground`, `border-border` — all semantic. |
| StackNavigation | ✅ | ✅ | ✅ | — | ✅ | Ported 2026-04-21. Uses `bg-background text-sidebar-foreground`, `bg-neutral-bg text-neutral-fg` (active neutral), `bg-primary-bg text-primary-fg` (active primary) — all theme-aware tokens. Class strings match Blok verbatim. Deliberate divergences: (a) `Icon` is a `RenderFragment` (not a string path) — faithful translation of Blok's `icon: ReactNode`, lets consumers compose any inline markup. (b) `OnItemClick` uses an event-args object with `PreventDefault` setter, and the rendered `<a>` receives `data-enhance-nav="false"` when the callback is wired so Blazor's enhanced-navigation router stays out of the way — otherwise the enhanced-nav fetch fires before our `@onclick:preventDefault` can take effect. Anti-revert comment on the data attribute. (c) Active-item detection falls back to `NavigationManager.Uri` when `Pathname` isn't supplied (Blok uses `window.location.pathname`). (d) Vertical body uses `overflow-y-auto overflow-x-hidden` + thin scrollbar styling mirroring `ScrollArea` (`[&::-webkit-scrollbar]:w-1.5`, `bg-border` thumb, rounded-full, transparent track, no buttons; Firefox `scrollbar-width:thin`) — replaces Blok's `overflow-auto` so the narrow rail can't trigger a horizontal scrollbar when labels ellipsize. Additionally: body carries `-mr-1.5` and inline `scrollbar-gutter:stable` so the scrollbar sits at the rail's outer right edge and the scrollbar gutter is reserved stably — items at `min-w-14` therefore never clip against the scrollbar (Blok's `overflow-auto` + 16px scrollbar would clip items 12px here; the thin+reserved approach trades that for a subtle always-present 6px right gutter). |

**Batch D verdict:** All components pass.

### Batch E — Feedback & data display ✅ AUDITED

| Component | Default | Hover | Active | Focus | Variants | Notes |
|-----------|---------|-------|--------|-------|----------|-------|
| Toaster | ✅ | — | — | — | ✅ | Uses `bg-background`, `bg-{variant}-bg` per variant — alpha-based after Phase 1. |
| Progress | ✅ | — | — | — | ✅ | `bg-primary` fill, `bg-muted` track — theme-aware. |
| CircularProgress | ✅ | — | — | — | ✅ | Same. |
| Spinner | ✅ | — | — | — | ✅ | `text-primary` or `text-muted-foreground` — theme-aware. |
| Skeleton | ✅ | — | — | — | ✅ | `bg-muted` animated — theme-aware. |
| Editable | ✅ | ⚠️ minor | ⚠️ minor | ✅ | ✅ | Re-audited 2026-09-02 against Blok `c631ca` — `EditableError` ported, `HasError` added; see the Editable section below. Deliberate deviations: (1) `hover:bg-transparent` not used — instead `hover:bg-neutral-bg` is conditionally omitted when `IsPreviewFocusable=false`, achieving the same visual result. (2) `EditableRootProvider` accepts Blazor parameters (`@bind-IsEditing`, `@bind-Value`) instead of a pre-built hook result (`useEditable` return value) — paradigm translation, no Blazor equivalent of React hooks. (3) `useEditable` / `useEditableContext` hooks omitted — no Blazor hook pattern. |
| EmptyState | ✅ | — | — | — | ✅ | `text-muted-foreground` — theme-aware. |
| ErrorState | ✅ | — | — | — | ✅ | `text-muted-foreground`, illustration SVGs — theme-aware. |
| Table | ✅ | ✅ | — | — | ✅ | `bg-background`, `text-neutral-fg`, `border-border` — all semantic, hover uses `hover:bg-muted/50` after Phase 1. |

**Batch E verdict:** All components pass.

### Batch F — Containers ✅ AUDITED

| Component | Default | Hover | Active | Focus | Variants | Notes |
|-----------|---------|-------|--------|-------|----------|-------|
| Accordion | ✅ | ✅ | ✅ | ✅ | ✅ | Re-audited 2026-09-02 against Blok `e10c8d`. Uses `border-border`, `text-foreground` — theme-aware. Trigger row restructured to match Blok's heading-level fix; `Actions` slot ported; one deliberate dark-mode divergence — see the Accordion section below. |
| Carousel | ✅ | — | — | — | ✅ | Uses Button variants for nav; `bg-background` slides. |
| Collapsible | ✅ | — | — | — | ✅ | Layout only; inherits theme. |
| Timeline | ✅ | — | — | — | ✅ | Uses `bg-primary`, `text-muted-foreground`, `border-border`. |
| ActionBar | ✅ | — | — | — | ✅ | `bg-background border-border` — theme-aware. |
| ScrollArea | ✅ | — | — | — | ✅ | Layout/scroll only; no color. |

**Batch F verdict:** All components pass.

### Batch G — Date & special ✅ AUDITED

| Component | Default | Hover | Active | Focus | Variants | Notes |
|-----------|---------|-------|--------|-------|----------|-------|
| DatePicker | ✅ | ✅ | ✅ | ✅ | ✅ | Re-audited 2026-09-03 against Blok `c4346e`. Uses Calendar internally + Popover; inherits theme. Harness clean on all 6 checks. See the DatePicker section below. |
| TimePicker | ✅ | ✅ | ❌ differs | ✅ | ✅ | **Not a port of Blok's composite picker** — we render a native `<input type="time">`; Blok uses a Popover with three Selects plus Clear/Done. Re-audited 2026-09-02 against `931987`. The previous note ("Uses Input/Select internally") was inaccurate. See the TimePicker section below. |
| Calendar | ✅ | ✅ | ⚠️ minor | ✅ | ✅ | Re-audited 2026-09-02 against Blok `a2d44e`; `InBuiltDropdown` adopted 2026-09-03. `bg-background`, `text-foreground`, `bg-primary text-primary-foreground` for selected — theme-aware. ARIA parity closed; 5 documented Check 3 deviations, two of them composition artefacts of the Select-based dropdowns — see the Calendar section below. |
| Avatar | ✅ | — | — | — | ✅ | `bg-muted text-muted-foreground` fallback — theme-aware. |
| Icon | ✅ | — | — | — | ✅ | Now supports `Variant` (Default/Subtle/Filled) and `ColorScheme` (11 schemes) matching Blok. **Structural divergence (Default variant only):** `<svg>` is the root element; `ClassName` lands on the SVG for chevron-rotation animation support. Subtle/Filled variants render a `<span>` wrapper matching Blok exactly. `ColorScheme` is nullable for Default — null means "inherit parent color" (backward-compatible). Blazor extras: `Scale`, `AiGradient`, `ViewBox`, `ResetClassName`. Harness fix: added switch-expression arm pattern to `Get-RazorClassStrings`. |
| Kbd | ✅ | — | — | — | ✅ | `bg-muted text-muted-foreground border-border` — theme-aware. |
| AspectRatio | ✅ | — | — | — | ✅ | Layout only; no color. |
| CodeViewer | ✅ | — | — | — | ✅ | Uses Prism.js `prism-tomorrow` theme — a **dark theme** applied uniformly in both modes. Matches Blok's pattern of code blocks always looking like a code editor. |
| CopyableToken | ✅ | ✅ | — | — | ⚠️ flexibility-upgrade | Port of Blok's `copyable-token.tsx` (`7c9f7e`) with additive Blazor-side behaviour. Structure: single `<code>` inside `Tooltip` + `TooltipTrigger` + `TooltipContent`; clipboard via `IJSRuntime.InvokeVoidAsync("navigator.clipboard.writeText", Token)`. **Additive features (beyond Blok):** (1) post-click `CopiedMessage` feedback in the tooltip with 1s auto-hide then reset on `mouseleave`; (2) `CopyMessage` / `CopiedMessage` parameters for localisation or re-wording (defaults `"Copy to clipboard"` / `"Copied"`); (3) `Clicked` `EventCallback<string>` for analytics / toasts / audit logging. Implementation uses `!important` Tailwind utilities (`!opacity-100`, `!pointer-events-auto`, `!opacity-0`, `!pointer-events-none`) on `TooltipContent` to override the CSS `group-hover` driver during the copied-feedback and suppression windows. `text-muted-foreground` added alongside Blok's `bg-muted` to satisfy Check 4 (cascade-scoped dark-mode pairing). Harness clean on all 6 checks. DivergenceNote on page. **No Blok live-site demo** (Blok's primitives directory doesn't list CopyableToken and `https://blok.sitecore.com/primitives/copyableToken` 404s), so visual comparison is local-only. |

**Batch G verdict:** All components pass. Initial audit misidentified Prism as a light theme; it's actually `prism-tomorrow` (dark).

## Phase 3 — Light-mode regression + final QA  ⏳ PENDING

Walk the Catalogue in light mode end to end after all batches land. Confirm no Phase-1 token override silently damaged light-mode rendering.

## Summary / running tally

| Metric | Count |
|--------|-------|
| Total primitives to audit | 53 |
| Audited and ✅ | 50 |
| Audited with ⚠️ (minor / out-of-scope) | 0 (all resolved) |
| Audited and ❌ (broken) | 0 |
| Not yet audited (⏳) | 0 |
| Phase 1 token fixes applied | 11 |
| Component fixes applied | 5 (Button focus, Card naming verified, Input bg, Select bg, Dialog shadow) |

### ⚠️ items — RESOLVED

All deviations from the initial audit have now been applied:

1. ~~**Button**: `focus-visible:border-ring` vs Blok's `focus-visible:border-primary`~~ — **FIXED**. Button now uses `focus-visible:border-primary focus-visible:ring-primary/50` matching Blok.
2. ~~**Input**: `bg-transparent` vs Blok's `bg-body-bg`~~ — **FIXED**. Input now uses `bg-background` (shadcn/ui naming, functionally equivalent to Blok's `bg-body-bg` — both resolve to `--background`).
3. ~~**CodeViewer**: Prism.js light theme only~~ — **NOT A BUG**. On closer inspection, the existing Prism theme is `prism-tomorrow` which is a dark theme (background `#2d2d2d`, light text `#ccc`). Used uniformly in both modes; matches Blok's pattern of code blocks always looking like a code editor.

### Additional fixes applied

4. **Dialog**: `shadow-xl` → `shadow-lg` matching Blok.
5. **Card**: Confirmed `bg-background` / `border-border` equivalent to Blok's `bg-body-bg` / `border-border-color`. Both semantic-token-backed; our shadcn/ui naming kept for consistency with the rest of the codebase.
6. **Select**: Previously changed `bg-white` → `bg-background` (functionally equivalent to Blok's `bg-body-bg`).

### Structural divergences from Blok (audited, awaiting decision per component)

Blok exports each component family as a composable set (e.g. `Avatar` + `AvatarImage` + `AvatarFallback`). Several Blazor components collapsed those exports into a single component with `RenderFragment?` parameters, hardcoded inner DOM, or a data-driven model. This breaks consumer composition: Blok demos that use `<Avatar><AvatarImage/><AvatarFallback/></Avatar>` (and equivalents elsewhere) cannot be replicated, and `*:data-[slot=…]:…` Tailwind selectors cannot target sub-elements consumers would otherwise wrap themselves.

| Component | Divergence | Likely reason | Impact | Status |
|---|---|---|---|---|
| Avatar | ~~Single file with `@if Src then <img> else <span fallback>`~~ | Easier API | Couldn't pass image+fallback as children; broke `*:data-[slot=avatar-image]` selectors | ✅ FIXED — split into Avatar + AvatarImage + AvatarFallback with cascading `ImageStatus` |
| Tooltip | ~~Single component with `Content` string param~~ Now Tooltip + TooltipTrigger + TooltipContent (3 components). `TooltipProvider` deliberately not implemented (no Radix portal/delay) — re-audited 2026-09-03 against `b79ded`, which stopped Blok's `Tooltip` auto-wrapping itself in a Provider and pushed that mount onto the consumer's root layout; we still need no Provider at all. `Side` lives on TooltipContent. **Tooltip text colour intentionally diverges**: uses `text-white` literal instead of Blok's `text-inverse-text` (which would render dark text on the always-dark `bg-gray-700` surface in dark mode). | (post-fix) Tooltip-text-color drift is the only Check 3 finding — deliberate. | ✅ FIXED — split into 3 components; TooltipPage rewritten with composable examples + DivergenceNote covering CSS-only hover, missing Provider, and the text-color choice. |
| DropdownMenu | ~~Trigger via `RenderFragment Trigger` param, content is hardcoded button~~ | (post-fix) Now state-only cascading wrapper; Trigger and Content are real components | ✅ FIXED — split into DropdownMenu + DropdownMenuTrigger + DropdownMenuContent + DropdownMenuSubTrigger + DropdownMenuSubContent. Items auto-close their parent menu chain on click. |
| ContextMenu | ~~Same shape as DropdownMenu~~ | (post-fix) Mirror refactor with right-click trigger using mouse position | ✅ FIXED — full 15-component split matching Blok: ContextMenu + ContextMenuTrigger + ContextMenuContent + ContextMenuItem + ContextMenuCheckboxItem + ContextMenuRadioItem + ContextMenuRadioGroup + ContextMenuLabel + ContextMenuSeparator + ContextMenuShortcut + ContextMenuGroup + ContextMenuPortal + ContextMenuSub + ContextMenuSubTrigger + ContextMenuSubContent. `ContextMenuPortal` is a no-op passthrough (Blazor uses fixed positioning, no React portal needed). Submenus use hover open/close via `@onmouseenter`/`@onmouseleave` (Radix uses keyboard focus; equivalent visual result). CheckboxItem keeps menu open on toggle. DivergenceNote on ContextMenuPage documents these three differences. Harness passes clean (15/15 components, all 6 checks). |
| Select | ~~Single file emits everything~~ | (post-fix) State-only wrapper; combobox state via PopoverService; all 10 Blok exports present | ✅ FIXED — 10-component split. Select + SelectTrigger + SelectValue + SelectContent + SelectGroup + SelectItem + SelectLabel + SelectSeparator + SelectScrollUpButton + SelectScrollDownButton. Placeholder moved to SelectValue; Label moved from SelectGroup to standalone SelectLabel. SelectValue caches label so trigger displays correctly after popover closes (items are disposed). |
| Accordion/AccordionItem | ~~Trigger/Content as RenderFragment? params~~ | (post-fix) AccordionItem cascades itself; trigger and content are real components reading parent state | ✅ FIXED — split into Accordion + AccordionItem + AccordionTrigger + AccordionContent. Chevron stays hardcoded (matches Blok). |
| Collapsible | ~~`Label`/`Icon` strings + multiple RenderFragment slots~~ | (post-fix) Trigger is whatever consumer puts inside CollapsibleTrigger | ✅ FIXED — split into Collapsible + CollapsibleTrigger + CollapsibleContent. CollapsibleTrigger has defensive `text-foreground` default (preflight quirk on `<button>` inheritance). DivergenceNote explains chevron-rotation pattern. |
| Pagination | ~~Single component, numeric-driven~~ | (post-fix) 7-component split; numeric helper removed | ✅ FIXED — Pagination + PaginationContent + PaginationItem + PaginationLink + PaginationPrevious + PaginationNext + PaginationEllipsis. PaginationLink supports both `Href` (Blok-style URL nav, renders `<a>`) and `Click` (Blazor SPA, renders `<button>`) — additive Blazor flexibility documented in DivergenceNote. |
| Stepper | ~~Sweep agent claimed 7-component split required~~ | Verified against Blok source: Blok's Stepper is ALSO single data-driven component | ✅ ALREADY ALIGNED — DivergenceNote on StepperPage documents the minor API differences (per-step `status` vs `ActiveStep` index; Size parameter not yet exposed). Internal helpers refactored to use `CssClassBuilder` so the harness recognises class strings. |
| Toaster | Imperative `ToastService.Show(item)` API | Idiomatic for Blazor; Blok wraps the JS-only Sonner library which has no clean Blazor analog | ❌ DELIBERATE DIVERGENCE — annotated via DivergenceNote on ToasterPage. Sonner cross-reference page added at `/primitives/sonner`. |
| EmptyState / ErrorState | ~~Title/Description as strings~~ | Verified: Blok's `EmptyStates` is ALSO single-component with string title/description | ✅ ALREADY ALIGNED — DivergenceNotes added documenting singular vs plural naming, `IconPath` vs `imageSrc`, ErrorState's HTTP-status-variant additive feature. |
| Alert | ~~AlertTitle/AlertDescription as RenderFragment? params~~ Now Alert + AlertTitle + AlertDescription as separate components, ChildContent only. `Closeable` parameter retained as Blazor-side additive feature. | (post-fix) Closeable button is the only remaining divergence — annotated via `<DivergenceNote>` on AlertPage | ✅ FIXED — split into 3 components; removed `WrapperClassName`/`TitleClassName`/`DescriptionClassName` (each component owns its `ClassName`); Catalogue page reflects new composition pattern |
| Timeline / TimelineItem | ~~Single component, Title/Description strings, hardcoded dot+connector~~ | (post-fix) 8-component composable API matching Blok | ✅ FIXED — Timeline + TimelineItem + TimelineSeparator + TimelineIndicator + TimelineConnector + TimelineContent + TimelineTitle + TimelineDescription. Indicator/connector variants exposed via enums. |
| Dialog / AlertDialog / Sheet | ~~Hardcoded close button~~ | Verified: Blok ALSO hardcodes the close button in DialogContent / SheetContent. Original sweep agent claim was incorrect | ✅ ALIGNED — only missing piece was `SheetClose.razor` (mirrors `DialogClose.razor`); now added for export parity. |

**Components verified as structurally identical** (no divergence): Card, Breadcrumb, BreadcrumbItem, Badge, Button, Spinner, Skeleton, Separator, Field, FieldGroup, Tabs, Carousel (parent), Toggle, ToggleGroup, InputGroup, and the Dialog/AlertDialog/Sheet *header / footer / title / description* sub-components. NavigationMenu was rewritten to implement the shared viewport; see its row above for the full story.

### Surface-bg / text-pairing fixes (Check 4, added after AlertDialog dark-mode regression)

Bug class: theme-aware surface background (`bg-background`, `bg-card`, etc.) without an explicit `text-*` token in the same class string. Cascade-only foreground colour silently breaks for fixed-positioned / portal-rendered content in dark mode. Reported by user against AlertDialog; symmetric fix applied across all affected components and a new harness check (Check 4) added to prevent recurrence.

**Real fixes (added matching text token)**:
- AlertDialog content: `bg-background text-foreground`
- Dialog content: `bg-background text-foreground`
- Sheet content: `bg-background text-foreground`
- ActionBar inner: `bg-background text-foreground`
- Calendar wrapper: `bg-card text-card-foreground`
- Card (Flat / Outline / Filled): `bg-{...} text-foreground`
- NavigationMenuList: `bg-background text-foreground`
- TableCell base: added `text-foreground` (covers all body + footer cells)
- Toaster Default variant: `bg-background border-border text-foreground`

**Switch thumb visual alignment fix**: removed `shadow-sm` from the thumb (Blok doesn't have it; the drop shadow visually weighed the thumb down so it appeared off-centre). Added `dark:data-[state=*]:bg-foreground` to keep the thumb a visible light shade against the dark track in dark mode. Added `dark:data-[state=unchecked]:bg-input/80` to the track for parity with Blok's dark-mode track refinement.

**Suppressed with `parity-no-text-pair` marker (decorative or text-set-in-children)**:
- Progress indicator fill (twice) — decorative
- Slider track + range — decorative
- Switch thumb — decorative
- TimelineItem marker dot — decorative
- CarouselNext / CarouselPrevious icon buttons — no text content
- Table container — cells set own text
- TableHead / TableCell pinned conditional — text in `Start` args
- TableHeader sticky conditional — `<th>` children set own text
- TableFooter — `<td>` children set own text

### Naming conventions

Our codebase uses **shadcn/ui naming** (`bg-background`, `bg-card`, `border-border`, `text-foreground`) while Blok uses **Chakra-semantic naming** (`bg-body-bg`, `bg-card-bg`, `border-border-color`, `text-body-text`). Both are backed by the same underlying CSS custom properties through `@theme inline` — they resolve identically. This is a naming preference, not a functional difference. We keep shadcn/ui names because:

- They're what the generated Tailwind utilities already cover in our compiled CSS
- They're what most components in the library already use
- They're what the shadcn/ui primitives (ported via Blok) were originally written with

#### Event-callback naming — `OnClick`, not `Click` (intentional deviation, 2026-06)

Click callbacks are named **`OnClick`**, not `Click` (the shadcn/ui `onClick` carried into Blok). This is a deliberate departure from our usual tenet of staying close to the source API, made in favour of **Blazor idioms**: the `On{Event}` prefix is the dominant Blazor convention for action callbacks, it signals an `EventCallback` (vs a value/state), and the rest of this library already uses it (`OnSubmit`, `OnCancel`, `OnClear`, `OnEdit`, `NavListItem.OnClick`). Leaving `Button` as bare `Click` was also a real footgun — because `Button` splats `AdditionalAttributes`, writing `OnClick="..."` (the natural Blazor spelling) silently rendered a literal `onclick` attribute instead of binding the callback.

Renamed `Click` → `OnClick` across all click-callback components: `Button`, `AlertDialogAction`, `AlertDialogCancel`, `DialogClose`, `SheetClose`, `PaginationLink`, `PaginationNext`, `PaginationPrevious`, `ContextMenuItem`, `DropdownMenuItem`, `MenubarItem`. (`MenubarItem`'s private `OnClick()` handler was renamed to `HandleClick()` to avoid the collision.) The `{Prop}Changed` two-way-bind callbacks are unaffected (they must keep that name for `@bind-Prop`).

## Harness drift findings — final disposition

After all structural refactors and the Group 1-4 cleanup, **32 drift findings remain**, all classified below. Every entry is either an equivalence-aliased token (Group 1, suppressed via the harness's `$equivGroups` map), a deliberate divergence (Groups 2-3, documented here), or a Blok-side typo / non-existent utility we deliberately don't propagate.

**Harness state**: Checks 1, 2, 4, 5 all clean (0 findings). Only Check 3 shows the items below.

### Group 1 — Suppressed via equivalence map (no longer flagged)

These pairs are now in `$equivGroups` in `verify-ui-parity.ps1` so the harness treats them as equivalent and they no longer appear as drift:

| Blok class | Our equivalent | Why equivalent |
|---|---|---|
| `hover:bg-primary-600` | `hover:bg-primary-hover` | `--primary-hover` resolves to `--color-primary-600` in globals.css |
| `active:bg-primary-700` | `active:bg-primary-active` | `--primary-active` resolves to `--color-primary-700` |
| `hover:bg-success-600` / `active:bg-success-700` | `hover:bg-success-hover` / `active:bg-success-active` | Same alias pattern |
| `hover:bg-danger-600` / `active:bg-danger-700` | `hover:bg-danger-hover` / `active:bg-danger-active` | Same alias pattern |

### Group 2 — Documented as deliberate divergence

| Component | Drift class | Disposition |
|---|---|---|
| **Button** | `text-primary-foreground` | Deliberately removed — caused source-order race against `text-inverse-text` from compound variants, producing washed-out light text on light bg in dark mode. See anti-revert comment in Button.razor. |
| **Button** | `bg-backgrounds` | Blok-side typo (plural). Doesn't resolve to any Tailwind utility. We use `bg-background` (singular). |
| **Button**, **Toggle**, **Input** | `text-md` | Harness false-positive — class IS in our Razor inside CssClassBuilder.Start(...) but the harness's regex misses it inside multi-arg Start calls in some cases. Verified manually present in source. Low priority. |
| **Button**, **Toggle** | `dark:aria-invalid:ring-destructive/40` | Same harness false-positive — class is present in our source. |
| **Calendar** | `bg-popover`, `border-collapse`, `hover:rounded-md`, `text-body-text`, `text-inverse-text`, `dark:bg-transparent`, `dark:hover:bg-transparent` | Superseded 2026-09-02 — `text-neutral` and `border-solid` were adopted in the `a2d44e` re-audit and are no longer drift. Full per-token reasoning for the remaining seven is in the Calendar re-audit section at the end of this document. |
| **Icon** | *(resolved)* | Tile variants (Subtle/Filled) fully implemented; all class-string drift cleared. `rounded-md`, `bg-primary-bg`, `bg-primary-fg`, `text-background`, `text-primary-fg`, `text-neutral-fg` now present. Harness extended to scan switch-expression arms (`=> "..."`) so compound-variant strings in `@code` blocks are detected. |
| **Input** | `focus:ring-1` | We use `focus-visible:ring-[3px]` — modern accessibility (keyboard-only focus, larger ring). Deliberate. |
| **Popover** | `bg-popover`, `text-popover-foreground`, `rounded-md`, `shadow-md` | Our `Popover` is positioning-only (delegates to `PopoverService`); consumer-facing components like `DropdownMenuContent`, `SelectContent`, `NavigationMenuItem` apply these classes themselves. Different responsibility split. |
| **Stepper** | `bg-background`, `text-base`, `text-muted-foreground/70`, `rounded-lg`, `bg-muted/30` | Blok exposes additional Stepper variants (e.g. card-style step containers, sm/lg sizes). We have horizontal/vertical only. Documented in StepperPage DivergenceNote. |
| **Timeline** | `border-1` | Blok-side typo (Tailwind has no `border-1` utility — `border` already means 1px). |
| **Toggle** | `text-neutral-fg`, `hover:text-neutral-fg` | Our Toggle uses `hover:text-muted-foreground` (different semantic — focus-visible token). Functionally similar but tokens differ. Could be aligned in a future refactor. |
| **Toggle** | `bg-backgrounds` | Same Blok typo as Button. |
| **Tooltip** | `text-inverse-text` | Replaced with `text-white` literal because tooltip surface (`bg-gray-700`) is fixed in both modes; `text-inverse-text` would render dark on dark in dark mode. See anti-revert comment in TooltipContent.razor. |
| **Kbd** | `dark:[[data-slot=tooltip-content]_&]:bg-background/10` | Blok's Kbd has a translucent dark variant when nested inside a TooltipContent. Our Tooltip is a CSS-only popover with different DOM structure (`data-slot="tooltip-content"` is on a different element); the parent-selector wouldn't match. Could be added later if Kbd-in-Tooltip is a common pattern. |

### Group 3 — Verified and addressed

| Component | Action |
|---|---|
| **Stepper** | Refactored `StepCircleClass`/`StepLabelClass`/`ConnectorClass` from `state switch` literal returns to `CssClassBuilder.Start/With` chains so the harness can extract the class strings. (Switch returning literal class strings is invisible to the harness's regex-based extraction.) Decorative connector marked with `parity-no-text-pair`. |
| **Stepper** completed-step | `text-inverse-text` → `text-white` literal (same Tooltip-class fix — fixed `bg-primary` surface needs always-white text). |

### Group 4 — Real fixes applied

| Component | Fix |
|---|---|
| **Skeleton** | `bg-[var(--color-gray-200)]` → `bg-neutral-50` for semantic alignment with Blok. Refactored to use `CssClassBuilder` and added `parity-no-text-pair` (placeholder, no text content). |

### What Phase 1 fixed
- `whitealpha` → `whiteAlpha` case mismatch bug (systemic, silently broken since port)
- Alpha-based `-bg` and `-bg-active` tokens for all colour schemes (primary, danger, warning, success, info, neutral)
- Full `info` family overrides (was missing entirely)
- `warning` hover/active states (was missing)
- `--primary-foreground` aligned with Blok (`primary-200` not `blackAlpha-900`)
- `--color-neutral-fg` aligned (`whiteAlpha-700` not `whiteAlpha-600`)
- `--color-neutral-bg-active` aligned (`whiteAlpha-300` not `whiteAlpha-200`)
- Direct colour tokens (`--color-success`, `--color-danger`, `--color-warning`, `--color-info`) now flip in dark
- `--input` and `--ring` values aligned with Blok

## Resizable (migrated 2026-04-22)

**Status:** ✅ Parity — class strings match Blok verbatim; JS-module resize engine replaces `react-resizable-panels`.

| Check | Result |
|---|---|
| Check 1 — compiled utility coverage | ✅ Clean after build |
| Check 2 — runtime-composed class detection | ✅ Clean |
| Check 3 — Blok class-string drift | ⚠️ Two deliberate additions (see below) |
| Check 4 — surface bg without text token | ✅ Clean |

**Deliberate additions vs Blok source:**

- **`ResizablePanel` adds `overflow-hidden`** — `react-resizable-panels` sets `overflow: hidden` on panels internally via JS; our port adds it as a Tailwind class because there's no library to do it. Consumers can override with `ClassName="overflow-auto"`.
- **`ResizableHandle` adds `tabindex="0"`, `role="separator"`, `aria-orientation`** — the React library adds these accessibility attributes internally. Our handle sets them as static HTML attributes for equivalent keyboard and screen-reader behaviour.

**Implementation detail (not a divergence):** The resize engine is a colocated JS module (`Resizable.razor.js`) using pointer capture events rather than `react-resizable-panels`. The consumer API (three components, same parameters, same `data-slot` values) is identical.


## Upstream primitives deliberately not ported (recorded 2026-09-02)

Both were surfaced by the `/blok audit` blanket scan against Blok `main` @ `e2651dc` as new `src/components/ui/*.tsx` files with no row in `MIGRATION_STATUS.md`. Both are recorded there as `Won't Do`. Neither is blocked by a React-only dependency in the way Chart or Command are — these are *design* decisions about what belongs in a UI kit.

### FileTree (`file-tree.tsx`, `36e50d`, added 2026-05-25) — superseded by `TreeView`

**Decision:** not ported. `Components/Extra/TreeView` stands as the library's hierarchy primitive.

Blok's `FileTree` models a file/folder browser specifically. Its node type is `{ type: "file" | "folder" }`, it hardcodes lucide `File` / `Folder` / `ChevronRight` glyphs, and the folder-expand affordance is baked into the primitive rather than parameterised.

Our `TreeView` is the deliberately more general form of the same idea:

| | Blok `FileTree` | BlazorUI `TreeView` |
|---|---|---|
| Node model | fixed `FileTreeNode` with `file` / `folder` kinds | any `TItem`, via `GetItemValue` / `GetItemText` / `GetItemChildren` accessors |
| Icons | lucide file/folder glyphs baked in | caller-supplied per node |
| Selection | single | `TreeSelectionMode.Single` or `Multiple` |
| Node content | filename text | arbitrary content, incl. badges |

The generic primitive **composes into** a file tree — pass file/folder items and file glyphs and you have Blok's component. The file-specific primitive does not compose back out into a generic tree. For a UI kit, the generic form is the more useful shape to ship, so porting `FileTree` alongside it would add a narrower, redundant primitive.

**Consequence for consumers coming from Blok:** there is no `<FileTree>` tag. Use `<TreeView TItem="...">` and supply the accessors and icons. `TreeView`'s Catalogue page carries a `<DivergenceNote>` making this explicit.

**Re-evaluate if:** Blok's `FileTree` grows behaviour that is genuinely file-specific and non-trivial to express through `TreeView`'s accessors (drag-to-move, rename-in-place, lazy directory loading).

### VirtualizedSelect (`virtualized-select.tsx`, `2f60d1`, added 2026-05-20) — Blazor virtualizes natively

**Decision:** not ported.

Blok's `VirtualizedSelect` wraps `react-window` (windowed list rendering) over `react-select` so that long option lists render only the visible rows. In React this needs a dedicated primitive because there is no framework-level virtualization.

Blazor has virtualization in the framework: `Virtualize<TItem>` ships in `Microsoft.AspNetCore.Components.Web` and renders only the items in view, with `ItemsProvider` for paged/remote sourcing. It composes directly inside our existing `SelectContent` / `ComboboxList` option lists, so a consumer with a 50,000-row option set already has the capability without a new component.

Porting `VirtualizedSelect` would mean reimplementing a framework feature as a library primitive, and would additionally fork our `Select` / `Combobox` into virtualized and non-virtualized variants that must then be kept at parity with each other.

**Consequence for consumers coming from Blok:** there is no `<VirtualizedSelect>` tag. Wrap the option list in `<Virtualize>` inside a normal `Select` or `Combobox`.

**Re-evaluate if:** `Virtualize<TItem>` proves incompatible with the popover-hosted option lists in practice — specifically if the fixed-height/scroll-container requirements clash with `ComboboxContent`'s in-place `position:fixed` rendering.


## Accordion re-audit — 2026-09-02 (Blok `537976` → `e10c8d`)

Triggered by `/blok update accordion`. Upstream commit `e10c8d` ("Heading levels should only increase by one") restructured `AccordionTrigger`; the re-audit also surfaced four pre-existing gaps the harness cannot see.

### Aligned to Blok

| Change | Before | After |
|---|---|---|
| Trigger wrapper element | `<h3 class="flex">` | `<div class="flex items-center hover:bg-blackAlpha-50 transition-colors w-full min-w-0">` — Radix `Header asChild`, matching `e10c8d` |
| Hover surface | on the `<button>` | on the row wrapper, so the highlight spans the full row including `Actions` |
| Button cross-axis alignment | `items-start` | `items-center` (pre-existing drift; Blok has always had `items-center`) |
| Button `min-w-0` | absent | present — allows the label to ellipsize instead of forcing the row wider |
| Chevron `shrink-0` | absent | present — chevron no longer squashes when the label is long |
| Chevron rotation selector | `[&[data-state=open]_svg]` (descendant) | `[&[data-state=open]>svg]` (direct child), matching Blok |
| `Actions` slot | not ported | `RenderFragment? Actions` renders beside the toggle button with `@onclick:stopPropagation` |

The descendant→direct-child selector swap also fixes a latent bug: with `_svg`, any `<svg>` a consumer put inside the trigger label would also rotate on open. Verified in-browser that `rotate: 180deg` applies to the open item's chevron only.

**Heading semantics note.** Blok's fix removes the heading element entirely — accordion triggers are no longer inside an `<h3>`. Confirmed zero `<h3>` inside `[data-slot=accordion-item]` in both our render and Blok's. This resolves the axe "heading levels should only increase by one" violation but leaves the accordion without heading landmarks. We mirror Blok. A `HeadingLevel` parameter would be a superset of both behaviours if this is ever raised as an accessibility requirement.

### Deliberate divergence — dark-mode hover

**Added `dark:hover:bg-whiteAlpha-100` to the trigger row. Blok has no dark-mode hover variant.**

Blok's row hover is `hover:bg-blackAlpha-50` only — a fixed 4%-black tint (`#0000000a`). Measured in the browser, that resolves to `rgba(0, 0, 0, 0.04)` over a `rgb(40, 40, 40)` page in dark mode: effectively invisible. Verified this reproduces on Blok's own live site — hovering an accordion row at `https://blok.sitecore.com/primitives/accordion` in dark mode produces no visible change. It is an upstream bug, not a local port error.

The fix follows the pattern already shipped in this library: `TreeView` and `TreeNode` both use `hover:bg-blackAlpha-50 dark:hover:bg-whiteAlpha-100`. Accordion was the only component using the black-alpha hover without the dark counterpart.

Harness blindness: this class of bug is invisible to all six checks. Check 5 flags fixed-shade *backgrounds* paired with flipping *text* tokens; Check 6 flags alpha-based `--color-X-N` token aliases with no `.dark` override. A direct `hover:bg-blackAlpha-*` utility with no dark variant is neither.

### Harness gap found during this audit

Check 3 (Blok class-string drift) reported **0 findings for Accordion both before and after** these changes, while five Blok utilities were genuinely missing from our markup (`items-center`, `min-w-0`, `w-full`, `shrink-0`, `transition-colors`).

Cause: `tools/verify-ui-parity.ps1` filters drift candidates through

```
-and ($_ -match '^(bg-|text-|border-|ring-|hover:|active:|focus:|dark:|rounded-|shadow-)')
```

Only colour, border and shadow utilities survive. Every **layout** utility — flex/grid alignment, sizing, spacing, `min-w-*`, `shrink-*`, `transition-*` — is discarded before comparison. Check 3 therefore cannot detect structural or layout drift from Blok, which is exactly the category `e10c8d` changed.

Not fixed in this pass — widening the filter needs a review of the false-positive volume across all 60+ components first, and would be its own change. Recorded here so "harness clean" is not read as "matches Blok".


## Calendar re-audit — 2026-09-02 (Blok `17d1fb` → `a2d44e`)

Triggered by `/blok update Calendar`. Five upstream commits in range; the substantive ones are `6d5ecd` (in-built dropdown) and the `fcdbb6` / `10eb7d` / `b82ead` / `a2d44e` accessibility sweep.

### Aligned to Blok

| Change | Detail |
|---|---|
| `AriaLabels` parameter | New `CalendarAriaLabels` record (`PreviousMonth`, `NextMonth`, `MonthDropdown`, `YearDropdown`), mirroring the subset of react-day-picker's `labels` that Blok exposes plus its `monthDropdownAriaLabel` prop. Follows the existing `DatePickerAriaLabels` convention. |
| Nav button ARIA | Prev/next now carry `aria-label`, and `aria-disabled="true"` + `tabindex="-1"` when `MinDate`/`MaxDate` leave no month to navigate to. `PreviousMonth()`/`NextMonth()` guard on the same condition, matching Blok's `if (previousMonth) onPreviousClick?.(e)`. |
| Dropdown ARIA | Month and year `<select>` overlays now carry `aria-label`. |
| Day button attributes | Added `aria-label` ("Select " + long-form date), `data-day` (`yyyy-MM-dd`), `data-selected`, `data-range-start` / `-end` / `-middle`, and `aria-pressed`. These were present in Blok at `17d1fb` already — pre-existing drift this re-audit closed. |
| `aria-selected` → `aria-pressed` | The day cell is a `<button>`, which does not support `aria-selected`. Blok's `CalendarDayButton` uses `aria-pressed`; Blok's `aria-selected` lives on the react-day-picker gridcell, which our flat div grid has no equivalent of. No CSS in this library selects on `aria-selected`, so the swap is inert visually. |
| Nav chevron colour | Added `text-neutral size-4`, matching Blok's `Chevron` component. Verified `--color-neutral` flips (`blackAlpha-500` light → `whiteAlpha-600` dark), so no dark-mode contrast issue. |
| Day border | `border border-transparent` → `border border-solid border-transparent`, matching Blok. |
| Range-middle cells | `bg-primary-bg text-foreground rounded-none` → `bg-primary-bg text-primary-fg hover:bg-primary hover:text-inverse-text rounded-none`, matching Blok. **This fixed a real gap:** middle-of-range days previously had no hover state at all. Verified in-browser that hovering a middle day now highlights it. |

### Resolved 2026-09-03 — Blok's `InBuiltDropdown` is now ported

Commit `6d5ecd` replaced react-day-picker's native `<select>` month/year dropdowns with Blok's own `Select` composite (`SelectTrigger` / `SelectContent` / `SelectItem`), exported as `InBuiltDropdown`. We now render the same composite.

**The blocker recorded in the previous revision of this section was wrong, and the correction is worth keeping.** It claimed the port was unsafe because our `Select` renders through `PopoverService` / the `<Popovers>` host while `Calendar` itself sits inside the DatePicker's popover — nesting a PopoverService popup inside a PopoverService popup, where the host is documented not to re-render on originating-component state changes. Two things were conflated:

- **The staleness rule applies to captured fragments, not to component instances.** `<Popovers>` renders the `RenderFragment` it captured, so markup the *originating* component re-renders into that fragment goes stale. `Calendar` is a component instance living in `PopoverItem`'s render tree; its own `StateHasChanged` re-renders it like any other component, wherever it sits.
- **The two popups are DOM siblings, not nested.** Both are rendered by the `Popovers` host, so the outer popup's `transform` never captures the inner popup's `position: fixed`, and the inner overlay (later in DOM at the same `z-50`) correctly takes clicks without dismissing the outer popover.

Verified in-browser across six scenarios before adopting: standalone open/select; the nested Select inside the DatePicker popover; month change while nested (grid re-renders inside the still-open outer popover); date selection after using the dropdown; dark mode nested year dropdown; and the two-month panel (four `Select` instances, per-panel offset maths — picking Dec on panel 2 gives Nov + Dec).

**Accepted cost:** `Calendar` is no longer self-contained — it now requires `AddSitecoreBlokUI()` and a root-level `<Popovers />`. `CalendarPage` carries an `<InstallationNote>` saying so, and noting that without them the grid and arrows still work while the dropdowns will not open. The Known Feature Gap entry on `Home.razor` has been removed.

Blok's `SelectContent` class string carries a `borde` typo (a non-existent utility). Not propagated, per the standing rule on Blok-side typos.

### Deliberate deviations (remaining Check 3 drift — 5 tokens reported by the harness)

| Blok token | Why not adopted |
|---|---|
| `bg-popover` | Was on Blok's `opacity-0` native select overlay. Since the `InBuiltDropdown` port the surface comes from our `SelectContent`, which sets `bg-popover text-popover-foreground` in its own file — the harness does not scan it under Calendar, so the token reads as missing here. Composition artefact, not drift. |
| `border-collapse` | Blok renders the day grid as a `<table>`; we render a flat div grid. Structural divergence already covered by the `<DivergenceNote>` on `CalendarPage`. |
| `hover:rounded-md` | Blok puts this on the day-button base. Our rounding is per-state (`rounded-md` / `rounded-l-md` / `rounded-none`), so a base `hover:rounded-md` would round range-middle cells on hover and break the continuous range bar. |
| `text-body-text` | Not counted by the harness — `$equivGroups` canonicalises it to `text-foreground`, which the file carries. Blok sets a base text token on every day button. We leave the base uncoloured and let the root's `text-card-foreground` cascade, because setting a base `text-*` alongside per-state `text-white` reintroduces the source-order race documented for Button. |
| `text-inverse-text` | Selected and range-endpoint cells use a literal `text-white`. The surface is a fixed `bg-primary-500` that does not flip, so `text-inverse-text` would resolve dark-on-blue in dark mode. Same class as the Tooltip fix; harness Check 5 enforces it. Anti-revert comment is in `Calendar.razor`. |
| `border-input` | Previously on our own month/year wrapper `<div>`s, which the `InBuiltDropdown` port removed. The token now lives in `SelectTrigger.razor`, outside the harness's Calendar scope. Composition artefact, not drift — same class of false positive as `bg-popover` above. |

`dark:bg-transparent` and `dark:hover:bg-transparent` are no longer drift: both were contingent on the `InBuiltDropdown` decision and are now genuinely present on the ported `SelectTrigger` class string.

### Extras we have that Blok does not

- **Root border and shadow.** Blok's root is `w-fit bg-card` with no border, radius or shadow. Ours adds `rounded-lg border border-border shadow-sm text-card-foreground` so a standalone `<Calendar />` reads as a finished surface rather than floating text. Verified this does not double-border inside `DatePicker`: the picker passes `ClassName="border-0 shadow-none"` and the computed border resolves to `0px`, with the popover supplying `shadow-lg rounded-lg`.
- **Fixed six-week grid.** `GetWeeksForMonth` always renders 6 rows; Blok renders only the weeks the month needs, so its calendar changes height between months. Ours keeps a stable height (equivalent to react-day-picker's `fixedWeeks`, which Blok does not set). Deliberate — avoids layout jump when navigating months, which matters most inside the DatePicker popover.

### Verified in browser (light + dark)

Nav labels, dropdown labels and 42 day buttons carry the expected ARIA; the min/max example correctly reports `aria-disabled="true"` / `tabindex="-1"` on the previous-month button with 3 disabled days. Dark mode: calendar surface `rgb(40,40,40)`, range-middle `bg rgba(217,212,255,0.12)` with `text-primary-fg` `rgb(217,212,255)`, range endpoints `rgb(110,63,255)` with literal white, nav chevrons `rgba(255,255,255,0.68)`. All legible; no dark-mode contrast failures.


## ToggleGroup / Toggle — `Square` and `Rounded` variants added 2026-09-03

Closes the single real gap found in the Check 3 triage below. `ToggleGroup` reported `rounded-full` as missing because Blok has a `rounded` variant we never ported.

Reading the source turned up more than the one token. Blok's `toggleVariants` has **four** shapes — `default`, `outline`, `square`, `rounded` — where we had two:

| | Blok | Ours (before) |
|---|---|---|
| Variants | `default`, `outline`, `square`, `rounded` | `Default`, `Outline` |
| Sizes | `default` `h-10 px-4`, `sm` `h-8 px-3`, `xs` `h-6 px-2` | `Default` `h-9 px-2`, `Sm` `h-8 px-1.5`, `Lg` `h-11 px-3` |

Check 3 only ever saw `rounded-full` because the size utilities are layout tokens and its filter discards those — the same blind spot recorded for the Accordion re-audit.

### What was done

`ToggleVariant` gains `Square` and `Rounded`. `Square` renders identically to `Default` (both `rounded-md`), exactly as in Blok, and exists so code written against Blok ports across unchanged. `Rounded` is `rounded-full` on both the group wrapper and its items.

Rounding moved out of the base class string and onto the variant arms in `Toggle`, `ToggleGroupItem` and `ToggleGroup`. It had to: `CssClassBuilder` concatenates without resolving Tailwind conflicts, so a base `rounded-md` sitting alongside `Rounded`'s `rounded-full` would have been settled by CSS rule order rather than intent. Verified in browser — rounded group and items compute `9999px`, default groups stay at `6px`.

### What was deliberately not done

- **The size scale is left alone.** Aligning it would rename `Lg` out of existence and change the default height from `h-9` to `h-10` — a breaking change for consumers, and not something to fold into a variant addition. Recorded on `TogglePage`'s `<DivergenceNote>` as an open decision.
- **Blok coerces `outline` to `default` inside a group**; we do not. Adopting that would remove the only way to get a bordered toggle group and would silently change behaviour for existing consumers. Documented on `ToggleGroupPage`.

### Note on a false alarm

While verifying, `getComputedStyle` reported `rgba(0, 0, 0, 0)` for the background of a pressed `Toggle`, which looked like the pressed state failing to render. It is not: a synthetic `<button>` carrying the identical class list computed the expected tint, and the screenshot shows the pressed `Rounded` toggle with its background clearly drawn. Treated as a measurement artefact — the same pattern that produced two earlier false alarms today when computed styles were sampled too soon after a state or theme change. Worth remembering: read the pixels before believing `getComputedStyle` on a just-changed element.

Harness: ToggleGroup PASS clean. Toggle still reports its 5 pre-existing findings (`text-md`, `text-neutral-fg`, `hover:text-neutral-fg`, `dark:aria-invalid:ring-destructive/40`, `bg-backgrounds`), unchanged by this work and part of the same size/token scale divergence above.

## Check 3 triage — 2026-09-03

Two harness defects were fixed today, and the newly-visible findings triaged. Both defects made the harness report *less* than reality, so nothing here is a regression in the library — it is coverage that never existed.

### Defect 2 — a comment with parentheses blinds a whole file

The `CssClassBuilder` call pattern is `\b(?:Start|With|Reset)\s*\(((?:[^()"]|"[^"]*")*)\)`. The argument block **cannot contain a bare `(`**, so a `//` comment with parentheses inside a builder chain makes the entire `Start(...)` call fail to match, and every class string in that file is dropped — reported as drift against Blok.

Self-inflicted and caught in triage: the comment added to `InputGroup.razor` earlier today ("…addon (which is w-full) stays on the same row…") blinded that file completely, producing 5 phantom findings. Fixed by stripping `//` comments (only outside string literals) before parsing. InputGroup went 5 → 0, and the library total 71 → 65.

This is the more dangerous of the two defects: it fails **silently and retroactively**, so a well-meaning explanatory comment can quietly switch off drift detection for a component.

### Disposition of the newly-visible components

| Component | Findings | Disposition |
|---|---|---|
| **InputGroup** | ~~5~~ 0 | Phantom, caused by Defect 2. Resolved. |
| **DatePicker** | 5 — `border-input`, `border-1`, `rounded-md`, `text-md`, `dark:aria-invalid:ring-destructive/40` | **Composition artefact.** Blok's trigger carries the full input-style class string inline; ours delegates to the `Button` component, which supplies border, radius and invalid states from its own file. Tokens are in the rendered DOM, just not in `DatePicker.razor`. |
| **TimePicker** | 5 — `border-1`, `text-md`, `dark:aria-invalid:ring-destructive/40`, `dark:bg-input/30`, `dark:hover:bg-input/50` | **Deliberate, already documented.** We render a native `<input type="time">`; Blok renders a Popover with three Selects. The two share no DOM structure, so token-level comparison is meaningless here — see the TimePicker section below. |
| **ScrollArea** | 5 — `border-l`, `border-l-transparent`, `border-t`, `border-t-transparent`, `bg-border` | **Structural divergence.** Those style Radix's rendered scrollbar/thumb elements. We use the native scrollbar with the library's canonical thin-scrollbar utilities instead of rendering scrollbar DOM, so there is nothing to put them on. |
| **ContextMenu** | 2 — `focus:bg-accent`, `focus:text-accent-foreground` | **Deliberate token choice.** Our menu items use `hover:bg-neutral-bg` / `focus:text-foreground`, consistent with `DropdownMenuItem`. Changing it would make context menus inconsistent with dropdown menus for the sake of the diff. |
| **SearchInput** | 1 — `focus:border-0` | **Composition artefact.** Blok wraps `Input`, so it must suppress that component's own border; we render a bare `<input>` with no border to suppress. Same shape as `InputGroupTextarea`. |
| **StackNavigation** | 1 — `shadow-none` | **False positive on both sides.** In Blok it is `className?.includes("shadow-none")` — a sentinel string in logic, not a rendered class. Ours does the same (`ClassName?.Contains( "shadow-none" )`). Behaviour matches; the harness is reading a string comparison as a utility. |
| **ToggleGroup** | 1 — `rounded-full` | **Real gap.** Blok has a `rounded` variant (`resolvedVariant === "rounded" ? "rounded-full" : "rounded-md"`); we only ever emit `rounded-md`. The one finding in this batch that is a genuine missing feature. Not yet implemented. |

### What this leaves

Of the 19 findings across the eight newly-compared components, **one is a real gap** (ToggleGroup's `rounded` variant). The rest are composition artefacts, deliberate choices already recorded, or the harness misreading sentinel strings.

That ratio is itself the useful result: Check 3 compares token *sets* between files, so any component that delegates styling to a child component, or diverges structurally, will always report the delegated tokens as missing. The check is a regression guard on class strings within a file — not evidence of parity, and not a to-do list.

### Suggested harness follow-ups

Not done, listed so they are not lost:

- Treat string literals that feed a `data-*` attribute as values, not class names — this is the source of the `icon-xs` / `inline-start` / `block-end` entries in Check 1.
- Consider resolving tokens across the whole rendered component family rather than per Blok-file name, which would remove the composition-artefact class of false positive entirely.
- Re-baseline Check 4's 27 findings; they have never been triaged and the header claimed the check was clean.

## Harness fix — Check 3 was silently skipping hyphenated Blok sources

Found while porting InputOtp, and it invalidates some earlier "harness clean" claims in this document.

`verify-ui-parity.ps1` resolves a Razor file to its Blok source by lowercasing the component name and, on a 404, stripping trailing PascalCase words until something resolves. Blok file names are **kebab-case**, so:

- `InputOtp` → `inputotp.tsx` (404) → strip `Otp` → **`input.tsx` (200)** — diffed against the wrong component.
- `StackNavigation` → `stacknavigation.tsx` (404) → strip `Navigation` → `stack.tsx` (404) → repeats → `$blokSrc` stays null → `continue`. **Check 3 never ran for it at all**, and reported clean by omission.

Fixed by trying the kebab-case form of the full name first (`input-otp.tsx`, `stack-navigation.tsx`) before the word-stripping fallback, which is still needed for genuine sub-components (`AccordionItem` → `accordion`). The related-file aggregation also had to learn that `$baseName` may now contain hyphens while our file names never do.

### Measured effect

| | Before | After |
|---|---|---|
| Distinct Blok sources compared | 16 | 24 |
| Check 3 drift findings | 46 | 71 |

Eight components were being compared for the first time: `context-menu`, `date-picker`, `input-group`, `scroll-area`, `search-input`, `stack-navigation`, `time-picker`, `toggle-group`.

### Corrections to earlier entries in this document

The Check 3 results recorded today for **DatePicker** ("PASS, 0 findings on all six checks"), **StackNavigation** and **TimePicker** ("PASS, 0 drift findings") were **vacuous for Check 3** — their sources were never fetched. Checks 1, 2, 4, 5 and 6 for those components are unaffected and still stand. Re-running now: DatePicker 5 findings, TimePicker 5, StackNavigation 1, ContextMenu 2, ScrollArea 5, SearchInput 1, ToggleGroup 1, InputGroup 11.

None of these have been triaged. They are a new backlog, and some will be the usual composition artefacts and deliberate deviations rather than real drift — the TimePicker note in this document already explains why that component shares no structure with its Blok source at all.

Checks 1 (41 findings) and 4 (27) are untouched by this fix; those counts are library-wide and pre-date it, so the "Checks 1, 2, 4, 5, 6 clean" claim at the top of this document is stale and should be re-baselined.

## InputOtp — ported from Backlog 2026-09-03 (Blok `a02fe3`)

Blok wraps the React package `input-otp`, which renders one real `<input>` stretched invisibly across the slots and publishes a per-slot context. The same shape is rebuilt here: `InputOtp` owns a single transparent, focusable input and cascades itself; `InputOtpSlot` reads its character and active state back off it. Keeping one real input is what makes typing, backspace, paste, autofill and `one-time-code` autocomplete work natively instead of being reimplemented per slot.

Four components, matching Blok's four exports. Named `InputOtp` rather than `InputOTP` — .NET casing for three-letter acronyms, and it matches the existing `MIGRATION_STATUS.md` row; count, roles and markup are unchanged.

### The maxlength trap

The first implementation set `maxlength` on the hidden input and filtered rejected characters in the input handler. That is broken, and the browser test caught it: typing `9x8y77` into a digits-only control produced `98`.

`maxlength` counts the **raw** text. A character rejected by `Pattern` still lands in the element, still consumes the length budget, and leaves the element's text running ahead of the bound value — which is also what the caret position is measured against. Two changes fix it:

- `maxlength` is no longer set on the element; the limit is enforced in `OnInput`.
- A colocated `InputOtp.razor.js` pushes the accepted value back into the element whenever the two diverge, restoring the caret to the end. Blazor will not do this itself, because when a character is rejected the bound value never changed, so there is nothing for it to re-render.

### Verified in browser

Per-keystroke on a `^[0-9]$` control: `x` rejected with the raw text resynced to `9`; `y` rejected; digits accumulate to `9876`; a further `5` past `MaxLength` refused; `OnComplete` fired with `9876`. Pasting `A1B2C3D4` into a six-slot control truncates to `A1B2C3` and resyncs the raw text. The active slot shows the ring and the blinking caret, and a full value shows none. Dark mode: white characters on `rgba(255,255,255,0.55)` borders.

Harness: `-Component InputOtp` PASS, 0 findings on all six checks — and, with the resolution fix above, that is now measured against the correct `input-otp.tsx`.

## DropdownMenu — missing exports ported 2026-09-03 (Blok `7de47e` → `82a49e`)

Clears the last `Partial` row. Six Blok exports were missing; all six are now ported, and the row's upstream drift (`82a49e`, `aria-haspopup` on the trigger and sub-trigger) is closed in the same pass.

| Component | Notes |
|---|---|
| `DropdownMenuItemText` / `ItemTitle` / `ItemDescription` | Straight ports of Blok's three presentational divs; `ItemText` stacks the other two into a two-line item body. `text-subtle-text` was already defined in `colors.css` with a `.dark` override, so no token work was needed. |
| `DropdownMenuCheckboxItem` | `Checked` / `CheckedChanged` two-way binding, `role="menuitemcheckbox"`, `aria-checked` and `data-state`. Indicator is a `Check` icon in the same absolutely-positioned `left-2` span Blok uses. |
| `DropdownMenuRadioGroup` | Blok's is a bare passthrough because Radix's `RadioGroup` owns the selected value. Blazor has no equivalent, so ours holds `Value` / `ValueChanged` and cascades itself — the same pattern used across this library. |
| `DropdownMenuRadioItem` | Reads the parent group to decide its checked state, `role="menuitemradio"`, dot indicator. |

`DropdownMenuPortal` is **deliberately not ported**: it is Radix plumbing for escaping overflow containers, and our menu content already renders through `PopoverService` at the layout root, which is the same escape by a different route. Recorded on the row rather than left as an apparent gap.

Both new interactive items close the whole ancestor chain on select — sub, then root dropdown, then context menu — reusing `DropdownMenuItem`'s existing cascade set. That matches Radix's default, which Blok does not override with `preventDefault`.

### Verified in browser

Checkbox items report `role="menuitemcheckbox"` with correct `aria-checked` / `data-state`, the disabled one carries `data-disabled`, toggling updates the bound field (`Sidebar: True · Toolbar: True`) and dismisses the menu. Radio items report `role="menuitemradio"`, selecting `Size` updates the binding and, on reopening, only `Size` is checked and carries the dot. The two-line item stacks in a column with the description at `rgba(0,0,0,0.55)` in light and `rgba(255,255,255,0.68)` in dark — the `--color-subtle-text` override doing its job — on a `rgb(40,40,40)` surface.

Harness: `-Component DropdownMenu` PASS, **0 findings on all six checks**.

## InputGroup — missing exports ported 2026-09-03 (Blok `37c0d3` → `589c0c`)

Closes the `Partial` status recorded earlier in the day. `InputGroupButton` and `InputGroupTextarea` were the two Blok exports we lacked, and the row's upstream drift (`589c0c`, an `aria-label` passthrough) sat on the former, so the drift could not be closed without the port.

### InputGroupButton

Wraps `Button`, as Blok does, with its own pill size scale — `Xs`, `Sm`, `IconXs`, `IconSm` (new `InputGroupButtonSize` enum) — emitted as `data-size` and as class strings copied from `inputGroupButtonVariants`.

Wrapping `Button` is safe here for a specific reason worth recording: **`Button` gates every one of its own size utilities on `CssClassBuilder.ContainsAny( ClassName, ... )`**, so the `h-`/`px-`/`rounded-`/`size-` utilities passed down suppress Button's built-ins rather than colliding with them. `CssClassBuilder` is plain concatenation with no twMerge equivalent, so without that gate the override would be decided by Tailwind's emitted rule order. Verified in browser: xs 24px high with 8px padding, sm 32px with 10px, icon-xs 24×24, icon-sm 32×32, all `rounded-full` — matching Blok's variant table exactly.

`AriaLabel` is a first-class parameter rather than a splatted attribute, and the component carries an anti-trap comment: **do not pass `title` through to it.** `Button.Title` is the button's visible text, so a splatted `title` renders as content — the same trap that bit `SidebarRail` earlier today. This is now the second component to hit it; `Button.Title` shadowing the HTML `title` attribute is worth revisiting as an API question.

### InputGroupTextarea

Renders a bare `<textarea>` rather than wrapping our `Textarea`, matching how the sibling `InputGroupInput` renders a bare `<input>` instead of wrapping `Input`. Blok wraps `Textarea` and strips its chrome with `border-0` / `rounded-none` / `bg-transparent`, which works there because `cn()` is twMerge and physically removes the conflicting base utilities. Ours would keep both `border` and `border-0` on the element. Owning the full class string keeps it deterministic.

One consequence caught in the browser: Blok's version inherits `w-full` from the `Textarea` it wraps, so rendering bare dropped it and the control collapsed to the intrinsic `cols=20` width (168px). `w-full` added explicitly.

### Pre-existing bug this port exposed

The multi-line example rendered as a squashed strip because **`InputGroup` was missing four of Blok's alignment variant groups**:

```
has-[>[data-align=inline-start]]:[&>input]:pl-2
has-[>[data-align=inline-end]]:[&>input]:pr-2
has-[>[data-align=block-start]]:h-auto  ...:flex-col  ...:[&>input]:pb-3
has-[>[data-align=block-end]]:h-auto    ...:flex-col  ...:[&>input]:pt-3
```

The `flex-col` pair is load-bearing: a block-start/block-end addon is `w-full`, so without it the addon stays on the control's row and squeezes it to nothing. Also added Blok's `has-[button:focus-visible]` focus ring and the `[.border-t]` / `group-has-[>input]` padding refinements on the addon.

**Check 3 could not have caught this.** Every one of those tokens ends in a layout utility (`flex-col`, `h-auto`, `pl-2`), and the harness filters drift candidates down to colour, border and shadow utilities — the blind spot already documented for the Accordion re-audit. It took rendering a block-end addon for the first time to surface it.

### Harness

`-Component InputGroup` reports 6 Check 1 findings and 2 Check 3. Measured against a baseline taken with the two new files moved aside, **the port adds exactly two Check 1 entries and no drift**: `icon-xs` and `icon-sm`, which are `data-size` attribute *values* the harness mistakes for class names — the same false positive as the pre-existing `inline-start` / `inline-end` / `block-start` / `block-end` on `InputGroupAddon`, which are `data-align` values. The 2 Check 3 findings (`text-md`, `focus:ring-1`) are pre-existing and attributed to `input.tsx`.

Worth a future fix in the harness: string literals in `switch` arms feeding a `data-*` attribute should not be treated as class names.

## Select — broken merge resolution repaired 2026-09-03

Merge `0224a81` (`fix/selectfield-collapsed-label` into `fix/select-label-and-popover-teardown`) left `Select.razor` not compiling: `GetDisplayLabel` referenced `SelectedLabel`, which the merge had removed.

The two branches had solved the same problem — showing the selected label before the dropdown is first opened — in different ways:

| Branch | Approach |
|---|---|
| `fix/selectfield-collapsed-label` (`f15e889`) | `internal string? SelectedLabel`, a single "last selected label" field set in `Register` / `SetValue`, plus a `DisplayLabel` parameter |
| `fix/select-label-and-popover-teardown` (`02bd0e6`) | `ValueLabel` parameter plus `LabelCache`, a dictionary keyed by value |

The merge kept this branch's storage but the other branch's method body, leaving the dangling reference. Resolved by preserving both intents: `DisplayLabel` stays the highest-priority parameter, then the value-keyed cache, then `ValueLabel`, then a registered item's label, then the raw value. The cache is the right primitive for the middle step — being keyed by value it cannot go stale when `Value` changes externally without a click, which is precisely the hazard the other branch's own comment described.

**Follow-up worth a decision:** `DisplayLabel` and `ValueLabel` are now two parameters covering nearly the same ground. They should probably be one, but collapsing them is an API change on a shipped component, so it is left alone here.

## Accessibility sweep re-audits — 2026-09-03

Blok ran a large accessibility pass across `main` (commits `fcdbb6`, `565e2b`, `589c0c`, `e417e5`, `bc7553`, `10eb7d`, `82a49e`, `b82ead`, `99b9ef`). It changed **no class strings** in any component below — it is entirely ARIA attributes and, in two cases, element choice. Check 3 is therefore blind to all of it, which is why these rows sat drifted while the harness reported clean.

| Component | SHA | Adopted |
|---|---|---|
| Checkbox | `2d994e` → `589c0c` | Blok added an explicit `aria-label` prop, separate from any visible label. Ours already put `aria-label` on the control but sourced it from `Label`, which is *also* the visible text — so the accessible name could not be set independently. Added an `AriaLabel` parameter falling back to `Label`, leaving existing usage unchanged. |
| Combobox | `4a6b44` → `4a6b17` | `TriggerAriaLabel` (default "Open listbox") and `ClearAriaLabel` (default "Clear selection") on `ComboboxInput`; `RemoveButtonAriaLabel` (default "Remove") plus `role="button"` on the chip remove control; `aria-hidden="true"` on the decorative chevron and close icons. `ComboboxClear` is internal to Blok's `ComboboxInput`, not an export, so our file count stays correct. |
| NavigationMenu | `2d994e` → `b82ead` | The root `<nav>` is a landmark, so two menus on a page need distinct accessible names. Default `aria-label` of "Navigation menu" / "Inline navigation menu" depending on `Viewport`, placed **before** the `@attributes` splat so a consumer-supplied `aria-label` still overrides it — that is Blazor's precedence rule and it reproduces Blok's `ariaLabel ?? default`. `aria-haspopup="true"` on the trigger. |
| Sidebar | `3c1b7d` → `68c4af` | Rail class string aligned to Blok — `h-full! min-h-0! min-w-0! rounded-none px-0 hover:bg-transparent` added, closing the `rounded-none` Check 3 finding. **Element deliberately NOT changed** — see below. |

### Sidebar rail — why it stays a plain `<button>`

Blok `68c4af` (with `234cfa`) routes `SidebarRail` through the Blok `Button` with `variant="ghost"`, and the six leading utilities above exist purely to neutralise that Button's sizing, radius, padding and hover fill.

Ported literally, this breaks. **Our `Button` has a `Title` parameter that is the button's visible text, not the HTML `title` attribute.** The rail needs `title="Toggle Sidebar"` for its native tooltip; passed to `<Button>`, that attribute binds to the parameter instead of reaching the DOM and renders "Toggle Sidebar" as visible content spilling out of the 16px rail. Verified in-browser before reverting: `scrollWidth` 55 against `clientWidth` 16, with the text visible beside the sidebar.

The rail therefore stays a bare `<button>` and keeps the class string for parity — the neutralising utilities are harmless no-ops without Button's base styles. An anti-revert comment in `SidebarRail.razor` records the reason, because routing it through `Button` looks like an obvious tidy-up.

Worth noting how this was nearly missed: **`SidebarRail` had no Catalogue coverage at all.** It was documented in the API table but rendered by no example, so the regression was invisible to both the harness and the page. A `<SidebarRail />` has been added to the "Leading icons" example (live markup and its `Code` string), and the rail is now confirmed to toggle the sidebar `expanded` → `collapsed`.

### Still outstanding

- **DropdownMenu** and **InputGroup** — blocked on the missing exports recorded in `MIGRATION_STATUS.md`; both rows are now `Partial`.

### StackNavigation (`7c9f7e` → `82a49e`) — adopted 2026-09-03, following Blok exactly

Three changes, all structural rather than visual (Check 3 stayed clean throughout):

1. **Root `<aside>` → `<div>`** (`99b9ef`). `<aside>` is a landmark and the rail is routinely placed inside another landmark, which is a violation.
2. **The list container is a `<nav>` only when it can be named.** New `AriaLabel` parameter: set, the container renders as `<nav aria-label="…">`; unset, a plain `<div>`. An unnamed nav landmark — or several — is the finding the audit raised. The item markup is shared between both branches via a `RenderItems` fragment so it exists once.
3. **Items with `OnItemClick` render as `<button>`, not `<a>`**, with `aria-haspopup="true"` and `aria-expanded` bound to the active state. Blok's reasoning is that such items are expandable controls, not links.

**The trade-off was put to the user and Blok's rule was chosen deliberately.** Our API allows an item to carry both a `Path` and an `OnItemClick`; under Blok's rule those are no longer anchors, so middle-click and ctrl-click no longer open them in a new tab. Items without `OnItemClick` remain real links. The `<DivergenceNote>` on `StackNavigationPage` has been rewritten to state this consequence.

A welcome side effect: the old `data-enhance-nav="false"` workaround is gone. It existed because Blazor's enhanced navigation fetched the next page before `@onclick:preventDefault` could run. A `<button>` has no browser default to prevent, so the whole class of problem disappears — `HandleItemClick` still navigates via `NavigationManager` when the consumer leaves `PreventDefault` false.

Verified in browser: 6 button items (`aria-haspopup="true"`, `aria-expanded`, no `href`) alongside 27 anchors on the same page; no `<aside>` from this component; clicking a button item updates both the active path and the last-clicked label; layout unchanged.

## Popover re-audit — 2026-09-03 (Blok `2d994e` → `4f751c`)

Two commits touched `popover.tsx` in the range — `c8ef1c` ("Fix ARIA labels in popover") and `4f751c` ("fixed the popover forcemount issue") — but **`git diff 2d994e..HEAD -- src/components/ui/popover.tsx` is empty**: the changes were applied and then reverted upstream. The file's content is byte-identical to the revision this row was last audited against, so the row is bumped to the current last-touched SHA with no code change on our side.

The one substantive thing this pass added is unrelated to upstream drift: `Popover` gained optional `Role` / `AriaLabel` passthrough parameters so `DatePicker` could give its popup an accessible dialog name (see the DatePicker section below). Both default to null and no existing consumer is affected.

### Standing Check 3 findings — composition artefacts, not drift

`pwsh ./tools/verify-ui-parity.ps1 -Component Popover` reports 4 findings: `bg-popover`, `text-popover-foreground`, `rounded-md`, `shadow-md`. Blok puts these utilities directly on `PopoverContent`. Ours is a **headless positioning primitive** — `PopoverItem` renders only position, z-index and the popper CSS variables, and every consumer supplies the surface through `ClassName`:

- `SelectContent` → `bg-popover text-popover-foreground … rounded-md border shadow-md`
- `DatePicker` → `shadow-lg rounded-lg`

The tokens exist in the rendered DOM; they just live in the consumer's file, which the harness does not scan under Popover. Same class of false positive as Calendar's `border-input`. Not fixed — moving the surface onto the shared container would force every consumer to the same look and break `DatePicker`'s larger radius and shadow.

## Tooltip re-audit — 2026-09-03 (Blok `2d994e` → `b79ded`)

One commit: `b79ded` ("Remove tooltip provider from the tooltip"). Blok's `Tooltip` no longer wraps itself in `TooltipProvider`; the Provider is still exported and Blok's own install docs now instruct consumers to mount it once in the root layout.

No change on our side, and the row is bumped. Worth recording precisely, because it is easy to misread as upstream converging on our shape: it did not. We implement **no Provider at all** — hover is CSS `group-hover` and the delay is per-tooltip on `TooltipContent` — so the divergence still stands, it has just changed character. Before `b79ded` Blok's Provider was implicit and per-tooltip; now it is an explicit consumer install step that we do not have. The `<DivergenceNote>` on `TooltipPage` has been reworded to say this rather than implying the Provider simply went away.

No class strings changed in the range. Harness unchanged: the only Check 3 finding remains the deliberate `text-inverse-text` → `text-white` swap documented above.

## DatePicker re-audit — 2026-09-03 (Blok `6cb4ad` → `c4346e`)

Found while verifying the Calendar `InBuiltDropdown` port; confirmed pre-existing by reproducing it against unmodified `d165b64` code, so it is not a regression from that port.

`<DatePicker @bind-Value="..." />` behaved as a **range** picker: clicking a day set `RangeStart` instead of `Value`, the trigger rendered `"10 Sept 2026 — ..."`, and the popover never closed. The Catalogue's own "Default" example demonstrated it.

Cause: `DatePicker` always wires `RangeStartChanged` / `RangeEndChanged` when it renders `Calendar`, and `Calendar.IsRange` falls back to `RangeStartChanged.HasDelegate || RangeEndChanged.HasDelegate` whenever its `Range` parameter is null. `DatePicker` passed its own raw `Range` parameter (null unless the consumer set it) rather than its resolved `IsRange`, so the fallback made every DatePicker a range picker.

Fix: pass `Range="@IsRange"` — the already-correct resolved value — with an anti-revert comment, since `Range="@Range"` looks like the obvious form and reintroduces the bug.

### Upstream changes in the range

Four commits touch `date-picker.tsx`; **no class string changed anywhere in the range**. The substance is three edits:

| Commit | Change | Disposition |
|---|---|---|
| `6d5ecd` | `CustomDropdown` export deleted and `components={{ Dropdown: CustomDropdown }}` dropped — the month/year dropdown moved into `calendar.tsx` as `InBuiltDropdown`. | Already resolved. Our `Calendar` port put the `Select` composite in the same place, so `DatePicker` correctly owns none of it. |
| `149679` | Trigger `aria-label` becomes undefined once a date (or `range.from`) is displayed, so the visible formatted date is the accessible name rather than being overridden by the placeholder. The `DatePickerAriaLabels.popoverTrigger` doc comment was rewritten to say so. | Adopted. `TriggerAriaLabel` now returns null when `ShowsDate`; the XML doc on `DatePickerAriaLabels.PopoverTrigger` mirrors Blok's wording. Blazor omits null attributes entirely, so the attribute is absent rather than empty. |
| `c4346e` | `PopoverContent` gains `aria-label="Choose date"` — axe flagged dialog nodes without an accessible name. | Adopted, with a Blazor-side prerequisite (below). |

### Popover needed a role before the name could mean anything

Blok's `PopoverContent` is a Radix dialog and already carries `role="dialog"`, so upstream only had to add the name. Our popup container had **no role at all** — `aria-label` on a role-less element is ignored by assistive technology, so copying just the label would have produced dead markup that looks correct in a diff.

`Popover` therefore gains two optional passthrough parameters, `Role` and `AriaLabel`, forwarded by `PopoverItem` onto the popup container. Both default to null, so every existing consumer is unaffected — importantly `SelectContent`, which must stay role-less on the container because it renders its own `role="listbox"` inside. `DatePicker` sets `Role="dialog" AriaLabel="Choose date"`. Documented on the Popover Catalogue page.

### Verified in browser

Empty triggers expose `aria-label` from the placeholder ("Pick a date", "Choose a date...", "Pick a date range"). After selecting 15 Sept the single-date trigger reads `15 Sept 2026` with the `aria-label` attribute **absent**, and the popup closes. The range trigger after picking a start date reads `8 Sept 2026 — ...`, `aria-label` absent, popup still open awaiting the end date. While open the popup container reports `role="dialog"`, `aria-label="Choose date"`.

Harness: DatePicker PASS, 0 findings on all six checks.

### Not addressed — pre-existing Popover drift

`pwsh ./tools/verify-ui-parity.ps1 -Component Popover` reports 4 Check 3 findings (`bg-popover`, `text-popover-foreground`, `rounded-md`, `shadow-md`). These pre-date this work and are a composition artefact: our `Popover` is a headless positioning primitive and each consumer supplies the surface through `ClassName` (`SelectContent` passes `bg-popover text-popover-foreground … rounded-md border shadow-md`; `DatePicker` passes `shadow-lg rounded-lg`). Blok puts those utilities directly on `PopoverContent`. Left alone — the Popover row's own re-audit is still outstanding, though its upstream diff `2d994e..HEAD` is empty (see the audit backlog).

## Editable re-audit — 2026-09-02 (Blok `17d1fb` → `c631ca`)

Triggered by `/blok update Editable`. Three upstream commits: `cde96b` / `7233c8` (add + refine the error surface) and `c631ca` (empty-state styling).

### Aligned to Blok

| Change | Detail |
|---|---|
| **`EditableError` ported** | New 10th export. `<div role="alert" aria-live="polite" data-slot="editable-error">`, absolutely positioned under the field. Renders nothing when it has neither `Errors` nor `ChildContent`; one message renders as a `<span>`, several as a `<ul class="list-disc list-inside space-y-1">`. Restores export parity — Blok exports 10, we had 9. |
| **`HasError` parameter** | On both `Editable` and `EditableRootProvider`. Mirrors Blok exactly: initial edit state becomes `StartWithEditView \|\| HasError`, and both cancel and submit set the edit state to `HasError` rather than `false`, so an invalid value keeps the field open for correction. |
| Root wrapper | `inline-flex flex-col gap-1` → `+ relative`. Load-bearing: it is the containing block for the absolutely-positioned error. |
| `EditablePreview` | `min-h-[2rem]` → `min-h-8`. Empty state no longer uses `text-muted-foreground italic` — Blok now sets `text-foreground` for both empty and filled, so the token is unconditional. |
| `EditableInput` | `h-10` → `h-8`, matching the `h-8` Blok added to its input class. Verified rendering at 32px. |

**Paradigm translation (not a divergence):** Blok's `EditableError` takes `errors?: { message?: string }[]`, shaped for react-hook-form. Ours takes `IEnumerable<string>?`, which is what Blazor's `EditContext.GetValidationMessages()` already returns, so it drops straight in. Same behavioural surface.

### Deliberate divergence — error surface token

**`bg-white` → `bg-popover`.** Blok's `EditableError` hardcodes `bg-white` alongside `text-destructive`.

`--destructive` flips between modes in this library: `--color-danger-500` (mid red) in light, `--color-danger-200` (pale red) in dark. On a permanently-white surface that yields pale-red-on-white in dark mode — the fixed-shade-bg + flipping-text-token failure that harness Check 5 exists to catch, and the same class as the Tooltip `text-inverse-text` fix. A white box would also read as wrong against the dark page.

`bg-popover` is the library's semantic floating-surface token and is what Tooltip and DropdownMenu already use. Verified in the browser: dark mode renders `rgb(40, 40, 40)` with `rgb(255, 204, 200)` text — legible. Check 4 is satisfied because a `text-*` token is present in the same string; Check 5 does not fire because the background is no longer fixed-shade.

I could not pixel-compare against Blok's own error box: their live "Editable with Error" demo shows the red input border but did not surface the message element during the pass, so the reasoning above rests on the source class string and our token definitions rather than a side-by-side capture.

### Remaining Check 3 drift (2 tokens)

| Blok token | Why not adopted |
|---|---|
| `hover:bg-transparent` | Pre-existing and previously documented — we conditionally omit `hover:bg-neutral-bg` when `IsPreviewFocusable=false` instead, which reaches the same visual result. |
| `bg-white` | The deliberate divergence above. |

### Verified in browser (light + dark)

Error element carries `role="alert"` / `aria-live="polite"`; root computes `position: relative` and the error `position: absolute` with `bottom: -30px`, confirming Blok's `bottom-[calc(-100%+var(--spacing)*0.5)]` compiles (Tailwind normalises the missing whitespace around `+` into valid `calc(-100% + var(--spacing) * .5)`). Full `HasError` cycle exercised: empty value starts in edit mode with the message shown; entering a value and blurring clears the error, leaves edit mode, and the preview shows the new value. Input renders at 32px and preview `min-height` at 32px in both themes.


## TimePicker re-audit — 2026-09-02 (Blok `53ab50` → `931987`)

Triggered by `/blok update timepicker`. Four upstream commits (`7f64d5`, `c4346e`, `d2f740`, merge `931987`), all accessibility.

### What the upstream change actually was

The audit sweep flagged TimePicker as the highest visual-drift risk on a count of 14 changed `className` lines. That count was misleading: every one of those lines moved only because Biome re-wrapped the JSX when new attributes were added. **No class string changed.** The real diff is:

- `aria-label="Choose time"` on the `PopoverContent`
- `htmlFor` on each `<label>`, paired with a new `id` on the matching `SelectTrigger` (`time-picker-hour` / `-minute` / `-period`)
- `aria-label` of `Hour` / `Minute` / `Period` on each `SelectTrigger`

### Why almost none of it is applicable

Our TimePicker is **not a port of Blok's component**. Blok composes a `Popover` → three `Select`s (Hour 1-12, Minute 00-59, Period AM/PM) with `<label>`s, a `:` separator, and Clear / Done buttons, behind a trigger `Button` that renders the formatted time and a clock icon. Ours is a single native `<input type="time">` in a wrapper `<div>` — 30 lines against Blok's ~200.

There is no popover, no `Select`, and no `<label>` in our render tree, so there is nothing for the new `htmlFor`/`id` pairs or the per-select `aria-label`s to attach to.

**Applied:** the one transferable piece — a default accessible name. The input now carries `aria-label`, defaulting to `"Choose time"` (matching Blok's new `PopoverContent` label) via a new `AriaLabel` parameter. Before this the control had no accessible name at all. A `name` attribute can still be supplied through `AdditionalAttributes`; no default is set because an invented one would leak into form posts.

### Two records corrected

Both pre-existing and both wrong:

1. **The `<DivergenceNote>` on `TimePickerPage` described only the value-type difference** and asserted that the component formats 12-hour output "via a `Format` string". There is no `Format` parameter — the component's full parameter set is `Value`, `ValueChanged`, `Disabled`, `ClassName`, `AdditionalAttributes` (now plus `AriaLabel`). Rewritten to lead with the structural divergence and to state that display formatting follows the browser locale.
2. **This document's status row read "Uses Input/Select internally; inherits."** It uses neither — it is a bare native input. Row corrected and Check 3 downgraded from ✅ to ❌ differs.

### Harness blind spot — this is the clearest case so far

`pwsh ./tools/verify-ui-parity.ps1 -Component TimePicker` reports **PASS, 0 drift findings**, for a component that shares no DOM structure, no sub-components, and no interaction model with its Blok source.

Two filters combine to produce the false clean:

1. Check 3 only reports tokens matching `^(bg-|text-|border-|ring-|hover:|active:|focus:|dark:|rounded-|shadow-)`. Blok's distinguishing classes here are layout and sizing (`w-[70px]`, `flex flex-col gap-2`, `p-4`, `text-xs`, `font-medium`, `mt-6`) — all discarded before comparison.
2. Of the colour/border tokens that do survive, our single input happens to carry most of them (`border-input`, `rounded-md`, `focus-visible:border-primary`), so the set difference is empty.

Check 3 measures token overlap, not structural correspondence. It cannot detect a wholly different component. Combined with the Accordion finding (layout drift invisible), the rule is: **the harness is a regression guard on class strings, not evidence of parity.** Structural parity needs the export count check from Phase 3 step 2 and a source read.

### Open decision — port Blok's composite picker?

Not started; needs a call before any code.

Rebuilding to Blok's shape means: a `Popover`-hosted panel with three `Select`s, labels, Clear/Done buttons, and a formatted trigger button. It would give Blok-matching DOM and the AM/PM period model, at the cost of the native input's free keyboard entry, platform time UI, and mobile time wheel. It would also add a `<Popovers />` + `AddSitecoreBlokUI()` installation requirement to a component that currently has none, and inherits the same nested-popover consideration flagged for Calendar's `InBuiltDropdown` if a consumer puts a TimePicker inside another popover.

Until that is decided, the `Parity` badge on TimePicker's `MIGRATION_STATUS.md` row overstates the position — by this file's own legend, `Parity` means "class strings & structure match", and the structure does not. Flagged rather than changed, because no existing badge fits a deliberate whole-component substitution and the resolution may be to port it properly.
