# Migration Status

Component-by-component status of the PINGWorks BlazorUI port of the Sitecore Blok design system.

This file is the **single source of truth** for the [`blok-migration`](https://github.com/Sitecore/blok) skill's `/blok audit` command. The skill reads the `Blok Source` and `Last SHA` columns to decide which components to re-evaluate. A blanket directory scan still identifies new Blok primitives that don't yet have a row here.

- **Last evaluated:** 2026-04-22 (DatePicker AriaLabels update)
- **Blok repo:** [Sitecore/blok](https://github.com/Sitecore/blok) · branch `main`
- **Blok main HEAD:** [`26f35d9`](https://github.com/Sitecore/blok/commit/26f35d9cb36906585c1b443ddd799e6a100f2a64)
- **Audit tooling:** `pwsh ./tools/verify-ui-parity.ps1` — see [docs/ui-parity-audit.md](docs/ui-parity-audit.md)

## Status legend

| Badge | Meaning |
|---|---|
| ![Parity](https://img.shields.io/badge/Parity-22c55e?style=flat-square) | Faithful port of the Blok source. Harness clean; class strings & structure match. |
| ![Improved](https://img.shields.io/badge/Improved-3b82f6?style=flat-square) | Ported **plus** an additive Blazor-side improvement (parameter, API, flexibility). Differences annotated on the Catalogue page via `<DivergenceNote>`. |
| ![Additional](https://img.shields.io/badge/Additional-8b5cf6?style=flat-square) | Blazor-only component. No Blok source. Lives under `Components/Extra/`. |
| ![Backlog](https://img.shields.io/badge/Backlog-f59e0b?style=flat-square) | Present in Blok; not yet ported but intended. Candidate for `/blok migrate`. |
| ![Won't Do](https://img.shields.io/badge/Won%27t%20Do-6b7280?style=flat-square) | Present in Blok but deliberately not ported. Reason given in the description (heavy React/JS dependency, native Blazor alternative, or functionality already covered by another primitive). |

## How the `/blok audit` workflow uses this file

1. **Blanket scan for new primitives.** List `src/components/ui/*.tsx` on Blok's `main` branch; any primitive whose name is not a row in the table below is a new component.
2. **Per-row drift detection.** For every row with a `Blok Source`, resolve the current last-touched short SHA of that file on `main`. If it differs from `Last SHA`, the component needs a re-audit — run `/blok update <name>` or `/blok verify <name>`.
3. **Update the table.** After audit / update / verify completes clean, rewrite the component's `Last SHA` to the new value and update `Last evaluated` at the top.

Rows with status `Additional` have no Blok source and are skipped by the drift check. Rows with status `Backlog` still record a SHA so the skill can tell when the upstream source has changed since the last migration-window review (useful when deciding which backlog item to port next). `Won't Do` rows also track the SHA so a material Blok rewrite can be surfaced for re-evaluation, but absent a changed decision they're skipped by the audit reporter.

## Component status

| Component | Status | Blok Source | Last SHA | Description |
|---|---|---|---|---|
| Accordion | ![Parity](https://img.shields.io/badge/Parity-22c55e?style=flat-square) | [`accordion.tsx`](https://github.com/Sitecore/blok/blob/main/src/components/ui/accordion.tsx) | `537976` | Vertically-stacked collapsible content panels |
| ActionBar | ![Parity](https://img.shields.io/badge/Parity-22c55e?style=flat-square) | [`action-bar.tsx`](https://github.com/Sitecore/blok/blob/main/src/components/ui/action-bar.tsx) | `7c9f7e` | Floating multi-action toolbar |
| Alert | ![Improved](https://img.shields.io/badge/Improved-3b82f6?style=flat-square) | [`alert.tsx`](https://github.com/Sitecore/blok/blob/main/src/components/ui/alert.tsx) | `17d1fb` | Status banner; adds `Closeable` parameter |
| AlertDialog | ![Parity](https://img.shields.io/badge/Parity-22c55e?style=flat-square) | [`alert-dialog.tsx`](https://github.com/Sitecore/blok/blob/main/src/components/ui/alert-dialog.tsx) | `2d994e` | Modal confirmation dialog |
| AspectRatio | ![Parity](https://img.shields.io/badge/Parity-22c55e?style=flat-square) | [`aspect-ratio.tsx`](https://github.com/Sitecore/blok/blob/main/src/components/ui/aspect-ratio.tsx) | `2d994e` | Aspect-ratio preserving container |
| Avatar | ![Parity](https://img.shields.io/badge/Parity-22c55e?style=flat-square) | [`avatar.tsx`](https://github.com/Sitecore/blok/blob/main/src/components/ui/avatar.tsx) | `2d994e` | User image with fallback |
| Badge | ![Parity](https://img.shields.io/badge/Parity-22c55e?style=flat-square) | [`badge.tsx`](https://github.com/Sitecore/blok/blob/main/src/components/ui/badge.tsx) | `2d994e` | Inline label / status pill |
| Breadcrumb | ![Parity](https://img.shields.io/badge/Parity-22c55e?style=flat-square) | [`breadcrumb.tsx`](https://github.com/Sitecore/blok/blob/main/src/components/ui/breadcrumb.tsx) | `2d994e` | Hierarchical navigation trail |
| Button | ![Parity](https://img.shields.io/badge/Parity-22c55e?style=flat-square) | [`button.tsx`](https://github.com/Sitecore/blok/blob/main/src/components/ui/button.tsx) | `2d994e` | Clickable button with variants / sizes / colours |
| Calendar | ![Parity](https://img.shields.io/badge/Parity-22c55e?style=flat-square) | [`calendar.tsx`](https://github.com/Sitecore/blok/blob/main/src/components/ui/calendar.tsx) | `17d1fb` | Month-grid date picker surface |
| Card | ![Parity](https://img.shields.io/badge/Parity-22c55e?style=flat-square) | [`card.tsx`](https://github.com/Sitecore/blok/blob/main/src/components/ui/card.tsx) | `a80708` | Container with header / content / footer |
| Carousel | ![Parity](https://img.shields.io/badge/Parity-22c55e?style=flat-square) | [`carousel.tsx`](https://github.com/Sitecore/blok/blob/main/src/components/ui/carousel.tsx) | `17d1fb` | Horizontal slide scroller |
| Chart | ![Won't Do](https://img.shields.io/badge/Won%27t%20Do-6b7280?style=flat-square) | [`chart.tsx`](https://github.com/Sitecore/blok/blob/main/src/components/ui/chart.tsx) | `7c9f7e` | Recharts-backed; React-only. Established Blazor alternatives exist (ApexCharts.Blazor, ChartJs.Blazor, Radzen) — use those instead of porting. |
| Checkbox | ![Parity](https://img.shields.io/badge/Parity-22c55e?style=flat-square) | [`checkbox.tsx`](https://github.com/Sitecore/blok/blob/main/src/components/ui/checkbox.tsx) | `2d994e` | Binary / tri-state toggle control |
| CircularProgress | ![Parity](https://img.shields.io/badge/Parity-22c55e?style=flat-square) | [`circular-progress.tsx`](https://github.com/Sitecore/blok/blob/main/src/components/ui/circular-progress.tsx) | `a66d24` | Arc-based progress indicator |
| CodeViewer | ![Additional](https://img.shields.io/badge/Additional-8b5cf6?style=flat-square) | — | — | Syntax-highlighted code block (Prism) |
| Collapsible | ![Parity](https://img.shields.io/badge/Parity-22c55e?style=flat-square) | [`collapsible.tsx`](https://github.com/Sitecore/blok/blob/main/src/components/ui/collapsible.tsx) | `2d994e` | Single expand / collapse region |
| Combobox | ![Parity](https://img.shields.io/badge/Parity-22c55e?style=flat-square) | [`combobox.tsx`](https://github.com/Sitecore/blok/blob/main/src/components/ui/combobox.tsx) | `4a6b44` | Filterable select with search, single/multi-select, groups, chips |
| Command | ![Won't Do](https://img.shields.io/badge/Won%27t%20Do-6b7280?style=flat-square) | [`command.tsx`](https://github.com/Sitecore/blok/blob/main/src/components/ui/command.tsx) | `2d994e` | Command palette wrapping the React-only `cmdk` library. The Input + filtered list pattern can already be composed from `SearchInput` + `Popover` when needed. |
| ContextMenu | ![Parity](https://img.shields.io/badge/Parity-22c55e?style=flat-square) | [`context-menu.tsx`](https://github.com/Sitecore/blok/blob/main/src/components/ui/context-menu.tsx) | `2d994e` | Right-click menu |
| CopyableToken | ![Improved](https://img.shields.io/badge/Improved-3b82f6?style=flat-square) | [`copyable-token.tsx`](https://github.com/Sitecore/blok/blob/main/src/components/ui/copyable-token.tsx) | `7c9f7e` | Click-to-copy token label; adds post-click tooltip confirmation, configurable messages, and `Clicked` callback |
| DatePicker | ![Parity](https://img.shields.io/badge/Parity-22c55e?style=flat-square) | [`date-picker.tsx`](https://github.com/Sitecore/blok/blob/main/src/components/ui/date-picker.tsx) | `6cb4ad` | Date input with calendar popover |
| Dialog | ![Parity](https://img.shields.io/badge/Parity-22c55e?style=flat-square) | [`dialog.tsx`](https://github.com/Sitecore/blok/blob/main/src/components/ui/dialog.tsx) | `2d994e` | Modal dialog container |
| DnD | ![Won't Do](https://img.shields.io/badge/Won%27t%20Do-6b7280?style=flat-square) | [`dnd-context.tsx`](https://github.com/Sitecore/blok/blob/main/src/components/ui/dnd-context.tsx) + `draggable` / `droppable` / `sortable` / `drag-overlay` | `17d1fb` | Built on `@dnd-kit` — React-specific hooks and context. Blazor DnD follows the native HTML5 drag-event model with a different idiom; reimplementing would be a large JS-interop project without a parity goal. |
| Drawer | ![Won't Do](https://img.shields.io/badge/Won%27t%20Do-6b7280?style=flat-square) | [`drawer.tsx`](https://github.com/Sitecore/blok/blob/main/src/components/ui/drawer.tsx) | `17d1fb` | Functionality already covered by `Sheet`, which supports side- and bottom-sliding panels. Adding Drawer as a separate primitive would duplicate behaviour. |
| DropdownMenu | ![Parity](https://img.shields.io/badge/Parity-22c55e?style=flat-square) | [`dropdown-menu.tsx`](https://github.com/Sitecore/blok/blob/main/src/components/ui/dropdown-menu.tsx) | `7de47e` | Popover menu with items and submenus |
| Editable | ![Parity](https://img.shields.io/badge/Parity-22c55e?style=flat-square) | [`editable.tsx`](https://github.com/Sitecore/blok/blob/main/src/components/ui/editable.tsx) | `17d1fb` | Inline-editable text with input and textarea modes |
| EmptyState | ![Parity](https://img.shields.io/badge/Parity-22c55e?style=flat-square) | [`empty-states.tsx`](https://github.com/Sitecore/blok/blob/main/src/components/ui/empty-states.tsx) | `17d1fb` | Empty-results placeholder |
| ErrorState | ![Improved](https://img.shields.io/badge/Improved-3b82f6?style=flat-square) | [`error-states.tsx`](https://github.com/Sitecore/blok/blob/main/src/components/ui/error-states.tsx) | `17d1fb` | Error placeholder; adds HTTP-status variant |
| Field | ![Parity](https://img.shields.io/badge/Parity-22c55e?style=flat-square) | [`field.tsx`](https://github.com/Sitecore/blok/blob/main/src/components/ui/field.tsx) | `17d1fb` | Form-field grouping wrapper |
| Filter | ![Backlog](https://img.shields.io/badge/Backlog-f59e0b?style=flat-square) | [`filter.tsx`](https://github.com/Sitecore/blok/blob/main/src/components/ui/filter.tsx) | `0253b9` | Multi-facet filter panel |
| Form | ![Won't Do](https://img.shields.io/badge/Won%27t%20Do-6b7280?style=flat-square) | [`form.tsx`](https://github.com/Sitecore/blok/blob/main/src/components/ui/form.tsx) | `2d994e` | Wraps `react-hook-form`. Superseded by native Blazor `EditForm` + `DataAnnotationsValidator` + `Input*` primitives — paradigm mismatch means a port would fight both frameworks. |
| HoverCard | ![Parity](https://img.shields.io/badge/Parity-22c55e?style=flat-square) | [`hover-card.tsx`](https://github.com/Sitecore/blok/blob/main/src/components/ui/hover-card.tsx) | `2d994e` | Hover-triggered popover card |
| Icon | ![Parity](https://img.shields.io/badge/Parity-22c55e?style=flat-square) | [`icon.tsx`](https://github.com/Sitecore/blok/blob/main/src/components/ui/icon.tsx) | `17d1fb` | SVG icon renderer (MDI) with `Variant` (Default/Subtle/Filled) and `ColorScheme` (11 schemes) |
| Input | ![Parity](https://img.shields.io/badge/Parity-22c55e?style=flat-square) | [`input.tsx`](https://github.com/Sitecore/blok/blob/main/src/components/ui/input.tsx) | `cc653d` | Text input control |
| InputGroup | ![Parity](https://img.shields.io/badge/Parity-22c55e?style=flat-square) | [`input-group.tsx`](https://github.com/Sitecore/blok/blob/main/src/components/ui/input-group.tsx) | `37c0d3` | Input with affixed addons |
| InputOtp | ![Backlog](https://img.shields.io/badge/Backlog-f59e0b?style=flat-square) | [`input-otp.tsx`](https://github.com/Sitecore/blok/blob/main/src/components/ui/input-otp.tsx) | `a02fe3` | One-time-password segmented input |
| Kbd | ![Parity](https://img.shields.io/badge/Parity-22c55e?style=flat-square) | [`kbd.tsx`](https://github.com/Sitecore/blok/blob/main/src/components/ui/kbd.tsx) | `17d1fb` | Keyboard-key glyph |
| Label | ![Parity](https://img.shields.io/badge/Parity-22c55e?style=flat-square) | [`label.tsx`](https://github.com/Sitecore/blok/blob/main/src/components/ui/label.tsx) | `2d994e` | Form label element |
| Menubar | ![Parity](https://img.shields.io/badge/Parity-22c55e?style=flat-square) | [`menubar.tsx`](https://github.com/Sitecore/blok/blob/main/src/components/ui/menubar.tsx) | `2d994e` | App-level horizontal menu bar |
| NavigationMenu | ![Parity](https://img.shields.io/badge/Parity-22c55e?style=flat-square) | [`navigation-menu.tsx`](https://github.com/Sitecore/blok/blob/main/src/components/ui/navigation-menu.tsx) | `2d994e` | Multi-level menu with flyout panels |
| Pagination | ![Improved](https://img.shields.io/badge/Improved-3b82f6?style=flat-square) | [`pagination.tsx`](https://github.com/Sitecore/blok/blob/main/src/components/ui/pagination.tsx) | `17d1fb` | Page nav; adds Blazor `Click` callback alongside `Href` |
| Popover | ![Parity](https://img.shields.io/badge/Parity-22c55e?style=flat-square) | [`popover.tsx`](https://github.com/Sitecore/blok/blob/main/src/components/ui/popover.tsx) | `2d994e` | Anchor-positioned popover surface |
| Progress | ![Parity](https://img.shields.io/badge/Parity-22c55e?style=flat-square) | [`progress.tsx`](https://github.com/Sitecore/blok/blob/main/src/components/ui/progress.tsx) | `2d994e` | Linear progress bar |
| RadioGroup | ![Improved](https://img.shields.io/badge/Improved-3b82f6?style=flat-square) | [`radio-group.tsx`](https://github.com/Sitecore/blok/blob/main/src/components/ui/radio-group.tsx) | `2d994e` | Radio options; adds inline `Label` helper on item |
| Resizable | ![Parity](https://img.shields.io/badge/Parity-22c55e?style=flat-square) | [`resizable.tsx`](https://github.com/Sitecore/blok/blob/main/src/components/ui/resizable.tsx) | `17d1fb` | Draggable panel splitter |
| ScrollArea | ![Parity](https://img.shields.io/badge/Parity-22c55e?style=flat-square) | [`scroll-area.tsx`](https://github.com/Sitecore/blok/blob/main/src/components/ui/scroll-area.tsx) | `2d994e` | Custom-scrollbar container |
| SearchInput | ![Parity](https://img.shields.io/badge/Parity-22c55e?style=flat-square) | [`search-input.tsx`](https://github.com/Sitecore/blok/blob/main/src/components/ui/search-input.tsx) | `0253b9` | Input with search icon and clear button |
| Select | ![Parity](https://img.shields.io/badge/Parity-22c55e?style=flat-square) | [`select.tsx`](https://github.com/Sitecore/blok/blob/main/src/components/ui/select.tsx) | `2d994e` | Drop-down value picker (10-component split) |
| Separator | ![Parity](https://img.shields.io/badge/Parity-22c55e?style=flat-square) | [`separator.tsx`](https://github.com/Sitecore/blok/blob/main/src/components/ui/separator.tsx) | `2d994e` | Divider line |
| Sheet | ![Parity](https://img.shields.io/badge/Parity-22c55e?style=flat-square) | [`sheet.tsx`](https://github.com/Sitecore/blok/blob/main/src/components/ui/sheet.tsx) | `2d994e` | Edge-sliding panel (side drawer) |
| Sidebar | ![Parity](https://img.shields.io/badge/Parity-22c55e?style=flat-square) | [`sidebar.tsx`](https://github.com/Sitecore/blok/blob/main/src/components/ui/sidebar.tsx) | `3c1b7d` | App-level collapsible side navigation |
| Skeleton | ![Parity](https://img.shields.io/badge/Parity-22c55e?style=flat-square) | [`skeleton.tsx`](https://github.com/Sitecore/blok/blob/main/src/components/ui/skeleton.tsx) | `e78784` | Loading-placeholder block |
| Slider | ![Parity](https://img.shields.io/badge/Parity-22c55e?style=flat-square) | [`slider.tsx`](https://github.com/Sitecore/blok/blob/main/src/components/ui/slider.tsx) | `2d994e` | Range-value track |
| Spinner | ![Parity](https://img.shields.io/badge/Parity-22c55e?style=flat-square) | [`spinner.tsx`](https://github.com/Sitecore/blok/blob/main/src/components/ui/spinner.tsx) | `ab8d9f` | Spinning loading indicator |
| Stack | ![Additional](https://img.shields.io/badge/Additional-8b5cf6?style=flat-square) | — | — | Flexbox layout helper (vertical / horizontal) |
| StackNavigation | ![Parity](https://img.shields.io/badge/Parity-22c55e?style=flat-square) | [`stack-navigation.tsx`](https://github.com/Sitecore/blok/blob/main/src/components/ui/stack-navigation.tsx) | `7c9f7e` | Icon-and-label navigation rail with vertical or horizontal orientation |
| Stepper | ![Parity](https://img.shields.io/badge/Parity-22c55e?style=flat-square) | [`stepper.tsx`](https://github.com/Sitecore/blok/blob/main/src/components/ui/stepper.tsx) | `17d1fb` | Multi-step progress indicator |
| Switch | ![Parity](https://img.shields.io/badge/Parity-22c55e?style=flat-square) | [`switch.tsx`](https://github.com/Sitecore/blok/blob/main/src/components/ui/switch.tsx) | `2d994e` | On / off toggle control |
| Table | ![Parity](https://img.shields.io/badge/Parity-22c55e?style=flat-square) | [`table.tsx`](https://github.com/Sitecore/blok/blob/main/src/components/ui/table.tsx) | `6403f5` | Tabular data layout |
| Tabs | ![Parity](https://img.shields.io/badge/Parity-22c55e?style=flat-square) | [`tabs.tsx`](https://github.com/Sitecore/blok/blob/main/src/components/ui/tabs.tsx) | `2d994e` | Tabbed content switcher |
| Text | ![Additional](https://img.shields.io/badge/Additional-8b5cf6?style=flat-square) | — | — | Typography primitive |
| Textarea | ![Parity](https://img.shields.io/badge/Parity-22c55e?style=flat-square) | [`textarea.tsx`](https://github.com/Sitecore/blok/blob/main/src/components/ui/textarea.tsx) | `17d1fb` | Multi-line text input |
| ThemeToggle | ![Additional](https://img.shields.io/badge/Additional-8b5cf6?style=flat-square) | — | — | Light / dark mode switcher |
| TimePicker | ![Parity](https://img.shields.io/badge/Parity-22c55e?style=flat-square) | [`time-picker.tsx`](https://github.com/Sitecore/blok/blob/main/src/components/ui/time-picker.tsx) | `53ab50` | Time input control |
| Timeline | ![Parity](https://img.shields.io/badge/Parity-22c55e?style=flat-square) | [`timeline.tsx`](https://github.com/Sitecore/blok/blob/main/src/components/ui/timeline.tsx) | `17d1fb` | Vertical event timeline |
| Toaster | ![Improved](https://img.shields.io/badge/Improved-3b82f6?style=flat-square) | [`sonner.tsx`](https://github.com/Sitecore/blok/blob/main/src/components/ui/sonner.tsx) | `17d1fb` | Imperative `ToastService.Show(...)` API; `/sonner` stub page |
| Toggle | ![Parity](https://img.shields.io/badge/Parity-22c55e?style=flat-square) | [`toggle.tsx`](https://github.com/Sitecore/blok/blob/main/src/components/ui/toggle.tsx) | `2d994e` | Single-toggle button |
| ToggleGroup | ![Parity](https://img.shields.io/badge/Parity-22c55e?style=flat-square) | [`toggle-group.tsx`](https://github.com/Sitecore/blok/blob/main/src/components/ui/toggle-group.tsx) | `6255ac` | Segmented toggle group |
| Tooltip | ![Improved](https://img.shields.io/badge/Improved-3b82f6?style=flat-square) | [`tooltip.tsx`](https://github.com/Sitecore/blok/blob/main/src/components/ui/tooltip.tsx) | `2d994e` | Per-tooltip `Delay` on `TooltipContent` (Blok delays via Provider only); `ClassName` on `Tooltip` root; CSS Anchor Positioning (`position: fixed` + `position-anchor`) so tooltips escape ancestor `overflow` clipping without a JS portal |
| TreeView | ![Additional](https://img.shields.io/badge/Additional-8b5cf6?style=flat-square) | — | — | Hierarchical collapsible tree |

## Summary

| Status | Count |
|---|---|
| ![Parity](https://img.shields.io/badge/Parity-22c55e?style=flat-square) | 52 |
| ![Improved](https://img.shields.io/badge/Improved-3b82f6?style=flat-square) | 7 |
| ![Additional](https://img.shields.io/badge/Additional-8b5cf6?style=flat-square) | 5 |
| ![Backlog](https://img.shields.io/badge/Backlog-f59e0b?style=flat-square) | 2 |
| ![Won't Do](https://img.shields.io/badge/Won%27t%20Do-6b7280?style=flat-square) | 5 |
| **Total ported** | **59** of **61** Blok primitives (excluding 5 Won't Do) |

## Notes on classification

- **Improved rows** also carry a `<DivergenceNote>` on their Catalogue page describing the Blazor-side addition. See [docs/ui-parity-audit.md](docs/ui-parity-audit.md) for the full reasoning.
- **`sonner` → `Toaster`** is the one deliberate renaming: Blok's `Sonner` wraps the JS-only Sonner library; our idiomatic `ToastService.Show(...)` API is imperative. The Catalogue carries a `/primitives/sonner` cross-reference stub.
- **Won't Do rationale summary** — Chart (React chart lib; Blazor alternatives exist), Command (cmdk; composable from existing primitives), DnD (@dnd-kit React-only; different Blazor idiom), Drawer (covered by Sheet), Form (native Blazor EditForm supersedes react-hook-form). Re-evaluate only if a business need forces the question.
- **Backlog items** are legitimate ports with no blocking rationale — prioritisation only. Pick the next one for `/blok migrate` based on consumer demand.
- **Excluded internal Blok files** (not tracked as primitives): `inputOtp.tsx` (typo-duplicate of `input-otp.tsx`), `select-react.tsx` (alternate implementation of `select`).

---

## Chunks coverage notes (v1 — Foundations + Layouts)

Chunks are tracked as "Extras" — no rows in the table above.