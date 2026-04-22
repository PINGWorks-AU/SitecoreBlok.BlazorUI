---
name: blok-migration
description: Migrate, update, audit, or verify Sitecore Blok design system components in the PINGWorks BlazorUI library. Handles the full lifecycle — porting a Blok primitive, re-syncing with upstream changes, and running the UI parity harness against single components or the whole library.
trigger: /blok
---

# /blok

Migrate components from the Sitecore Blok design system (React/shadcn) to idiomatic Blazor, audit existing components against the latest Blok source for changes, or verify UI parity of one or all components using the `tools/verify-ui-parity.ps1` harness and apply fixes for any findings.

## Usage

```
/blok migrate <component-name>        # migrate a new component from Blok to BlazorUI
/blok update <component-name>         # re-audit an existing component against current Blok source
/blok audit                           # scan for new/changed components in the Blok registry
/blok catalogue <component-name>      # create or update the catalogue page only
/blok verify <component-name>         # run the parity harness for one component, fix findings
/blok verify all                      # run the parity harness for every primitive, fix findings
```

### Natural-language equivalents

These phrases map to `/blok update`:
- "update / re-audit component **X** against Blok" → `/blok update X`
- "add / replicate / match Blok's [props|attributes|API|interface|surface] for **X**" → `/blok update X`
- "bring **X** closer to Blok" → `/blok update X`
- "**X** is missing [prop] that Blok supports" → `/blok update X`
- any change to a Blok component's parameters, behaviour, or API to align with upstream Blok source → `/blok update <component>`

These phrases map to `/blok verify`:
- "verify the ui of component **X** against Blok" → `/blok verify X`
- "check ui parity of **X**" → `/blok verify X`
- "check component **X** against Blok" → `/blok verify X`
- "check ui parity for all components against Blok" → `/blok verify all`
- "verify all components against Blok" → `/blok verify all`

## Source References

- **Blok registry:** `https://blok.sitecore.com/r/{name}.json` — canonical component source
- **Blok primitives page:** `https://blok.sitecore.com/primitives/{name}` — visual reference and examples
- **Blok repo:** `https://github.com/Sitecore/blok` — full source including CSS tokens
- **`MIGRATION_STATUS.md`** — repo-root snapshot of every component's status and the last Blok commit SHA we evaluated it against. Read this first for any `/blok audit` or `/blok update` run; update it whenever you finish one.

## Project Structure

- **Library:** `PINGWorks.SitecoreBlok.BlazorUI/`
  - Components: `Components/{Family}/{Name}.razor` — grouped by component family (see Component File Rules in Phase 2). Example: `Components/Avatar/Avatar.razor`, `Components/Avatar/AvatarImage.razor`, `Components/Avatar/AvatarFallback.razor`. Standalone components also get their own single-file folder (`Components/Button/Button.razor`).
  - Extra: `Components/Extra/{Name}/{Name}.razor` — Blazor-side additions with no Blok equivalent, each in its own subfolder (e.g. `Components/Extra/ThemeToggle/`).
  - Enums: `Enums.cs` (one section per component)
  - Icons: `IconSvg.cs` (MDI SVG paths)
  - Illustrations: `IllustrationSvg.cs` (inline SVG with Color/Neutral variants)
  - Services: `Services/` (PopoverService, ToastService)
  - DI: `Ioc/IServiceCollectionExtensions.cs` (exposes `AddSitecoreBlokUI`)
  - CSS: `wwwroot/css/blok/` (Tailwind theme tokens)
  - JS: `wwwroot/js/sitecoreUI.js` (element bounds, window bounds)
- **Catalogue:** `PINGWorks.SitecoreBlok.BlazorUI.Catalogue/`
  - Pages: `Components/Pages/Primitives/{Name}Page.razor`
  - Shared: `ComponentPage.razor`, `ComponentExample.razor`, `DivergenceNote.razor`, `InstallationNote.razor`, `ComponentInteractivity.cs` (enum `Interactive` | `Ssr`)
  - Nav: `Components/Layout/NavMenu.razor` — alphabetical list auto-built from `MigrationStatusService.Ported` plus a small `Stubs` array. Parity entries render without a badge (the default); `Improved` uses `BadgeColor.Blue`, `Additional` uses `BadgeColor.Primary`.
  - Index: `Components/Pages/Primitives/Index.razor` — `/primitives` grid, auto-built from `MigrationStatusService.Ported` + `Stubs`. Cards show the `Description` from `MIGRATION_STATUS.md` and the same `Improved` / `Additional` badges as the NavMenu.
  - Home: `Components/Pages/Home.razor` — "Available Components" pulls from `MigrationStatusService.Ported` + `Stubs`; "Backlog" pulls from `.Backlog`; "Won't Do" renders a table from `.WontDo` (Description column becomes the Reason cell).
  - Status: `Services/MigrationStatusService.cs` — parses `MIGRATION_STATUS.md` (linked from the repo root via the `.csproj`) at startup. Exposes `Ported` / `Backlog` / `WontDo` for Razor components to bind against. Registered as a singleton in `Program.cs`.
- **Icons:** `PINGWorks.SitecoreBlok.BlazorUI.Icons/` — full MDI + illustration + favicon sets in a separate package (`PINGWorks.SitecoreBlok.BlazorUI.Icons` namespace)

## Code Style Preferences
- Observe formatting instructions in the `.editorconfig` file in the root of the project if available
 
In the absence of an `.editorconfig` file, or where the file does not provide sufficient formatting guidence, use these rules:
- **Use TABS, not spaces, for indentation in every new or edited file** — `.razor`, `.cs`, `.csproj`, `.css`, `.js`, `.json`, and solution-level config. This is non-negotiable and applies to files you create from scratch as well as any section you edit inside an existing file. If an existing file is mixed, prefer converting the whole file rather than leaving mixed indentation. The repo's `.editorconfig` enforces this for `.cs` — apply the same rule to all other source files. Never introduce 2-space or 4-space leading indentation in new content.
- use PascalCase for all class members
- in `.razor` files prefer `@code{}` block to code behind file, except in cases of more than 5 methods that are
  not directly bound to UI elements. All methods, fields and properties bound to UI elements must be present in
  the `.razor` file. Additional methods added to keep the code clean and readable can be moved to a code behind
  file where it aids clarity. Partial class specs where a Regex is being compiler-generated necessitate code behind.
- prefer child classes for ViewModels and other supporting types where the only use of the type is internal to the
  component
- always put `@` directives at the beginning of `.razor` files with the exception of `@code{}` which must go at the bottom
- for `.razor` files that have supporting code-behind files add `[Inject]` properties in the code-behind where an injected
  service is predominantly used by code in the code-behind rather than in the `.razor`; and use `@inject` in the `.razor`
  where the injected service is predominantly used in the markup
- specify `@` directives each on a new line, in the order `@namespace`, `@page`, `@layout`, `@typeparam`, `@attribute`, `@implements`, `@inject`, `@rendermode` — `@namespace` ALWAYS comes first (see Component File Rules in Phase 2)
- where property values are required for correct operation of a component, specify the property as `public required` and apply
  the attributes `[Parameter, EditorRequired]` in a `@code{}` block

## Migration Workflow

### Phase 1: Source Analysis

Before writing any code, thoroughly analyse the Blok source:

1. **Fetch the registry JSON** — `https://blok.sitecore.com/r/{name}.json`
2. **Extract the complete TSX source** — every export, prop, CSS class string, variant, compound variant
3. **Document every visual element:**
   - Container: border, shadow, padding, border-radius, background
   - Interactive states: default, hover, active/pressed, focus-visible, disabled, open/closed
   - Borders: which elements have them, color, width, radius
   - Backgrounds: per-state colors including dark mode variants
   - Shadows: elevation levels, hover transitions
   - Animations: transitions, durations, timing functions
   - Typography: font size, weight, color per element
   - Icons: which MDI icons are used, sizes
4. **Identify all exports** — each exported function becomes a separate Blazor component
5. **Identify dependencies** — does it use Radix primitives? Popover? Other components?
6. **Check the visual reference** — `https://blok.sitecore.com/primitives/{name}` for live examples

### Phase 2: Blazor Implementation

Apply these patterns strictly:

#### Component File Rules
- One `.razor` file per exported function (e.g. `CardHeader.razor`, `CardContent.razor`)
- **Group by component family in a subfolder under `Components/`.** The folder is named after the parent/root component; every sibling export that only makes sense inside that family lives in the same folder. Standalone components that have no siblings also get their own folder named after them. Never put loose `.razor` files directly under `Components/`.
  - `Components/Avatar/` → `Avatar.razor`, `AvatarImage.razor`, `AvatarFallback.razor`
  - `Components/Card/` → `Card.razor`, `CardHeader.razor`, `CardContent.razor`, `CardFooter.razor`, `CardTitle.razor`, `CardDescription.razor`, `CardAction.razor`
  - `Components/Button/` → `Button.razor` (standalone still gets its own folder)
  - `Components/Extra/ThemeToggle/` → `ThemeToggle.razor`, `ThemeToggleStartupScript.razor`, `ThemeToggle.razor.js` — colocated JS module lives with its component
  - `Components/Extra/TreeView/` → `TreeView.razor`, `TreeNode.razor`
- **Every `.razor` component MUST declare `@namespace PINGWorks.SitecoreBlok.BlazorUI` as its first line.** Without this directive Blazor derives the namespace from the folder path (e.g. `...BlazorUI.Components.Avatar`), which breaks the flat `@using PINGWorks.SitecoreBlok.BlazorUI` in `_Imports.razor` and makes every sibling reference fail to compile. The directive is non-negotiable on new files; when editing an existing file that's missing it, add it.
- Inline `@code` blocks (no code-behind `.razor.cs` files)
- UTF-8 without BOM. If a file inherited a BOM from a previous generation step, strip it — a BOM in the middle of a file (from prepending content above one that had a BOM) becomes a non-whitespace character that breaks directives like `@implements` and `@inject`.

**Canonical template for a new component file** (tabs, not spaces):

```razor
@namespace PINGWorks.SitecoreBlok.BlazorUI

@* any other directives: @inherits, @implements, @inject, @rendermode *@

<div data-slot="..." class="@CssClass" @attributes="AdditionalAttributes">
	@ChildContent
</div>

@code {
	[Parameter] public string? ClassName { get; set; }
	[Parameter] public RenderFragment? ChildContent { get; set; }
	[Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object>? AdditionalAttributes { get; set; }

	private string CssClass => CssClassBuilder.Start( "..." )
											.With( ClassName )
											.Build();
}
```

#### Structural Preservation Rules
**Mirror Blok's component structure unless it's literally impossible or idiomatically wrong in Blazor.** This is non-negotiable and the most common source of subtle bugs.

The rules:
- **Same number of exports** — if Blok exports `Avatar`, `AvatarImage`, `AvatarFallback` (3), create three `.razor` files with the matching names. Do NOT collapse them into a single component with `@if` switching even if it looks "simpler" — consumers cannot pass the sub-components as children, breaking demos like `<Avatar><AvatarImage src="..."/><AvatarFallback>RH</AvatarFallback></Avatar>`.
- **Same DOM element types** — `<span>` stays a `<span>`, `<div>` stays a `<div>`, `<button>` stays a `<button>`. Tailwind `*:` and `has-[]:` selectors and consumer CSS often depend on element type matching. Don't substitute element types for "Blazor reasons" unless Blazor has a hard requirement.
- **Same `data-slot` attributes** — names and presence must match Blok exactly. Consumer Blok demos target sub-elements with `[data-slot=…]` selectors and `*:data-[slot=…]:…` utilities.
- **Same children pattern** — if Blok renders children via `{children}` (i.e. `ChildContent` slot), use `RenderFragment? ChildContent` and let consumers pass real components. Don't replace consumer-passed sub-components with a `RenderFragment? Title` / `RenderFragment? Description` / `RenderFragment? Footer` parameter API just to "improve discoverability" — that breaks the Blok composition model.
- **Same wrapping** — don't add wrapper elements that Blok doesn't have, and don't collapse wrapper elements that Blok has. Both break CSS positioning, descendant selectors, and `has-[]` rules.

**When divergence IS justified** (rare):
- Blazor has no equivalent to a React-only feature (e.g. Radix `forwardRef`, `asChild`) — document the divergence in `docs/ui-parity-audit.md` with the specific Radix/React feature being substituted.
- Blok's pattern depends on a primitive we deliberately don't pull in (e.g. context-radix-portal-with-focus-trap) — document why.
- An idiomatic Blazor pattern would be objectively more correct (e.g. `EventCallback<T>` instead of an inline arrow function prop) — those are leaf-level API choices, not structural.

**Human approval is required before implementing any divergence — no exceptions.**

Before writing divergent code, STOP and present the following to the user in the chat:

1. **What diverges** — which component export, prop, DOM structure, or behaviour cannot match Blok, and what the Blok source does.
2. **Why** — the specific technical reason Blok's pattern cannot be faithfully reproduced (Radix primitive, React-only API, JS-only browser feature, etc.).
3. **UX or API impact** — what the consumer loses or has to do differently.
4. **Gap-closing opportunity** — is there a Blazor-native alternative that could close or narrow the gap (e.g. a Blazor state machine replicating a Radix primitive, JS interop replacing a browser-only hook)?
5. **Explicit yes/no question** — "Should I proceed with this divergence, or would you prefer I implement [gap-closing alternative] first?"

Wait for an explicit answer before writing any divergent code. **A `<DivergenceNote>` records a decision that has already been approved — it is not a substitute for getting approval.**

History shows that divergences introduced without human review tend to miss realistic implementation paths (NavigationMenu's shared viewport was skipped in favour of per-item popups when the correct behaviour was achievable; the gap was only caught during a later verify pass).

If you find yourself collapsing two Blok components into one Blazor component, **stop**. Either implement them as separate components, or surface the structural question to the user and wait for approval.

For state coordination between parent and children that Radix handles via React context, use `CascadingValue<TParent>` with the parent registering helper methods that children call. See `Avatar`/`AvatarImage`/`AvatarFallback` for the canonical pattern (parent tracks `ImageStatus`, image children update it on `@onload`/`@onerror`, fallback child reads it to decide whether to render).

#### Prefer RenderFragment over strings (Blazor-flexibility upgrade)
Where Blok's source accepts a string for a label/title/description (e.g. `Toast` props `title?: string` and `description?: string`), prefer `RenderFragment? ChildContent` (or a named `RenderFragment? Title` slot) so Blazor consumers can pass rich content — formatted spans, inline icons, kbd, links, status badges. This is **the only place where structural divergence from Blok is encouraged rather than tolerated**. The string-only API is a React/JSX limitation we don't share.

When you take this upgrade, you MUST:
- Surface the divergence on the Catalogue page using `<DivergenceNote>` (see below). Users moving between Blok and BlazorUI need to know the API differs.
- Record the divergence in `docs/ui-parity-audit.md` with status `⚠️ flexibility-upgrade` so the audit table reflects it.

#### Catalogue divergence annotations
When a Blazor component's API **deliberately differs from Blok's** in a way a user might trip over (additive parameter, RenderFragment instead of string, hardcoded internal markup, missing component split, behavioural difference), the Catalogue page MUST start with a `<DivergenceNote>` block (see `Catalogue/Components/Shared/DivergenceNote.razor`). Inside, list:
- What's different (parameter name, behaviour, or substituted slot)
- Why we did it (Blazor idiom, Blazor-side flexibility, missing primitive, etc.)
- What the user has to do differently from Blok

Example: Alert's `Closeable` parameter is a Blazor-only addition; AlertPage's `<DivergenceNote>` calls it out at the top of the page.

Do NOT use `<DivergenceNote>` for cosmetic deviations or token naming differences (e.g. `bg-background` vs `bg-body-bg` is an equivalence, not a divergence). Reserve it for API shape differences a user can see in their own code.

#### Catalogue cross-reference pages (renamed-from-Blok components)
When a Blok component name **doesn't match** the BlazorUI name (because we renamed for Blazor convention or because Blok wraps a JS-only library), create a **stub page under the Blok name** that points to our equivalent. Pattern: see `SonnerPage.razor` (Blok's `Sonner` → BlazorUI's `Toaster`). The stub:
- Uses the Blok name as the page title
- Has a `<DivergenceNote>` saying "Called X in BlazorUI" with a link to the actual page
- Has no `<ComponentExample>` blocks
Register the stub in the `Stubs` arrays of both `NavMenu.razor` and `Home.razor`, and in `Index.razor`'s `BuildGrouped()` so users coming from Blok can find it. Stubs don't go into `MIGRATION_STATUS.md` — they're Catalogue-side redirects, not migration entries. The actual implementation page also gets a `<DivergenceNote>` explaining the renaming and (if relevant) the API differences.

#### Components in `Components/Extra/`
Any component under `Components/Extra/{Name}/` is a Blazor-side addition with **no Blok equivalent**. Same folder-per-family rule as top-level components: each Extra component lives in its own subfolder (`Extra/Stack/Stack.razor`, `Extra/ThemeToggle/ThemeToggle.razor` + colocated `ThemeToggle.razor.js`, `Extra/TreeView/TreeView.razor` + `TreeView/TreeNode.razor`). Same `@namespace PINGWorks.SitecoreBlok.BlazorUI` directive on the first line.

Its Catalogue page MUST start with a `<DivergenceNote>` saying:
- "Blazor-side addition; no equivalent in the Blok design system."
- Why we ship it (real product need, ergonomic helper, Blazor-only concern)
- Whether the styling tracks Blok tokens (so it sits naturally with ported primitives)

Examples already in place: `StackPage`, `TextPage`, `ThemeTogglePage`, `TreeViewPage`, `CodeViewerPage`. Use these as templates.

#### Parameter Rules
- `[Parameter] string? ClassName` — ONLY if the source accepts `className` and the HTML emits it
- `[Parameter] RenderFragment? ChildContent` — ONLY if the source renders children
- `[Parameter(CaptureUnmatchedValues = true)] Dictionary<string, object>? AdditionalAttributes` — ONLY on leaf elements that forward `{...props}`
- Never add these parameters by default — each must be justified by the source

#### Styling Rules
- Use `CssClassBuilder.Start(...).With(class, condition).Build()` for all class composition
- **Class names MUST be full literal strings.** Never assemble Tailwind utilities from variables (e.g. `$"bg-{color}-500"`, `$"text-{size}"`). The Tailwind CLI only scans source for full literal class names — composed ones are invisible to it, so the corresponding CSS is never generated even when the runtime value is a valid utility. For conditional classes, use `.With("literal-class", condition)` on `CssClassBuilder`, or a ternary between two literal strings: `$"{(isActive ? "text-primary" : "text-muted-foreground")} {ClassName}"`. The parity harness (`tools/verify-ui-parity.ps1`) catches violations.
- **Icon renders the `<svg>` directly** (no wrapper). Both descendant (`[&_svg]`, `has-[svg]`) and direct-child (`[&>svg]`, `has-[>svg]`) selectors work. Either is fine; prefer the form Blok's source uses to minimise drift. When you need to rotate/transform the icon, pass the transform classes in `ClassName` — they land on the same `<svg>` the rotation selector targets, so transitions animate.
- **Pair theme-aware surface backgrounds with an explicit text token in the same class string.** When a class string sets `bg-background`, `bg-card`, `bg-popover`, `bg-muted`, `bg-accent`, `bg-primary`, `bg-secondary`, or `bg-destructive` (unprefixed — i.e. the default state), it MUST also set a matching `text-*` token. Without the pairing the foreground colour relies on CSS inheritance from the body, which silently breaks for fixed-positioned and portal-rendered content (Dialog, AlertDialog, Sheet, Toast) — dark-mode text comes out the wrong colour. Common pairings: `bg-background text-foreground`, `bg-card text-card-foreground`, `bg-popover text-popover-foreground`, `bg-primary text-white`. The parity harness flags violations under Check 4. For genuinely decorative surfaces (slider tracks, progress fills, indicator dots, icon-only buttons, table wrappers whose cells set their own text), add the marker class `parity-no-text-pair` to the same class string and a one-line comment explaining why.
- Add `data-slot="{name}"` matching the source's data-slot values
- CSS classes must match the source exactly including:
  - Hover states (`hover:bg-*`, `hover:text-*`)
  - Focus states (`focus-visible:ring-*`, `focus-visible:border-*`)
  - Dark mode (`dark:bg-*`, `dark:text-*`)
  - Data attribute states (`data-[state=open]:*`, `data-[state=checked]:*`)
  - Compound variants (variant + color combinations)
- Use semantic tokens (`text-primary-fg`, `bg-neutral-bg`) not numbered palette values (`text-primary-600`)
- Check `globals.css` for missing tokens — add any the source uses that we don't have

#### Scrollable Surface Rules (consistent scrollbar styling + no-clip guarantee)
Any time a component you port has an internal scrolling region — sidebar rails (StackNavigation, Sidebar), ScrollArea viewports, popover menus with item lists (DropdownMenu, Combobox), long-table wrappers, sheet/dialog bodies, code blocks, calendar month grids — apply this triage BEFORE shipping. Blok frequently uses bare `overflow-auto`, which is lowest-effort but gives the chunky 16px native scrollbar, may trigger an unwanted horizontal bar, and often clips `min-w-*` children. Our library ships a consistent thin-scrollbar treatment and we want every new scroll surface to match it.

**1. The canonical scrollbar look** — matches the `ScrollArea` primitive. Use these utilities + inline style verbatim unless the component has a real reason to diverge (document in `docs/ui-parity-audit.md` if it does):

```razor
class="... [&::-webkit-scrollbar]:w-1.5 [&::-webkit-scrollbar-track]:bg-transparent [&::-webkit-scrollbar-thumb]:rounded-full [&::-webkit-scrollbar-thumb]:bg-border [&::-webkit-scrollbar-button]:hidden [&::-webkit-scrollbar-button]:!h-0 [&::-webkit-scrollbar-button]:!w-0"
style="scrollbar-width:thin;scrollbar-color:var(--border) transparent;"
```

- `w-1.5` (6px) is the rail-friendly default. Wider surfaces (ScrollArea viewports, sheet bodies) may use `w-2.5` (10px) to match the `ScrollArea` reference. Don't pick arbitrary widths — align to one of these two.
- The thumb is always `bg-border rounded-full`, track always `bg-transparent`, scrollbar buttons always suppressed.
- The inline Firefox style is required because Tailwind v4 exposes no utility for `scrollbar-width` / `scrollbar-color`. Don't move it to the class string — it won't work.

**2. Horizontal scrollbar suppression** — never ship a surface where both axes scroll unless the primitive really needs 2D scroll (large tables, image zoom). For the vertical case: `overflow-y-auto overflow-x-hidden`. For the horizontal case (navbars, carousels): `overflow-x-auto overflow-y-hidden`. Blok's `overflow-auto` is almost always wrong for our narrow rails and popups — it's a low-effort shadcn default, not a deliberate 2D-scroll design decision. Fixing it is a strict UX improvement and the harness (Check 3) won't flag it as drift because you're **adding** classes, not removing Blok's.

**3. The no-clip scrollbar-gutter pattern** — use this whenever the scrollable region is narrow enough that the scrollbar would overlap `min-w-*` or fixed-width children (any navigation rail, any narrow popover, any `w-14`-to-`w-32` range surface where children carry a `min-w-*`). Three pieces, all load-bearing:

```csharp
// On the scrollable body:
class="... flex-1 overflow-y-auto overflow-x-hidden -mr-1.5 [&::-webkit-scrollbar]:w-1.5 ..."
style="scrollbar-gutter:stable;scrollbar-width:thin;scrollbar-color:var(--border) transparent;"
```

- `-mr-1.5` — negative right margin equal to half the scrollbar width, extending the viewport into what was the parent's right padding. The scrollbar sits at the outer edge of the rail instead of crowding content. Match the value to your scrollbar width (use `-mr-1.5` with `w-1.5`, `-mr-2.5` with `w-2.5`).
- `scrollbar-gutter:stable` — reserves the gutter always, so the layout doesn't jump when content transitions between scrollable and not. Without this, the `-mr-*` causes items to widen when there's no scrollbar and narrow when there is.
- `overflow-y-auto overflow-x-hidden` — keeps the horizontal scrollbar suppressed.

Anti-revert comment: when you apply this pattern, write a comment on the computed class property calling out all three pieces as load-bearing ("DO NOT remove — each one is required for the no-clip guarantee"). A well-intentioned future editor may assume one of them is cosmetic and reintroduce the clip bug.

**4. When you CAN skip this** — surfaces where every child is fluid-width (`w-full`, no `min-w-*`) and the default 16px native scrollbar wouldn't clip anything. Plain body text in a dialog, card content, tooltip — apply the canonical scrollbar look for consistency but skip the `-mr-*` / `scrollbar-gutter` pieces.

**5. Parity with Blok** — Blok's `overflow-auto` is not deliberate. It's the shadcn-via-tailwind default. Picking `overflow-y-auto overflow-x-hidden` with the scrollbar-gutter pattern is a Blazor-side UX polish, not a behavioural divergence. Document under "Deliberate deviations" in the component's audit row. The harness Check 3 only flags missing Blok tokens, so adding the scroll utilities is silent there.

#### Composition Rules
- Parent-child communication: `CascadingValue`/`CascadingParameter` with Register/Unregister pattern
- DO NOT use `IsFixed` on CascadingValue if child components need to re-render when parent state changes
- State binding: `Value`/`ValueChanged` for inputs, `Open`/`OpenChanged` for modals, `Checked`/`CheckedChanged` for toggles
- Events: `EventCallback` for actions, not `Action` delegates

#### Enum Rules
- Define in `Enums.cs` under a comment header `// ComponentName ***...`
- One enum per concept: `{Component}Variant`, `{Component}Size`, `{Component}Color`
- Record types for data models (e.g. `StepperStep`)

#### JS Interop Rules
- Avoid JS where possible — prefer CSS-only solutions (e.g. Tooltip uses CSS hover, not JS)
- For positioning: use `SitecoreUI.getElementBounds` from `sitecoreUI.js`
- For floating UI: use fixed positioning with JS-calculated coordinates (like DropdownMenu, ContextMenu)
- Accordion/Collapsible animation: use `grid-template-rows: 0fr/1fr` transition, NOT Radix keyframes

#### Service Rules
- Floating UI (popovers, selects, date pickers): register with `PopoverService`
- Toast notifications: use `ToastService`
- New services: register in `IServiceCollectionExtensions.cs`

#### Complex Component Patterns

Components with live state coupling between parent and popup children (Combobox, typeaheads, multi-select with filter) are a distinct class from simple open/close primitives (Select, DatePicker, DropdownMenu-as-menu). Before implementing, answer these questions. Revisions during Combobox migration traced back to one of these being missed.

**1. Popup rendering strategy — `PopoverService` or in-place?**

| Strategy | When to use | Examples |
|---|---|---|
| `PopoverService` + `<Popovers>` host | One-shot popup; user picks a value and it closes | Select, DatePicker, DropdownMenu with static items |
| In-place `position:fixed` + computed coords | Popup's children must re-render as the parent's state changes (filter text, highlighted item, dynamic list content) | Combobox, DropdownMenu (for highlight tracking), filterable pickers |

The `PopoverService`/`<Popovers>` host renders captured RenderFragments and does NOT re-render when the originating component's state changes. That's fine for "click item → close" flows but breaks any flow where the popup needs to react to state updates in the parent (live filtering, real-time highlight, keyboard nav). For those, mirror the `DropdownMenu` pattern: JS-compute `X`/`Y` bounds on Open, store on the parent, render the popup directly in the parent's render tree with `fixed` + inline `left:{X}px;top:{Y}px;` style, guarded by `@if (IsOpen)` for the backdrop + always-mounted for the content (see pattern 2).

**2. Always-mount vs `@if`-guard the popup content**

If the popup tracks per-item metadata (labels, display names, cached option data) that MUST survive open/close cycles, render the popup DOM always and hide with `display:none` (via `style`) when closed. Items' `OnInitialized` fires once and their `Register()` call populates a `Dictionary` on the parent that outlives the popup's visibility. Guard only the backdrop + transient chrome with `@if (IsOpen)`.

If instead items are stateless (just rendered labels, no parent-side registration), `@if (IsOpen)` on the whole popup is fine.

Combobox needs always-mounted because: (a) post-selection the input must display the selected item's label even when popup is closed; (b) the multi-select chips must display labels by value even when items have been unmounted; (c) a pre-selected Value on a disabled combobox must still resolve to its label.

**3. Post-register re-render for parent-consumed child state**

When the parent component renders content whose value depends on state a child populates in `OnInitialized` (typically via `Register()`), the parent's first render runs BEFORE the children's OnInitialized fires. The parent renders with stale state; by the time children register, the output is already flushed.

Pattern: add a `bool ChildrenDirty` flag. `Register()` sets it to true. Override `OnAfterRenderAsync` to observe the flag, clear it, and call `InvokeAsync(StateHasChanged)`. The second render sees the populated state. Bounded — second render doesn't set the flag again because `Children.Contains(item)` is true on re-register.

This pattern applies wherever a parent computes display content from a cache populated by children. Combobox uses it for `LabelCache`, `ComboboxList` uses it for its `Children` order list.

**4. Three-state filter input semantics**

For filterable text inputs where the user can also clear the input to explicit empty:

| `InputValue` state | Meaning | Display |
|---|---|---|
| `null` | User has never typed into this session | Fall back to selected label (or placeholder) |
| `""` (empty string) | User cleared the input deliberately | Show empty (placeholder appears); optionally clear the selected Value |
| `"text"` | User typed | Show the text; apply filter |

Use `is null` / `is not null` to distinguish, **not** `string.IsNullOrEmpty`. The latter conflates "never touched" with "explicitly cleared" and produces surprising UX where backspacing to empty snaps back to the label.

If backspacing to empty should also clear the selection: detect `text == string.Empty` in the input handler and null out `Value` (and invoke `ValueChanged`). Combobox does this.

**5. Highlight state responds to keyboard AND mouse**

Any component with an internal "highlighted item" concept (combobox, menu, submenu, select, complex picker) must update its highlight state from two input streams:

- Keyboard (`@onkeydown` on the input/trigger): ArrowUp, ArrowDown, Home, End, typeahead.
- Mouse (`@onmouseenter` on each item): hovering moves the highlight to that item.

Both paths mutate the same `HighlightedValue` parent state through a shared helper (`SetHighlight(string? value)` on the parent). Missing the mouse path is a common omission — the keyboard path is obvious from `@base-ui`/Radix source, but the mouse path is implicit in the React primitive's hit-test behaviour.

**6. Filter-aware visibility for list composites**

If the component contains grouping structure (ComboboxGroup + ComboboxLabel + ComboboxSeparator, or equivalents) AND supports filtering, each of these wrapper elements must know how to hide itself when its children become invisible:

- **Group heading** (`ComboboxLabel`): hide when parent group has no items matching the current filter. Implemented by the group tracking its items and exposing `HasVisibleItems()`; the label reads `ParentGroup.HasVisibleItems()` and conditionally renders.
- **Separator** (`ComboboxSeparator`): hide when it would be the first visible element, the last visible element, or adjacent to another visible separator. Implemented by the list container tracking its direct children (groups + separators) in render order and exposing `ShouldRenderSeparator(sep)` that walks siblings to check the visibility of neighbours.

Skipping this leaves stranded group headings and stacked separators around filtered-out groups — which the user will catch on first filter test.

**7. `Improved` badge criteria (strict)**

A component gets `Improved` ONLY when it adds a **concrete, user-visible feature that Blok's source does not have**. Examples: Alert's `Closeable` parameter, Pagination's `Click` callback. Pair the badge with:

- A `<DivergenceNote>` on the Catalogue page describing the addition.
- An entry under "Additions Beyond Blok" on `Home.razor` with a Primary badge and description.
- A note in `docs/ui-parity-audit.md` under the component's row.

Do NOT mark as `Improved` for:

- **Paradigm translation** — React hooks to Blazor bindings (e.g. `useComboboxAnchor` omitted, `@bind-Value` instead of React `value`/`onChange`). That's Parity; the component's behaviour surface is the same.
- **Missing features** — advanced keyboard affordances, focus trapping, portal APIs not ported. Those go in "Known Feature Gaps", not "Additions".
- **Renamed APIs** — stub cross-reference pages handle these (Sonner → Toaster).

Rule: if you cannot write the "Additions Beyond Blok" line without hedging ("sort of", "kind of", "in Blazor-idiom this is…"), it is not Improved. Mark it `Parity` and document the paradigm notes in the DivergenceNote as "paradigm translation, not a behavioural divergence" language.

### Phase 3: Post-Implementation Review

After creating the component, perform this review before proceeding:

1. **Re-read the Blok registry JSON** side-by-side with the Blazor code
2. **Verify all exports exist** as separate Blazor components — count the exported functions in Blok's source; count the matching `.razor` files in `Components/`. The numbers must match. Single missing file = structural divergence; fix before continuing (see Structural Preservation Rules in Phase 2).
3. **Verify the emitted DOM matches Blok's** — element types (`<span>`/`<div>`/`<button>`), `data-slot` attribute names, wrapper presence, sibling order. Run the component once in the Catalogue and inspect the DOM tree against Blok's live demo.
4. **Verify all variants/sizes/colors** are mapped with correct CSS classes
5. **Verify hover, focus, active, disabled** states match the source
6. **Verify dark mode classes** are present where the source has them
7. **Verify data-slot attributes** match the source
8. **Verify border/shadow/background** on every element matches
9. **Test: `dotnet build`** must pass with zero warnings
10. **Run the UI parity harness** (see below) — must exit clean for the component
11. **Compare visually** against `https://blok.sitecore.com/primitives/{name}`
12. **Update the API docs of every shared component touched.** If implementing this component required extending or changing the API of a shared library component (e.g. adding an `IconSize` value, adding a parameter to `CssClassBuilder`, extending a service), update the `ApiProperty` description for that parameter in the *shared component's own catalogue page* in the same pass. Grep for the shared component's enum/parameter name in `Catalogue/Components/Pages/Primitives/` to find the right `ApiProperty` line.

#### UI Parity Harness — MANDATORY before marking the component complete

Run: `pwsh ./tools/verify-ui-parity.ps1 -Component {Name}`

The harness performs four checks scoped to the component you just migrated:

1. **Compiled-utility coverage** — every Tailwind class you reference must be in `sitecore-blok.css`. Catches typos like `font-regular` (should be `font-normal`), deprecated utility names, and accidentally-misspelled prefixes.
2. **Runtime-composed class detection** — flags any `$"bg-{color}-500"`-style interpolation that Tailwind CLI cannot see. **If this finds anything, the migration is not complete** — rewrite those spots to use full literal class names, with conditional selection via ternary between literals or `.With(class, condition)` on `CssClassBuilder`.
3. **Blok class-string drift** — diffs your class strings against the Blok source and reports non-trivial divergences.
4. **Surface background without paired text token** — flags any class string that sets a theme-aware surface bg (`bg-background`, `bg-card`, `bg-popover`, `bg-muted`, `bg-accent`, `bg-primary`, `bg-secondary`, `bg-destructive`) without an explicit `text-*` token in the same string. This is the bug class that caused AlertDialog text to be invisible in dark mode. Fix by adding the matching text token (e.g. `bg-background text-foreground`) or, for genuinely decorative surfaces, the suppression marker `parity-no-text-pair`.

If the harness exits non-zero, surface the findings to the user and resolve them before declaring the component migration done. **Parity verification is non-negotiable.** If a finding is a deliberate Blazor-only choice (not a bug), document it in `docs/ui-parity-audit.md` with a justification and suppress it explicitly.

### Phase 4: Catalogue Integration

Every component must have a catalogue page. Follow this checklist:

1. **Create page** at `Catalogue/Components/Pages/Primitives/{Name}Page.razor`
   - Route: `@page "/primitives/{name-lowercase}"`
   - Must include: `@rendermode InteractiveServer`
   - Must include: `@using PINGWorks.SitecoreBlok.BlazorUI.Catalogue.Services` (needed for the `ApiElement` / `ApiProperty` record types referenced in the `@code` block — see step 2).
   - Use `ComponentPage` wrapper with Title, Description, and `ApiElements="@XxxElements"`.
   - **Classify interactivity on `ComponentPage`** — every primitive page must set `Interactivity` to one of:
     - `ComponentInteractivity.Interactive` → blue "Requires interactive-mode" badge. Use when the component's primary API depends on a Blazor interactive circuit: `@onclick` / `@bind-Value` / `EventCallback` / `IJSRuntime` / `CascadingValue` state that needs to re-render on user events. If the component works but its event callbacks would be no-ops in static SSR, it's Interactive.
     - `ComponentInteractivity.Ssr` → yellow "Supports SSR mode" badge. Use for pure-markup/CSS primitives (Badge, Card, Separator, Skeleton, Tooltip-via-CSS-hover, layout wrappers like Stack/Field/InputGroup). Closeable/dismissible extras on an otherwise-static component don't downgrade it — Alert is SSR because its core is a callout; the optional `Closeable` button is an additive feature.
     - The only page allowed to omit `Interactivity` is `Primitives/Index.razor` (the grid of links). The parity harness does not enforce this; a sweep like `grep -L "ComponentInteractivity" Catalogue/Components/Pages/Primitives/*.razor` outside `Index.razor` should come back empty.
   - **Flag supporting-script dependencies** — set `RequiresScripts="true"` on `ComponentPage` when the component depends on a script being wired into the host page by the consumer. Examples: `CodeViewer` needs Prism.js + prism.css linked in `App.razor`; `ThemeToggle` needs `<ThemeToggleStartupScript />` placed in `<head>` to avoid a light-mode flash on first paint. Components whose JS module is a colocated `.razor.js` auto-loaded via `JS.InvokeAsync<IJSObjectReference>("import", ...)` do NOT get this flag — the consumer has no work to do. Whenever `RequiresScripts="true"`, you MUST also add an `<InstallationNote>` block (see step 6) listing the exact tags/components to add.
   - **Two-section page structure: Examples + API.** Each page is split into an `Examples` H2 section and an `API` H2 section, with a right-rail "On this page" ToC derived automatically from registered sections. The structure is:
     1. Leading blocks (`<InstallationNote>`, `<DivergenceNote>`) sit DIRECTLY inside `<ComponentPage>`, BEFORE `<ExamplesSection>`. Order: InstallationNote → DivergenceNote.
     2. All `<ComponentExample>` blocks sit inside a single `<ExamplesSection>` wrapper. `ExamplesSection` renders the "Examples" H2 + registers the section in the ToC.
     3. The `ApiElements="@XxxElements"` parameter on `<ComponentPage>` drives the "API" section: an elements-summary table followed by one H3 subsection per element with a property table. Shared components (`ApiSection`, `ApiElementsTable`, `ApiElementDetail`, `ApiPropertiesTable`, `DescriptionRenderer`) handle rendering — pages just supply data. See **step 2** for how to author the `ApiElement[]` array.
   - Use `ComponentExample` blocks with Title, Code string, and live ChildContent
   - Code strings show Blazor markup (not React)
   - **The Code string MUST match the rendered ChildContent verbatim.** Same components, same parameter names, same parameter values, same content text, same nesting. Drift between the "Code" tab (what the user copies) and the "Preview" tab (what they see) is confusing — users will copy code that produces a different result. Acceptable: condensing whitespace and using `...` shorthand only when the example is illustrating one specific feature and unrelated content has been elided. **Not** acceptable: examples that shows `<p>simple text</p>` in the Code tab while rendering three Buttons in the Preview tab. When you change the live example, update the Code string in the same edit. When you change the Code string, update the live example to match.
   - Include examples for: default, each variant, each size, disabled, interactive states

   **Canonical page skeleton** (see `ButtonPage.razor`, `AlertPage.razor`, `AccordionPage.razor` for real examples):

   ```razor
   @page "/primitives/xyz"
   @using PINGWorks.SitecoreBlok.BlazorUI.Catalogue.Services
   @rendermode InteractiveServer

   <ComponentPage Title="Xyz"
                  Description="One sentence describing what this does"
                  Interactivity="ComponentInteractivity.Interactive"
                  ApiElements="@XyzElements">

       @* Leading blocks go HERE — directly inside ComponentPage, before ExamplesSection *@
       <InstallationNote>...</InstallationNote>
       <DivergenceNote>...</DivergenceNote>

       <ExamplesSection>
           <ComponentExample Title="Default" Code="...">...</ComponentExample>
           <ComponentExample Title="Variants" Code="...">...</ComponentExample>
           @* ... more examples ... *@
       </ExamplesSection>

   </ComponentPage>

   @code {
       // See step 2 for authoring rules.
       private static readonly ApiElement[] XyzElements = [ ... ];
   }
   ```

2. **Author the `ApiElements` array** — this populates the API section with an elements-summary table and a per-element property table. One `ApiElement` per Blazor component in the family (root, children, grandchildren).

   **Shape:**
   ```csharp
   private static readonly ApiElement[] XyzElements =
   [
       new ApiElement(
           Name: "Xyz",
           Description: "One sentence describing the element's purpose. Reference `OtherElement` or `XyzVariant.Default` in backticks.",
           Depth: 0,
           Properties:
           [
               new ApiProperty( "Variant",              "XyzVariant",                  false, "Visual style. Default: `XyzVariant.Default`. Other values: `Outline`, `Ghost`." ),
               new ApiProperty( "ChildContent",         "RenderFragment?",             false, "Body content." ),
               new ApiProperty( "AdditionalAttributes", "Dictionary<string, object>?", false, "Captured unmatched attributes, forwarded to the root element." ),
           ]
       ),
       new ApiElement( Name: "XyzItem", Description: "...", Depth: 1, Properties: [ ... ] ),
       // grandchildren at Depth: 2
   ];
   ```

   **Rules:**
   - **One `ApiElement` per user-facing element.** Internal plumbing components (e.g. cascading-state helpers a consumer never writes) don't belong in the array. Public data types the consumer constructs (e.g. `StepperStep`, `StackNavigationItem`) CAN be documented as elements when they're part of the public API surface.
   - **`Depth`**: 0 for root, 1 for direct children, 2 for grandchildren. Pattern it on typical markup nesting, not strict DOM nesting — e.g. Timeline has `Timeline → TimelineItem → TimelineSeparator/TimelineContent/TimelineTitle/...`, so the sub-components are depth 2 under `TimelineItem`. The depth controls visual indentation in the elements-summary table.
   - **`IsRequired`**: `true` for parameters marked `[EditorRequired]`, OR for parameters the component fundamentally needs to function (would throw / render broken output without). Otherwise `false`. Use the `bool` directly — no backtick styling on this column.
   - **`Type`**: use the signature as it appears in markup — `string?`, `RenderFragment?`, `EventCallback`, `EventCallback<string>`, `ButtonVariant`, `Dictionary<string, object>?`. Not CLR names like `System.String`. Nullable types include the `?`.
   - **Descriptions**: ONE USEFUL SENTENCE. Do not echo the parameter name ("The variant parameter sets the variant"). Do explain what the parameter CONTROLS or when to use it. ALWAYS include defaults inline when the Razor source has one: "Default: `false`.", "Default: `ButtonVariant.Default`. Other values: `Outline`, `Ghost`, `Link`."
   - **Backticks** in descriptions mark inline code. A shared `DescriptionRenderer` component parses them: backtick content becomes a `<code>` span (globally styled in `catalogue.css`), and if the content (or its dotted root — e.g. `Accordion` in `Accordion.Item`) matches an element name in the same `ApiElements` array, the `<code>` becomes an auto-generated jump link to that element's H3. Use backticks around: type names, enum values, enum short-forms, element names, and property cross-references. Don't use backticks around prose.
   - **Preserve existing `@code` members** (event handlers / state fields used by examples). Merge the `private static readonly ApiElement[] XxxElements = [...];` declaration INTO the existing `@code` block — do not create a second block.
   - **Right-rail ToC is automatic.** Don't wire anything explicit. `ExamplesSection` registers the `Examples` H2; each `ComponentExample Title` becomes an H3 entry. `ApiSection` (rendered via the `ApiElements` parameter) registers the `API` H2; each `ApiElement.Name` becomes an H3. The registry preserves markup order by using `<CascadingValue>` to scope children to their parent section — consumers of the pattern don't need to think about it.

3. **Add a row to `MIGRATION_STATUS.md`** — Both the Catalogue NavMenu and the Home page's "Available Components" grid derive their lists from this file via `PINGWorks.SitecoreBlok.BlazorUI.Catalogue.Services.MigrationStatusService`. Pick the right status badge (`Parity`, `Improved`, or `Additional`) when adding the row; no Razor edits to NavMenu or Home are needed. Alphabetical insertion in the table.

4. **Primitives Index** — no manual edit. `Components/Pages/Primitives/Index.razor` pulls names, descriptions, and status badges from `MigrationStatusService.Ported` + the `Stubs` array, auto-groups by first letter. The row you added in `MIGRATION_STATUS.md` appears on `/primitives` automatically with its `Improved` / `Additional` badge (Parity entries render without a badge, matching the NavMenu convention).

5. **Stub pages only** — if you're adding a cross-reference stub page (e.g. `SonnerPage` pointing to `Toaster`), the stub has no row in `MIGRATION_STATUS.md`. Add an entry to the small `Stubs` arrays in all three places: `Components/Layout/NavMenu.razor`, `Components/Pages/Home.razor`, and `Components/Pages/Primitives/Index.razor` (the last one needs a `Description` string because the index card shows one). For first-class migrated components, skip this step — the service handles everything.

6. **Add an `<InstallationNote>` block at the top of the page** if the component needs setup beyond adding the `@using PINGWorks.SitecoreBlok.BlazorUI` and dropping the tag into markup. "Needs setup" means any of:
   - **Service registration** — the component injects one of the services exposed by `AddSitecoreBlokUI()` (`PopoverService`, `ToastService`, `GlobalTheme`). Always include `builder.Services.AddSitecoreBlokUI();` in the note.
   - **Root-level layout component** — the component depends on a singleton placed once in `MainLayout.razor` / `App.razor` (e.g. `<Popovers @rendermode="InteractiveServer" />`, `<Toaster @rendermode="InteractiveServer" />`).
   - **Host-page scripts or stylesheets** — consumer must add `<script>` / `<link>` tags or a helper component to `App.razor` (e.g. Prism assets for `CodeViewer`; `<ThemeToggleStartupScript />` for `ThemeToggle`). Any component that needs this ALSO gets `RequiresScripts="true"` in step 1.
   - **Infrastructure inherited from another component** — if your component uses `Popover` internally (Select, DatePicker), the consumer still needs the `<Popovers />` container and `PopoverService`; the note must call that out even though the component doesn't inject `PopoverService` directly. Do not assume consumers will follow the dependency chain on their own.
   
   The note lives BEFORE `<DivergenceNote>` — installation comes before API-shape notes. Use a numbered `<ol>` with `<CodeViewer>` blocks inside each `<li>` for anything a consumer should copy-paste; the codeviewer makes the snippets readable in both light and dark mode and matches the code-sample styling used elsewhere in the Catalogue. For renamed-from-Blok stubs (e.g. `SonnerPage`), a short one-liner that points to the canonical page's setup is sufficient. Skip `<InstallationNote>` entirely for components whose only requirement is "import the tag" (Badge, Card, Separator, Icon, most pure-SSR primitives, and interactive components whose JS module auto-loads via `JS.InvokeAsync<IJSObjectReference>("import", ...)` such as NavigationMenu).

7. **Add a `<DivergenceNote>` block at the top of the page** if the Blazor API differs from Blok in any way a user might notice — additive parameter (e.g. Alert's `Closeable`), string-to-RenderFragment upgrade, behavioural difference, or hardcoded internal markup we add. Skip if the API is a faithful Blok port. The note should explain what's different and why. When both `<InstallationNote>` and `<DivergenceNote>` are present, InstallationNote goes first.

8. **Update Home page status sections** — the Home page has four dynamic sections:

   **Available Components** (auto) — pulled from `MigrationStatusService.Ported` + the `Stubs` array. No manual edit when you add a Parity/Improved/Additional row to `MIGRATION_STATUS.md`.

   **Backlog** (auto) — pulled from `MigrationStatusService.Backlog`. Flips a component from Missing-to-ported by changing the status in `MIGRATION_STATUS.md`; no Razor edit needed. If the newly-migrated component was listed as Backlog, update its row's badge to `Parity` / `Improved` / `Additional` and it automatically leaves this section.

   **Won't Do** (auto) — pulled from `MigrationStatusService.WontDo`. Rendered as a table; the reason in the `Description` column is surfaced so the rationale is always visible. Edit the description directly in `MIGRATION_STATUS.md` if the wording needs updating.

   **Known Feature Gaps** (manual HTML block, still hand-edited in `Home.razor`):
   - If the migrated component resolves a listed gap, remove that gap entry
   - If the component has known limitations vs the Blok source (e.g. no keyboard nav, no Radix focus trapping, CSS-only hover instead of JS delay), add a new gap entry with a `<Badge ColorScheme="BadgeColor.Warning" Size="BadgeSize.Sm">` and description
   - Be specific about what's missing and what the Blok source provides

   **Additions Beyond Blok** (manual HTML block, still hand-edited in `Home.razor`):
   - If the component has features, parameters, or behaviours that the Blok source doesn't have, add a new entry with `<Badge ColorScheme="BadgeColor.Primary" Size="BadgeSize.Sm">` and description
   - Examples: user-configurable properties the source hardcodes, unified APIs the source splits into separate components, Blazor-idiomatic patterns like RenderFragment bodies

9. **Build and verify** — `dotnet build` from solution root, run the Catalogue
10. **Final parity gate** — run `pwsh ./tools/verify-ui-parity.ps1 -Component {Name}` one more time after the Catalogue page is in place. Zero findings required before moving on.
11. **Run the Verify flow to close out the migration** — the migrate process does NOT end at harness-clean. Execute the single-component verify flow (Phase 6 below) on the component you just migrated. That flow:
	- re-runs the parity harness and remediates any remaining findings,
	- pulls the Blok source side-by-side for a deeper class-string comparison,
	- performs the Chrome MCP visual pass (local vs Blok in both light and dark mode) when the extension is connected,
	- updates `docs/ui-parity-audit.md` with the component's status and any documented deliberate deviations.
	
	Only after the verify flow completes is the migration considered done. If Chrome MCP is unavailable, say so explicitly rather than skipping silently — the harness alone is not a visual-parity guarantee.

### Phase 5: Audit Mode (`/blok audit`)

`MIGRATION_STATUS.md` at the repo root is the canonical record of what was last evaluated and against which Blok SHA. Audit mode reads that file, diffs it against the current state of Blok `main`, and reports candidates for re-audit and new-component migration. Always read `MIGRATION_STATUS.md` before doing any upstream fetching — it dictates scope.

**Inputs the audit uses**
- `MIGRATION_STATUS.md` — per-component `Last SHA` snapshots plus the `Blok main HEAD` recorded at the time of last evaluation.
- `https://github.com/Sitecore/blok` `main` branch — current last-touched commit SHA for every file under `src/components/ui/`.
- `PINGWorks.SitecoreBlok.BlazorUI/Components/` — local component folders, to sanity-check that table rows still correspond to real components.

**Workflow**

1. **Read `MIGRATION_STATUS.md`** and extract:
   - The `Blok main HEAD` recorded at the top of the file.
   - Every row's `Component`, `Status`, `Blok Source` file path, and `Last SHA`.
2. **Resolve current Blok state**. Without `gh` CLI available, clone or update a shallow sparse-checkout of Blok and run `git log -1 --format='%h' -- <file>` per source file — or use `https://api.github.com/repos/Sitecore/blok/commits?path=<file>&per_page=1` if rate limits allow.
3. **Drift check (subtle changes)** — for every row with a `Blok Source`:
   - If the current last-touched SHA (first 6 chars) differs from the recorded `Last SHA`, the file has changed since we last evaluated. Flag for re-audit.
   - Fetch the updated tsx source (`raw.githubusercontent.com/Sitecore/blok/main/src/components/ui/<file>`) and diff it against our Razor implementation: CSS class strings, new props / exports, variant options, semantic tokens.
   - For `![Backlog]` rows, a changed SHA just means the upstream source shifted since last migration-window review — still worth noting when planning the next port.
   - For `![Won't Do]` rows, a changed SHA is low priority but not ignored: a material Blok rewrite (e.g. dropping the `@dnd-kit` dependency) can flip the decision. Surface these with a muted note in the audit report so the user can re-evaluate if they want.
4. **New-component scan (blanket search)** — list the current contents of `src/components/ui/*.tsx` on `main`. Any file whose base name (minus `.tsx`, normalised to the row's name convention) has no matching row in `MIGRATION_STATUS.md` is a new primitive candidate. Filter out obvious duplicates / alternates that Blok keeps for internal reasons (e.g. `inputOtp.tsx`, `select-react.tsx`).
5. **Report back three buckets** to the user:
   - **Changed since last audit** (recorded SHA differs from current) — group by Parity / Improved / Missing status from the table; each with a compact diff summary of what shifted.
   - **New in upstream** (no row in the table) — candidate for `/blok migrate <name>`.
   - **Unchanged** — count only, no per-row noise.
   Include the old and new SHA per changed row so the user can decide.
6. **After the user acts** (via `/blok update`, `/blok migrate`, or `/blok verify`), rewrite the corresponding row's `Last SHA` in `MIGRATION_STATUS.md` and update the `Last evaluated` date. For newly-migrated components, insert a new row alphabetically. Never silently update a row without completing the actual re-audit — the file is a record of evaluations, not a to-do list.

The drift check is the whole reason this file exists: the primitives page and registry JSON can hide small class-string or prop changes behind a primitive that still renders the same gallery example. Diffing at the commit level surfaces those edits before they cause silent parity regressions.

### Phase 6: Verify Mode (`/blok verify <name>` and `/blok verify all`)

End-to-end UI-parity verification that wraps the parity harness, Blok source diffing, and a judgement-call fix loop. Use this when the user asks to verify a specific component, or to sweep every primitive against Blok.

#### Dispatch

Natural-language phrases this mode responds to:

| User phrase | Scope |
|---|---|
| "verify the ui of component X against Blok" | single: `X` |
| "check component X against Blok" | single: `X` |
| "check ui parity of X" | single: `X` |
| "/blok verify X" | single: `X` |
| "check ui parity for all components against Blok" | all |
| "verify all components against Blok" | all |
| "/blok verify all" or `/blok verify` with no name | all |

#### Single-component flow — `/blok verify <name>`

1. **Run the harness, scoped**:
   ```powershell
   pwsh ./tools/verify-ui-parity.ps1 -Component <Name>
   ```
   The harness writes `docs/ui-parity-report.md` and exits non-zero if anything is found.

2. **Read the report** and process findings by check (there are four):

   - **Check 1 — Compiled-utility coverage**: token referenced in Razor is not in `sitecore-blok.css`. Causes:
     - New utility never previously used → **expected** the first time; the next `dotnet build` triggers Tailwind CLI and generates it. Note and move on.
     - Typo / misspelled utility (e.g. `font-regular`, `text-md-5`, invented names) → **fix** by correcting the class name.

   - **Check 2 — Runtime-composed class detection**: the Razor assembles a class name from variables like `$"text-{color}-500"`. Tailwind CLI can't see these. Rewrite to use full literals: either `.With("text-primary-500", condition)` on `CssClassBuilder` or a ternary between literal strings. **Never suppress.**

   - **Check 3 — Blok class-string drift**: a class present in the Blok source file is absent from our Razor. Possible actions:
     - **Fix** — add the class if it produces a visible rendering difference.
     - **Suppress as equivalent** — if it's a shadcn/ui ↔ Chakra-semantic naming difference (e.g. `bg-body-bg` ↔ `bg-background`), add a pair to the `$equivGroups` canonical map in the harness.
     - **Document as deliberate** — if it's a Blazor-only choice (e.g. Blok's coloured Icon tile variants we don't support since our Icon has no wrapper), add a note to `docs/ui-parity-audit.md` under the component row.

   - **Check 4 — Surface background without paired text token**: class string sets a theme-aware surface bg (e.g. `bg-background`, `bg-card`, `bg-popover`) with no `text-*` token. The default text colour relies on body cascade — which fails for fixed-positioned and portal-rendered content. Possible actions:
     - **Fix** — add the matching text token to the same class string: `bg-background text-foreground`, `bg-card text-card-foreground`, `bg-popover text-popover-foreground`, `bg-primary text-white`. This is the right answer in 90% of cases.
     - **Suppress as decorative** — if the element genuinely has no text content (slider track, progress fill, indicator dot, icon-only button) or its text comes from a child component that sets it explicitly (Table containers / thead / tfoot whose cells set their own text), add the marker class `parity-no-text-pair` to the same class string AND add a one-line comment above explaining why. **Do not use the marker as a default escape hatch — it must reflect reality.**
     - **Conditional `.With` strings**: the harness checks each string literal independently. If the surface bg is in a conditional `.With` and the text token is in `Start(...)`, the harness still flags the conditional string. Add the marker there with a comment naming the source of the text token.

3. **Fetch the Blok source side-by-side** to deepen the comparison:
   - Source: `https://raw.githubusercontent.com/Sitecore/blok/main/src/components/ui/<name>.tsx`
   - Live demo: `https://blok.sitecore.com/primitives/<name>`

4. **Apply fixes**. Add an anti-revert comment at the top of the component or near the specific class string if the fix is non-obvious (e.g. the ActionBar `opacity` animation, fixed-position transform workarounds). Anti-revert comments should:
   - Say what the correct pattern is.
   - Say what breaks if it's reverted (e.g. `breaks fixed positioning of submenus`).

5. **Re-run the harness** — must exit clean. If Check 1 still flags `X new utility`, a `dotnet build` run is sufficient (Tailwind regenerates it). Any other finding means the fix is incomplete.

6. **Update `docs/ui-parity-audit.md`** — record the component with status `✅`, `⚠️ minor`, or `❌ differs`, plus a short note on what was fixed, suppressed, or deliberately diverged.

7. **Visual verification via Chrome MCP — light + dark mode**. The static harness catches class-string drift but cannot see rendered pixels. After the harness exits clean, run the browser-driven comparison against the live Blok reference:

   **Preconditions**
   - Chrome MCP extension connected (`mcp__Claude_in_Chrome__tabs_context_mcp` returns tabs without a "not connected" error). If it's offline, tell the user and stop — do not claim visual parity from the harness alone.
   - Local Catalogue dev server running. Start with the project-directory Tailwind binary on PATH:
     ```bash
     cd <repo-root>
     PATH="$PATH:$(pwd)/PINGWorks.SitecoreBlok.BlazorUI:$(pwd)/PINGWorks.SitecoreBlok.BlazorUI.Catalogue" \
       dotnet run --project PINGWorks.SitecoreBlok.BlazorUI.Catalogue --urls "http://localhost:5199"
     ```
     Run in background. Poll `curl -s -o /dev/null -w "%{http_code}" http://localhost:5199/primitives/<name>` until it returns 200.

   **Coverage rule — every example on the page, every example is exercised**

   A captured screenshot of the page header only proves the first one or two examples render. The Catalogue page for a primitive typically has multiple examples (Default, Variants, Chips, Disabled, …) — all of them must be verified. Failing to scroll uncovered a silent chip-label bug during the first Combobox migration.

   Before taking pair-comparison screenshots, enumerate every `<ComponentExample>` on the Catalogue page:

   ```js
   Array.from(document.querySelectorAll('[data-component="component-example"], h2, h3'))
     .filter(e => e.closest('[data-component="component-page"]') || e.tagName.match(/^H[23]$/))
     .map(e => e.textContent.trim())
   ```

   (Or just read the `.razor` source — each `<ComponentExample Title="…">` block is one example.) Work through the list in order:

   1. **Scroll the example into view** (`element.scrollIntoView({block:'center'})` via `javascript_tool`, or `scroll_to` on the `computer` tool with the example's element reference). The top one or two examples being the only ones visible is a failure mode — you **must** visit every example below the fold.

   2. **Exercise the primary interaction** for that example in light mode, in sequence:
      - Click the control (focus → open state if applicable).
      - If it accepts typing (Combobox, Input, SearchInput), type a few chars, capture the filtered/typed state.
      - If it's selectable (Combobox, Select, RadioGroup, Checkbox), select an item, capture the post-selection state.
      - If it has close/clear/remove affordances, invoke them, capture the post-dismiss state.
      - For components with display-after-selection (Combobox single/multi, Select, DatePicker), **reopen** and confirm the selection still shows with the correct label / tick. This catches label-cache drift after dropdown close.
      - For components with chips / tags / multi-select badges, close the popover by clicking outside and **confirm the chip still shows its label**, not the raw value. This is the specific failure mode that hit Combobox — after dispose, cached labels must survive.
      - For disabled variants, confirm that a pre-selected value renders with its proper `Label` (not the lowercase `Value`). Another class of "items only register when open" bugs.

   3. **Repeat the exercise in dark mode.** Toggle local dark (`document.documentElement.classList.add("dark")`) and walk the entire page again — chip text colour, popup contrast, selected-item highlight, empty-state text, all variant backgrounds. Dark-mode regressions are often scoped to specific compound variants (e.g. `bg-primary-background + text-foreground` failing only for chip tokens) so each example is a distinct test.

   4. **Take screenshots at every interactive state** worth comparing, not just the resting state. Minimum per example:
      - Resting (light)
      - Open / active (light)
      - Resting (dark)
      - Open / active (dark)

      Plus any state reported as buggy during a previous iteration (e.g. "post-selection input display" becomes its own required capture if the user has complained about it before).

   If Chrome MCP is unavailable, say so explicitly and stop — the harness alone does not prove per-example correctness, and you cannot substitute "it probably works" for running the exercise.

   **Procedure — one MCP tab per surface (4 captures total)**

   For each component you are verifying, capture *four* screenshots in this order and save them to disk so you can present them to the user:

   | # | Surface | URL | Theme |
   |---|---|---|---|
   | 1 | Local Catalogue, light | `http://localhost:5199/primitives/<name>` | default |
   | 2 | Local Catalogue, dark | `http://localhost:5199/primitives/<name>` | `.dark` class applied |
   | 3 | Blok reference, light | `https://blok.sitecore.com/primitives/<name>` | default |
   | 4 | Blok reference, dark | `https://blok.sitecore.com/primitives/<name>` | Blok's own dark toggle |

   Exact Chrome MCP flow:

   1. Call `mcp__Claude_in_Chrome__tabs_context_mcp` with `createIfEmpty: true` to get a tab ID.
   2. `mcp__Claude_in_Chrome__navigate` to the local light URL. Wait ~1s for Blazor SSR hydration.
   3. `mcp__Claude_in_Chrome__computer` action `screenshot` with `save_to_disk: true` — this is capture #1.
   4. Toggle local dark mode by executing JS via `mcp__Claude_in_Chrome__javascript_tool`:
      ```js
      document.documentElement.classList.add("dark")
      ```
      Take capture #2 with `screenshot` + `save_to_disk: true`.
   5. Navigate to the Blok URL for the component. Let the page settle (Blok is a Next.js site).
   6. Take capture #3 (default is light mode on Blok).
   7. Toggle Blok dark mode. Blok's live site exposes a theme toggle button — use `mcp__Claude_in_Chrome__find` with query `"theme toggle"` or `"dark mode button"` to locate it, then click with `computer` / `left_click`. If the toggle can't be found via `find`, fall back to setting the class via JS:
      ```js
      document.documentElement.classList.add("dark")
      ```
      Take capture #4.

   **Comparison pass**

   After captures are on disk, walk through pair-by-pair:

   - **Pair 1 vs 3** (local light vs Blok light): do the borders, shadows, radii, spacing, typography, and colors match visually? Any missing elements? Any extra?
   - **Pair 2 vs 4** (local dark vs Blok dark): same question, but scrutinise dark-mode token application — it's the most common source of silent drift (e.g. `--border` not mapped in `.dark`, hover states losing contrast, placeholder text invisible).

   Specific checks to make on every component:
   - Border color and weight in both themes.
   - Shadow presence and intensity (BlazorUI shadows flip to white tints in dark mode — confirm that's visible, not blown out).
   - Text contrast — use visual judgement; call out anything that looks washed out against the dark background. Pay particular attention to any element that combines a theme-aware surface (chip, badge, pill, tag) with `text-foreground` or another flipping text token — those tend to produce invisible dark-mode text when the surface token doesn't flip. Check every example on the page, not just the first one.
   - Hover / focus states — if the component has interactive elements, hover over at least one in each theme (`computer` action `hover` with a coordinate inside the element) and re-capture to compare the hover treatment.
   - Icon sizing and color inheritance — the harness Check 4 catches direct-child svg selector bugs statically, but visual inspection catches cases where the icon renders but looks wrong (e.g. wrong stroke weight, wrong color token).
   - Label persistence after state change — for any component that displays a label derived from a selected value (Combobox single label, ComboboxChip, Select, MultiSelect), explicitly cycle through "initial → open → select → close → reopen" and "initial → open → select → close → blur → refocus" and confirm the label remains correct (not the raw value). This is where cache-based bugs hide.

   **Reporting**

   Present the four screenshots to the user grouped by theme (light pair first, then dark pair). For each pair, state "matches" or list concrete discrepancies with the element and the difference (e.g. "border is `--border` token on local but `--border-color-a11y` on Blok in dark mode — contrast is lower on local"). If any differences would affect the user's experience, propose a fix before closing the verify session.

   **Cleanup**

   - Kill the background Catalogue process.
   - Do NOT leave the dark-mode JS applied on the Blok site tab — it persists across page loads on Next.js and might confuse the next verification run. Execute `document.documentElement.classList.remove("dark")` before closing, or close the tab.

   **Scope note**

   This visual pass is meant for components that have a Blok source. For components in `Components/Extra/` that have no Blok equivalent (e.g. `TreeView`, `Stack`, `CodeViewer`), skip the Blok side of the comparison — run captures #1 and #2 only and evaluate against the library's own design-token consistency (borders, hover tones, spacing, typography matching sibling components in the same theme).

8. **Announce completion** only when the harness exits clean, the visual verification has run (or was explicitly skipped due to MCP unavailability), and the audit row is updated.

#### All-components flow — `/blok verify all`

1. **Run the harness unscoped**:
   ```powershell
   pwsh ./tools/verify-ui-parity.ps1
   ```
   Expect the report to list findings across many components.

2. **Group findings by Blok component family** (Accordion covers Accordion + AccordionItem + …; Dialog covers Dialog + DialogHeader + DialogTitle + …). The harness already aggregates the drift check this way; Checks 1 and 2 are per-file.

3. **Process in the same batches as Phase 2** (7 batches, ~50 primitives total):
   - A: Button, Card, Input, Dialog, Badge, Alert, Separator
   - B: Checkbox, RadioGroup, Select, Switch, Textarea, SearchInput, InputGroup, Field, Label, Toggle, ToggleGroup, Slider
   - C: Tooltip, Popover, Sheet, AlertDialog, DropdownMenu, ContextMenu
   - D: Breadcrumb, Pagination, Tabs, NavigationMenu, Stepper
   - E: Toaster, Progress, CircularProgress, Spinner, Skeleton, EmptyState, ErrorState, Table
   - F: Accordion, Carousel, Collapsible, Timeline, ActionBar, ScrollArea
   - G: DatePicker, TimePicker, Calendar, Avatar, Icon, Kbd, AspectRatio, CodeViewer

4. **Per batch**, apply the single-component flow (steps 2–6) to each component in the batch.

5. **Announce batch progress** to the user: "Batch A (7/7 clean). Starting Batch B…" Lets them interrupt or review mid-stream.

6. **Final re-run**: after all batches, run `pwsh ./tools/verify-ui-parity.ps1` one more time. Report the final counts in each check.

7. **Summary to user**: counts fixed vs suppressed vs deliberate deviations, plus a link to the updated `docs/ui-parity-audit.md`.

#### Safety rules for Verify Mode

- **Never bulk-apply Check 3 suggestions without a per-component judgement.** A drift finding may be intentional, an equivalence, or a real bug — they look identical in the report.
- **Always add anti-revert comments** when fixing non-obvious class patterns (animation approaches, transform-breaks-fixed-positioning workarounds). The user's git history has shown that without these, fixes are silently undone.
- **Never mark verify complete with outstanding findings.** If the user interrupts, leave the audit doc accurate so the next session can resume.

## Common Pitfalls (learned from this project)

- **Don't collapse Blok sub-components into RenderFragment params or `@if` branches** — if Blok exports `Avatar`, `AvatarImage`, `AvatarFallback` as three separate functions, create three `.razor` files with the same names. Collapsing them (with `@if Src then <img> else <span fallback>` or with `RenderFragment? Image` / `RenderFragment? Fallback` parameters) breaks consumer composition: Blok demos like `<div class="*:data-[slot=avatar]:transition-all"><Avatar><AvatarImage/><AvatarFallback/></Avatar></div>` either won't compile or won't apply CSS to the right elements. Same applies to `Card` + `CardHeader` + `CardContent` + `CardFooter`, `Dialog` + `DialogHeader` + `DialogTitle` + `DialogDescription` + `DialogFooter`, etc. — always one Blazor file per Blok export. See the Structural Preservation Rules section in Phase 2 for the full guidance.
- **Toaster is the canonical example of a deliberate platform-idiom divergence** — Blok wraps the Sonner React library which uses declarative `<Toast><ToastTitle/><ToastAction/></Toast>` composition. We deliberately keep our imperative `ToastService.Show(...)` API because that's how Blazor apps idiomatically manage transient UI notifications. The `ToasterPage` carries a `<DivergenceNote>` explaining this, and a stub `SonnerPage` exists at `/primitives/sonner` redirecting users coming from Blok. When you find another Blok component whose React/JS pattern fights against Blazor idioms (rare), follow this same Toaster + Sonner-stub pattern rather than forcing the React shape into Blazor.
- **Don't use `bg-neutral-bg` for hover** when the source uses `hover:bg-primary hover:text-inverse-text` (Calendar days)
- **Don't forget `@onclick:stopPropagation`** on buttons inside clickable containers (Toast action/close buttons)
- **Don't use `@if` for open/close** when CSS transitions are needed — keep elements in DOM and toggle opacity/transform
- **Don't hardcode `duration-300`** — put durations in parameters when the source allows configuration
- **Tailwind only generates classes it scans** — if the Catalogue uses classes not in the library, it needs its own Tailwind build
- **`IsFixed` on CascadingValue** prevents child re-renders — remove it for interactive parent-child components (Tabs, RadioGroup)
- **Overflow containers clip fixed-position children** — render floating submenus as portals outside the parent menu DOM
- **The Blok Tailwind build runs BeforeTargets="ResolveStaticWebAssetsInputs"** — not AfterTargets="Build" — or the CSS arrives empty in the static asset manifest
- **Never compose Tailwind class names from variables at runtime** — Tailwind CLI scans source files for full literal class strings. A class assembled like `$"bg-{color}-500"` will not be detected and the CSS will not be generated, even if the runtime value is a valid utility. Use full literals and select between them via `.With(class, condition)` or ternaries between literal strings. The parity harness (`tools/verify-ui-parity.ps1`) catches these automatically.
- **Icon emits `<svg>` directly with no wrapper** — `ClassName` lands on the `<svg>` itself, and selectors like `[&_svg]`, `[&>svg]`, `has-[svg]`, `has-[>svg]` all match the same element. When you pass `transition-transform` via `ClassName`, it co-locates with any rotation selector targeting the svg, so animations work. (Historical: the Icon used to wrap the svg in a span to host `Variant`/`ColorScheme` background tiles. The wrapper was removed because no consumer used those parameters and the wrapper broke chevron rotation animations in Accordion.)
- **Surface bg without text token = invisible dark-mode text** — if you set `bg-background`, `bg-card`, `bg-popover`, `bg-muted`, `bg-accent`, `bg-primary`, `bg-secondary`, or `bg-destructive` on an element that contains text, you MUST also set a matching `text-*` token in the same class string. Cascade-only foreground colour silently breaks for fixed-positioned and portal-rendered content — the AlertDialog/Dialog/Sheet text was invisible in dark mode for exactly this reason until Check 4 of the parity harness was added. Decorative surfaces (slider tracks, progress fills, indicator dots) get the suppression marker `parity-no-text-pair` instead.
- **CSS variables only flip for elements that USE the variable directly — not for inherited computed values.** Our dark mode is scoped by adding `.dark` to a wrapper element (`<div data-dark-mode-target>` in MainLayout). Inside that scope, `--color-foreground` redefines to white. But an element only renders white if it itself has `text-foreground` (or another `var(--color-foreground)` consumer). An element that just inherits color from its parent gets the parent's COMPUTED color from where the parent was — which, if the parent was outside the dark scope or didn't have its own `text-foreground` class, was light-mode dark. **The wrapper that owns the `.dark` toggle MUST have `text-foreground` on it** so the variable resolves in-scope and the cascade carries the correct computed value to every descendant. Without that, every primitive that doesn't itself set a text token shows the wrong colour in dark mode — exactly why the Collapsible bug looked like a button-inheritance issue when it was actually a variable-scoping issue. Tailwind preflight is irrelevant here (it IS loaded by Tailwind v4's `@import "tailwindcss"`); the fix is on the wrapper, not on individual buttons. Apply this same rule whenever you add a new dark-mode-scoped wrapper or fork the layout.
- **Hardcoded fixed-shade backgrounds (e.g. `bg-gray-700`) require literal-colour text** (e.g. `text-white`), NOT flipping tokens (e.g. `text-foreground`, `text-inverse-text`). The token would flip to a colour that no longer contrasts with the always-fixed surface in one mode. Tooltip is the canonical example: surface is `bg-gray-700` in both modes, so `text-inverse-text` (dark in dark mode) renders invisible. Use `text-white` literal and add an anti-revert comment. Caught by harness **Check 5**.

- **Alpha-based tokens MUST have a dark-mode override.** Any `--color-X-N: var(--color-blackAlpha-N)` definition in `colors.css` is subtle dark on white — invisible on the dark page bg without a `.dark { --color-X-N: var(--color-whiteAlpha-N) }` override in `globals.css`. Skeleton's `bg-neutral-50` was the trigger: defined as `blackAlpha-50`, never flipped, rendered invisibly in dark mode. The fix is systemic — flip the entire `neutral-50` through `neutral-900` palette in `.dark { }`. Caught by harness **Check 6**.

- **Wrapper `<span>` containing an `<Icon>` needs `flex items-center justify-center` to centre the SVG vertically.** A bare `<span>` is `display: inline` and aligns to the text baseline — an SVG inside ends up offset a few pixels below true centre. Add `inline-flex items-center justify-center` (or `flex` if the span occupies a flex slot of its own). Checkbox's `data-slot="checkbox-indicator"` span hit this; Blok's CheckboxIndicator includes those classes by default in its Radix wrapper.

- **Don't add extra shadows / borders to "thumb" or "indicator" elements that Blok doesn't have.** Switch's circular thumb appeared visually low because we added `shadow-sm` (Blok doesn't); the drop shadow extends below the thumb and the eye perceives the centre of the visual mass — including the shadow — as below the geometric centre. Same risk with any draggable thumb (Slider, Switch), drag handle, or indicator dot. Strip extra visual weight that doesn't appear in Blok's class string. Drift check (Check 3) catches in the Blok→ours direction; the reverse (extras we have, Blok doesn't) requires reading Blok source side-by-side or visual diff via Chrome MCP.

- **State-selector tokens (`data-[state=*]:`, `aria-[invalid]:`, `[&_svg]:`) are now caught by Check 3 drift detection.** The previous filter stripped any token containing `[...]` to avoid false positives on arbitrary values (`bg-[#ff0000]`, `min-h-[4rem]`). It now distinguishes: split the token on `:`, inspect the LAST segment — only skip if THAT (the actual utility, not the variant prefix) is an arbitrary value. Catches state-selector drift like `dark:data-[state=unchecked]:bg-foreground` that the old filter missed. If you see new drift findings on state-selector tokens after pulling, they're real divergences worth investigating, not filter noise.

- **`Icon` component: `ResetClassName` opts out of layout defaults only — it preserves `SizeClass` derived from `IconSize`.** The original behaviour stripped EVERYTHING including the size class, leaving the SVG with no dimensions and rendering at viewBox-intrinsic 24×24, which clips invisibly inside containers smaller than 24px (Checkbox indicator was 16px). Always pass `IconSize` when you need a specific size; `ResetClassName` only removes `inline-block align-middle shrink-0`.

- **Native form controls (`<select>`, `<input type="date|color|file|range">`, scrollbars) need `color-scheme` on `<html>`, not on a deep wrapper.** Browser chrome reads the document root's `color-scheme` to decide which UI variant to render — setting `.dark { color-scheme: dark }` on a wrapper div doesn't reach the popup. Required: (1) JS toggle sets `document.documentElement.style.colorScheme`, (2) a synchronous head script reads localStorage and applies the same on first paint to avoid FOUC. See `Catalogue/Components/Extra/ThemeToggleStartupScript.razor` and `Components/Extra/ThemeToggle.razor.js` for the canonical pattern.

- **`<select>` with `opacity-0` overlay needs `<option>`-level styling in dark mode.** Edge and some Chromium versions ignore `color-scheme` on transparent form controls when rendering the open dropdown popup. Add `dark:[&_option]:bg-background dark:[&_option]:text-foreground` to the `<select>`'s class so the option list itself carries explicit bg/colour — Chromium honours these on `<option>` regardless of the parent's transparency. Calendar's month/year selects use this pattern.

- **Don't set `text-*` in BOTH variant base AND compound variant for the same colour scheme.** Two `text-*` utilities at equal CSS specificity end up in a source-order race; the winner depends on Tailwind's compile order, which can flip between modes when one token is theme-flipping and the other isn't. Button's Default-variant base used to set `text-primary-foreground` while the Default+Primary compound set `text-inverse-text`. Both applied. In dark mode, `text-primary-foreground` (light blue) won the race, producing washed-out light text on the light primary surface. Pick ONE place to set text colour per (variant × colour-scheme) cell — usually the compound, since it knows the bg.

- **Add `name` attributes to native form controls** (`<input>`, `<select>`, `<textarea>`). Browsers warn in DevTools when form controls lack `name` (or `id`), and certain assistive tech and form-fill features rely on it. Even controls used purely for UI (e.g. Calendar's `opacity-0` select overlay) should have a `name` for cleanliness — the warning is loud and noisy.

- **Catalogue pages must classify interactivity and installation requirements — the badges are a contract with the consumer.** Every page under `Catalogue/Components/Pages/Primitives/` (except `Index.razor`) needs `Interactivity="ComponentInteractivity.Interactive"` or `Interactivity="ComponentInteractivity.Ssr"` on its `<ComponentPage>`. The Interactive badge is a promise that the component needs a Blazor circuit (or JS interop) to deliver its primary API; the SSR badge is a promise that the component renders and functions identically as pure markup. Get this wrong and someone will wire an interactive component into a static-SSR page expecting it to work, or conversely assume an SSR-safe component needs an interactive-server render mode when it doesn't. Components that also need consumer-wired host-page scripts (`CodeViewer` → Prism, `ThemeToggle` → startup script) add `RequiresScripts="true"` for the warning badge. Components that need a service registration (`AddSitecoreBlokUI()`) OR a root-level layout component (`<Popovers />`, `<Toaster />`) OR inherit infrastructure from another component that does (Select, DatePicker via Popover) must also add an `<InstallationNote>` block — the warning badge only shows up when scripts are needed, but the installation section is required whenever setup is needed beyond the `@using` line. Auto-loaded JS modules (`JS.InvokeAsync<IJSObjectReference>("import", "./_content/...")`) do not count as consumer-wired scripts and need no `RequiresScripts` or `<InstallationNote>`.

- **Visual verification must exercise every Catalogue example — not just the one above the fold, and not just in light mode.** The first Combobox migration shipped with three live bugs that Chrome MCP capture #1 (light, resting) could not surface: chip labels reverting to raw values after dropdown close, dark-mode chip text invisible on a never-flipped `--primary-background`, and the filter staying sticky after selection. All three were in the Multi-select-with-chips example, which was below the fold. The takeaway is a process rule: the verify flow iterates through every `<ComponentExample>` block, scrolls it into view, and exercises the primary user interaction — click, type, select, clear, dismiss, blur-and-refocus — and repeats the full walk in dark mode. Captures of resting-state markup alone are insufficient; interactive state is where per-variant regressions hide. See Phase 6 "Coverage rule — every example on the page, every example is exercised" for the checklist.

- **`PopoverService`-hosted popups are not live-state-reactive.** The `<Popovers>` host renders whatever `RenderFragment` was captured when the popover opened. It does NOT re-render when the originating component's state changes. If the popup's content must react to parent state — filter text updating visible items, highlight changing on keystroke, dynamic content — the Popovers host will show stale output. Symptom: typing into a combobox doesn't filter the visible list. Fix: render the popup in-place in the originating component's tree with `fixed` + inline `left`/`top` styles (DropdownMenu pattern). Reserve `PopoverService` for one-shot popups where the user opens, picks, and closes — Select, DatePicker, plain DropdownMenu. See Phase 2 "Complex Component Patterns" rule 1.

- **`fixed` must be in the CSS class whenever you set inline `left`/`top`.** Without `position: fixed` (or `absolute`), `left`/`top`/`right`/`bottom` styles are inert — the browser silently ignores them and the element flows in normal document order. Symptom: a popup renders but appears at a completely unrelated position (often downstream in the flex/block flow). Add `fixed` to the class string AND set `left`/`top` via inline style. DropdownMenuContent and ComboboxContent both do this; an anti-revert comment on the class explains it.

- **Always-mount popups that track per-value state.** If the popup's items register with the parent (to populate a `Dictionary<string, Label>` cache or similar), don't guard the whole popup with `@if (IsOpen)` — items unmount on close, parent loses the cache, next-time labels fall back to raw Values. Instead mount the popup DOM always and set `style="display:none"` when `!IsOpen`. Guard only the backdrop + transient chrome with `@if`. Combobox must do this so: (a) the input displays the selected label when the dropdown is closed, (b) ComboboxChip renders labels after the user moves focus away, (c) pre-selected `Value` on a disabled combobox resolves to the correct `Label` without ever opening.

- **Post-register re-render for parent-consumed child state.** Parent's first render happens BEFORE children's `OnInitialized` fires — so any content in the parent whose value depends on a cache children populate via `Register()` will be stale on initial render. Pattern: set `ChildrenDirty = true` in `Register()`, then in the parent's `OnAfterRenderAsync` observe and clear the flag, calling `InvokeAsync(StateHasChanged)`. Second render picks up the populated cache. Bounded — subsequent `Register()` calls for the same item don't flip the flag because `Children.Contains(item)` is true. Combobox's `LabelCacheDirty` and `ComboboxList.ChildrenDirty` both use this pattern; without it the initial popup open shows raw values until something else triggers a re-render.

- **Distinguish `null` from `""` for filter-input state.** `string.IsNullOrEmpty(text)` conflates "never typed" and "explicitly cleared" into one state. For components that let the user backspace the field clear as a reset gesture (Combobox), use `is null` vs `is not null` checks:
  - `InputValue is null` → fall back to the selected label (placeholder case)
  - `InputValue is ""` → user deleted; show empty; optionally clear the backing `Value` too
  - `InputValue is "text"` → user typing; show text; apply filter
  
  Without this distinction, backspacing to empty "snaps back" to the selected label — which the user will catch within a minute of interacting with the field.

- **Highlight state tracks keyboard AND mouse — not one or the other.** Components with an internal "highlighted item" (Combobox, Menu, submenu, complex picker) need two input streams updating the same `HighlightedValue`: `@onkeydown` on the trigger/input for arrow keys / Home/End, and `@onmouseenter` on each item for hover. React primitives wire both implicitly via `@base-ui`/Radix source; porting to Blazor it's easy to add only the keyboard path (since the source's keyboard handlers are obvious from the tsx). Mouse hover is usually the FIRST thing the user tries — omit it and the component feels broken even though keyboard nav works.

- **Filterable lists with grouping structure need filter-aware wrapper visibility.** `ComboboxLabel` inside a `ComboboxGroup` must hide when all items in the group are filtered out. `ComboboxSeparator` between groups must hide when stranded (first visible, last visible, or adjacent to another visible separator). Pattern: the group tracks its items, exposes `HasVisibleItems()`; the list tracks its direct children in render order, exposes `ShouldRenderSeparator(sep)`. Label and separator read these and conditionally render. Skipping this leaves stranded headings and stacked separator lines around filtered-out groups — the first bug a user catches when they type into a filter.

- **`Improved` badge requires a concrete addition — paradigm translation is Parity.** A component is `Improved` only if it has a user-visible feature Blok does not (Alert's `Closeable`, Pagination's `Click` callback). Mark it `Improved` and you must also write the "Additions Beyond Blok" entry on `Home.razor` with a Primary badge describing the specific feature. Do NOT mark as `Improved`: (a) paradigm translations (React hooks → Blazor bindings, `useXAnchor` hooks omitted because Blazor has `@ref`); (b) missing features you haven't yet ported — those belong in "Known Feature Gaps" with Warning badges; (c) renamings handled by cross-reference stubs (Sonner → Toaster). Rule: write the "Additions Beyond Blok" line first. If you can't without hedging, the component is `Parity`, not `Improved`. The audit cycle that produced Combobox marked it `Improved` because "it has Blazor-idiomatic bindings" — but that is paradigm, not behaviour. Downgraded to `Parity` after review.

- **Blok's `overflow-auto` on narrow surfaces is almost always an unintended UX bug — replace it with the canonical no-clip scrollbar pattern.** Any time a ported component has an internal scrolling region (navigation rails, popover item lists, long tables, sheet bodies, code blocks, calendar month grids) and Blok's source uses bare `overflow-auto`, audit it against these questions before shipping:
  1. **Is a horizontal scrollbar wanted here?** For rails, popups, side panels — almost never. Use `overflow-y-auto overflow-x-hidden` (or `overflow-x-auto overflow-y-hidden` for horizontal surfaces). Blok's `overflow-auto` is a shadcn default, not an endorsement of 2D scroll.
  2. **Does the surface carry `min-w-*` or fixed-width children?** If yes, the 16px native scrollbar will clip them when content overflows — StackNavigation hit this on its `min-w-14` items against Blok's default scrollbar. Apply the three-piece scrollbar-gutter pattern from the Scrollable Surface Rules in Phase 2: `-mr-*` + `scrollbar-gutter:stable` + `overflow-y-auto overflow-x-hidden`. Write an anti-revert comment calling out all three pieces as load-bearing.
  3. **Does the scrollbar look match the rest of the library?** Apply the canonical thin-scrollbar utilities (`[&::-webkit-scrollbar]:w-1.5 ...`) and the inline Firefox style. A page that mixes native chunky scrollbars and our thin styled ones reads as inconsistent. ScrollArea is the reference — match its thumb colour (`bg-border`), shape (`rounded-full`), track (`bg-transparent`), and button suppression. The only legitimate width tiers are `w-1.5` (rails, popups) and `w-2.5` (ScrollArea-width viewports, sheet bodies).
  4. **Did you verify the overflow state?** Chrome MCP visual pass or manual browser test: scroll the surface past its fold, confirm no horizontal scrollbar appears, confirm no item (especially the active-state background) clips against the vertical scrollbar. Resting-state screenshots hide overflow bugs.

  Taking this pattern is a deliberate UX polish, not a behavioural divergence. The harness Check 3 won't flag it (we're adding, not subtracting Blok utilities). Document the change in the component's `docs/ui-parity-audit.md` row as a scrollbar-consistency fix.
