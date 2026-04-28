# Chunks — Headers Family Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development to implement this plan task-by-task.

**Goal:** Build the `Headers/` family (7 chunks: `AppHeader`, `AppBrand`, `PageHeader`, `SectionHeader`, `SubHeader`, `Toolbar`, `AnnouncementBar`) plus the foundations they need (`Tone` and `Density` shared enums + their helper classes).

**Architecture:** Same as Plan 1. Each chunk is a `.razor` file under `Components/Chunks/Headers/`. New shared enums go to `Components/Chunks/Enums.cs` (Plan 1's location). Helper classes for the new enums live under `Components/Chunks/Shared/` per spec §5.12. Catalogue pages mirror the Plan 1 template (full Code, `ApiElements`).

**Spec reference:** `docs/superpowers/specs/2026-04-28-chunks-design.md`. Headers roster is §7.2; conventions are §5.1–§5.12.

**Status:** Plan 2 of 7. Plan 1 (Foundations + Layouts) is complete and committed, plus an in-flight wrapper-styling refactor (see `~/.claude/plans/robust-scribbling-sonnet.md`) that established the §5.13/§5.14/§5.15 conventions below. This plan adds 7 chunks; subsequent plans cover Navigation, Content, Forms, Data, Marketplace.

**Workflow conventions (carry-over from Plan 1 + new patterns from the wrapper-styling refactor):**

*Workflow:*
- Subagents do NOT commit and do NOT stage. Leave changes unstaged for the user to review and commit at their cadence.

*C# style (per `feedback_csharp_style.md` memory):*
- Tabs for indentation; PascalCase for all class members; expression bodies with `=>` on a new line; spaces inside parens; method/constructor argument lists on one line unless >8 args.

*Catalogue pages (always):*
- Declare `@rendermode InteractiveServer` at the top regardless of the chunk's `Interactivity` flag — the `Tabs` primitive inside `<ComponentExample>` (Preview/Code/Primitives switcher) requires interactive mode.
- Declare `Interactivity="ComponentInteractivity.Ssr|Interactive"` on `<ComponentPage>` — this documents the *consumer's* requirement when using the chunk in their own apps.
- `Code="..."` strings contain the full markup (no `...` abbreviations).
- Every `<ComponentExample>` must author a `Primitives="..."` value showing the equivalent first-principles markup (raw HTML + Blok primitives) — per spec §5.14. The Primitives tab stays as raw HTML even when the Code tab uses `<Text>` etc.
- Define `private static readonly ApiElement[] {Chunk}Elements = [ … ]` and pass it via `ApiElements="@…Elements"` to `<ComponentPage>`.
- Slot content uses `<Text>` instead of raw `<p>`/`<span>`/`<h*>`/`<div class="p-* …">` typography. Buttons stay as `<button>`.

*Wrapper-styling parameters (spec §5.13 — apply where structurally meaningful):*
- `bool Borders` (default `true`) — toggles `border-*` Tailwind on the chunk's region wrappers.
- `bool Gutters` (default `true`) — toggles internal padding (`p-4` baseline; the chunk decides what padding/gap to apply when on).
- `Alignment HeaderAlignment` (default `Alignment.Center`) — flex `items-*` on header content rows.
- Per-region width: `<Region>Width: Size` (mapped via `SizeClasses.Width`).
- Per-region fill: bare `BgFilled` for single-region chunks, `<Region>BgFilled` for multi-region (e.g. `HeaderBgFilled`).
- **`h-14` and other vertical-sizing classes are conditional on `Gutters`** — don't bake fixed heights into the baseline class string.
- **Never combine `flex-row-reverse` with CSS `order` properties** (they cancel out). Use `flex-row` + `PlacementClasses.AsideOrder(Placement)` instead.

*Helpers available (already implemented in the wrapper-styling refactor):*
- `AlignmentClasses.Items(Alignment) → "items-*"`.
- `SizeClasses.Width(Size) → "w-*"` and `SizeClasses.MaxWidth(Size) → "max-w-*"`.
- `OrientationClasses.Flex/Divide`, `PlacementClasses.AsideOrder/ShowAside`, `PositionClasses.ToSheetSide`.
- New helpers needed for this plan: `ToneClasses` (Tone → Tailwind colour classes), `DensityClasses` (Density → padding/gap/height classes).

*`Text` component (in `Components/Extra/Text/`) is now powerful:*
- `Kind` (TextKind: P/Span/Div/H1–H6) — the rendered HTML element.
- `Size` (Size enum) — maps to `text-xs`..`text-8xl` via inlined switch.
- `Alignment` (Alignment enum) — `text-start/center/end/justify`.
- Bool flags: `Bold`, `SemiBold`, `Italic`, `Muted`, `Border`, `Rounded`, `FullWidth`, `FullHeight`, `BgFilled`.
- `Padding` and `Margin` are nullable ints (Tailwind p-{0..12} / m-{0..8}).
- `ClassName` for escape-hatch passthrough.

*`InteractiveRenderMode` parameter pattern:*
- Chunks that mount interactive primitives (e.g. AnnouncementBar with a dismiss button) should expose `IComponentRenderMode? InteractiveRenderMode` and forward to those primitives. Default null (inherits from consumer's surrounding context).
- Idiomatic consumer value: `RenderMode.InteractiveServer` (static field) — NOT `new InteractiveServerRenderMode()`.

---

## File Structure

**Library:**
- Modify: `PINGWorks.SitecoreBlok.BlazorUI/Components/Chunks/Enums.cs` — append `Tone` and `Density` enums.
- Create: `PINGWorks.SitecoreBlok.BlazorUI/Components/Chunks/Shared/ToneClasses.cs` — `Tone` → Tailwind class lookups.
- Create: `PINGWorks.SitecoreBlok.BlazorUI/Components/Chunks/Shared/DensityClasses.cs` — `Density` → padding/gap class lookups.
- Create: 7 chunks under `PINGWorks.SitecoreBlok.BlazorUI/Components/Chunks/Headers/`:
  - `AppHeader.razor`
  - `AppBrand.razor`
  - `PageHeader.razor`
  - `SectionHeader.razor`
  - `SubHeader.razor`
  - `Toolbar.razor`
  - `AnnouncementBar.razor`

**Catalogue:**
- Modify: `PINGWorks.SitecoreBlok.BlazorUI.Catalogue/Services/ChunksManifest.cs` — append 7 Headers entries.
- Create: 7 catalogue pages under `PINGWorks.SitecoreBlok.BlazorUI.Catalogue/Components/Pages/Chunks/Headers/`:
  - `AppHeaderPage.razor`, `AppBrandPage.razor`, `PageHeaderPage.razor`, `SectionHeaderPage.razor`, `SubHeaderPage.razor`, `ToolbarPage.razor`, `AnnouncementBarPage.razor`

---

## Phase 1 — Foundations

### Task 1: Add `Tone` and `Density` enums to `Components/Chunks/Enums.cs`

Append to the end of the file (preserving the existing 3 enums):

```csharp
public enum Tone { Info, Success, Warning, Danger, Neutral }
public enum Density { Comfortable, Compact }
```

Build: `dotnet build PINGWorks.SitecoreBlok.BlazorUI/PINGWorks.SitecoreBlok.BlazorUI.csproj` — expect Build succeeded, 0 warnings, 0 errors.

### Task 2: Create `ToneClasses.cs`

`PINGWorks.SitecoreBlok.BlazorUI/Components/Chunks/Shared/ToneClasses.cs`:

```csharp
namespace PINGWorks.SitecoreBlok.BlazorUI;

/// <summary>
/// Tailwind class lookups for the shared <see cref="Tone"/> enum.
/// Consumed by Chunks that surface a tone-coloured affordance (Callout, AnnouncementBar, ConfirmDialog).
/// </summary>
internal static class ToneClasses
{
	/// <summary>Foreground text colour for the given tone.</summary>
	public static string Text( Tone tone )
		=> tone switch
		{
			Tone.Info     => "text-info-fg",
			Tone.Success  => "text-success-fg",
			Tone.Warning  => "text-warning-fg",
			Tone.Danger   => "text-danger-fg",
			Tone.Neutral  => "text-foreground",
			_             => "text-foreground",
		};

	/// <summary>Background fill for the given tone (subtle Blok background tokens).</summary>
	public static string Bg( Tone tone )
		=> tone switch
		{
			Tone.Info     => "bg-info-bg",
			Tone.Success  => "bg-success-bg",
			Tone.Warning  => "bg-warning-bg",
			Tone.Danger   => "bg-danger-bg",
			Tone.Neutral  => "bg-neutral-bg",
			_             => "bg-neutral-bg",
		};

	/// <summary>Border colour for the given tone (matches the foreground hue).</summary>
	public static string Border( Tone tone )
		=> tone switch
		{
			Tone.Info     => "border-info-fg",
			Tone.Success  => "border-success-fg",
			Tone.Warning  => "border-warning-fg",
			Tone.Danger   => "border-danger-fg",
			Tone.Neutral  => "border-border",
			_             => "border-border",
		};
}
```

### Task 3: Create `DensityClasses.cs`

`PINGWorks.SitecoreBlok.BlazorUI/Components/Chunks/Shared/DensityClasses.cs`:

```csharp
namespace PINGWorks.SitecoreBlok.BlazorUI;

/// <summary>
/// Tailwind class lookups for the shared <see cref="Density"/> enum.
/// Consumed by Chunks whose vertical rhythm shrinks in compact mode (Toolbar, DataToolbar).
/// </summary>
internal static class DensityClasses
{
	/// <summary>Vertical+horizontal padding for a horizontal strip at the given density.</summary>
	public static string Padding( Density density )
		=> density switch
		{
			Density.Comfortable => "px-4 py-2",
			Density.Compact     => "px-3 py-1",
			_                   => "px-4 py-2",
		};

	/// <summary>Inter-item gap for a flex strip at the given density.</summary>
	public static string Gap( Density density )
		=> density switch
		{
			Density.Comfortable => "gap-3",
			Density.Compact     => "gap-2",
			_                   => "gap-3",
		};

	/// <summary>Min-height for a strip at the given density (so empty strips don't collapse).</summary>
	public static string Height( Density density )
		=> density switch
		{
			Density.Comfortable => "min-h-12",
			Density.Compact     => "min-h-9",
			_                   => "min-h-12",
		};
}
```

Build full solution: `dotnet build` — expect 0 warnings, 0 errors.

---

## Phase 2 — Catalogue manifest update

### Task 4: Append 7 Headers entries to `ChunksManifest.cs`

`PINGWorks.SitecoreBlok.BlazorUI.Catalogue/Services/ChunksManifest.cs` — within the `All` array, after the last Layouts entry, add:

```csharp
		// Headers
		new( "Headers", "AppHeader",       "app-header",       "Sticky top bar with backdrop blur — slots: Brand, Nav, Actions",        ComponentInteractivity.Ssr ),
		new( "Headers", "AppBrand",        "app-brand",        "Logo + product name + optional version chip",                           ComponentInteractivity.Ssr ),
		new( "Headers", "PageHeader",      "page-header",      "Top of an in-page area: Title, Description, Breadcrumbs, Actions",      ComponentInteractivity.Ssr ),
		new( "Headers", "SectionHeader",   "section-header",   "Smaller heading band inside a ContentSection",                          ComponentInteractivity.Ssr ),
		new( "Headers", "SubHeader",       "sub-header",       "Context strip below AppHeader (env switcher, ambient breadcrumbs)",     ComponentInteractivity.Ssr ),
		new( "Headers", "Toolbar",         "toolbar",          "Horizontal action strip — slots: Start, Center, End. Density enum",     ComponentInteractivity.Ssr ),
		new( "Headers", "AnnouncementBar", "announcement-bar", "Top-of-app dismissible banner — Message + Tone + optional Action",      ComponentInteractivity.Interactive ),
```

(`AnnouncementBar` is `Interactive` because the dismiss button toggles state. The other six are pure-markup SSR.)

Build catalogue: `dotnet build PINGWorks.SitecoreBlok.BlazorUI.Catalogue/PINGWorks.SitecoreBlok.BlazorUI.Catalogue.csproj`.

---

## Phase 3 — Headers chunks (one task per chunk + catalogue page)

Each task implements ONE chunk + its catalogue page together (same TDD pairing as Plan 1). Build after each. Per Phase-1-of-Plan-1 pattern, the implementer's responsibility is to implement the chunk per its API description and produce the catalogue page following the established template (see `Catalogue/Components/Pages/Chunks/Layouts/SplitShellPage.razor` for the canonical example).

### Task 5: `AppHeader` + `AppHeaderPage`

**AppHeader.razor** at `Components/Chunks/Headers/AppHeader.razor`:

```razor
@namespace PINGWorks.SitecoreBlok.BlazorUI

@*
	AppHeader — sticky top bar with backdrop blur. Replaces the Catalogue's hand-built
	header. Slots: Brand (left), Nav (center-left), Actions (right). All RenderFragment?
	per spec §5.1.
*@

<header class="@HeaderClass">
	<div class="flex items-center gap-6 px-6 h-14">
		@if ( Brand is not null )
		{
			<div class="flex items-center">
				@Brand
			</div>
		}

		@if ( Nav is not null )
		{
			<nav class="flex items-center gap-4">
				@Nav
			</nav>
		}

		@if ( Actions is not null )
		{
			<div class="ml-auto flex items-center gap-1">
				@Actions
			</div>
		}
	</div>
</header>

@code {
	[Parameter] public RenderFragment? Brand { get; set; }
	[Parameter] public RenderFragment? Nav { get; set; }
	[Parameter] public RenderFragment? Actions { get; set; }

	[Parameter] public bool Sticky { get; set; } = true;
	[Parameter] public bool Borders { get; set; } = true;

	private string HeaderClass
		=> CssClassBuilder.Start( "z-40 bg-background/95 backdrop-blur" )
			.With( "sticky top-0", Sticky )
			.With( "border-b border-border", Borders )
			.Build();
}
```

**AppHeaderPage.razor** at `Catalogue/Components/Pages/Chunks/Headers/AppHeaderPage.razor`:

```razor
@page "/chunks/app-header"
@rendermode InteractiveServer
@using PINGWorks.SitecoreBlok.BlazorUI.Catalogue.Services

<ComponentPage Title="AppHeader" Description="Sticky top bar with backdrop blur — Brand, Nav, Actions slots" Interactivity="ComponentInteractivity.Ssr" ApiElements="@AppHeaderElements">

	<ExamplesSection>

		<ComponentExample Title="Default" Code="@("<AppHeader>\n    <Brand>\n        <span class=\"font-semibold\">Brand</span>\n    </Brand>\n    <Nav>\n        <a href=\"#\" class=\"text-sm text-muted-foreground hover:text-foreground\">Home</a>\n        <a href=\"#\" class=\"text-sm text-muted-foreground hover:text-foreground\">Docs</a>\n    </Nav>\n    <Actions>\n        <Button Variant=\"ButtonVariant.Ghost\" Size=\"ButtonSize.Sm\">Sign in</Button>\n    </Actions>\n</AppHeader>")">
			<div class="border rounded-lg overflow-hidden">
				<AppHeader>
					<Brand><span class="font-semibold">Brand</span></Brand>
					<Nav>
						<a href="#" class="text-sm text-muted-foreground hover:text-foreground">Home</a>
						<a href="#" class="text-sm text-muted-foreground hover:text-foreground">Docs</a>
					</Nav>
					<Actions>
						<Button Variant="ButtonVariant.Ghost" Size="ButtonSize.Sm">Sign in</Button>
					</Actions>
				</AppHeader>
			</div>
		</ComponentExample>

	</ExamplesSection>

</ComponentPage>

@code {
	private static readonly ApiElement[] AppHeaderElements =
	[
		new ApiElement(
			Name: "AppHeader",
			Description: "Sticky top bar with backdrop-blurred background. Designed to sit at the top of an app shell. Three slots — Brand on the left, Nav in the centre-left, Actions on the right (auto-aligned via `ml-auto`).",
			Depth: 0,
			Properties:
			[
				new ApiProperty( "Brand",    "RenderFragment?",  false, "Left-most slot. Typically holds an `AppBrand` chunk or a logo + product-name link." ),
				new ApiProperty( "Nav",      "RenderFragment?",  false, "Top-level navigation slot. Typically holds a row of `<a>`/`<NavLink>` elements." ),
				new ApiProperty( "Actions",  "RenderFragment?",  false, "Right-aligned slot for sign-in / theme-toggle / settings buttons." ),
				new ApiProperty( "Sticky",   "bool",             false, "When `true` (default), the header sticks to the top of the scrolling viewport. Set to `false` for headers that scroll with content." ),
				new ApiProperty( "Borders", "bool",             false, "When `true` (default), renders a 1px bottom border. Set to `false` for borderless headers." ),
			]
		),
	];
}
```

Build full solution. Expect Build succeeded, 0 warnings, 0 errors.

### Task 6: `AppBrand` + `AppBrandPage`

**AppBrand.razor**:

```razor
@namespace PINGWorks.SitecoreBlok.BlazorUI

@*
	AppBrand — logo + product name + optional version chip. Reusable inside or outside AppHeader.
*@

<a href="@Href" class="flex items-center gap-2 no-underline text-foreground hover:text-primary transition-colors">
	@if ( Logo is not null )
	{
		<span class="flex items-center">
			@Logo
		</span>
	}
	<span class="text-base font-bold tracking-tight">@Name</span>
	@if ( !string.IsNullOrEmpty( Version ) )
	{
		<Badge ColorScheme="BadgeColor.Neutral" Size="BadgeSize.Sm">@Version</Badge>
	}
</a>

@code {
	[Parameter] public string Name { get; set; } = "";
	[Parameter] public string? Href { get; set; } = "/";
	[Parameter] public string? Version { get; set; }
	[Parameter] public RenderFragment? Logo { get; set; }
}
```

**AppBrandPage.razor**:

```razor
@page "/chunks/app-brand"
@rendermode InteractiveServer
@using PINGWorks.SitecoreBlok.BlazorUI.Catalogue.Services

<ComponentPage Title="AppBrand" Description="Logo + product name + optional version chip — reusable inside or outside AppHeader" Interactivity="ComponentInteractivity.Ssr" ApiElements="@AppBrandElements">

	<ExamplesSection>

		<ComponentExample Title="Name only" Code="@("<AppBrand Name=\"Blok Blazor\" />")">
			<AppBrand Name="Blok Blazor" />
		</ComponentExample>

		<ComponentExample Title="With version chip" Code="@("<AppBrand Name=\"Blok Blazor\" Version=\"v1.2\" />")">
			<AppBrand Name="Blok Blazor" Version="v1.2" />
		</ComponentExample>

		<ComponentExample Title="With logo slot" Code="@("<AppBrand Name=\"Blok Blazor\">\n    <Logo>\n        <Icon Svg=\"@IconSvg.Github\" ClassName=\"size-5\" />\n    </Logo>\n</AppBrand>")">
			<AppBrand Name="Blok Blazor">
				<Logo>
					<Icon Svg="@IconSvg.Github" ClassName="size-5" />
				</Logo>
			</AppBrand>
		</ComponentExample>

	</ExamplesSection>

</ComponentPage>

@code {
	private static readonly ApiElement[] AppBrandElements =
	[
		new ApiElement(
			Name: "AppBrand",
			Description: "Compact branding row: logo (slot) + product name (text) + optional version badge. Wraps the whole row in an `<a>` so the brand is clickable; `Href` defaults to `/`.",
			Depth: 0,
			Properties:
			[
				new ApiProperty( "Name",    "string",           true,  "Product name. Rendered as bold text." ),
				new ApiProperty( "Href",    "string?",          false, "Anchor href. Default: `/`. Set to `null` to render as a non-link span (advanced — not exposed via this API yet; pass `\"#\"` if you need a click-no-op)." ),
				new ApiProperty( "Version", "string?",          false, "Optional version label rendered in a neutral badge to the right of the name." ),
				new ApiProperty( "Logo",    "RenderFragment?",  false, "Logo slot. Typically holds an `<Icon>` or `<svg>`. Renders to the left of the name." ),
			]
		),
	];
}
```

### Task 7: `PageHeader` + `PageHeaderPage`

**PageHeader.razor**:

```razor
@namespace PINGWorks.SitecoreBlok.BlazorUI

@*
	PageHeader — top of an in-page area. Title and Description are text props;
	Breadcrumbs / Actions / Status are RenderFragment slots.
*@

<div class="space-y-3 pb-6 border-b border-border">
	@if ( Breadcrumbs is not null )
	{
		<div>
			@Breadcrumbs
		</div>
	}

	<div class="flex items-start gap-4">
		<div class="flex-1 min-w-0 space-y-1">
			<div class="flex items-center gap-3">
				<h1 class="text-2xl font-bold tracking-tight text-foreground">@Title</h1>
				@if ( Status is not null )
				{
					<div>@Status</div>
				}
			</div>
			@if ( !string.IsNullOrEmpty( Description ) )
			{
				<p class="text-sm text-muted-foreground">@Description</p>
			}
		</div>

		@if ( Actions is not null )
		{
			<div class="flex-shrink-0 flex items-center gap-2">
				@Actions
			</div>
		}
	</div>
</div>

@code {
	[Parameter] public string Title { get; set; } = "";
	[Parameter] public string? Description { get; set; }
	[Parameter] public RenderFragment? Breadcrumbs { get; set; }
	[Parameter] public RenderFragment? Actions { get; set; }
	[Parameter] public RenderFragment? Status { get; set; }
}
```

**PageHeaderPage.razor**:

```razor
@page "/chunks/page-header"
@rendermode InteractiveServer
@using PINGWorks.SitecoreBlok.BlazorUI.Catalogue.Services

<ComponentPage Title="PageHeader" Description="Top of an in-page area: Title, Description, plus Breadcrumbs, Actions, Status slots" Interactivity="ComponentInteractivity.Ssr" ApiElements="@PageHeaderElements">

	<ExamplesSection>

		<ComponentExample Title="Title + description only" Code="@("<PageHeader Title=\"Settings\" Description=\"Manage your project configuration.\" />")">
			<PageHeader Title="Settings" Description="Manage your project configuration." />
		</ComponentExample>

		<ComponentExample Title="With actions" Code="@("<PageHeader Title=\"Sites\" Description=\"All sites in this project.\">\n    <Actions>\n        <Button Variant=\"ButtonVariant.Outline\" Size=\"ButtonSize.Sm\">Filter</Button>\n        <Button Size=\"ButtonSize.Sm\">New site</Button>\n    </Actions>\n</PageHeader>")">
			<PageHeader Title="Sites" Description="All sites in this project.">
				<Actions>
					<Button Variant="ButtonVariant.Outline" Size="ButtonSize.Sm">Filter</Button>
					<Button Size="ButtonSize.Sm">New site</Button>
				</Actions>
			</PageHeader>
		</ComponentExample>

		<ComponentExample Title="With status badge" Code="@("<PageHeader Title=\"Production\" Description=\"Live environment.\">\n    <Status>\n        <Badge ColorScheme=\"BadgeColor.Success\">Healthy</Badge>\n    </Status>\n</PageHeader>")">
			<PageHeader Title="Production" Description="Live environment.">
				<Status>
					<Badge ColorScheme="BadgeColor.Success">Healthy</Badge>
				</Status>
			</PageHeader>
		</ComponentExample>

	</ExamplesSection>

</ComponentPage>

@code {
	private static readonly ApiElement[] PageHeaderElements =
	[
		new ApiElement(
			Name: "PageHeader",
			Description: "Top of an in-page area: page title, description, plus optional breadcrumbs above and actions/status to the right of the title. Renders with a 1px bottom border so the header visually divides from the page body.",
			Depth: 0,
			Properties:
			[
				new ApiProperty( "Title",       "string",           true,  "Page title. Rendered as `<h1>` in `text-2xl font-bold`." ),
				new ApiProperty( "Description", "string?",          false, "Sub-title text. Rendered in `text-muted-foreground` below the title." ),
				new ApiProperty( "Breadcrumbs", "RenderFragment?",  false, "Slot rendered above the title. Typically a `BreadcrumbBar` chunk." ),
				new ApiProperty( "Actions",     "RenderFragment?",  false, "Right-aligned action slot. Typically one or two `Button`s." ),
				new ApiProperty( "Status",      "RenderFragment?",  false, "Inline slot rendered next to the title. Typically a `Badge` showing environment / health / state." ),
			]
		),
	];
}
```

### Task 8: `SectionHeader` + `SectionHeaderPage`

**SectionHeader.razor**:

```razor
@namespace PINGWorks.SitecoreBlok.BlazorUI

@*
	SectionHeader — smaller heading band inside a ContentSection. h2 instead of h1, less padding.
*@

<div class="flex items-start gap-4 pb-3 border-b border-border">
	<div class="flex-1 min-w-0">
		<h2 class="text-lg font-semibold tracking-tight text-foreground">@Title</h2>
		@if ( !string.IsNullOrEmpty( Description ) )
		{
			<p class="text-sm text-muted-foreground mt-0.5">@Description</p>
		}
	</div>

	@if ( Actions is not null )
	{
		<div class="flex-shrink-0 flex items-center gap-2">
			@Actions
		</div>
	}
</div>

@code {
	[Parameter] public string Title { get; set; } = "";
	[Parameter] public string? Description { get; set; }
	[Parameter] public RenderFragment? Actions { get; set; }
}
```

**SectionHeaderPage.razor**:

```razor
@page "/chunks/section-header"
@rendermode InteractiveServer
@using PINGWorks.SitecoreBlok.BlazorUI.Catalogue.Services

<ComponentPage Title="SectionHeader" Description="Smaller heading band inside a section — Title, Description, Actions slot" Interactivity="ComponentInteractivity.Ssr" ApiElements="@SectionHeaderElements">

	<ExamplesSection>

		<ComponentExample Title="Title + description" Code="@("<SectionHeader Title=\"Members\" Description=\"People with access to this project.\" />")">
			<SectionHeader Title="Members" Description="People with access to this project." />
		</ComponentExample>

		<ComponentExample Title="With action" Code="@("<SectionHeader Title=\"API keys\" Description=\"Active credentials for this environment.\">\n    <Actions>\n        <Button Variant=\"ButtonVariant.Outline\" Size=\"ButtonSize.Sm\">New key</Button>\n    </Actions>\n</SectionHeader>")">
			<SectionHeader Title="API keys" Description="Active credentials for this environment.">
				<Actions>
					<Button Variant="ButtonVariant.Outline" Size="ButtonSize.Sm">New key</Button>
				</Actions>
			</SectionHeader>
		</ComponentExample>

	</ExamplesSection>

</ComponentPage>

@code {
	private static readonly ApiElement[] SectionHeaderElements =
	[
		new ApiElement(
			Name: "SectionHeader",
			Description: "Smaller heading band inside a section. Use as the top of a `ContentSection`. Rendered as `<h2>` (smaller than PageHeader's `<h1>`) with optional description and right-aligned actions.",
			Depth: 0,
			Properties:
			[
				new ApiProperty( "Title",       "string",           true,  "Section title. Rendered as `<h2>` in `text-lg font-semibold`." ),
				new ApiProperty( "Description", "string?",          false, "Sub-title text in muted foreground." ),
				new ApiProperty( "Actions",     "RenderFragment?",  false, "Right-aligned action slot." ),
			]
		),
	];
}
```

### Task 9: `SubHeader` + `SubHeaderPage`

**SubHeader.razor**:

```razor
@namespace PINGWorks.SitecoreBlok.BlazorUI

@*
	SubHeader — context strip below AppHeader (env switcher, ambient breadcrumbs, scope chip).
*@

<div class="@SubHeaderClass">
	<div class="px-6 py-2 flex items-center gap-3">
		@ChildContent
	</div>
</div>

@code {
	[Parameter] public RenderFragment? ChildContent { get; set; }
	[Parameter] public bool Borders { get; set; } = true;

	private string SubHeaderClass
		=> CssClassBuilder.Start( "bg-subtle-bg" )
			.With( "border-b border-border", Borders )
			.Build();
}
```

**SubHeaderPage.razor**:

```razor
@page "/chunks/sub-header"
@rendermode InteractiveServer
@using PINGWorks.SitecoreBlok.BlazorUI.Catalogue.Services

<ComponentPage Title="SubHeader" Description="Context strip below AppHeader — env switcher, ambient breadcrumbs, scope chip" Interactivity="ComponentInteractivity.Ssr" ApiElements="@SubHeaderElements">

	<ExamplesSection>

		<ComponentExample Title="Default" Code="@("<SubHeader>\n    <span class=\"text-sm text-muted-foreground\">Environment:</span>\n    <Badge ColorScheme=\"BadgeColor.Blue\">Production</Badge>\n    <span class=\"text-sm text-muted-foreground\">·</span>\n    <span class=\"text-sm\">Sites &gt; Marketing</span>\n</SubHeader>")">
			<div class="border rounded-lg overflow-hidden">
				<SubHeader>
					<span class="text-sm text-muted-foreground">Environment:</span>
					<Badge ColorScheme="BadgeColor.Blue">Production</Badge>
					<span class="text-sm text-muted-foreground">·</span>
					<span class="text-sm">Sites &gt; Marketing</span>
				</SubHeader>
			</div>
		</ComponentExample>

	</ExamplesSection>

</ComponentPage>

@code {
	private static readonly ApiElement[] SubHeaderElements =
	[
		new ApiElement(
			Name: "SubHeader",
			Description: "A subtle horizontal strip rendered below an AppHeader. Use for ambient context that's relevant across pages — current environment, scope (project/team), or breadcrumbs that belong outside the page header.",
			Depth: 0,
			Properties:
			[
				new ApiProperty( "ChildContent", "RenderFragment?",  false, "Strip content. Free-form — typically text + badges + dividers." ),
				new ApiProperty( "Borders",     "bool",             false, "When `true` (default), renders a 1px bottom border." ),
			]
		),
	];
}
```

### Task 10: `Toolbar` + `ToolbarPage`

**Toolbar.razor**:

```razor
@namespace PINGWorks.SitecoreBlok.BlazorUI

@*
	Toolbar — horizontal action strip. Slots: Start, Center, End. Density enum
	(Comfortable | Compact) varies padding/gap/height via DensityClasses.
*@

<div class="@ToolbarClass">
	@if ( Start is not null )
	{
		<div class="flex items-center @InnerGapClass">
			@Start
		</div>
	}

	@if ( Center is not null )
	{
		<div class="flex-1 flex items-center justify-center @InnerGapClass">
			@Center
		</div>
	}
	else
	{
		<div class="flex-1"></div>
	}

	@if ( End is not null )
	{
		<div class="flex items-center @InnerGapClass">
			@End
		</div>
	}
</div>

@code {
	[Parameter] public RenderFragment? Start { get; set; }
	[Parameter] public RenderFragment? Center { get; set; }
	[Parameter] public RenderFragment? End { get; set; }

	[Parameter] public Density Density { get; set; } = Density.Comfortable;
	[Parameter] public bool Borders { get; set; } = false;

	private string ToolbarClass
		=> CssClassBuilder.Start( "flex items-center w-full" )
			.With( DensityClasses.Padding( Density ) )
			.With( DensityClasses.Height( Density ) )
			.With( DensityClasses.Gap( Density ) )
			.With( "border border-border rounded-md bg-background", Borders )
			.Build();

	private string InnerGapClass
		=> DensityClasses.Gap( Density );
}
```

**ToolbarPage.razor**:

```razor
@page "/chunks/toolbar"
@rendermode InteractiveServer
@using PINGWorks.SitecoreBlok.BlazorUI.Catalogue.Services

<ComponentPage Title="Toolbar" Description="Horizontal action strip — Start, Center, End slots. Density (Comfortable / Compact)" Interactivity="ComponentInteractivity.Ssr" ApiElements="@ToolbarElements">

	<ExamplesSection>

		<ComponentExample Title="Comfortable (default)" Code="@("<Toolbar Borders=\"true\">\n    <Start>\n        <Button Variant=\"ButtonVariant.Outline\" Size=\"ButtonSize.Sm\">Filter</Button>\n        <Button Variant=\"ButtonVariant.Outline\" Size=\"ButtonSize.Sm\">Sort</Button>\n    </Start>\n    <End>\n        <Button Size=\"ButtonSize.Sm\">New item</Button>\n    </End>\n</Toolbar>")">
			<Toolbar Borders="true">
				<Start>
					<Button Variant="ButtonVariant.Outline" Size="ButtonSize.Sm">Filter</Button>
					<Button Variant="ButtonVariant.Outline" Size="ButtonSize.Sm">Sort</Button>
				</Start>
				<End>
					<Button Size="ButtonSize.Sm">New item</Button>
				</End>
			</Toolbar>
		</ComponentExample>

		<ComponentExample Title="Compact density" Code="@("<Toolbar Density=\"Density.Compact\" Borders=\"true\">\n    <Start>\n        <Button Variant=\"ButtonVariant.Ghost\" Size=\"ButtonSize.Xs\">Refresh</Button>\n    </Start>\n    <End>\n        <span class=\"text-xs text-muted-foreground\">12 items</span>\n    </End>\n</Toolbar>")">
			<Toolbar Density="Density.Compact" Borders="true">
				<Start>
					<Button Variant="ButtonVariant.Ghost" Size="ButtonSize.Xs">Refresh</Button>
				</Start>
				<End>
					<span class="text-xs text-muted-foreground">12 items</span>
				</End>
			</Toolbar>
		</ComponentExample>

		<ComponentExample Title="With center" Code="@("<Toolbar Borders=\"true\">\n    <Start>\n        <Button Variant=\"ButtonVariant.Ghost\" Size=\"ButtonSize.Sm\">Back</Button>\n    </Start>\n    <Center>\n        <span class=\"text-sm font-semibold\">Page title</span>\n    </Center>\n    <End>\n        <Button Size=\"ButtonSize.Sm\">Save</Button>\n    </End>\n</Toolbar>")">
			<Toolbar Borders="true">
				<Start>
					<Button Variant="ButtonVariant.Ghost" Size="ButtonSize.Sm">Back</Button>
				</Start>
				<Center>
					<span class="text-sm font-semibold">Page title</span>
				</Center>
				<End>
					<Button Size="ButtonSize.Sm">Save</Button>
				</End>
			</Toolbar>
		</ComponentExample>

	</ExamplesSection>

</ComponentPage>

@code {
	private static readonly ApiElement[] ToolbarElements =
	[
		new ApiElement(
			Name: "Toolbar",
			Description: "Horizontal action strip. Three slots — `Start` (left-aligned), `Center` (centred, takes remaining space), `End` (right-aligned). When `Center` is null, an empty flex-1 spacer fills the middle so `End` stays right-aligned. Density controls padding, gap, and min-height via `DensityClasses` (spec §5.12).",
			Depth: 0,
			Properties:
			[
				new ApiProperty( "Start",    "RenderFragment?",  false, "Left-aligned slot. Typically filter/sort buttons or a back button." ),
				new ApiProperty( "Center",   "RenderFragment?",  false, "Centred slot. Optional — when null, a spacer fills the middle. Typically a title or step indicator." ),
				new ApiProperty( "End",      "RenderFragment?",  false, "Right-aligned slot. Typically the primary action button." ),
				new ApiProperty( "Density",  "Density",          false, "Vertical rhythm. Default: `Density.Comfortable`. Other value: `Compact` (smaller padding, gap, height). Shared `Density` enum per spec §5.10." ),
				new ApiProperty( "Borders", "bool",             false, "When `true`, renders a border + rounded background. Default: `false` (use this for in-context toolbars; `true` for free-standing ones)." ),
			]
		),
	];
}
```

### Task 11: `AnnouncementBar` + `AnnouncementBarPage`

**AnnouncementBar.razor**:

```razor
@namespace PINGWorks.SitecoreBlok.BlazorUI

@*
	AnnouncementBar — top-of-app dismissible banner. Tone (Info/Success/Warning/Danger/Neutral)
	uses ToneClasses for the tonal Tailwind classes. Dismiss button hides the bar locally;
	consumers can persist via OnDismiss callback if needed.
*@

@if ( IsVisible )
{
	<div class="@BarClass" role="status">
		<div class="flex-1 flex items-center gap-3 min-w-0">
			<span class="text-sm">@Message</span>
			@if ( Action is not null )
			{
				<div class="flex items-center">
					@Action
				</div>
			}
		</div>

		@if ( Dismissible )
		{
			<button type="button" @onclick="HandleDismiss"
					class="flex-shrink-0 inline-flex items-center justify-center size-7 rounded-full opacity-70 hover:opacity-100 hover:bg-black/5 transition-colors"
					aria-label="Dismiss">
				<Icon Svg="@IconSvg.Close" Scale="0.85" ResetClassName />
			</button>
		}
	</div>
}

@code {
	[Parameter] public string Message { get; set; } = "";
	[Parameter] public RenderFragment? Action { get; set; }

	[Parameter] public Tone Tone { get; set; } = Tone.Info;
	[Parameter] public bool Dismissible { get; set; } = true;
	[Parameter] public EventCallback OnDismiss { get; set; }

	private bool IsVisible = true;

	private string BarClass
		=> CssClassBuilder.Start( "flex items-center gap-3 px-4 py-2 border-b" )
			.With( ToneClasses.Bg( Tone ) )
			.With( ToneClasses.Text( Tone ) )
			.With( ToneClasses.Border( Tone ) )
			.Build();

	private async Task HandleDismiss()
	{
		IsVisible = false;
		if ( OnDismiss.HasDelegate )
			await OnDismiss.InvokeAsync();
	}
}
```

**AnnouncementBarPage.razor**:

```razor
@page "/chunks/announcement-bar"
@rendermode InteractiveServer
@using PINGWorks.SitecoreBlok.BlazorUI.Catalogue.Services

<ComponentPage Title="AnnouncementBar" Description="Top-of-app dismissible banner. Tone enum + optional action slot" Interactivity="ComponentInteractivity.Interactive" ApiElements="@AnnouncementBarElements">

	<ExamplesSection>

		<ComponentExample Title="Info (default)" Code="@("<AnnouncementBar Message=\"New version available — refresh to update.\" Tone=\"Tone.Info\" />")">
			<div class="border rounded-lg overflow-hidden">
				<AnnouncementBar Message="New version available — refresh to update." Tone="Tone.Info" />
			</div>
		</ComponentExample>

		<ComponentExample Title="Warning with action" Code="@("<AnnouncementBar Message=\"Your trial ends in 3 days.\" Tone=\"Tone.Warning\">\n    <Action>\n        <Button Variant=\"ButtonVariant.Outline\" Size=\"ButtonSize.Sm\">Upgrade</Button>\n    </Action>\n</AnnouncementBar>")">
			<div class="border rounded-lg overflow-hidden">
				<AnnouncementBar Message="Your trial ends in 3 days." Tone="Tone.Warning">
					<Action>
						<Button Variant="ButtonVariant.Outline" Size="ButtonSize.Sm">Upgrade</Button>
					</Action>
				</AnnouncementBar>
			</div>
		</ComponentExample>

		<ComponentExample Title="Danger, non-dismissible" Code="@("<AnnouncementBar Message=\"Production database is read-only — maintenance in progress.\" Tone=\"Tone.Danger\" Dismissible=\"false\" />")">
			<div class="border rounded-lg overflow-hidden">
				<AnnouncementBar Message="Production database is read-only — maintenance in progress." Tone="Tone.Danger" Dismissible="false" />
			</div>
		</ComponentExample>

	</ExamplesSection>

</ComponentPage>

@code {
	private static readonly ApiElement[] AnnouncementBarElements =
	[
		new ApiElement(
			Name: "AnnouncementBar",
			Description: "Top-of-app banner with a tone-coloured background. The dismiss button hides the bar locally; consumers can persist via the `OnDismiss` callback. Uses the shared `Tone` enum and the `ToneClasses` helper for tonal Tailwind classes (spec §5.12).",
			Depth: 0,
			Properties:
			[
				new ApiProperty( "Message",     "string",          true,  "Banner text. Rendered in `text-sm` next to the optional action." ),
				new ApiProperty( "Action",      "RenderFragment?", false, "Optional action slot rendered next to the message. Typically one `Button`." ),
				new ApiProperty( "Tone",        "Tone",            false, "Tonal style. Default: `Tone.Info`. Other values: `Success`, `Warning`, `Danger`, `Neutral`. Drives background, foreground, and border colours via `ToneClasses`." ),
				new ApiProperty( "Dismissible", "bool",            false, "When `true` (default), shows a close button on the right. Set to `false` for non-dismissible banners (e.g. critical maintenance notices)." ),
				new ApiProperty( "OnDismiss",   "EventCallback",   false, "Fires when the user clicks dismiss. The bar hides itself locally regardless; this callback is for persistence (e.g. setting a cookie so the banner doesn't reappear)." ),
			]
		),
	];
}
```

---

## Phase 4 — Final smoke

### Task 12: Full clean build + curl smoke check

Run: `dotnet build`. Expect Build succeeded, 0 warnings, 0 errors.

Stop any running dev server, run:

```
dotnet run --project PINGWorks.SitecoreBlok.BlazorUI.Catalogue/PINGWorks.SitecoreBlok.BlazorUI.Catalogue.csproj --launch-profile https
```

Then for each Headers chunk page, curl-verify HTTP 200:

```
for page in app-header app-brand page-header section-header sub-header toolbar announcement-bar; do
  curl -ks -o /dev/null -w "/$page  HTTP=%{http_code}\n" "https://localhost:5117/chunks/$page"
done
```

Expect all 7 to return 200.

Stop the dev server.

---

## Per-chunk parameter additions (apply to each chunk task before implementing)

The drafted chunk code earlier in this plan was written before the wrapper-styling refactor. Apply these deltas to each chunk's `@code` block + razor markup before generating the chunk:

### AppHeader (Task 5)
- Already has `Sticky` and `Borders` (renamed from `Bordered`). Add `bool Gutters = true`, `Alignment HeaderAlignment = Alignment.Center`, `bool BgFilled = true` (replaces hard-coded `bg-background/95 backdrop-blur` — apply only when BgFilled).
- Roll `flex items-center gap-6 px-6 h-14` into computed `HeaderRowClass` with `h-14 px-6 gap-4` conditional on Gutters; `items-*` from HeaderAlignment; baseline `flex`.
- Catalogue example: drop the `Brand`/`Nav`/`Actions` slot Tailwind, use `<Text>` for any inline labels.

### AppBrand (Task 6)
- Skip wrapper-styling params (single inline element, not a region wrapper). Keep `Name`, `Href`, `Version`, `Logo` as-is.
- Catalogue: still add `Primitives="..."` to every example.

### PageHeader (Task 7)
- Add `bool Borders = true` (controls bottom-border under the title row), `bool Gutters = true` (controls vertical padding `pb-6` and the actions-row gap), `Alignment HeaderAlignment = Alignment.Center`.
- Replace inline `flex items-start gap-4` and `pb-6 border-b border-border` with computed classes.

### SectionHeader (Task 8)
- Add `bool Borders = true`, `bool Gutters = true`, `Alignment HeaderAlignment = Alignment.Center`.
- Same conditional pattern as PageHeader, smaller magnitudes (e.g. `pb-3` instead of `pb-6`).

### SubHeader (Task 9)
- Already has `Bordered` (rename to `Borders`). Add `bool Gutters = true` (controls inner `px-6 py-2`), `bool BgFilled = true` (controls `bg-subtle-bg`).

### Toolbar (Task 10)
- Already has `Density` (Comfortable/Compact) which provides padding/gap/height via `DensityClasses`, AND `Borders` (renamed from `Bordered`). Decision: **keep `Density` for granularity, drop the redundant `bool Gutters` from this chunk** — Density is the more expressive control here. Document this in the chunk's @* *@ doc comment.
- Add `bool BgFilled = false` (default false — Toolbar typically doesn't have its own bg unless `Borders=true`).
- `Alignment` doesn't apply (toolbar is a horizontal row).

### AnnouncementBar (Task 11)
- Already has `Tone`, `Dismissible`, `OnDismiss`. Add `bool Borders = true` (controls `border-b`), `bool Gutters = true` (controls `px-4 py-2`).
- Don't add `BgFilled` — the tonal background IS the chunk's defining visual.

### All catalogue pages (Tasks 5–11)
- `@rendermode InteractiveServer` at top.
- Every `<ComponentExample>` has `Code="..."` and `Primitives="..."`.
- Slot content uses `<Text>` not raw HTML where applicable (e.g. labels, subtitles).
- Add a second example per chunk that demonstrates Borders/Gutters off variants where the chunk supports them.

## Acceptance criteria

- [ ] `Tone` and `Density` enums added to `Components/Chunks/Enums.cs`.
- [ ] `ToneClasses.cs` and `DensityClasses.cs` created under `Components/Chunks/Shared/`.
- [ ] All 7 Headers chunks exist under `Components/Chunks/Headers/` and build cleanly. Each chunk has the wrapper-styling params per the per-chunk delta section above (defaults: `Borders=true`, `Gutters=true`, `HeaderAlignment=Center` where applicable).
- [ ] No chunk uses `Bordered`; all use `Borders` consistently.
- [ ] All 7 catalogue pages exist under `Catalogue/Components/Pages/Chunks/Headers/` with explicit `Interactivity` declarations, `@rendermode InteractiveServer`, ApiElements, AND `Primitives="..."` content on every example.
- [ ] Catalogue example slot content uses `<Text>` — no raw `<p class="...">`/`<span class="font-...">` etc. left.
- [ ] `ChunksManifest.cs` lists all 7 Headers entries.
- [ ] `dotnet build` from repo root: 0 warnings, 0 errors.
- [ ] All 7 Headers pages return HTTP 200 on curl. Chrome-verify each one: Preview renders, Code tab shows the new `<Text>`-based markup, Primitives tab shows the equivalent first-principles HTML.
- [ ] No commits made by subagents — all changes left unstaged for the user.

---

## Session-restart notes (next session pick-up)

When resuming in a new session, recap:

1. Read this plan, plus `~/.claude/plans/robust-scribbling-sonnet.md` (progress section at top) for the conventions established in the prior session.
2. Read `docs/superpowers/specs/2026-04-28-chunks-design.md` for spec conventions §5.1–§5.12 (already in spec) and the new conventions §5.13/§5.14/§5.15 (NOT yet in spec — added to spec at start of new session if not done).
3. Inspect the current state of the 8 Layout chunks under `Components/Chunks/Layouts/` — they are the canonical pattern. Mimic `AppShell.razor`, `PageShell.razor`, `SplitShell.razor` shape exactly.
4. Inspect the canonical catalogue page `Catalogue/Components/Pages/Chunks/Layouts/AppShellPage.razor` for the catalogue-page pattern (3 tabs, ApiElements, slot content using `<Text>`).
5. Check `feedback_csharp_style.md` memory before generating any C#.
6. The user reviews/commits each task — leave changes unstaged.
