# PINGWorks.SitecoreBlok.BlazorUI

[![License: Apache 2.0](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](LICENSE)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg)](CONTRIBUTING.md)

**Build better products faster** with idiomatic Blazor components ported from the Sitecore Blok design system.

## Overview

PINGWorks.SitecoreBlok.BlazorUI is a Blazor Razor Class Library that provides production-ready UI components for building martech applications. It is an unofficial port of [Sitecore's Blok design system](https://blok.sitecore.com), translating the React/shadcn component library into idiomatic Blazor patterns.

The library includes:

- **Design Tokens** — Colors, typography, spacing, shadows, and border radius via Tailwind CSS custom properties
- **Primitives** — 60+ primitive components (buttons, cards, dialogs, form fields, tables, and more), one-to-one with the upstream Blok library
- **Chunks** — 85 opinionated compositions of Primitives across 7 families (Layouts, Headers, Navigation, Content, Forms, Data, Marketplace) — page envelopes, shells, headers, nav patterns, KPI tiles, full-page state views, form fields with Touched-tracking, data-table pages, and Sitecore Marketplace extension-point shells, all built on top of Primitives.
- **Theming** — Light and dark mode support via semantic CSS tokens
- **Icons** — 300+ Material Design Icons available as static SVG path constants
- **Catalogue** — A companion Blazor web app for browsing and previewing all components

Note: The Catalogue contains implementation notes valuable for consumers of the project. You
can also view this online at https://blok-blazor-catalogue.ping-works.com.au. Of particular interest,
there are annotations for each component that identify whether they support SSR or require Interactive-mode
rendering, and whether there are additional elements that are required, such as companion components,
services or scripts. Be sure to review the catalogue for up-to-date information.


## Features

- **Idiomatic Blazor** — Components use `[Parameter]`, `EventCallback`, `CascadingValue`, and `RenderFragment` patterns native to Blazor
- **Composable** — Sub-components (e.g. `CardHeader`, `CardContent`, `CardFooter`) compose freely, matching the shadcn composition model
- **Tailwind CSS** — All styling via Tailwind utility classes, compiled at build time
- **Type-Safe** — Enum-driven variants for sizes, colors, and styles with full IntelliSense support
- **NuGet Distribution** — Published as a NuGet package with symbols, readme, and license

## Architecture

### Solution Structure

```
/
├── SitecoreBlok.BlazorUI.slnx                     # Solution file
├── LICENSE                                        # Apache 2.0
├── CONTRIBUTING.md
├── CODE_OF_CONDUCT.md
├── MIGRATION_STATUS.md                            # Per-component port status vs Blok
├── theme.md                                       # Tailwind v4 theme reference
├── tailwindcss-windows-x64.exe                    # Downloaded on build (gitignored)
│
├── .claude/                                       # Claude Code skills + install scripts
├── .github/                                       # Issue / discussion / PR templates
├── docs/
│   └── ui-parity-audit.md                         # UI parity audit notes
├── tools/
│   ├── verify-ui-parity.ps1                       # Parity harness
│   └── build-icon-metadata.ps1
│
├── PINGWorks.SitecoreBlok.BlazorUI/               # Component library (RCL, NuGet)
│   ├── Components/                                # All .razor components
│   │   ├── <PrimitiveName>/                       # One folder per Blok primitive (Button, Card, Dialog, …)
│   │   ├── Chunks/                                # Higher-level compositions of primitives
│   │   │   ├── Enums.cs                           # Chunks-shared enums (Tone, Density, Trend, Columns, …)
│   │   │   ├── Shared/                            # CSS-class helpers (ToneClasses, TrendClasses, SizeClasses, …)
│   │   │   ├── Layouts/  Headers/  Navigation/    # 7 chunk families
│   │   │   └── Content/  Forms/  Data/  Marketplace/
│   │   └── Extra/                                 # Catalogue-extra components (Text, ThemeToggle, …)
│   ├── Services/                                  # PopoverService, ToastService, GlobalTheme
│   ├── Ioc/                                       # DI extensions (AddSitecoreBlokUI)
│   ├── ThirdPartyNotices/                         # Blok, shadcn, Tailwind, PrismJS
│   ├── wwwroot/
│   │   ├── css/blok/                              # Tailwind theme + tokens
│   │   └── js/                                    # JS interop modules
│   ├── Enums.cs                                   # All variant/size/color enums
│   ├── CssClassBuilder.cs                         # Fluent CSS class utility
│   └── IconSvg.cs                                 # Curated ~300 SVG path constants
│
├── PINGWorks.SitecoreBlok.BlazorUI.Icons/         # Companion icon package (NuGet)
│   ├── IconSvg.cs                                 # Full ~7,500-icon set
│   ├── IllustrationSvg.cs
│   └── FaviconSvg.cs
│
└── PINGWorks.SitecoreBlok.BlazorUI.Catalogue/     # Catalogue web app
    ├── Components/
    │   ├── Layout/                                # MainLayout, NavMenu
    │   ├── Pages/
    │   │   ├── Primitives/                        # Per-primitive demo pages
    │   │   └── Chunks/                            # Per-chunk demo pages, grouped by family
    │   └── Shared/                                # ComponentPage, ComponentExample, DivergenceNote, HostContextNote
    └── wwwroot/                                   # Catalogue-specific assets
```

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download) or later
- Tailwind CSS CLI — downloaded automatically to the repo root on first build by the `DownloadTailwindCLI` MSBuild target. Pin a specific version with `-p:TailwindCliUrl=https://github.com/tailwindlabs/tailwindcss/releases/download/<tag>/tailwindcss-windows-x64.exe`.

### Installation

**1. Add the NuGet package**

```bash
dotnet add package PINGWorks.SitecoreBlok.BlazorUI
```

**2. Register services in `Program.cs`**

```csharp
builder.Services.AddSitecoreBlokUI();
```

**3. Include the stylesheet in `App.razor`**

```html
<link rel="stylesheet" href="_content/PINGWorks.SitecoreBlok.BlazorUI/css/sitecore-blok.css" />
```

**4. Add using directives to `_Imports.razor`**

```razor
@using PINGWorks.SitecoreBlok.BlazorUI
@using PINGWorks.SitecoreBlok.BlazorUI.Services
```

### Running the Catalogue

```bash
cd PINGWorks.SitecoreBlok.BlazorUI.Catalogue
dotnet run
```

## Usage Examples

### Button

```razor
<Button>Primary</Button>
<Button Variant="ButtonVariant.Outline" ColorScheme="ButtonColor.Neutral">Outline</Button>
<Button Variant="ButtonVariant.Ghost" ColorScheme="ButtonColor.Danger">Delete</Button>
```

### Card with Hover Elevation

```razor
<Card Style="CardStyle.Outline" Elevation="CardElevation.Xs" HoverElevation="CardElevation.Md">
    <CardHeader>
        <CardTitle>Project Name</CardTitle>
        <CardDescription>A brief description of the project.</CardDescription>
    </CardHeader>
    <CardContent>Content goes here.</CardContent>
    <CardFooter>
        <Button Size="ButtonSize.Sm">View</Button>
    </CardFooter>
</Card>
```

### Tabs

```razor
<Tabs DefaultValue="account">
    <TabsList>
        <TabsTrigger Value="account">Account</TabsTrigger>
        <TabsTrigger Value="settings">Settings</TabsTrigger>
    </TabsList>
    <TabsContent Value="account">Account settings here.</TabsContent>
    <TabsContent Value="settings">App settings here.</TabsContent>
</Tabs>
```

### Dialog

```razor
<Button Click="() => dialogOpen = true">Open Dialog</Button>

<Dialog Open="dialogOpen" OpenChanged="v => dialogOpen = v">
    <DialogHeader>
        <DialogTitle>Are you sure?</DialogTitle>
        <DialogDescription>This action cannot be undone.</DialogDescription>
    </DialogHeader>
    <DialogFooter>
        <Button Variant="ButtonVariant.Ghost" Click="() => dialogOpen = false">Cancel</Button>
        <Button ColorScheme="ButtonColor.Danger">Confirm</Button>
    </DialogFooter>
</Dialog>
```

### Chunks — composed page patterns

Chunks are higher-level compositions of Primitives that absorb the Tailwind class arrangements you'd otherwise write by hand. Examples:

```razor
@* App-shell envelope: dark-mode wrapper, popover/toaster mount, header/sidebar/content/footer slots *@
<AppShell InteractiveRenderMode="@RenderMode.InteractiveServer">
    <Header>
        <AppBrand Name="My App" Version="v1.0" />
    </Header>
    <Sidebar>
        <NavList>
            <NavListItem Href="/" IconSvg="@IconSvg.Information" Label="Dashboard" Active="true" />
            <NavListItem Href="/sites" IconSvg="@IconSvg.Information" Label="Sites" />
        </NavList>
    </Sidebar>
    <Content>
        <PageHeader Title="Sites" Description="All sites in this project.">
            <Actions>
                <Button>New site</Button>
            </Actions>
        </PageHeader>
        @* … *@
    </Content>
</AppShell>

@* Form field with built-in Touched-tracking and required-empty error styling *@
<TextField Label="Email" Type="TextField.TextFieldType.Email"
           Value="@email" ValueChanged="@(v => email = v)"
           Required="true" HelpText="We'll never share your email." />

@* Tonal callout, KPI tile, and confirm dialog *@
<Callout Title="Heads up" Tone="Tone.Warning" IconSvg="@IconSvg.AlertCircle">
    Saving will overwrite your current draft.
</Callout>
<KpiTile Label="Monthly users" Value="12,480" Delta="+8.4% vs last month" Trend="Trend.Up" />
<ConfirmDialog Title="Delete item?" Message="This action cannot be undone."
               Tone="Tone.Danger" Open="@confirmOpen"
               OpenChanged="@(v => confirmOpen = v)" OnConfirm="HandleDelete" />
```

The full chunk roster (85 chunks across Layouts, Headers, Navigation, Content, Forms, Data, Marketplace) is browsable at `/chunks` in the Catalogue.

## AI Assisted Component Migration

A dedicated Claude Code skill (`blok-migration`) is available to accelerate porting, updating, and verifying components against the Blok source. It wraps the full workflow — fetching the Blok source, creating the Razor components, registering them in the catalogue, running the parity harness (`tools/verify-ui-parity.ps1`), and fixing any findings — so contributors can delegate the mechanical parts of the work and spend their attention on judgement-call review.

The skill ships with the repo under [`.claude/skills/blok-migration/`](.claude/). Install it into your local Claude Code by running the included installer from the repo root:

```powershell
# Windows (PowerShell 7+) — also works on macOS / Linux with PowerShell installed
pwsh ./.claude/install-skills.ps1
```

```bash
# macOS / Linux (bash)
./.claude/install-skills.sh
```

The script copies every skill under `.claude/skills/` in this repo to `~/.claude/skills/` on your machine and tells Claude Code to restart the session to pick them up. See [`.claude/README.md`](.claude/README.md) for what's included.

Once installed, the skill is invoked by either a slash command or by natural-language phrasing. Always start any of these in a Claude Code session with the repo open.

### Trigger phrases

| Goal | Slash command | Natural-language phrase |
|------|---------------|-------------------------|
| Port a brand-new Blok primitive | `/blok migrate <name>` | "migrate the Blok `<name>` component" |
| Re-sync an existing component with the latest Blok source | `/blok update <name>` | "update the Blok `<name>` component" |
| Scan the Blok registry for new or changed primitives | `/blok audit` | "audit the Blok registry for new components" |
| Generate or refresh only the catalogue page | `/blok catalogue <name>` | "create the catalogue page for `<name>`" |
| Run the UI parity harness for one component and fix findings | `/blok verify <name>` | "verify the ui of component `<name>` against Blok" |
| Run the UI parity harness for every primitive and fix findings | `/blok verify all` | "check ui parity for all components against Blok" |

Replace `<name>` with the component name (e.g. `Button`, `Checkbox`, `DropdownMenu`).

### What the skill does end-to-end

For `migrate` and `update`:

1. Fetches the component's registry JSON (`https://blok.sitecore.com/r/<name>.json`) and the TSX source from the Blok GitHub repo.
2. Identifies every exported function, every variant, every compound variant, every dark-mode class — and documents them before writing code.
3. Writes the Razor components under `PINGWorks.SitecoreBlok.BlazorUI/Components/`, one `.razor` file per exported function, following the library's established patterns (CssClassBuilder, data-slot attributes, enum-driven variants, RenderFragment for composable children).
4. Registers the component in the catalogue (`NavMenu.razor`, the Primitives index, the Home page, and a new `Components/Pages/Primitives/<Name>Page.razor` with samples for each variant).
5. Updates the catalogue's Home page status sections if the component resolves a known gap or adds behaviour beyond Blok.
6. Runs `pwsh ./tools/verify-ui-parity.ps1 -Component <Name>` and resolves all findings before declaring the migration complete.

For `verify`:

1. Runs the parity harness against the scope you specified (single component or everything).
2. Processes findings by each of the harness's four checks:
   - Missing compiled Tailwind utilities (fix typos or note rebuild-required entries)
   - Runtime-composed class names (rewrite as literal strings)
   - Class-string drift from the Blok source (fix, or mark as an intentional equivalence/Blazor-only divergence)
   - Direct-child svg selectors that silently break because our `Icon` component wraps the svg in a span (fix with descendant selectors)
3. Applies fixes with anti-revert comments where the correct pattern is non-obvious.
4. Updates `docs/ui-parity-audit.md` with the component's outcome.
5. Re-runs the harness until it exits clean before announcing the work as done.

### Recommended process when creating or updating a component

This is the sequence that has produced the most reliable results:

1. **Decide the scope.** New primitive? Use `migrate`. Changed upstream? Use `update`. Visual bug report? Use `verify`.

2. **Invoke the skill.** Prefer the natural-language phrasing — it's more forgiving of typos and dispatches the same workflow. Example: *"migrate the Blok Stepper component"*.

3. **Stay in the loop during structural decisions.** The skill asks before making judgement calls (e.g. "Blok has both a `Separator` and a standalone `Divider`; should these be one Blazor component or two?"). Don't silently approve — your answer shapes the public API.

4. **Let the harness run to completion.** After the skill writes code, it will invoke `tools/verify-ui-parity.ps1`. Resist the temptation to skip this. Every regression we've seen in the past was something the harness would have caught.

5. **Review anti-revert comments before committing.** Where the skill encoded a non-obvious fix (e.g. `[&_svg]` instead of `[&>svg]`, `transition: grid-template-rows` instead of `max-height`, `opacity` instead of `translate-y` on the `ActionBar` outer wrapper), it leaves a `NOTE —` comment explaining why. Read these during code review; don't simplify them away.

6. **Do the visual spot-check yourself.** The harness catches class-string issues, not visual ones. After `dotnet run` on the catalogue:
   - Navigate to `/primitives/<name>`.
   - Open the equivalent page on `https://blok.sitecore.com/primitives/<name>` side-by-side.
   - Compare in **both light and dark mode** (toggle via the theme switcher in the top-right of the Catalogue).
   - Check every variant, size, colour scheme, and interactive state (default, hover, focus, active, disabled).
   - Compare spacing, typography sizes, shadows, border radius — things CSS variables can hide.

7. **File anything visible as feedback to the skill.** If the visual pass finds a divergence, report it back in the same Claude Code session ("the Stepper completed-step indicator has a purple ring in dark mode; the Blok source uses white"). The skill will investigate, fix, and re-run the harness.

8. **Commit only when both the harness and your eyes agree.** The skill won't declare work complete until the harness is clean, and you shouldn't merge until the visuals also match.

### Visual verification requirements

Visual verifications are performed using the **Google Chrome MCP** (Model Context Protocol) server. For the skill to drive a browser and capture the side-by-side comparisons against the live Blok reference site, the MCP extensions must be installed and configured in either **Google Chrome** or **Microsoft Edge** on your local machine. Without this, the skill cannot open pages, toggle dark mode, or inspect rendering, and any visual pass will be skipped.

Visual verifications are a vital step in ensuring that rendering and behaviours are correct and valid. The class-string parity harness (`tools/verify-ui-parity.ps1`) catches structural drift, but it does not — and cannot — catch visual regressions like incorrect spacing, wrong elevation, broken focus rings, or dark-mode colour mismatches. Only a real browser rendering against the live Blok reference can catch these.

Note that the skills will complete processing without a working Chrome/Edge MCP setup — they do not fail hard when the browser is unavailable. **However, contributions will not be accepted that aren't fully tested visually, including in dark mode.** If you cannot run the visual verification locally, say so explicitly on your PR rather than implying the work is complete; a reviewer with a configured environment will need to run the pass before the change can merge.

### Why a skill and not a script

The skill couples three things a pure script can't: fetching and understanding the Blok source, writing idiomatic Blazor (composed sub-components, enum-driven variants, idiomatic parameter naming), and applying context-sensitive judgement calls on drift findings (fix vs suppress vs document-as-deliberate). The script-based parity harness (`tools/verify-ui-parity.ps1`) is the mechanical verification backbone; the skill orchestrates around it.

See [`CONTRIBUTING.md`](CONTRIBUTING.md) for the component patterns and coding standards the skill follows (and which any manual contribution should also follow).

## Contributing

We welcome contributions from the community.

- **Repository**: [https://github.com/PINGWorks-AU/SitecoreBlok.BlazorUI](https://github.com/PINGWorks-AU/SitecoreBlok.BlazorUI)
- **Issues**: [https://github.com/PINGWorks-AU/SitecoreBlok.BlazorUI/issues](https://github.com/PINGWorks-AU/SitecoreBlok.BlazorUI/issues)
- **Pull Requests**: [https://github.com/PINGWorks-AU/SitecoreBlok.BlazorUI/pulls](https://github.com/PINGWorks-AU/SitecoreBlok.BlazorUI/pulls)
- Please read our [Contributing Guide](CONTRIBUTING.md) for details on the development workflow and component patterns.
- All participants are expected to follow our [Code of Conduct](CODE_OF_CONDUCT.md).

## Acknowledgements

This library is built upon the work of:

- [Sitecore Blok](https://github.com/Sitecore/blok) — The original design system (Apache 2.0)
- [shadcn/ui](https://github.com/shadcn-ui/ui) — The component architecture foundation (MIT)
- [Tailwind CSS](https://github.com/tailwindlabs/tailwindcss) — The utility-first CSS framework (MIT)

## License

This project is licensed under the Apache License 2.0. See the [LICENSE](LICENSE) file for details.

Third-party license notices are included in the [ThirdPartyNotices](PINGWorks.SitecoreBlok.BlazorUI/ThirdPartyNotices/) directory.
