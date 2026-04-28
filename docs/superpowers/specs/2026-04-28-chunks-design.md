# Chunks — Design Spec

**Date:** 2026-04-28
**Owner:** Richard Hauer (PING Works)
**Library:** `PINGWorks.SitecoreBlok.BlazorUI`
**Status:** Approved for implementation planning

## 1. Purpose

Introduce a new component family — *Chunks* — that sits above the existing Primitives. Chunks are opinionated compositions of one or more Primitives, designed to lower the authoring effort required to assemble idiomatic, well-spaced, accessible Sitecore-Blok-styled pages and panels. They package the recurring Tailwind class arrangements, structural slots, and primitive choices that consumers would otherwise repeat.

Chunks are layout- and pattern-shaped helpers only. They do **not** carry domain knowledge (no Sitecore items, sites, users, or product entities). Domain-specific compositions are reserved for a future *Bloks* port that mirrors Sitecore's upstream `src/components/bloks/*` and lives in its own namespace.

The name "Chunk" was chosen deliberately to avoid collision with Sitecore's "Blok" — the future Bloks port will fork cleanly into a sibling namespace.

## 2. Goals

- Reduce authoring friction for the most common page and panel layouts in Sitecore Marketplace apps and martech UIs built on this library.
- Encode the literal Tailwind class strings that Tailwind's scanner requires, so consumers never assemble class names from string fragments.
- Provide enough coverage that the existing Catalogue's `MainLayout.razor`, `NavMenu.razor`, and `ComponentPage.razor` can be refactored to consume Chunks in a follow-up effort (this refactor is **out of scope** for the current initiative).
- Stay consistent with the existing Primitives in idiom, theming, render-mode behaviour, and Catalogue treatment so consumers don't context-switch between two styles of API.
- Provide explicit shells for the three Sitecore Marketplace extension points whose host chrome imposes constraints: Custom Field, Pages Context Panel, Dashboard Widget.

## 3. Non-goals

- Porting Sitecore Blok's own `src/components/bloks/*` (e.g. `top-bar.tsx`, `site-card.tsx`, `dashboard-widget.tsx`, `pinned-sites-section.tsx`). These are domain-specific and will be reproduced faithfully in a future `Bloks/` namespace.
- Refactoring the Catalogue (`MainLayout`, `NavMenu`, `ComponentPage`) to consume Chunks during this initiative. Coverage must be sufficient to make that refactor possible later, but the refactor itself is a follow-up.
- Building a generic `FormField` wrapper. Per-field-type chunks (`TextField`, `SelectField`, etc.) carry semantic value and are preferred. Custom-control consumers compose `FormLabel` + their control + help/error manually.
- State-management or async-orchestration chunks (no `<DataLoader>`, `<AsyncBoundary>`, etc.). Visual chunks for loading/error/empty states exist; orchestration is the consumer's responsibility.
- Drift-checking against any upstream source. Chunks are categorised as "Extras" — they have no Sitecore Blok equivalent and never enter `MIGRATION_STATUS.md` or the `/blok` audit.

## 4. Scope summary

**84 Chunks across 7 families**, all under `PINGWorks.SitecoreBlok.BlazorUI/Components/Chunks/<Family>/`. The seven families:

| Family | Count | Purpose |
|---|---|---|
| Layouts | 8 | Outer page envelopes and shell wrappers around Dialog/Sheet. |
| Headers | 7 | Sticky bars, page/section headers, brand, toolbar, announcements. |
| Navigation | 9 | Nav lists, sidebars, rails, breadcrumbs, tab bars, account menus, back links. |
| Content | 19 | Sections, containers, KPI tiles, hero/callout, full-page state views, skeletons. |
| Forms | 25 | Form shell + section + grid + actions, plus per-field-type wrappers and login/confirm dialogs. |
| Data | 11 | Data-table page, toolbar, pagination, detail/settings pages, list/result patterns. |
| Marketplace | 5 | Shells for all five Sitecore Marketplace extension points. |

## 5. API conventions

These twelve rules apply to every Chunk and are non-negotiable across the family.

**5.1. Strict-shape regions are `RenderFragment?` parameters.** When a Chunk has a fixed set of named regions (e.g. `AppShell` has Header / Sidebar / Content / Footer), those regions are declared as `[Parameter] public RenderFragment? Header { get; set; }` etc. — never as separate child components like `<AppShellHeader>`. This makes invalid compositions un-typeable: a consumer cannot accidentally insert an `<Icon>` or `<Dialog>` between regions.

**5.2. Open-ended children use real subcomponents.** When a Chunk holds an arbitrary list of similar children (e.g. `NavList` of `NavListItem`s, `MetricGroup` of `KpiTile`s, `BreadcrumbBar` of `BreadcrumbItem`s where the consumer prefers composition over a `List<T>` parameter), use real subcomponents passed via `ChildContent`. The rule of thumb: if a `List<T>` parameter would feel just as natural, prefer subcomponents; otherwise prefer named `RenderFragment` slots.

**5.3. Variant control is enum properties on the parent.** `PageShell.AsidePlacement`, `Toolbar.Density`, `KpiTile.Trend`, etc. No string-typed variants. The enum *types* themselves are standardised across Chunks per §5.10. CSS classes stay literal strings; assemble via `CssClassBuilder` exactly the way `Stack.razor` already does — Tailwind's scanner depends on this.

**5.4. Single-purpose content uses text properties.** `Title`, `Description`, `HelpText`, `Label`, `Href`, `IconSvg` are plain `string?` (or appropriate primitive type). When richer content is needed, a sibling `RenderFragment?` (`TitleContent`, `DescriptionContent`) is added; the fragment wins when both are set. Error styling on `*Field` chunks and `FormLabel` is a `bool Error` flag — see §7.5; consumers render any error *message* themselves and the field reserves no space for one.

**5.5. No domain knowledge.** Chunks know about layout, spacing, and primitive choice. They never know about Sitecore items, sites, users, projects, or any other entity. Domain-aware compositions are reserved for the future `Bloks/` namespace, which must be able to co-exist cleanly with `Chunks/`.

**5.6. Render-mode neutral by default.** A Chunk that only emits markup is annotated with no `@rendermode` directive (it inherits from the consumer). A Chunk that needs interactivity (e.g. `AppShell` mounting `Popovers` and `Toaster`) does not hard-code `InteractiveServer`; it forwards the choice to the consumer through a parameter or by exposing the interactive sub-primitives as `RenderFragment` slots that the consumer fills in with their own render mode.

**5.7. Catalogue page per Chunk, mirroring Primitives.** Each Chunk gets `Catalogue/Components/Pages/Chunks/<Name>Page.razor` using `<ComponentPage>`. Each page **must explicitly declare** `Interactivity="ComponentInteractivity.Ssr"` or `Interactivity="ComponentInteractivity.Interactive"` — same convention as Primitives.

**5.8. Catalogue navigation.** A new top-nav link "Chunks" sits alongside "Primitives" in `MainLayout.razor`. Route is `/chunks`. The left-nav lists Chunks grouped by the seven families. Each Chunk page reuses `<ComponentPage>` with the same Examples + API columns Primitives use today.

**5.9. Chunks are "Extras" — no upstream tracking, but downstream-impact-aware.** Chunks have no row in `MIGRATION_STATUS.md`, no Last SHA, and never trigger their own drift-check. **However**, when the `/blok` audit detects an upstream API change in a Primitive (e.g. a parameter renamed, a slot removed, a Razor signature broken), the skill must additionally identify every Chunk that consumes that Primitive (a recursive search across `Components/Chunks/**/*.razor` for the Primitive's tag name) and flag those Chunks for review in the same audit report. Chunks are downstream consumers of Primitives; a Primitive's API shift is a Chunk's potential breakage. The skill never edits Chunks autonomously — it only surfaces them for the implementer's attention. All required services register through the existing `AddSitecoreBlokUI()` extension method; no separate registration entry points.

**5.10. Standardised enums across Chunks.** Where multiple Chunks accept the same kind of variant — tone, sizing, density, columns, direction, trend, side — they reference a single shared enum type. Where an existing enum in the project-root `Enums.cs` already fits a Chunk's concept (e.g. `Size`), the Chunk reuses it without modification. **All new Chunks-shared enums live in a separate file at `Components/Chunks/Enums.cs`** — the project-root `Enums.cs` stays canonical to Blok primitives, with no Chunks-side additions. Both files share the `PINGWorks.SitecoreBlok.BlazorUI` namespace, so consumer code resolves either file's enums identically. **This rule applies only to Chunks; existing Primitive enums are untouched and not consolidated.** When a Chunk wraps a Primitive that has its own per-Primitive enum (`AlertVariant`, `BadgeColor`, `IconColorScheme`, `ButtonColor`, etc.), the Chunk either surfaces the Primitive enum directly or — if the Chunk is exposing a higher-level concept like `Tone` — translates internally; it does not create a parallel duplicate enum on the Chunk's API. The canonical Chunk-shared enums are:

| Enum | Status | Used by |
|---|---|---|
| `Size` | existing (reuse — lives in project-root `Enums.cs`) | `Container.MaxWidth`, `CenteredShell.MaxWidth`, any `Gap`/sizing prop |
| `Position` *(new)* | add to `Components/Chunks/Enums.cs` — `{ Top, Right, Bottom, Left }` | `SheetShell.Side` (translates internally to the primitive's `SheetSide`); future `TooltipShell`, `PopoverShell`, `DropdownMenuShell` if added later |
| `Orientation` *(new)* | add to `Components/Chunks/Enums.cs` — `{ Horizontal, Vertical }` | `SplitShell.Direction`, `MetricGroup.Direction` |
| `Tone` *(new)* | add to `Components/Chunks/Enums.cs` — `{ Info, Success, Warning, Danger, Neutral }` | `Callout.Tone`, `AnnouncementBar.Tone`, `ConfirmDialog.Tone` |
| `Density` *(new)* | add to `Components/Chunks/Enums.cs` — `{ Comfortable, Compact }` | `Toolbar.Density`, `DataToolbar.Density` |
| `Trend` *(new)* | add to `Components/Chunks/Enums.cs` — `{ Up, Down, Neutral }` | `KpiTile.Trend`, `StatCard.Trend` |
| `Columns` *(new)* | add to `Components/Chunks/Enums.cs` — `{ One, Two, Three, Four }` | `CardGrid.Columns`, `FormGrid.Columns` |
| `Placement` *(new)* | add to `Components/Chunks/Enums.cs` — `{ Left, Right, None }` (and any others as needed) | `PageShell.AsidePlacement` |

**5.11. Required-state error styling on `*Field` chunks.** Every `*Field` chunk tracks an internal `Touched` state. A field becomes Touched after a focus-then-blur cycle on its wrapped control — i.e. after the user has actually interacted with the field. Once Touched, if `Required` is `true` and the bound value is empty, the field renders in error styling regardless of any consumer-supplied `Error` parameter. The consumer-supplied `Error` is OR-combined with this internal computation, so external validation (server-side errors, custom rules) still forces error styling without disturbing the touched-tracking. Each `*Field` exposes `EventCallback<bool> ErrorChanged` so consumers can observe the computed error state (e.g. to disable a Submit button or render a message panel elsewhere). "Empty" is defined per control type:

- `TextField` / `PasswordField` / `TextAreaField` / `SearchField`: bound `Value` is null, empty, or whitespace
- `SelectField` / `ComboboxField` / `RadioGroupField`: bound `Value` is null / unset
- `CheckboxField` / `SwitchField` / `ToggleField`: bound `Checked` is `false`
- `ToggleGroupField`: no option selected
- `DateField` / `TimeField`: bound `Value` is `null`
- `SliderField`: `Required` is a no-op (slider always has a value)

The `Touched` reset (e.g. on form reset) is the consumer's responsibility — they pass a key/parameter to force the Chunk to re-mount, or use a future explicit `Reset()` API if one is added.

**5.12. Per-shared-enum helper classes.** When a shared enum from §5.10 is consumed by two or more Chunks (e.g. `Tone` is used by `Callout`, `AnnouncementBar`, `ConfirmDialog`), the enum-to-Tailwind-class mapping lives in a single internal helper class — *not* duplicated as a `switch` expression in each Chunk. The helper sits next to the Chunks that consume it (under `Components/Chunks/Shared/`) and exposes one static method per CSS context: text colour, background colour, border colour, hover variant, focus ring, etc. Example shape:

```csharp
internal static class ToneClasses
{
    public static string Text(Tone tone) => tone switch
    {
        Tone.Info     => "text-info-foreground",
        Tone.Success  => "text-success-foreground",
        Tone.Warning  => "text-warning-foreground",
        Tone.Danger   => "text-danger-foreground",
        Tone.Neutral  => "text-foreground",
    };

    public static string Bg(Tone tone) => tone switch
    {
        Tone.Info     => "bg-info-bg",
        Tone.Success  => "bg-success-bg",
        Tone.Warning  => "bg-warning-bg",
        Tone.Danger   => "bg-danger-bg",
        Tone.Neutral  => "bg-subtle-bg",
    };

    public static string Border(Tone tone) => tone switch { /* … */ };
}
```

The helper file lives under the existing `@source '../../components'` directive in `blok.css`, so Tailwind v4's recursive scanner picks up every literal Tailwind class inside it without any config change. Consumer Chunks call `ToneClasses.Text(Tone)` etc. through `CssClassBuilder`; they never inline the switch. **No global `Colors` constants** — Sitecore Blok's semantic CSS tokens (`text-foreground`, `text-primary`, `bg-subtle-bg`) and these per-shared-enum helpers are the only two layers of class-name indirection allowed.

The expected helpers (one file each, under `Components/Chunks/Shared/`):

| Helper | For shared enum | Methods |
|---|---|---|
| `ToneClasses` | `Tone` | `Text`, `Bg`, `Border`, `Icon`, plus `*Hover` variants where used |
| `TrendClasses` | `Trend` | `Text`, `Icon` (arrow direction implied) |
| `DensityClasses` | `Density` | `Padding`, `Gap`, `Height` |
| `OrientationClasses` | `Orientation` | `Flex`, `Divide` |
| `PositionClasses` | `Position` | `Side`, `OffsetClass` |
| `PlacementClasses` | `Placement` | `Side` (wraps both `Left`/`Right` cases; `None` returns empty) |
| `ColumnsClasses` | `Columns` | `Grid` (e.g. `grid-cols-1` … `grid-cols-4`) |

A helper without consumers in the v1 implementation is not added until a second consumer exists — this prevents speculative helpers.

## 6. Folder layout

```
PINGWorks.SitecoreBlok.BlazorUI/
└── Components/
    └── Chunks/
        ├── Enums.cs                        ← Chunks-shared enums (§5.10) — kept separate from project-root Enums.cs
        ├── Shared/                         ← per-shared-enum helper classes (§5.12)
        │   ├── ToneClasses.cs
        │   ├── TrendClasses.cs
        │   ├── DensityClasses.cs
        │   ├── OrientationClasses.cs
        │   ├── PositionClasses.cs
        │   ├── PlacementClasses.cs
        │   └── ColumnsClasses.cs
        ├── Layouts/
        │   ├── AppShell.razor
        │   ├── PageShell.razor
        │   ├── CenteredShell.razor
        │   ├── SplitShell.razor
        │   ├── ListDetailShell.razor
        │   ├── BlankShell.razor
        │   ├── DialogShell.razor
        │   └── SheetShell.razor
        ├── Headers/
        ├── Navigation/
        ├── Content/
        ├── Forms/
        ├── Data/
        └── Marketplace/

PINGWorks.SitecoreBlok.BlazorUI.Catalogue/
└── Components/
    └── Pages/
        └── Chunks/
            ├── Index.razor                ← /chunks landing
            ├── Layouts/
            │   ├── AppShellPage.razor     ← /chunks/app-shell etc.
            │   └── ...
            ├── Headers/
            ├── Navigation/
            ├── Content/
            ├── Forms/
            ├── Data/
            └── Marketplace/
```

`MainLayout.razor` (Catalogue) gains a `"Chunks"` top-nav link. The Chunks left-nav mirrors the Primitives sidebar pattern, grouping entries by family.

## 7. The roster — 84 Chunks

### 7.1 Layouts/ — 8

| Chunk | Purpose | Key API |
|---|---|---|
| `AppShell` | Outer dark-mode + popover/toaster mount; the chrome a top-level Blazor app sits inside. | Slots: `Header`, `Sidebar`, `Content`, `Footer`. Render-mode forwarding for Popovers/Toaster (see §10.1). |
| `PageShell` | In-app page envelope sitting inside `AppShell.Content`. | Slots: `Header`, `Body`, `Aside`, `Footer`. Enum `AsidePlacement` (shared `Placement`). |
| `CenteredShell` | Single centered column for auth / error / empty-app states. | `MaxWidth` (existing `Size`). ChildContent. |
| `SplitShell` | Two-pane layout (master/detail). | Slots: `Start`, `End`. Props: `Direction` (shared `Orientation`), `Resizable`, `Ratio`. |
| `ListDetailShell` | Opinionated `SplitShell` for list-on-left / detail-on-right with selection state. | Slots: `List`, `Detail`, `Empty`. |
| `BlankShell` | No-chrome envelope — sets background and font only. For print, login pre-shell, full-bleed marketing pages. | ChildContent. |
| `DialogShell` | Opinionated wrapper around the `Dialog` primitive that hides the 5-component composition. | Props: `Title`, `Description`, `Open`, `OpenChanged`. Slots: `Body`, `Actions`. |
| `SheetShell` | Same idea for `Sheet` (side/bottom panel). | Props: `Side` (shared `Position`; translated to primitive `SheetSide` internally), `Open`, `OpenChanged`. Slots: `Header`, `Body`, `Actions`. |

### 7.2 Headers/ — 7

| Chunk | Purpose | Key API |
|---|---|---|
| `AppHeader` | Sticky top bar with backdrop blur. Replaces the Catalogue's hand-built header. | Slots: `Brand`, `Nav`, `Actions`. Props: `Sticky` (default true), `Bordered`. |
| `AppBrand` | Logo + product name + optional version chip. Reusable inside or outside `AppHeader`. | Props: `Name`, `Href`, `Version`. Slot: `Logo` (RenderFragment for SVG). |
| `PageHeader` | Top of an in-page area. Used as a bare title (slots empty) or with breadcrumbs/actions/status. | Props: `Title`, `Description`. Slots: `Breadcrumbs`, `Actions`, `Status`. |
| `SectionHeader` | Heading band inside a `ContentSection`. | Props: `Title`, `Description`. Slot: `Actions`. |
| `SubHeader` | Context strip below `AppHeader` (env switcher, ambient breadcrumbs, scope chip). | Slot: `ChildContent`. Props: `Bordered`. |
| `Toolbar` | Horizontal action strip — usable above tables, in section headers, or freestanding. | Slots: `Start`, `Center`, `End`. Enum `Density` (shared `Density`). |
| `AnnouncementBar` | Top-of-app dismissible banner. | Props: `Message`, `Tone` (shared `Tone` enum), `Dismissible`. Slot: `Action`. |

### 7.3 Navigation/ — 9

| Chunk | Purpose | Key API |
|---|---|---|
| `NavList` | Vertical link list with consistent spacing. | ChildContent: `NavListItem` / `NavGroup`. |
| `NavListItem` | Single link row. | Props: `Href`, `IconSvg`, `Label`, `Active`, `Badge`, `OnClick`. |
| `NavGroup` | Labelled grouping inside a `NavList`. | Props: `Label`, `Collapsible`, `DefaultOpen`. ChildContent. |
| `AppSidebar` | Pre-wired `Sidebar` with brand area + nav slot + footer slot. Replaces the Catalogue's `NavMenu` body. | Slots: `Brand`, `Nav`, `Footer`. |
| `NavRail` | Narrow icon-only rail (different idiom from `Sidebar`). | ChildContent of `NavListItem`. |
| `BreadcrumbBar` | Declarative breadcrumb taking `IList<BreadcrumbItem>`. | Props: `Items`. Record: `BreadcrumbItem(Label, Href, IconSvg?)`. |
| `TabBar` | Top-of-page tab navigation with route-aware active state. | Props: `Items`, `Selected`, `SelectedChanged`. Record: `TabDefinition(Label, Href, IconSvg?, Badge?)`. |
| `AccountMenu` | Avatar trigger + DropdownMenu shell with name/email header. | Props: `Name`, `Email`, `AvatarUrl`, `Initials`. Slot: `Items`. |
| `BackLink` | Single "← Back to X" element. | Props: `Href`, `Label`. |

### 7.4 Content/ — 19

| Chunk | Purpose | Key API |
|---|---|---|
| `ContentSection` | Section header + body in standard padding/spacing. | Props: `Title`, `Description`. Slots: `Actions`, `ChildContent`. |
| `Container` | Max-width content centerer. Equivalent to MudContainer. | `MaxWidth` (existing `Size`). ChildContent. |
| `PageContent` | Vertical stack of sections with consistent gap. Used inside `PageShell.Body`. | Enum `Gap`. ChildContent. |
| `CardGrid` | Responsive grid of cards. | `Columns` (shared `Columns`), `Gap` (existing `Size`). ChildContent. |
| `FeatureCard` | Icon + Title + Description card; for landing/onboarding pages. | Props: `IconSvg`, `Title`, `Description`. Slot: `Footer`. |
| `ActionCard` | Card with click target + trailing arrow; for navigation grids. | Props: `Title`, `Description`, `Href`, `IconSvg`, `OnClick`. |
| `MediaCard` | Thumbnail-first card. Image + Title/Description + Actions. | Props: `ImageUrl`, `ImageAlt`, `Title`, `Description`. Slots: `Actions`, `Overlay`. |
| `KpiTile` | Single big-number stat tile. Use without `Delta`/`Trend`/`IconSvg` for a minimal stat; populate them for the full feature. | Props: `Label`, `Value`, `Delta`, `Trend` (shared `Trend`), `IconSvg`. |
| `StatCard` | Card-based KPI variant; sparkline + actions slots. | Props: `Label`, `Value`, `Trend` (shared `Trend`). Slots: `Sparkline`, `Actions`. |
| `MetricGroup` | Horizontal arrangement of `KpiTile`s with dividers. | ChildContent: `KpiTile`. |
| `Hero` | Landing-style intro band. | Props: `Title`, `Subtitle`. Slots: `Actions`, `Media`. |
| `Callout` | Visually distinct aside (gentler than `Alert`). | Props: `Title`, `Tone` (shared `Tone` enum), `IconSvg`. Slot: `ChildContent`. |
| `EmptyStatePanel` | `EmptyState` wrapped in section-level chrome. | Props: `Title`, `Description`, `IconSvg`. Slot: `Action`. |
| `EmptyView` | Full-page empty state (vs panel-level). | Same API as `EmptyStatePanel`; sized to viewport. |
| `ErrorStatePanel` | `ErrorState` wrapped in section chrome. | Props: `Title`, `Description`, `Status`. Slot: `Action`. |
| `ErrorView` | Full-page error. | Same API as `ErrorStatePanel`. |
| `LoadingPanel` | Spinner + optional message centered in min-height block. | Props: `Message`, `MinHeight`. |
| `LoadingView` | Full-page loading. | Props: `Message`. |
| `SkeletonCard` | Skeleton-of-a-card preset for grid loading. | Props: `Lines`, `WithHeader`, `WithFooter`. |

### 7.5 Forms/ — 25

#### Structural

| Chunk | Purpose | Key API |
|---|---|---|
| `FormShell` | Page-level form envelope. | Internally composes `PageHeader` + sections + `FormActions`. Props: `Title`, `Description`, `OnSubmit`. Slots: `Sections`, `Actions`. |
| `FormSection` | Labelled grouping of fields. | Props: `Title`, `Description`. ChildContent. |
| `FormGrid` | Multi-column field layout. | `Columns` (shared `Columns`), `Gap` (existing `Size`). ChildContent. |
| `FormActions` | Sticky-bottom action row. | Slots: `Start` (Cancel), `End` (Submit). Props: `Sticky`. |
| `FormLabel` | Form-context wrapper around the `Label` primitive. | Props: `For`, `Required`, `Optional`, `bool Error`. ChildContent. |

#### Per-field-type (14)

Each `*Field` has the same envelope shape: `Label` (above or inline depending on control) + control + (optional `HelpText` below). There is **no reserved space for an error message** — error rendering is the consumer's responsibility.

Common props on every `*Field`: `Label` (string), `HelpText` (string), `bool Error`, `bool Required`, `bool Disabled`, `Id` (string). Each then surfaces the wrapped primitive's own parameters.

`Error` is a **`bool` styling flag**, not a message: when `true` the field's control border, label colour, and any associated focus-ring switch to the danger/error visual state defined by Blok's semantic tokens. The same flag applies to `FormLabel` so a consumer composing a custom control can still get a coordinated error-styled label. The consumer renders the actual error *message* themselves — typically a small `<p class="text-danger text-sm">…</p>` or a `Callout` chunk — wherever they want it, including outside the field if appropriate.

| Chunk | Wraps | Field-specific notes |
|---|---|---|
| `TextField` | `Input` | Enum `Type = Text|Email|Number|Tel|Url`. Surfaces `Value`, `ValueChanged`, `Placeholder`. |
| `PasswordField` | `Input` (type=password) | Adds show/hide toggle button. Surfaces `Value`, `ValueChanged`. |
| `TextAreaField` | `Textarea` | Surfaces `Rows`, `Resize`. |
| `SelectField` | `Select` | Surfaces `Items`, `Value`, `ValueChanged`, `Placeholder`. |
| `ComboboxField` | `Combobox` | Surfaces all Combobox parameters. |
| `CheckboxField` | `Checkbox` | Inline (label-right) layout; supports tri-state. |
| `RadioGroupField` | `RadioGroup` | Vertical group; label-above layout. |
| `SwitchField` | `Switch` | Inline (label-right) layout. |
| `ToggleField` | `Toggle` | Inline. |
| `ToggleGroupField` | `ToggleGroup` | Horizontal multi-button group. |
| `DateField` | `DatePicker` | |
| `TimeField` | `TimePicker` | |
| `SliderField` | `Slider` | Surfaces min/max/step/value. |
| `SearchField` | `SearchInput` | For search-as-form-field cases (filter forms). |

#### Composite

| Chunk | Purpose | Key API |
|---|---|---|
| `InlineForm` | Single-row form for search-and-go / subscribe / quick-add. | Props: `OnSubmit`. ChildContent. |
| `WizardShell` | Stepper header + per-step body + Back/Next footer. | Props: `Steps`, `CurrentStep`, `CurrentStepChanged`. Slot: `StepContent`. Scoped service `WizardState` (see §10.2). |
| `SearchBar` | Toolbar pattern: `SearchInput` + filter slot + result-count slot. | Props: `Query`, `QueryChanged`, `ResultCount`. Slot: `Filters`. |
| `FilterBar` | Horizontal filter chip row. | Slots: `Filters`, `ClearAction`. |
| `LoginForm` | Opinionated login template (email/password/submit + footer slot). | Props: `OnSubmit`, `Title`, `Description`. Slots: `Footer`. |
| `ConfirmDialog` | `Dialog`-based confirm-action wrapper. | Props: `Title`, `Message`, `ConfirmLabel`, `CancelLabel`, `Tone` (shared `Tone` enum — typically `Danger` or `Warning`), `Open`, `OpenChanged`, `OnConfirm`. |

### 7.6 Data/ — 11

| Chunk | Purpose | Key API |
|---|---|---|
| `DataPage` | Full data-table page. | Internally composes `PageHeader` + `DataToolbar` + table area + `DataPagination`. Props: `Title`, `Description`. Slots: `Toolbar`, `Table`, `Pagination`. |
| `DataToolbar` | Search + filter + view-switcher + bulk-action row above a table. | Slots: `Search`, `Filters`, `ViewSwitcher`, `Actions`. |
| `DataPagination` | Pre-styled pagination row using the `Pagination` primitive. | Props: `Page`, `PageChanged`, `PageSize`, `TotalItems`. |
| `DetailPage` | Record detail layout. | Props: `Title`, `Description`. Slots: `Main`, `Aside`, `Footer`. |
| `SettingsPage` | Side-tabs + content area. | Props: `Tabs`, `SelectedTab`, `SelectedTabChanged`. Slot: `Content`. |
| `ResultsList` | Vertical list of result-cards with selection, optional pagination, and EmptyState fallback baked in. | Props: `Items<T>`, `Selected`, `SelectedChanged`, `Pageable`, `Page`, `PageChanged`, `PageSize`, `TotalItems`. Slot: `ItemTemplate`, `Empty`. When `Pageable=true`, internally renders `DataPagination` below the list. |
| `KvList` | Label/value definition list. | Items: `KvList.Item Label Value`. ChildContent: `KvList.Item`. |
| `BulkActionBar` | Selection-driven action bar appearing above a table when rows selected. | Props: `SelectedCount`, `OnClear`. Slot: `Actions`. |
| `RowActions` | Table-row dropdown action menu pattern. | Slot: `Items` (DropdownMenuItem children). Props: `IconSvg` (defaults to MoreHorizontal). |
| `FilterChip` | Single removable filter chip with label and ✕. | Props: `Label`, `OnRemove`, `IconSvg`. |
| `EmptyTable` | Empty state inside a `Table` aware of column count. | Props: `ColumnCount`, `Message`, `IconSvg`. Slot: `Action`. |

### 7.7 Marketplace/ — 5

Shells for all five Sitecore Marketplace extension points. Three (`CustomField`, `ContextPanel`, `DashboardWidget`) impose layout constraints from the host; two (`FullScreen`, `Standalone`) have no chrome constraints but are included for completeness — they let consumers express *which* extension point they're targeting at the call site, give the Catalogue a place to document each host context, and provide a hook for any future host-specific tweaks Sitecore might introduce.

| Chunk | Host context | Constraints | Key API |
|---|---|---|---|
| `MarketplaceCustomFieldShell` | XMC Page Builder Custom Field dialog | Limited width and height; consumer fits the dialog. | Props: `Title` (optional). Slots: `Header` (optional), `Body`, `Actions`. |
| `MarketplaceContextPanelShell` | XMC Page Builder left context panel | `max-w-[600px]`, full-height column, vertically scrollable body. | Slots: `Header`, `Body` (scrollable), `Footer`. |
| `MarketplaceDashboardWidgetShell` | XMC Dashboard widget grid cell | Card-styled, fits the dashboard widget's allotted cell. | Props: `Title`. Slots: `Header` (Title text + actions slot), `Body`, `Footer`. |
| `MarketplaceFullScreenShell` | XMC Sites top-bar nav (full-screen iframe) | None — the host fills the viewport. | `ChildContent` only. Sets `min-h-screen` and Blok background/font defaults so the shell composes cleanly inside the iframe. |
| `MarketplaceStandaloneShell` | Cloud Portal homepage launch (new tab) | None — the app owns the whole tab. | `ChildContent` only. Same defaults as `MarketplaceFullScreenShell`; differs only in the Catalogue host-context documentation it carries. |

Each Marketplace Catalogue page includes a `<HostContextNote>` block (analogous to `<DivergenceNote>` on Primitives) describing the host environment the shell targets.

## 8. Catalogue integration

- New top-nav link **Chunks** in `Catalogue/Components/Layout/MainLayout.razor`, sitting alongside the existing **Primitives** link.
- New route `/chunks` with an Index page listing the seven families and linking to per-Chunk pages.
- Each Chunk page lives under `Catalogue/Components/Pages/Chunks/<Family>/<ChunkName>Page.razor`.
- Pages reuse `<ComponentPage Title=… Description=… Interactivity=… ApiElements=…>` exactly as Primitives do.
- Each Chunk page **must explicitly declare** `Interactivity="ComponentInteractivity.Ssr"` or `Interactivity="ComponentInteractivity.Interactive"` — this is a hard convention, not a default.
- The Chunks left-nav mirrors the Primitives left-nav in visual treatment, grouping Chunks by family with the family name as the group label.

## 9. Dependencies and integration with the existing library

- Chunks live in `PINGWorks.SitecoreBlok.BlazorUI/Components/Chunks/<Family>/`.
- Shared types (`BreadcrumbItem`, `TabDefinition`, `KvList.Item`, etc.) live alongside their Chunk's family folder, not in a global types folder, unless they're reused across families.
- Any new scoped services (e.g. `WizardState`) register through the existing `AddSitecoreBlokUI()` extension method in `Ioc/`. Over-registration is acceptable; services have library-unique interfaces.
- `_Imports.razor` in the Chunks folders includes the necessary Primitive namespaces.
- Chunks emit literal Tailwind class strings only, assembled via `CssClassBuilder` exactly the way `Stack.razor` does today. No runtime string concatenation of Tailwind utilities.
- Chunks are **not** added to `MIGRATION_STATUS.md`. The `/blok` audit skill skips them — they're "Extras" with no upstream Blok source.

## 10. Cross-cutting implementation notes

These are the non-obvious decisions an implementer needs flagged. Each is small enough to resolve during implementation but big enough to be worth preserving in the spec.

**10.1. AppShell render-mode forwarding.** The existing `Catalogue/Components/Layout/MainLayout.razor` mounts `<Popovers @rendermode="InteractiveServer" />` and `<Toaster @rendermode="InteractiveServer" />` because those primitives need interactivity. `AppShell` cannot hard-code the render mode (per convention 5.6). **Approach:** `AppShell` exposes `[Parameter] public IComponentRenderMode? InteractiveRenderMode { get; set; }` and applies that mode to the internally-rendered `Popovers` and `Toaster`. Consumers pass their chosen mode (typically `RenderMode.InteractiveServer` or `RenderMode.InteractiveAuto`) at the call site. Default is `null` (no render mode — inherits from the consumer's surrounding context, which fails fast if the host isn't already interactive — desired).

**10.2. Parent/child orchestration — CascadingValue vs service.** When a Chunk family has parent/child orchestration (e.g. `WizardShell` ↔ step content, `ListDetailShell` ↔ list/detail selection):

- **Use `CascadingValue`** when the orchestration state is either a JSON primitive (`int`, `string`, `bool`, `double`, etc.) or an object whose properties are all primitives only one tier deep (`record (int Index, bool IsLast, bool IsFirst)`). Blazor's change detection is reliable for these shapes.
- **Use a scoped service** when state is anything more complex — collections, nested objects, dictionaries, anything where a property is itself a reference type. Blazor's change detection on cascading values is reference-equality-based for non-primitives, which produces missed renders and stale UI.

For `WizardShell` specifically: if the wizard's orchestration state is just `int CurrentStep` (and the consumer owns the step list), `CascadingValue<int>` is fine. If the wizard also tracks a step-completion array or per-step validation state, register a scoped `WizardState` service through `AddSitecoreBlokUI()`. Implementation plan picks based on the actual state shape it ends up needing.

**10.3. Tailwind literal scanning.** Every Chunk that varies styling by enum must use `CssClassBuilder.With("literal-class", condition)` — never `$"text-{size}"`-style interpolation. The `Stack.razor` pattern (with its many explicit `gap-1`/`gap-2`/… branches) is the model.

**10.4. Naming collisions.** Some Chunk names (`Container`, `Hero`, `Toolbar`, `Callout`, `BackLink`) are common across UI libraries. Consumers may need fully-qualified using statements or aliases if they import multiple libraries. Documenting this in the Catalogue Index page is sufficient.

**10.5. Marketplace Catalogue host-context notes.** A new `<HostContextNote>` shared component (analogous to the existing `<DivergenceNote>`) appears on each Marketplace Chunk page describing the XMC/Cloud Portal host context — what the surrounding chrome looks like, what dimensions are imposed, and what behaviour the Chunk does/doesn't handle (e.g. "consumer is responsible for handling the postMessage SDK initialisation").

**10.6. Coverage check against the existing Catalogue.** A dogfood pass during implementation should verify that the following Catalogue files *could* be expressed using only Chunks (the actual refactor is out of scope, but the verification is part of the plan):
- `Catalogue/Components/Layout/MainLayout.razor` — should reduce to `AppShell` + `AppHeader` + `AppBrand` + nav + theme toggle.
- `Catalogue/Components/Layout/NavMenu.razor` — should reduce to `AppSidebar` + `NavList` + `NavGroup` + `NavListItem`.
- `Catalogue/Components/Shared/ComponentPage.razor` — should be expressible via `PageShell` + `PageHeader` + `PageContent` + `ContentSection`.

If a Chunk is missing for any of these reductions, the roster is wrong and must be amended.

## 11. Risks

1. **Tailwind literal-class scanning regressions.** Easy to slip into runtime string concatenation; `CssClassBuilder` discipline is the only guard. Mitigation: every Chunk reviewed for literal-only class output.
2. **Render-mode hard-coding.** Easy to slip an `InteractiveServer` directive into a Chunk that should be neutral. Mitigation: explicit convention 5.6, plus a code-review gate.
3. **Surface-area sprawl.** 84 Chunks is a lot. A pre-commit sprawl audit dropped three Chunks that were only subtly different from siblings (`PageTitle` → folded into `PageHeader` with empty slots; `Stat` → folded into `KpiTile` minimal mode; `PaginatedList` → folded into `ResultsList` with a `Pageable` flag). Mitigation: every Chunk has a one-line purpose and an explicit "would removing this make a real authoring task meaningfully harder?" justification — bar applied at design time, applied again during implementation if anything new feels redundant.
4. **Future Bloks-port collision.** Chunk names that overlap with upstream Blok bloks (e.g. `DashboardWidget`) could clash. Mitigation: Chunks live in `Components/Chunks/`; Bloks port will live in `Components/Bloks/` (or similar) — different namespace path makes co-existence trivial.
5. **API drift across the 25 Forms chunks.** With 14 per-field-type variants, surface consistency matters. Mitigation: `*Field` envelope shape (Label + control + optional HelpText, plus a `bool Error` styling flag with no reserved message region) is fixed; each Chunk only varies in surfaced primitive parameters.
6. **Enum drift across Chunks.** The risk that Chunk authors create per-Chunk variants of `Tone`, `Density`, `Direction`, etc. instead of reusing the shared enums (§5.10), or duplicate the enum-to-Tailwind switch instead of calling the shared helper (§5.12). Mitigation: §5.10 enumerates the canonical shared set; §5.12 mandates the helper-class pattern; Chunk PR review confirms neither parallel enums nor inlined switches have crept in. Existing Primitive enums are out of scope and untouched.
7. **`*Field` Touched-tracking subtleties.** Required-empty-on-touch (§5.11) is internal to each `*Field`. Edge cases: programmatic value changes (does that count as touched? — no), form-reset semantics (consumer's responsibility), bound `Value` flickering during async loads (Touched should not auto-reset). Mitigation: implementation plan defines a single `FieldTouchedTracker` helper used by every `*Field`; behaviour documented in each Catalogue page.

## 12. Out-of-scope reminders (consolidated)

- Sitecore Blok bloks port (separate future namespace).
- Catalogue refactor of `MainLayout` / `NavMenu` / `ComponentPage` (follow-up task; coverage verified during implementation).
- Generic `FormField` (replaced by per-type `*Field` chunks plus `FormLabel`).
- State / async-orchestration chunks.
- Per-Chunk drift-checking against upstream Blok (Chunks have no upstream — they're Extras). However, the `/blok` skill does cross-reference Chunks against changed Primitives per §5.9.

## 13. Acceptance criteria

This design is ready for an implementation plan when:

- The 7 family folders (plus `Shared/`) exist under `Components/Chunks/`.
- All 84 Chunks are present per §7, each conforming to the API conventions in §5.
- All 84 Chunks have a corresponding Catalogue page under `Catalogue/Components/Pages/Chunks/<Family>/`, each declaring `Interactivity` explicitly per §5.7.
- The seven shared-enum helper classes (§5.12) exist under `Components/Chunks/Shared/`.
- The `/blok` audit skill cross-references Chunks against changed Primitives per §5.9.
- `MainLayout.razor` (Catalogue) has a "Chunks" top-nav link routing to `/chunks`.
- The Chunks Index page at `/chunks` lists the 7 families with per-Chunk links.
- All required services register through `AddSitecoreBlokUI()`.
- A dogfood pass confirms `MainLayout.razor`, `NavMenu.razor`, and `ComponentPage.razor` *could* be refactored to consume only Chunks (refactor itself is out of scope).
- No row added to `MIGRATION_STATUS.md`.
