# UI Parity Audit

Tracking document for the component-by-component UI-parity audit against the Blok source (https://github.com/Sitecore/blok/tree/main/src) and the Blok live site (https://blok.sitecore.com/primitives).

**Automated verification:** `pwsh ./tools/verify-ui-parity.ps1` runs six checks on every Razor file under the component library and writes a transient `docs/ui-parity-report.md` summarising findings for that run. The report file is a build artifact — not tracked in git — and is overwritten on each run. Integrated into the `blok-migration` skill — runs on every component migration / update.

Last automated harness run (133 Razor files across primitives and their sub-components, covering 63 top-level Blok primitives — see [MIGRATION_STATUS.md](../MIGRATION_STATUS.md) for the authoritative per-primitive tally): **35 Check 3 drift findings remain (pre-existing, pending judgement). Checks 1, 2, 4, 5, 6 clean.**

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
| Editable | ✅ | ⚠️ minor | — | — | ✅ | Deliberate deviations: (1) `hover:bg-transparent` not used — instead `hover:bg-neutral-bg` is conditionally omitted when `IsPreviewFocusable=false`, achieving the same visual result. (2) `EditableRootProvider` accepts Blazor parameters (`@bind-IsEditing`, `@bind-Value`) instead of a pre-built hook result (`useEditable` return value) — paradigm translation, no Blazor equivalent of React hooks. (3) `useEditable` / `useEditableContext` hooks omitted — no Blazor hook pattern. |
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
| DatePicker | ✅ | ✅ | ✅ | ✅ | ✅ | Uses Calendar internally + Popover; inherits theme. |
| TimePicker | ✅ | ✅ | ✅ | ✅ | ✅ | Uses Input/Select internally; inherits. |
| Calendar | ✅ | ✅ | ⚠️ minor | ✅ | ✅ | Re-audited 2026-09-02 against Blok `a2d44e`. `bg-background`, `text-foreground`, `bg-primary text-primary-foreground` for selected — theme-aware. ARIA parity closed; 7 documented Check 3 deviations and one open decision (`InBuiltDropdown`) — see the Calendar section below. |
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
| Tooltip | ~~Single component with `Content` string param~~ Now Tooltip + TooltipTrigger + TooltipContent (3 components). `TooltipProvider` deliberately not implemented (no Radix portal/delay). `Side` lives on TooltipContent. **Tooltip text colour intentionally diverges**: uses `text-white` literal instead of Blok's `text-inverse-text` (which would render dark text on the always-dark `bg-gray-700` surface in dark mode). | (post-fix) Tooltip-text-color drift is the only Check 3 finding — deliberate. | ✅ FIXED — split into 3 components; TooltipPage rewritten with composable examples + DivergenceNote covering CSS-only hover, missing Provider, and the text-color choice. |
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

### Open decision — Blok's `InBuiltDropdown` is not ported

Commit `6d5ecd` replaced react-day-picker's native `<select>` month/year dropdowns with Blok's own `Select` component (`SelectTrigger` / `SelectContent` / `SelectItem`), exported as `InBuiltDropdown`. We still use a native `<select>` overlaid at `opacity-0`.

Not adopted in this pass because it is an architectural change, not a class-string one, and it carries a specific risk this library has already been bitten by: our `Select` renders through `PopoverService` / the `<Popovers>` host, and `Calendar` is itself rendered **inside** the DatePicker's popover. That nests a `PopoverService` popup inside a `PopoverService` popup, and the documented behaviour is that the `<Popovers>` host does not re-render when the originating component's state changes. It would also add a `<Popovers />` + `AddSitecoreBlokUI()` installation requirement to the otherwise self-contained `Calendar`.

Tracked as a Known Feature Gap on `Home.razor`. Awaiting a decision before implementing.

### Deliberate deviations (remaining Check 3 drift — 4 tokens reported by the harness)

| Blok token | Why not adopted |
|---|---|
| `bg-popover` | On Blok's `opacity-0` native select overlay, so visually inert. Our overlay instead sets `dark:[&_option]:bg-background dark:[&_option]:text-foreground`, which is the more robust fix for the dark-mode `<option>` popup bug documented in Common Pitfalls. |
| `border-collapse` | Blok renders the day grid as a `<table>`; we render a flat div grid. Structural divergence already covered by the `<DivergenceNote>` on `CalendarPage`. |
| `hover:rounded-md` | Blok puts this on the day-button base. Our rounding is per-state (`rounded-md` / `rounded-l-md` / `rounded-none`), so a base `hover:rounded-md` would round range-middle cells on hover and break the continuous range bar. |
| `text-body-text` | Not counted by the harness — `$equivGroups` canonicalises it to `text-foreground`, which the file carries. Blok sets a base text token on every day button. We leave the base uncoloured and let the root's `text-card-foreground` cascade, because setting a base `text-*` alongside per-state `text-white` reintroduces the source-order race documented for Button. |
| `text-inverse-text` | Selected and range-endpoint cells use a literal `text-white`. The surface is a fixed `bg-primary-500` that does not flip, so `text-inverse-text` would resolve dark-on-blue in dark mode. Same class as the Tooltip fix; harness Check 5 enforces it. Anti-revert comment is in `Calendar.razor`. |
| `dark:bg-transparent`, `dark:hover:bg-transparent` | Belong to `InBuiltDropdown`'s `SelectTrigger` and are below the harness's per-component reporting cap. Contingent on the open decision above. |

### Extras we have that Blok does not

- **Root border and shadow.** Blok's root is `w-fit bg-card` with no border, radius or shadow. Ours adds `rounded-lg border border-border shadow-sm text-card-foreground` so a standalone `<Calendar />` reads as a finished surface rather than floating text. Verified this does not double-border inside `DatePicker`: the picker passes `ClassName="border-0 shadow-none"` and the computed border resolves to `0px`, with the popover supplying `shadow-lg rounded-lg`.
- **Fixed six-week grid.** `GetWeeksForMonth` always renders 6 rows; Blok renders only the weeks the month needs, so its calendar changes height between months. Ours keeps a stable height (equivalent to react-day-picker's `fixedWeeks`, which Blok does not set). Deliberate — avoids layout jump when navigating months, which matters most inside the DatePicker popover.

### Verified in browser (light + dark)

Nav labels, dropdown labels and 42 day buttons carry the expected ARIA; the min/max example correctly reports `aria-disabled="true"` / `tabindex="-1"` on the previous-month button with 3 disabled days. Dark mode: calendar surface `rgb(40,40,40)`, range-middle `bg rgba(217,212,255,0.12)` with `text-primary-fg` `rgb(217,212,255)`, range endpoints `rgb(110,63,255)` with literal white, nav chevrons `rgba(255,255,255,0.68)`. All legible; no dark-mode contrast failures.
