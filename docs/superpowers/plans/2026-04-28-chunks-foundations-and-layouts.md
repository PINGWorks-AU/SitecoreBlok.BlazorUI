# Chunks — Foundations and Layouts Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build the Foundations infrastructure (shared enums, helper classes for `Position`/`Orientation`/`Placement`) and the complete `Layouts/` family (8 Chunks: `AppShell`, `PageShell`, `CenteredShell`, `SplitShell`, `ListDetailShell`, `BlankShell`, `DialogShell`, `SheetShell`), plus the Catalogue scaffold (`/chunks` route, top-nav link, route-aware left-nav).

**Architecture:** Per-shared-enum helper classes under `Components/Chunks/Shared/` (only those Layouts consumes). Each Chunk is a `.razor` file under `Components/Chunks/Layouts/` using `RenderFragment?` slots for strict-shape regions. Variants via shared enums in `Enums.cs`; class strings literal-assembled via the existing `CssClassBuilder`. Catalogue gains a `Chunks` top-nav link mirroring `Primitives`, with a route-aware left-nav (`NavMenu` on `/primitives`, `ChunksNavMenu` on `/chunks`).

**Tech Stack:** .NET 10, Blazor (Server + Static SSR via `<ComponentPage>`'s `Interactivity` switch), Tailwind v4 (auto-source-detection — `@source '../../components';` in `blok.css` already covers everything under `Components/Chunks/`). C#, no unit-test framework — the project uses Catalogue pages as the visual test harness; `tools/verify-ui-parity.ps1` catches regressions. **TDD adapted accordingly:** for each chunk, write the Catalogue page first (it fails to compile because the chunk doesn't exist yet), implement the chunk minimally, build passes, visually verify on the running Catalogue.

**Spec reference:** `docs/superpowers/specs/2026-04-28-chunks-design.md`. All conventions (§5.1–§5.12) bind every task. Layouts roster (§7.1) is the source of truth for chunk APIs.

**Status:** Plan 1 of 7. Plans 2–7 cover Headers, Navigation, Content, Forms, Data, Marketplace.

---

## File Structure

**Library (`PINGWorks.SitecoreBlok.BlazorUI/`)**
- Create: `Components/Chunks/Enums.cs` — Chunks-shared enums (`Position`, `Orientation`, `Placement`). Kept separate from the project-root `Enums.cs` so the latter stays canonical to Blok primitives (per spec §5.10). Same namespace as the rest of the library.
- Create: `Components/Chunks/Shared/PositionClasses.cs` — internal helper for `Position` → `SheetSide` translation + Tailwind class lookups.
- Create: `Components/Chunks/Shared/OrientationClasses.cs` — internal helper for flex/divide directions.
- Create: `Components/Chunks/Shared/PlacementClasses.cs` — internal helper for left/right/none aside placement.
- Create: `Components/Chunks/Layouts/AppShell.razor` — outer dark-mode + popover/toaster mount, `InteractiveRenderMode` parameter.
- Create: `Components/Chunks/Layouts/PageShell.razor` — in-app page envelope with `Header`/`Body`/`Aside`/`Footer` slots.
- Create: `Components/Chunks/Layouts/CenteredShell.razor` — single centered column.
- Create: `Components/Chunks/Layouts/SplitShell.razor` — two-pane split, optional resizable.
- Create: `Components/Chunks/Layouts/ListDetailShell.razor` — opinionated split for master/detail with selection.
- Create: `Components/Chunks/Layouts/BlankShell.razor` — no-chrome envelope.
- Create: `Components/Chunks/Layouts/DialogShell.razor` — wraps `Dialog` primitive.
- Create: `Components/Chunks/Layouts/SheetShell.razor` — wraps `Sheet` primitive, translates `Position` → `SheetSide`.
- Modify: `_Imports.razor` — add `@using PINGWorks.SitecoreBlok.BlazorUI` for Chunks namespace exposure (already there for primitives, but verify Chunks subnamespace works).

**Catalogue (`PINGWorks.SitecoreBlok.BlazorUI.Catalogue/`)**
- Create: `Services/ChunksManifest.cs` — single source of truth for chunk metadata (name, family, description, interactivity).
- Create: `Components/Pages/Chunks/Index.razor` — `/chunks` landing page listing all 7 families with chunk grid.
- Create: `Components/Pages/Chunks/Layouts/AppShellPage.razor` — Catalogue page with examples + API table for `AppShell`.
- Create: `Components/Pages/Chunks/Layouts/PageShellPage.razor`.
- Create: `Components/Pages/Chunks/Layouts/CenteredShellPage.razor`.
- Create: `Components/Pages/Chunks/Layouts/SplitShellPage.razor`.
- Create: `Components/Pages/Chunks/Layouts/ListDetailShellPage.razor`.
- Create: `Components/Pages/Chunks/Layouts/BlankShellPage.razor`.
- Create: `Components/Pages/Chunks/Layouts/DialogShellPage.razor`.
- Create: `Components/Pages/Chunks/Layouts/SheetShellPage.razor`.
- Create: `Components/Layout/ChunksNavMenu.razor` — left-nav rendered when on `/chunks*` routes.
- Modify: `Components/Layout/MainLayout.razor` — add `Chunks` top-nav link; route-aware left-nav switch between `NavMenu` and `ChunksNavMenu`.

---

## Conventions for every task

**Build verification:** `dotnet build` from the repo root (the solution file is `SitecoreBlok.BlazorUI.slnx`). Expected output for a passing build: `Build succeeded with N warning(s)` and exit code 0.

**Catalogue smoke run:** `dotnet run --project PINGWorks.SitecoreBlok.BlazorUI.Catalogue/PINGWorks.SitecoreBlok.BlazorUI.Catalogue.csproj` then navigate in a browser to the URL specified in each task. Each chunk's Catalogue page must render without exceptions and visually match the spec description.

**Commit style — REVISED MID-PLAN:** subagents must **NOT commit** and must **NOT stage** (`git add`) their changes. The user reviews each task's working-tree diff manually, decides on fixes, and stages/commits at their own cadence. **Wherever a task below shows a `git add` / `git commit` step, treat it as superseded by this rule** — replace those steps with "Leave changes unstaged for the user to review." (Tasks below have not all been edited individually; this convention overrides them.) Build verification is still required; only the commit/stage step is removed.

**Tabs vs spaces:** the existing codebase uses **tabs** for indentation in `.razor` and `.cs` files. Match that.

---

## Phase 1 — Foundations

### Task 1: Create `Components/Chunks/Enums.cs` with `Position`, `Orientation`, `Placement` enums

**Files:**
- Create: `PINGWorks.SitecoreBlok.BlazorUI/Components/Chunks/Enums.cs` — new file. Same namespace (`PINGWorks.SitecoreBlok.BlazorUI`) as the project-root `Enums.cs`, but kept separate to keep the project-root file canonical to Blok primitives (per spec §5.10).

- [ ] **Step 1: Create the file with this exact content**

```csharp
namespace PINGWorks.SitecoreBlok.BlazorUI;

// Chunks-shared enums. Lives separately from the project-root Enums.cs so the
// latter stays canonical to Blok primitives. Both files share this namespace,
// so consumer code resolves either file's enums identically.

public enum Position { Top, Right, Bottom, Left }
public enum Orientation { Horizontal, Vertical }
public enum Placement { Left, Right, None }
```

- [ ] **Step 2: Build to confirm no syntax errors**

Run: `dotnet build PINGWorks.SitecoreBlok.BlazorUI/PINGWorks.SitecoreBlok.BlazorUI.csproj`
Expected: `Build succeeded` exit code 0.

- [ ] **Step 3: Leave changes UNSTAGED** (per workflow change — do NOT commit, do NOT stage). The user reviews and commits each task themselves.

---

### Task 2: Create `Components/Chunks/Shared/PositionClasses.cs`

**Files:**
- Create: `PINGWorks.SitecoreBlok.BlazorUI/Components/Chunks/Shared/PositionClasses.cs`

- [ ] **Step 1: Create the file with this exact content**

```csharp
namespace PINGWorks.SitecoreBlok.BlazorUI;

/// <summary>
/// Tailwind class lookups and primitive translations for the shared <see cref="Position"/> enum.
/// Consumed by Chunks that wrap a side/edge-aware primitive (currently SheetShell).
/// </summary>
internal static class PositionClasses
{
	/// <summary>Translate a Chunk-level <see cref="Position"/> into the primitive <see cref="SheetSide"/>.</summary>
	public static SheetSide ToSheetSide( Position position ) => position switch
	{
		Position.Top    => SheetSide.Top,
		Position.Right  => SheetSide.Right,
		Position.Bottom => SheetSide.Bottom,
		Position.Left   => SheetSide.Left,
		_               => SheetSide.Right,
	};
}
```

- [ ] **Step 2: Build**

Run: `dotnet build PINGWorks.SitecoreBlok.BlazorUI/PINGWorks.SitecoreBlok.BlazorUI.csproj`
Expected: `Build succeeded` exit code 0.

- [ ] **Step 3: Commit**

```bash
git add PINGWorks.SitecoreBlok.BlazorUI/Components/Chunks/Shared/PositionClasses.cs
git commit -m "chunks: add PositionClasses helper"
```

---

### Task 3: Create `Components/Chunks/Shared/OrientationClasses.cs`

**Files:**
- Create: `PINGWorks.SitecoreBlok.BlazorUI/Components/Chunks/Shared/OrientationClasses.cs`

- [ ] **Step 1: Create the file with this exact content**

```csharp
namespace PINGWorks.SitecoreBlok.BlazorUI;

/// <summary>
/// Tailwind class lookups for the shared <see cref="Orientation"/> enum.
/// Consumed by Chunks that arrange children along an axis (currently SplitShell).
/// </summary>
internal static class OrientationClasses
{
	/// <summary>Flex direction class — "flex-row" or "flex-col".</summary>
	public static string Flex( Orientation orientation ) => orientation switch
	{
		Orientation.Horizontal => "flex-row",
		Orientation.Vertical   => "flex-col",
		_                      => "flex-row",
	};

	/// <summary>Divide-by class — adds a 1px divider between flex children along the cross axis.</summary>
	public static string Divide( Orientation orientation ) => orientation switch
	{
		Orientation.Horizontal => "divide-x divide-border",
		Orientation.Vertical   => "divide-y divide-border",
		_                      => "divide-x divide-border",
	};
}
```

- [ ] **Step 2: Build**

Run: `dotnet build PINGWorks.SitecoreBlok.BlazorUI/PINGWorks.SitecoreBlok.BlazorUI.csproj`
Expected: `Build succeeded` exit code 0.

- [ ] **Step 3: Commit**

```bash
git add PINGWorks.SitecoreBlok.BlazorUI/Components/Chunks/Shared/OrientationClasses.cs
git commit -m "chunks: add OrientationClasses helper"
```

---

### Task 4: Create `Components/Chunks/Shared/PlacementClasses.cs`

**Files:**
- Create: `PINGWorks.SitecoreBlok.BlazorUI/Components/Chunks/Shared/PlacementClasses.cs`

- [ ] **Step 1: Create the file with this exact content**

```csharp
namespace PINGWorks.SitecoreBlok.BlazorUI;

/// <summary>
/// Tailwind class lookups for the shared <see cref="Placement"/> enum.
/// Consumed by Chunks that locate a side region (currently PageShell.AsidePlacement).
/// </summary>
internal static class PlacementClasses
{
	/// <summary>Flex order class for the aside relative to the body.</summary>
	public static string AsideOrder( Placement placement ) => placement switch
	{
		Placement.Left  => "order-first",
		Placement.Right => "order-last",
		Placement.None  => "hidden",
		_               => "order-last",
	};

	/// <summary>Whether the aside should render at all.</summary>
	public static bool ShowAside( Placement placement ) => placement is Placement.Left or Placement.Right;
}
```

- [ ] **Step 2: Build**

Run: `dotnet build PINGWorks.SitecoreBlok.BlazorUI/PINGWorks.SitecoreBlok.BlazorUI.csproj`
Expected: `Build succeeded` exit code 0.

- [ ] **Step 3: Commit**

```bash
git add PINGWorks.SitecoreBlok.BlazorUI/Components/Chunks/Shared/PlacementClasses.cs
git commit -m "chunks: add PlacementClasses helper"
```

---

## Phase 2 — Catalogue scaffold

### Task 5: Create `ChunksManifest.cs` in the Catalogue

**Files:**
- Create: `PINGWorks.SitecoreBlok.BlazorUI.Catalogue/Services/ChunksManifest.cs`

- [ ] **Step 1: Create the file with this exact content**

```csharp
using PINGWorks.SitecoreBlok.BlazorUI.Catalogue.Services;

namespace PINGWorks.SitecoreBlok.BlazorUI.Catalogue.Services;

/// <summary>
/// Single source of truth for Chunk metadata in the Catalogue.
/// Chunks are not tracked in MIGRATION_STATUS.md (per spec §5.9), so this manifest
/// drives both the /chunks Index page and the ChunksNavMenu left-nav.
/// </summary>
public static class ChunksManifest
{
	public sealed record ChunkEntry(
		string Family,
		string Name,
		string Slug,
		string Description,
		ComponentInteractivity Interactivity );

	public static readonly ChunkEntry[] All =
	[
		// Layouts
		new( "Layouts", "AppShell",        "app-shell",         "Outer dark-mode + popover/toaster mount",                  ComponentInteractivity.Interactive ),
		new( "Layouts", "PageShell",       "page-shell",        "In-app page envelope with header/body/aside/footer slots", ComponentInteractivity.Ssr ),
		new( "Layouts", "CenteredShell",   "centered-shell",    "Single centered column for auth / error / empty states",   ComponentInteractivity.Ssr ),
		new( "Layouts", "SplitShell",      "split-shell",       "Two-pane (master/detail) layout, optional resizable",      ComponentInteractivity.Ssr ),
		new( "Layouts", "ListDetailShell", "list-detail-shell", "Opinionated SplitShell with selection state",              ComponentInteractivity.Interactive ),
		new( "Layouts", "BlankShell",      "blank-shell",       "No-chrome envelope; sets background and font only",        ComponentInteractivity.Ssr ),
		new( "Layouts", "DialogShell",     "dialog-shell",      "Opinionated wrapper around the Dialog primitive",          ComponentInteractivity.Interactive ),
		new( "Layouts", "SheetShell",      "sheet-shell",       "Edge-sliding panel; wraps Sheet primitive",                ComponentInteractivity.Interactive ),
	];

	public static IEnumerable<ChunkEntry> ByFamily( string family ) =>
		All.Where( e => string.Equals( e.Family, family, StringComparison.OrdinalIgnoreCase ) );

	public static IEnumerable<string> Families =>
		All.Select( e => e.Family ).Distinct();
}
```

- [ ] **Step 2: Build**

Run: `dotnet build PINGWorks.SitecoreBlok.BlazorUI.Catalogue/PINGWorks.SitecoreBlok.BlazorUI.Catalogue.csproj`
Expected: `Build succeeded` exit code 0.

- [ ] **Step 3: Commit**

```bash
git add PINGWorks.SitecoreBlok.BlazorUI.Catalogue/Services/ChunksManifest.cs
git commit -m "chunks: add ChunksManifest for catalogue metadata"
```

---

### Task 6: Create `/chunks` Index page

**Files:**
- Create: `PINGWorks.SitecoreBlok.BlazorUI.Catalogue/Components/Pages/Chunks/Index.razor`

- [ ] **Step 1: Create the file with this exact content**

```razor
@page "/chunks"
@using PINGWorks.SitecoreBlok.BlazorUI.Catalogue.Services

<PageTitle>Chunks - Blok Blazor</PageTitle>

<div class="space-y-8 py-4">
	<div>
		<h1 class="text-3xl font-bold tracking-tight">Chunks</h1>
		<p class="text-muted-foreground mt-2 text-base">
			Opinionated compositions of Primitives. Solve the most common page and panel layouts with literal Tailwind already arranged.
		</p>
	</div>

	<Separator />

	@foreach ( var family in ChunksManifest.Families )
	{
		<div class="space-y-2">
			<h2 class="text-sm font-semibold text-muted-foreground uppercase tracking-wide">@family</h2>
			<div class="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 gap-3">
				@foreach ( var entry in ChunksManifest.ByFamily( family ) )
				{
					<a href="@($"chunks/{entry.Slug}")" class="group no-underline">
						<div class="rounded-lg border border-border p-4 bg-background hover:border-primary/50 hover:shadow-sm transition-all">
							<div class="flex items-center gap-2">
								<p class="text-sm font-medium text-foreground group-hover:text-primary transition-colors flex-1 truncate">@entry.Name</p>
							</div>
							<p class="text-xs text-muted-foreground mt-1 line-clamp-2">@entry.Description</p>
						</div>
					</a>
				}
			</div>
		</div>
	}
</div>
```

- [ ] **Step 2: Build**

Run: `dotnet build PINGWorks.SitecoreBlok.BlazorUI.Catalogue/PINGWorks.SitecoreBlok.BlazorUI.Catalogue.csproj`
Expected: `Build succeeded` exit code 0.

- [ ] **Step 3: Commit**

```bash
git add PINGWorks.SitecoreBlok.BlazorUI.Catalogue/Components/Pages/Chunks/Index.razor
git commit -m "chunks: add /chunks index page"
```

---

### Task 7: Create `ChunksNavMenu.razor` left-nav

**Files:**
- Create: `PINGWorks.SitecoreBlok.BlazorUI.Catalogue/Components/Layout/ChunksNavMenu.razor`

- [ ] **Step 1: Create the file with this exact content**

```razor
@using PINGWorks.SitecoreBlok.BlazorUI.Catalogue.Services

<nav class="w-60 shrink-0 border-r border-border bg-background h-screen sticky top-0 overflow-y-auto">
	<div class="px-4 h-14 flex items-center border-b border-border">
		<a href="/" class="text-base font-bold text-foreground no-underline tracking-tight">Blok Blazor</a>
	</div>

	<div class="px-3 py-4 space-y-1">
		<NavLink href="/" class="flex items-center rounded-md px-3 py-1.5 text-sm hover:bg-neutral-bg no-underline text-foreground" Match="NavLinkMatch.All">
			Home
		</NavLink>
		<NavLink href="/chunks" class="flex items-center rounded-md px-3 py-1.5 text-sm hover:bg-neutral-bg no-underline text-foreground" Match="NavLinkMatch.All">
			All chunks
		</NavLink>

		@foreach ( var family in ChunksManifest.Families )
		{
			<div class="space-y-0.5 pt-3">
				<div class="px-3 pb-1 text-xs font-semibold text-muted-foreground uppercase tracking-wide">@family</div>
				@foreach ( var entry in ChunksManifest.ByFamily( family ) )
				{
					<NavLink href="@($"chunks/{entry.Slug}")" class="flex items-center gap-2 rounded-md px-3 py-2 text-sm font-medium hover:bg-neutral-bg no-underline text-foreground/70 hover:text-foreground transition-colors" ActiveClass="bg-primary-bg text-primary-fg font-semibold">
						<span class="flex-1 truncate">@entry.Name</span>
					</NavLink>
				}
			</div>
		}
	</div>
</nav>
```

- [ ] **Step 2: Build**

Run: `dotnet build PINGWorks.SitecoreBlok.BlazorUI.Catalogue/PINGWorks.SitecoreBlok.BlazorUI.Catalogue.csproj`
Expected: `Build succeeded` exit code 0.

- [ ] **Step 3: Commit**

```bash
git add PINGWorks.SitecoreBlok.BlazorUI.Catalogue/Components/Layout/ChunksNavMenu.razor
git commit -m "chunks: add ChunksNavMenu left-nav"
```

---

### Task 8: Update `MainLayout.razor` — top-nav `Chunks` link + route-aware left-nav

**Files:**
- Modify: `PINGWorks.SitecoreBlok.BlazorUI.Catalogue/Components/Layout/MainLayout.razor`

- [ ] **Step 1: Read the current file**

Run: `cat PINGWorks.SitecoreBlok.BlazorUI.Catalogue/Components/Layout/MainLayout.razor`
Confirm the structure matches the spec's expectation (top-level `<div data-dark-mode-target>`, sidebar `<NavMenu />`, top header with hard-coded `<a href="/primitives">` etc.).

- [ ] **Step 2: Apply two edits**

**Edit A — add `NavigationManager` injection and route-aware nav switching.** Change the first line (`@inherits LayoutComponentBase`) block by adding the inject directive and replacing `<NavMenu />` with a conditional render:

Old:
```razor
@inherits LayoutComponentBase

@* ... comment block ... *@
<div data-dark-mode-target class="text-foreground">
	<div class="flex h-screen overflow-hidden bg-subtle-bg">
		<NavMenu />
```

New:
```razor
@inherits LayoutComponentBase
@inject NavigationManager Nav

@* ... comment block ... *@
<div data-dark-mode-target class="text-foreground">
	<div class="flex h-screen overflow-hidden bg-subtle-bg">
		@if ( Nav.ToBaseRelativePath( Nav.Uri ).StartsWith( "chunks", StringComparison.OrdinalIgnoreCase ) )
		{
			<ChunksNavMenu />
		}
		else
		{
			<NavMenu />
		}
```

**Edit B — add a `Chunks` link to the top-nav bar.** In the header `<nav>` block (around line 19–25 of the file), add a `Chunks` link between `Primitives` and the existing graphics links:

Old:
```razor
<nav class="flex items-center gap-4">
    <a href="/primitives" class="text-sm text-muted-foreground no-underline hover:text-foreground transition-colors">Primitives</a>
    <a href="/graphics/theming" class="text-sm text-muted-foreground no-underline hover:text-foreground transition-colors">Theming</a>
```

New:
```razor
<nav class="flex items-center gap-4">
    <a href="/primitives" class="text-sm text-muted-foreground no-underline hover:text-foreground transition-colors">Primitives</a>
    <a href="/chunks" class="text-sm text-muted-foreground no-underline hover:text-foreground transition-colors">Chunks</a>
    <a href="/graphics/theming" class="text-sm text-muted-foreground no-underline hover:text-foreground transition-colors">Theming</a>
```

- [ ] **Step 3: Build**

Run: `dotnet build PINGWorks.SitecoreBlok.BlazorUI.Catalogue/PINGWorks.SitecoreBlok.BlazorUI.Catalogue.csproj`
Expected: `Build succeeded` exit code 0.

- [ ] **Step 4: Run the catalogue and visually verify**

Run: `dotnet run --project PINGWorks.SitecoreBlok.BlazorUI.Catalogue/PINGWorks.SitecoreBlok.BlazorUI.Catalogue.csproj`
Open: `https://localhost:<port>/chunks` (port is shown in the run output)
Expect: the `/chunks` Index page renders with a Layouts family section and 8 chunk cards. Top-nav shows `Home | Primitives | Chunks | …`. Left-nav is the `ChunksNavMenu`. Click a chunk card — the page will 404 because the per-chunk catalogue pages don't exist yet — that's the next phase.

Stop the run with Ctrl+C.

- [ ] **Step 5: Commit**

```bash
git add PINGWorks.SitecoreBlok.BlazorUI.Catalogue/Components/Layout/MainLayout.razor
git commit -m "chunks: wire Chunks top-nav link and route-aware left-nav"
```

---

## Phase 3 — `Layouts/AppShell`

### Task 9: Write the Catalogue page for `AppShell`

**Files:**
- Create: `PINGWorks.SitecoreBlok.BlazorUI.Catalogue/Components/Pages/Chunks/Layouts/AppShellPage.razor`

- [ ] **Step 1: Create the file with this exact content**

```razor
@page "/chunks/app-shell"
@rendermode InteractiveServer

<ComponentPage Title="AppShell" Description="Outer dark-mode + popover/toaster mount; the chrome a top-level Blazor app sits inside" Interactivity="ComponentInteractivity.Interactive">

	<ExamplesSection>

		<ComponentExample Title="Default" Code="@("<AppShell InteractiveRenderMode=\"@(new InteractiveServerRenderMode())\">\n    <Header>...</Header>\n    <Sidebar>...</Sidebar>\n    <Content>...</Content>\n</AppShell>")">
			<div class="h-96 border rounded-lg overflow-hidden">
				<AppShell InteractiveRenderMode="@(new InteractiveServerRenderMode())">
					<Header>
						<div class="flex items-center gap-4 px-4 h-12 border-b border-border bg-background">
							<span class="font-semibold">Header slot</span>
						</div>
					</Header>
					<Sidebar>
						<div class="w-48 border-r border-border bg-background p-4">
							<span class="text-sm">Sidebar slot</span>
						</div>
					</Sidebar>
					<Content>
						<div class="p-4">Content slot</div>
					</Content>
				</AppShell>
			</div>
		</ComponentExample>

	</ExamplesSection>

</ComponentPage>
```

- [ ] **Step 2: Build — expect failure**

Run: `dotnet build`
Expected: build FAILS with errors mentioning `AppShell` (component not found) and possibly `Header`/`Sidebar`/`Content` (RenderFragment slots not found). This is the "failing test" — confirming the chunk doesn't exist yet.

- [ ] **Step 3: No commit yet** — failing build never commits.

---

### Task 10: Implement `AppShell.razor`

**Files:**
- Create: `PINGWorks.SitecoreBlok.BlazorUI/Components/Chunks/Layouts/AppShell.razor`

- [ ] **Step 1: Create the file with this exact content**

```razor
@namespace PINGWorks.SitecoreBlok.BlazorUI

@*
	AppShell — outermost shell for a top-level Blazor app.
	Provides the dark-mode wrapper, ambient layout, and mounts the global Popovers
	and Toaster primitives with the consumer-supplied render mode.

	Slots: Header, Sidebar, Content, Footer (all RenderFragment? per spec §5.1).
*@

<div data-dark-mode-target class="text-foreground">
	<div class="flex h-screen overflow-hidden bg-subtle-bg">
		@if ( Sidebar is not null )
		{
			@Sidebar
		}

		<div class="flex-1 flex flex-col min-w-0">
			@if ( Header is not null )
			{
				<header class="sticky top-0 z-40 border-b border-border bg-background/95 backdrop-blur">
					@Header
				</header>
			}

			<main class="flex-1 flex flex-col min-h-0 overflow-hidden">
				<div class="flex-1 flex flex-col min-h-0 overflow-y-auto">
					@if ( Content is not null )
					{
						@Content
					}
				</div>
			</main>

			@if ( Footer is not null )
			{
				<footer class="border-t border-border bg-background">
					@Footer
				</footer>
			}
		</div>
	</div>

	<Popovers @rendermode="InteractiveRenderMode" />
	<Toaster @rendermode="InteractiveRenderMode" />
</div>

@code {
	[Parameter] public RenderFragment? Header { get; set; }
	[Parameter] public RenderFragment? Sidebar { get; set; }
	[Parameter] public RenderFragment? Content { get; set; }
	[Parameter] public RenderFragment? Footer { get; set; }

	/// <summary>
	/// Render mode applied to the internally-mounted Popovers and Toaster primitives.
	/// Consumers pass typically <c>new InteractiveServerRenderMode()</c> or
	/// <c>new InteractiveAutoRenderMode()</c>. Default null inherits from the surrounding
	/// host context — fails fast if the host isn't already interactive (desired).
	/// </summary>
	[Parameter] public IComponentRenderMode? InteractiveRenderMode { get; set; }
}
```

- [ ] **Step 2: Build — expect success**

Run: `dotnet build`
Expected: `Build succeeded` exit code 0.

- [ ] **Step 3: Run catalogue and visually verify**

Run: `dotnet run --project PINGWorks.SitecoreBlok.BlazorUI.Catalogue/PINGWorks.SitecoreBlok.BlazorUI.Catalogue.csproj`
Open: `https://localhost:<port>/chunks/app-shell`
Expect: the AppShell example renders inside its bordered preview box, showing Header / Sidebar / Content slots. Click around the catalogue's other tabs to confirm nothing else broke.

Stop the run with Ctrl+C.

- [ ] **Step 4: Commit**

```bash
git add PINGWorks.SitecoreBlok.BlazorUI/Components/Chunks/Layouts/AppShell.razor PINGWorks.SitecoreBlok.BlazorUI.Catalogue/Components/Pages/Chunks/Layouts/AppShellPage.razor
git commit -m "chunks: implement Layouts/AppShell with InteractiveRenderMode parameter"
```

---

## Phase 4 — `Layouts/PageShell`

### Task 11: Write the Catalogue page for `PageShell`

**Files:**
- Create: `PINGWorks.SitecoreBlok.BlazorUI.Catalogue/Components/Pages/Chunks/Layouts/PageShellPage.razor`

- [ ] **Step 1: Create the file with this exact content**

```razor
@page "/chunks/page-shell"

<ComponentPage Title="PageShell" Description="In-app page envelope sitting inside AppShell.Content" Interactivity="ComponentInteractivity.Ssr">

	<ExamplesSection>

		<ComponentExample Title="Aside on right (default)" Code="@("<PageShell AsidePlacement=\"Placement.Right\">\n    <Header>...</Header>\n    <Body>...</Body>\n    <Aside>...</Aside>\n</PageShell>")">
			<div class="h-80 border rounded-lg overflow-hidden">
				<PageShell AsidePlacement="Placement.Right">
					<Header>
						<div class="border-b border-border p-3"><span class="font-semibold">Header</span></div>
					</Header>
					<Body>
						<div class="p-4">Body</div>
					</Body>
					<Aside>
						<div class="w-48 border-l border-border bg-background p-3 h-full"><span class="text-sm">Aside</span></div>
					</Aside>
				</PageShell>
			</div>
		</ComponentExample>

		<ComponentExample Title="Aside on left" Code="@("<PageShell AsidePlacement=\"Placement.Left\">...</PageShell>")">
			<div class="h-80 border rounded-lg overflow-hidden">
				<PageShell AsidePlacement="Placement.Left">
					<Header><div class="border-b border-border p-3"><span class="font-semibold">Header</span></div></Header>
					<Body><div class="p-4">Body</div></Body>
					<Aside><div class="w-48 border-r border-border bg-background p-3 h-full"><span class="text-sm">Aside</span></div></Aside>
				</PageShell>
			</div>
		</ComponentExample>

		<ComponentExample Title="No aside" Code="@("<PageShell AsidePlacement=\"Placement.None\">\n    <Header>...</Header>\n    <Body>...</Body>\n</PageShell>")">
			<div class="h-80 border rounded-lg overflow-hidden">
				<PageShell AsidePlacement="Placement.None">
					<Header><div class="border-b border-border p-3"><span class="font-semibold">Header</span></div></Header>
					<Body><div class="p-4">Body fills remaining width</div></Body>
				</PageShell>
			</div>
		</ComponentExample>

	</ExamplesSection>

</ComponentPage>
```

- [ ] **Step 2: Build — expect failure**

Run: `dotnet build`
Expected: build FAILS with `PageShell` not found.

---

### Task 12: Implement `PageShell.razor`

**Files:**
- Create: `PINGWorks.SitecoreBlok.BlazorUI/Components/Chunks/Layouts/PageShell.razor`

- [ ] **Step 1: Create the file with this exact content**

```razor
@namespace PINGWorks.SitecoreBlok.BlazorUI

@*
	PageShell — in-app page envelope. Sits inside AppShell.Content.
	Slots: Header, Body, Aside, Footer (RenderFragment? per spec §5.1).
	AsidePlacement controls where (or if) the Aside renders.
*@

<div class="flex flex-col h-full min-h-0">
	@if ( Header is not null )
	{
		<div class="flex-shrink-0">
			@Header
		</div>
	}

	<div class="@BodyRowClass">
		<div class="flex-1 min-w-0 overflow-y-auto">
			@if ( Body is not null )
			{
				@Body
			}
		</div>

		@if ( PlacementClasses.ShowAside( AsidePlacement ) && Aside is not null )
		{
			<div class="@AsideClass">
				@Aside
			</div>
		}
	</div>

	@if ( Footer is not null )
	{
		<div class="flex-shrink-0">
			@Footer
		</div>
	}
</div>

@code {
	[Parameter] public RenderFragment? Header { get; set; }
	[Parameter] public RenderFragment? Body { get; set; }
	[Parameter] public RenderFragment? Aside { get; set; }
	[Parameter] public RenderFragment? Footer { get; set; }

	[Parameter] public Placement AsidePlacement { get; set; } = Placement.Right;

	private string BodyRowClass => CssClassBuilder.Start( "flex flex-1 min-h-0" )
		.With( "flex-row", AsidePlacement is Placement.Right )
		.With( "flex-row-reverse", AsidePlacement is Placement.Left )
		.Build();

	private string AsideClass => CssClassBuilder.Start( "flex-shrink-0" )
		.With( PlacementClasses.AsideOrder( AsidePlacement ) )
		.Build();
}
```

- [ ] **Step 2: Build — expect success**

Run: `dotnet build`
Expected: `Build succeeded` exit code 0.

- [ ] **Step 3: Visual verify**

Run the catalogue, open `https://localhost:<port>/chunks/page-shell`. Each of the three examples (`Aside on right`, `Aside on left`, `No aside`) renders correctly. Aside switches sides between examples. Stop with Ctrl+C.

- [ ] **Step 4: Commit**

```bash
git add PINGWorks.SitecoreBlok.BlazorUI/Components/Chunks/Layouts/PageShell.razor PINGWorks.SitecoreBlok.BlazorUI.Catalogue/Components/Pages/Chunks/Layouts/PageShellPage.razor
git commit -m "chunks: implement Layouts/PageShell with AsidePlacement"
```

---

## Phase 5 — `Layouts/CenteredShell`

### Task 13: Write the Catalogue page for `CenteredShell`

**Files:**
- Create: `PINGWorks.SitecoreBlok.BlazorUI.Catalogue/Components/Pages/Chunks/Layouts/CenteredShellPage.razor`

- [ ] **Step 1: Create the file with this exact content**

```razor
@page "/chunks/centered-shell"

<ComponentPage Title="CenteredShell" Description="Single centered column for auth, error, or empty-app states" Interactivity="ComponentInteractivity.Ssr">

	<ExamplesSection>

		<ComponentExample Title="Md width (default)" Code="@("<CenteredShell MaxWidth=\"Size.Md\">\n    <p>Content</p>\n</CenteredShell>")">
			<div class="h-64 border rounded-lg bg-subtle-bg">
				<CenteredShell MaxWidth="Size.Md">
					<div class="bg-background border border-border rounded-lg p-6 text-center">
						<p class="text-sm">Centered content (Md)</p>
					</div>
				</CenteredShell>
			</div>
		</ComponentExample>

		<ComponentExample Title="Sm width" Code="@("<CenteredShell MaxWidth=\"Size.Sm\">...</CenteredShell>")">
			<div class="h-64 border rounded-lg bg-subtle-bg">
				<CenteredShell MaxWidth="Size.Sm">
					<div class="bg-background border border-border rounded-lg p-6 text-center"><p class="text-sm">Centered content (Sm)</p></div>
				</CenteredShell>
			</div>
		</ComponentExample>

		<ComponentExample Title="Lg width" Code="@("<CenteredShell MaxWidth=\"Size.Lg\">...</CenteredShell>")">
			<div class="h-64 border rounded-lg bg-subtle-bg">
				<CenteredShell MaxWidth="Size.Lg">
					<div class="bg-background border border-border rounded-lg p-6 text-center"><p class="text-sm">Centered content (Lg)</p></div>
				</CenteredShell>
			</div>
		</ComponentExample>

	</ExamplesSection>

</ComponentPage>
```

- [ ] **Step 2: Build — expect failure** (`CenteredShell` not found).

---

### Task 14: Implement `CenteredShell.razor`

**Files:**
- Create: `PINGWorks.SitecoreBlok.BlazorUI/Components/Chunks/Layouts/CenteredShell.razor`

- [ ] **Step 1: Create the file with this exact content**

```razor
@namespace PINGWorks.SitecoreBlok.BlazorUI

@*
	CenteredShell — single centered column for auth, error, or empty-app states.
	MaxWidth uses the existing shared Size enum.
*@

<div class="flex items-center justify-center w-full h-full p-4">
	<div class="@ColumnClass">
		@ChildContent
	</div>
</div>

@code {
	[Parameter] public RenderFragment? ChildContent { get; set; }
	[Parameter] public Size MaxWidth { get; set; } = Size.Md;

	// Tailwind requires literal class strings; hard-code each branch.
	private string ColumnClass => CssClassBuilder.Start( "w-full" )
		.With( "max-w-3xs", MaxWidth is Size.Xs3 )
		.With( "max-w-2xs", MaxWidth is Size.Xs2 )
		.With( "max-w-xs",  MaxWidth is Size.Xs )
		.With( "max-w-sm",  MaxWidth is Size.Sm )
		.With( "max-w-md",  MaxWidth is Size.Md or Size.Default )
		.With( "max-w-lg",  MaxWidth is Size.Lg )
		.With( "max-w-xl",  MaxWidth is Size.Xl )
		.With( "max-w-2xl", MaxWidth is Size.Xl2 )
		.With( "max-w-3xl", MaxWidth is Size.Xl3 )
		.With( "max-w-4xl", MaxWidth is Size.Xl4 )
		.With( "max-w-5xl", MaxWidth is Size.Xl5 )
		.With( "max-w-6xl", MaxWidth is Size.Xl6 )
		.With( "max-w-7xl", MaxWidth is Size.Xl7 )
		.With( "max-w-full", MaxWidth is Size.Full )
		.Build();
}
```

- [ ] **Step 2: Build — expect success**.

- [ ] **Step 3: Visual verify** at `/chunks/centered-shell`.

- [ ] **Step 4: Commit**

```bash
git add PINGWorks.SitecoreBlok.BlazorUI/Components/Chunks/Layouts/CenteredShell.razor PINGWorks.SitecoreBlok.BlazorUI.Catalogue/Components/Pages/Chunks/Layouts/CenteredShellPage.razor
git commit -m "chunks: implement Layouts/CenteredShell with Size MaxWidth"
```

---

## Phase 6 — `Layouts/SplitShell`

### Task 15: Write the Catalogue page for `SplitShell`

**Files:**
- Create: `PINGWorks.SitecoreBlok.BlazorUI.Catalogue/Components/Pages/Chunks/Layouts/SplitShellPage.razor`

- [ ] **Step 1: Create the file with this exact content**

```razor
@page "/chunks/split-shell"

<ComponentPage Title="SplitShell" Description="Two-pane (master/detail) layout with optional resizable splitter" Interactivity="ComponentInteractivity.Ssr">

	<ExamplesSection>

		<ComponentExample Title="Horizontal" Code="@("<SplitShell Direction=\"Orientation.Horizontal\">\n    <Start>...</Start>\n    <End>...</End>\n</SplitShell>")">
			<div class="h-64 border rounded-lg overflow-hidden">
				<SplitShell Direction="Orientation.Horizontal">
					<Start><div class="p-4 bg-background h-full">Start pane</div></Start>
					<End><div class="p-4 bg-background h-full">End pane</div></End>
				</SplitShell>
			</div>
		</ComponentExample>

		<ComponentExample Title="Vertical" Code="@("<SplitShell Direction=\"Orientation.Vertical\">...</SplitShell>")">
			<div class="h-64 border rounded-lg overflow-hidden">
				<SplitShell Direction="Orientation.Vertical">
					<Start><div class="p-4 bg-background h-full">Top</div></Start>
					<End><div class="p-4 bg-background h-full">Bottom</div></End>
				</SplitShell>
			</div>
		</ComponentExample>

	</ExamplesSection>

</ComponentPage>
```

- [ ] **Step 2: Build — expect failure**.

---

### Task 16: Implement `SplitShell.razor`

**Files:**
- Create: `PINGWorks.SitecoreBlok.BlazorUI/Components/Chunks/Layouts/SplitShell.razor`

- [ ] **Step 1: Create the file with this exact content**

```razor
@namespace PINGWorks.SitecoreBlok.BlazorUI

@*
	SplitShell — two-pane master/detail layout.
	Direction uses the shared Orientation enum.
	Resizable=true wraps the panes in the Resizable primitive (delegates resize behaviour).
*@

@if ( IsResizable )
{
	<ResizablePanelGroup Direction="@( Direction is Orientation.Horizontal ? ResizableDirection.Horizontal : ResizableDirection.Vertical )">
		<ResizablePanel DefaultSize="@( Ratio * 100 )">
			@if ( Start is not null ) { @Start }
		</ResizablePanel>
		<ResizableHandle />
		<ResizablePanel DefaultSize="@( ( 1 - Ratio ) * 100 )">
			@if ( End is not null ) { @End }
		</ResizablePanel>
	</ResizablePanelGroup>
}
else
{
	<div class="@RootClass">
		<div class="flex-1 min-w-0 min-h-0 overflow-auto">
			@if ( Start is not null ) { @Start }
		</div>
		<div class="flex-1 min-w-0 min-h-0 overflow-auto">
			@if ( End is not null ) { @End }
		</div>
	</div>
}

@code {
	[Parameter] public RenderFragment? Start { get; set; }
	[Parameter] public RenderFragment? End { get; set; }

	[Parameter] public Orientation Direction { get; set; } = Orientation.Horizontal;

	// Property is `IsResizable` (not `Resizable`) to avoid colliding with the
	// existing `Resizable` primitive name when consumers have both in scope.
	[Parameter] public bool IsResizable { get; set; } = false;
	[Parameter] public double Ratio { get; set; } = 0.5;

	private string RootClass => CssClassBuilder.Start( "flex h-full w-full" )
		.With( OrientationClasses.Flex( Direction ) )
		.With( OrientationClasses.Divide( Direction ) )
		.Build();
}
```

The primitives this references — verified to exist:
- `ResizablePanelGroup` (in `Components/Resizable/ResizablePanelGroup.razor`) — takes `Direction` (`ResizableDirection`) and `ChildContent`.
- `ResizablePanel` — takes `DefaultSize` (double?), `MinSize`, `MaxSize`, `ChildContent`.
- `ResizableHandle` — divider; no required parameters.

The Catalogue page in Task 15 uses `Direction="Orientation.Horizontal"` only (not the resizable variant) — keep the catalogue page as-is. If you want to add a resizable example later, set `IsResizable="true"`.

- [ ] **Step 2: Build — expect success**.

- [ ] **Step 3: Visual verify** at `/chunks/split-shell`. Both Horizontal and Vertical examples render with the divider line between panes.

- [ ] **Step 4: Commit**

```bash
git add PINGWorks.SitecoreBlok.BlazorUI/Components/Chunks/Layouts/SplitShell.razor PINGWorks.SitecoreBlok.BlazorUI.Catalogue/Components/Pages/Chunks/Layouts/SplitShellPage.razor
git commit -m "chunks: implement Layouts/SplitShell with Orientation Direction"
```

---

## Phase 7 — `Layouts/ListDetailShell`

### Task 17: Write the Catalogue page for `ListDetailShell`

**Files:**
- Create: `PINGWorks.SitecoreBlok.BlazorUI.Catalogue/Components/Pages/Chunks/Layouts/ListDetailShellPage.razor`

- [ ] **Step 1: Create the file with this exact content**

```razor
@page "/chunks/list-detail-shell"
@rendermode InteractiveServer

<ComponentPage Title="ListDetailShell" Description="Opinionated SplitShell for list-on-left / detail-on-right with selection state" Interactivity="ComponentInteractivity.Interactive">

	<ExamplesSection>

		<ComponentExample Title="With selection" Code="@("<ListDetailShell SelectedId=\"@selected\" SelectedIdChanged=\"v => selected = v\">\n    <List>\n        @foreach (var item in items) { ... }\n    </List>\n    <Detail>...</Detail>\n    <Empty>Pick something</Empty>\n</ListDetailShell>")">
			<div class="h-80 border rounded-lg overflow-hidden">
				<ListDetailShell SelectedId="@selected" SelectedIdChanged="v => selected = v">
					<List>
						<div class="w-56 border-r border-border bg-background overflow-auto">
							@foreach ( var item in items )
							{
								<button type="button" @onclick="() => selected = item" class="@( selected == item ? "bg-primary-bg text-primary-fg" : "" ) w-full text-left px-4 py-2 text-sm hover:bg-neutral-bg">
									@item
								</button>
							}
						</div>
					</List>
					<Detail>
						<div class="p-6">
							<p class="text-sm text-muted-foreground">Selected:</p>
							<p class="text-lg font-semibold">@selected</p>
						</div>
					</Detail>
					<Empty>
						<div class="p-6 text-center text-muted-foreground">Pick an item to see details.</div>
					</Empty>
				</ListDetailShell>
			</div>
		</ComponentExample>

	</ExamplesSection>

</ComponentPage>

@code {
	private string? selected = null;
	private string[] items = [ "Alpha", "Bravo", "Charlie", "Delta" ];
}
```

- [ ] **Step 2: Build — expect failure**.

---

### Task 18: Implement `ListDetailShell.razor`

**Files:**
- Create: `PINGWorks.SitecoreBlok.BlazorUI/Components/Chunks/Layouts/ListDetailShell.razor`

- [ ] **Step 1: Create the file with this exact content**

```razor
@namespace PINGWorks.SitecoreBlok.BlazorUI
@typeparam TId

@*
	ListDetailShell — opinionated split layout: list on left, detail on right.
	Selection state is exposed through SelectedId / SelectedIdChanged so consumers can two-way bind.
	When SelectedId is null, the Empty slot renders instead of the Detail slot.

	Per spec §10.2 — orchestration state here is a single primitive (TId), so CascadingValue is appropriate
	(but in practice consumers two-way bind through the parameter pair, so no cascade is required).
*@

<div class="flex h-full w-full divide-x divide-border">
	<div class="flex-shrink-0">
		@if ( List is not null )
		{
			@List
		}
	</div>

	<div class="flex-1 min-w-0 overflow-auto">
		@if ( EqualityComparer<TId?>.Default.Equals( SelectedId, default ) )
		{
			@if ( Empty is not null )
			{
				@Empty
			}
		}
		else
		{
			@if ( Detail is not null )
			{
				@Detail
			}
		}
	</div>
</div>

@code {
	[Parameter] public RenderFragment? List { get; set; }
	[Parameter] public RenderFragment? Detail { get; set; }
	[Parameter] public RenderFragment? Empty { get; set; }

	[Parameter] public TId? SelectedId { get; set; }
	[Parameter] public EventCallback<TId?> SelectedIdChanged { get; set; }
}
```

- [ ] **Step 2: Build — expect success**.

- [ ] **Step 3: Visual verify** at `/chunks/list-detail-shell`. Initially the Empty slot shows ("Pick an item to see details"). Click an item — the Detail slot renders with the selected name. Clicking another item updates the detail.

- [ ] **Step 4: Commit**

```bash
git add PINGWorks.SitecoreBlok.BlazorUI/Components/Chunks/Layouts/ListDetailShell.razor PINGWorks.SitecoreBlok.BlazorUI.Catalogue/Components/Pages/Chunks/Layouts/ListDetailShellPage.razor
git commit -m "chunks: implement Layouts/ListDetailShell with generic selection state"
```

---

## Phase 8 — `Layouts/BlankShell`

### Task 19: Write the Catalogue page for `BlankShell`

**Files:**
- Create: `PINGWorks.SitecoreBlok.BlazorUI.Catalogue/Components/Pages/Chunks/Layouts/BlankShellPage.razor`

- [ ] **Step 1: Create the file with this exact content**

```razor
@page "/chunks/blank-shell"

<ComponentPage Title="BlankShell" Description="No-chrome envelope; sets background and font only" Interactivity="ComponentInteractivity.Ssr">

	<ExamplesSection>

		<ComponentExample Title="Default" Code="@("<BlankShell>\n    <p>Anything goes here</p>\n</BlankShell>")">
			<div class="h-48 border rounded-lg overflow-hidden">
				<BlankShell>
					<div class="p-4">
						<h3 class="text-lg font-semibold">Marketing band</h3>
						<p class="text-sm text-muted-foreground">No header, no nav, no chrome. Just the background and font.</p>
					</div>
				</BlankShell>
			</div>
		</ComponentExample>

	</ExamplesSection>

</ComponentPage>
```

- [ ] **Step 2: Build — expect failure**.

---

### Task 20: Implement `BlankShell.razor`

**Files:**
- Create: `PINGWorks.SitecoreBlok.BlazorUI/Components/Chunks/Layouts/BlankShell.razor`

- [ ] **Step 1: Create the file with this exact content**

```razor
@namespace PINGWorks.SitecoreBlok.BlazorUI

@*
	BlankShell — no-chrome envelope. Sets only the background and inherits the font.
	Useful for print, login pre-shells, full-bleed marketing pages.
*@

<div class="bg-subtle-bg text-foreground min-h-full w-full">
	@ChildContent
</div>

@code {
	[Parameter] public RenderFragment? ChildContent { get; set; }
}
```

- [ ] **Step 2: Build — expect success**.

- [ ] **Step 3: Visual verify** at `/chunks/blank-shell`.

- [ ] **Step 4: Commit**

```bash
git add PINGWorks.SitecoreBlok.BlazorUI/Components/Chunks/Layouts/BlankShell.razor PINGWorks.SitecoreBlok.BlazorUI.Catalogue/Components/Pages/Chunks/Layouts/BlankShellPage.razor
git commit -m "chunks: implement Layouts/BlankShell"
```

---

## Phase 9 — `Layouts/DialogShell`

### Task 21: Write the Catalogue page for `DialogShell`

**Files:**
- Create: `PINGWorks.SitecoreBlok.BlazorUI.Catalogue/Components/Pages/Chunks/Layouts/DialogShellPage.razor`

- [ ] **Step 1: Create the file with this exact content**

```razor
@page "/chunks/dialog-shell"
@rendermode InteractiveServer

<ComponentPage Title="DialogShell" Description="Opinionated wrapper around the Dialog primitive — hides the 5-component composition" Interactivity="ComponentInteractivity.Interactive">

	<ExamplesSection>

		<ComponentExample Title="Default" Code="@("<Button Click=\"() => open = true\">Open</Button>\n<DialogShell Open=\"open\" OpenChanged=\"v => open = v\" Title=\"Confirm\" Description=\"Sure?\">\n    <Body>...</Body>\n    <Actions>...</Actions>\n</DialogShell>")">
			<Button Click="() => open = true">Open dialog</Button>
			<DialogShell Open="open" OpenChanged="v => open = v" Title="Confirm action" Description="Are you sure you want to proceed?">
				<Body>
					<p class="text-sm">This will affect 12 items.</p>
				</Body>
				<Actions>
					<Button Variant="ButtonVariant.Ghost" Click="() => open = false">Cancel</Button>
					<Button Click="() => open = false">Confirm</Button>
				</Actions>
			</DialogShell>
		</ComponentExample>

	</ExamplesSection>

</ComponentPage>

@code {
	private bool open = false;
}
```

- [ ] **Step 2: Build — expect failure** (`DialogShell` not found).

---

### Task 22: Implement `DialogShell.razor`

**Files:**
- Create: `PINGWorks.SitecoreBlok.BlazorUI/Components/Chunks/Layouts/DialogShell.razor`

The Dialog primitive's API has been verified directly — its component files are `Dialog.razor`, `DialogHeader.razor`, `DialogTitle.razor`, `DialogDescription.razor`, `DialogFooter.razor`, `DialogClose.razor`. **There is no `DialogContent`** — content (header + body + footer) goes directly as children of `<Dialog>`. The Dialog primitive renders its own close button automatically.

- [ ] **Step 1: Create the file with this exact content**

```razor
@namespace PINGWorks.SitecoreBlok.BlazorUI

@*
	DialogShell — opinionated wrapper around the Dialog primitive.
	Hides the Dialog/DialogHeader/DialogTitle/DialogDescription/DialogFooter composition.

	The primitive Dialog has no `DialogContent` wrapper — body content goes directly
	between header and footer as a sibling of those children.
*@

<Dialog Open="@Open" OpenChanged="@OpenChanged">
	@if ( !string.IsNullOrEmpty( Title ) || !string.IsNullOrEmpty( Description ) )
	{
		<DialogHeader>
			@if ( !string.IsNullOrEmpty( Title ) )
			{
				<DialogTitle>@Title</DialogTitle>
			}
			@if ( !string.IsNullOrEmpty( Description ) )
			{
				<DialogDescription>@Description</DialogDescription>
			}
		</DialogHeader>
	}

	@if ( Body is not null )
	{
		@Body
	}

	@if ( Actions is not null )
	{
		<DialogFooter>
			@Actions
		</DialogFooter>
	}
</Dialog>

@code {
	[Parameter] public string? Title { get; set; }
	[Parameter] public string? Description { get; set; }

	[Parameter] public RenderFragment? Body { get; set; }
	[Parameter] public RenderFragment? Actions { get; set; }

	[Parameter] public bool Open { get; set; }
	[Parameter] public EventCallback<bool> OpenChanged { get; set; }
}
```

- [ ] **Step 2: Build — expect success**.

- [ ] **Step 3: Visual verify** at `/chunks/dialog-shell`. Click "Open dialog" — the dialog appears with title, description, body text, and Cancel/Confirm buttons. Both buttons close the dialog. The primitive's auto-rendered close button (top-right) also closes it.

- [ ] **Step 4: Commit**

```bash
git add PINGWorks.SitecoreBlok.BlazorUI/Components/Chunks/Layouts/DialogShell.razor PINGWorks.SitecoreBlok.BlazorUI.Catalogue/Components/Pages/Chunks/Layouts/DialogShellPage.razor
git commit -m "chunks: implement Layouts/DialogShell wrapping Dialog primitive"
```

---

## Phase 10 — `Layouts/SheetShell`

### Task 23: Write the Catalogue page for `SheetShell`

**Files:**
- Create: `PINGWorks.SitecoreBlok.BlazorUI.Catalogue/Components/Pages/Chunks/Layouts/SheetShellPage.razor`

- [ ] **Step 1: Create the file with this exact content**

```razor
@page "/chunks/sheet-shell"
@rendermode InteractiveServer

<ComponentPage Title="SheetShell" Description="Edge-sliding panel; wraps the Sheet primitive. Side uses the shared Position enum." Interactivity="ComponentInteractivity.Interactive">

	<ExamplesSection>

		<ComponentExample Title="From the right" Code="@("<Button Click=\"() => open = true\">Open</Button>\n<SheetShell Open=\"open\" OpenChanged=\"v => open = v\" Side=\"Position.Right\">\n    <Header>...</Header>\n    <Body>...</Body>\n    <Actions>...</Actions>\n</SheetShell>")">
			<Button Click="() => open = true">Open from right</Button>
			<SheetShell Open="open" OpenChanged="v => open = v" Side="Position.Right">
				<Header><h3 class="text-lg font-semibold">Filters</h3></Header>
				<Body><p class="text-sm">Sheet body content.</p></Body>
				<Actions>
					<Button Variant="ButtonVariant.Ghost" Click="() => open = false">Cancel</Button>
					<Button Click="() => open = false">Apply</Button>
				</Actions>
			</SheetShell>
		</ComponentExample>

	</ExamplesSection>

</ComponentPage>

@code {
	private bool open = false;
}
```

- [ ] **Step 2: Build — expect failure**.

---

### Task 24: Implement `SheetShell.razor`

**Files:**
- Create: `PINGWorks.SitecoreBlok.BlazorUI/Components/Chunks/Layouts/SheetShell.razor`

The Sheet primitive's API has been verified directly — its component files are `Sheet.razor`, `SheetHeader.razor`, `SheetTitle.razor`, `SheetDescription.razor`, `SheetFooter.razor`, `SheetClose.razor`. **There is no `SheetContent`** — body content goes directly between header and footer as a sibling. `Sheet.Side` accepts `SheetSide`.

- [ ] **Step 1: Create the file with this exact content**

```razor
@namespace PINGWorks.SitecoreBlok.BlazorUI

@*
	SheetShell — edge-sliding panel wrapper. Translates the shared Position enum
	to the primitive's SheetSide via PositionClasses.ToSheetSide.

	The primitive Sheet has no `SheetContent` wrapper — body content goes directly
	between header and footer as a sibling.
*@

<Sheet Open="@Open" OpenChanged="@OpenChanged" Side="@PositionClasses.ToSheetSide( Side )">
	@if ( Header is not null )
	{
		<SheetHeader>
			@Header
		</SheetHeader>
	}

	@if ( Body is not null )
	{
		@Body
	}

	@if ( Actions is not null )
	{
		<SheetFooter>
			@Actions
		</SheetFooter>
	}
</Sheet>

@code {
	[Parameter] public RenderFragment? Header { get; set; }
	[Parameter] public RenderFragment? Body { get; set; }
	[Parameter] public RenderFragment? Actions { get; set; }

	[Parameter] public Position Side { get; set; } = Position.Right;
	[Parameter] public bool Open { get; set; }
	[Parameter] public EventCallback<bool> OpenChanged { get; set; }
}
```

- [ ] **Step 2: Build — expect success**.

- [ ] **Step 3: Visual verify** at `/chunks/sheet-shell`. Click "Open from right" — the sheet slides in from the right edge with header / body / actions. Cancel and Apply close it (and so does the primitive's auto-rendered close button).

- [ ] **Step 4: Commit**

```bash
git add PINGWorks.SitecoreBlok.BlazorUI/Components/Chunks/Layouts/SheetShell.razor PINGWorks.SitecoreBlok.BlazorUI.Catalogue/Components/Pages/Chunks/Layouts/SheetShellPage.razor
git commit -m "chunks: implement Layouts/SheetShell wrapping Sheet primitive with Position translation"
```

---

## Phase 11 — Coverage check and final smoke test

### Task 25: Verify the existing Catalogue could be expressed using Chunks

Per spec §10.6, this is the dogfood check. We do **not** refactor the catalogue files — we just verify on paper that the chunks are sufficient.

- [ ] **Step 1: Add a comment block to MIGRATION_STATUS.md (no row added)**

Open: `MIGRATION_STATUS.md`
At the very bottom of the file, append:

```markdown

---

## Chunks coverage notes (v1 — Foundations + Layouts)

Chunks are tracked as "Extras" — no rows in the table above (per Chunks design spec §5.9).
Coverage check (spec §10.6) confirms that with Plan 1 complete, the Catalogue's MainLayout, NavMenu, and ComponentPage can be expressed using:

- `MainLayout.razor` → `AppShell` + (Header content via slot — `AppHeader` arrives in Plan 2/Headers).
- `NavMenu.razor` → blocked on `AppSidebar` (Plan 3/Navigation).
- `ComponentPage.razor` → `PageShell` + Header content (PageHeader arrives in Plan 2/Headers) + ContentSection body (arrives in Plan 5/Content).

Plan 1 alone does not unblock the full refactor — that requires Headers (Plan 2), Navigation (Plan 3), and Content (Plan 5). The refactor itself remains out of scope per spec §3.
```

- [ ] **Step 2: Commit**

```bash
git add MIGRATION_STATUS.md
git commit -m "chunks: document Plan 1 coverage status in MIGRATION_STATUS"
```

---

### Task 26: Final full-build and full-Catalogue smoke test

- [ ] **Step 1: Clean build**

Run: `dotnet build`
Expected: `Build succeeded` exit code 0, ideally with zero new warnings beyond pre-existing.

- [ ] **Step 2: Run the catalogue and walk every Chunks page**

Run: `dotnet run --project PINGWorks.SitecoreBlok.BlazorUI.Catalogue/PINGWorks.SitecoreBlok.BlazorUI.Catalogue.csproj`

Visit each in turn and confirm rendering + interactivity:
- `/chunks` — Index lists 8 Layouts chunks
- `/chunks/app-shell` — slots render
- `/chunks/page-shell` — three aside placements
- `/chunks/centered-shell` — three widths
- `/chunks/split-shell` — horizontal + vertical
- `/chunks/list-detail-shell` — selection toggles between Empty and Detail
- `/chunks/blank-shell` — content renders
- `/chunks/dialog-shell` — opens / closes
- `/chunks/sheet-shell` — slides in from right

Also visit `/primitives` to confirm primitives still work and the route-aware nav switches back to `NavMenu`.

Stop with Ctrl+C.

- [ ] **Step 3: Run the existing UI parity tool to confirm no Primitive regressions**

Run: `pwsh ./tools/verify-ui-parity.ps1`
Expected: passes (or at least no new failures vs. baseline).

- [ ] **Step 4: Tag the milestone (no commit needed if everything's already committed)**

Run: `git tag chunks-plan-1-complete`
Note: do not push the tag without explicit user approval per the project's no-auto-push convention.

---

## Acceptance criteria for this plan

- [ ] All 8 Layouts chunks exist under `Components/Chunks/Layouts/` and build cleanly.
- [ ] All 8 Layouts chunks have a Catalogue page under `Components/Pages/Chunks/Layouts/` with `Interactivity` declared explicitly.
- [ ] The 3 helper classes (`PositionClasses`, `OrientationClasses`, `PlacementClasses`) exist under `Components/Chunks/Shared/` and are consumed by at least one chunk each.
- [ ] The 3 shared enums (`Position`, `Orientation`, `Placement`) exist in `Enums.cs`.
- [ ] `/chunks` Index page lists 8 Layouts chunks grouped by family.
- [ ] `MainLayout.razor` has a `Chunks` top-nav link and switches between `NavMenu` and `ChunksNavMenu` based on route.
- [ ] `MIGRATION_STATUS.md` carries the Chunks coverage note (no rows added).
- [ ] Every Primitive page still renders (no regression).
- [ ] No Chunk hard-codes `@rendermode InteractiveServer` (per spec §5.6).
- [ ] All Chunk class strings are literals assembled via `CssClassBuilder` (per spec §5.3, §10.3).
