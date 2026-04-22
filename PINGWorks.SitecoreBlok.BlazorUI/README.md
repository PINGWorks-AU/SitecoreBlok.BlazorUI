# PINGWorks.SitecoreBlok.BlazorUI

Idiomatic Blazor components ported from the [Sitecore Blok](https://blok.sitecore.com) design system. Build Sitecore Marketplace apps and other martech UIs with a familiar component vocabulary in pure Blazor.

> **Unofficial port.** This library is maintained by [PING Works](https://ping-works.com.au) and is not affiliated with or endorsed by Sitecore. The original Blok design system is © Sitecore and licensed under Apache 2.0.

## Highlights

- **40+ primitives** — buttons, cards, dialogs, dropdowns, form fields, tables, tabs, toasts, and more
- **Composable** — sub-components (`CardHeader`, `CardContent`, `CardFooter`, …) compose freely, matching the shadcn model Blok is built on
- **Idiomatic Blazor** — `[Parameter]`, `EventCallback`, `CascadingValue`, `RenderFragment`; no thin React-over-Blazor wrappers
- **Type-safe variants** — enum-driven sizes, colours, and styles with full IntelliSense
- **Light & dark themes** — semantic CSS tokens; runtime theme toggle included
- **Tailwind compiled in** — the package ships pre-compiled CSS; consumers do not need a Tailwind toolchain
- **SSR & interactive** — components are annotated for both Server and WebAssembly render modes

## Requirements

- .NET 10 SDK or later
- A Blazor host (Server, WebAssembly, or Blazor Web App with either render mode)

## Installation

**1. Add the package**

```bash
dotnet add package PINGWorks.SitecoreBlok.BlazorUI
```

**2. Register services in `Program.cs`**

```csharp
using Microsoft.Extensions.DependencyInjection;

builder.Services.AddSitecoreBlokUI();
```

This registers the scoped services the library needs (`PopoverService`, `ToastService`, `GlobalTheme`).

**3. Reference the stylesheet in `App.razor` (or `index.html` for standalone WASM)**

```html
<link rel="stylesheet" href="@Assets["_content/PINGWorks.SitecoreBlok.BlazorUI/css/sitecore-blok.css"]" />
```

**4. Add using directives to `_Imports.razor`**

```razor
@using PINGWorks.SitecoreBlok.BlazorUI
@using PINGWorks.SitecoreBlok.BlazorUI.Services
```

## Usage

### Button

```razor
<Button>Primary</Button>
<Button Variant="ButtonVariant.Outline" ColorScheme="ButtonColor.Neutral">Outline</Button>
<Button Variant="ButtonVariant.Ghost" ColorScheme="ButtonColor.Danger">Delete</Button>
```

### Card

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

A live, browsable Catalogue of every component (with source for each variant) ships in the project repository.

## Theming

The library ships light and dark themes via semantic CSS custom properties. Theme state is exposed through the `GlobalTheme` scoped service and a ready-made `<ThemeToggle />` component:

```razor
<ThemeToggle />
```

To set the initial theme before first render (avoiding a flash), include the startup script in your host page:

```razor
<ThemeToggleStartupScript />
```

## Companion package — Icons

The main package includes a curated set of ~300 Material Design Icon paths as `IconSvg.*` constants:

```razor
<Icon Svg="@IconSvg.Check" />
```

If you need the **full** ~7,500-icon set (e.g. for SSR apps where bundle size is not a concern), install the companion package:

```bash
dotnet add package PINGWorks.SitecoreBlok.BlazorUI.Icons
```

For Blazor WebAssembly or mobile apps, prefer the curated set in the main package and add only the icons you actually use.

## A note on class strings

This is a Tailwind-based design system, so components emit fairly long literal `class` attributes. This is by design — the Tailwind CLI scans source for **literal** class strings, so all utilities are written out in full. If you extend a component, follow the same rule (or use `CssClassBuilder`); never assemble class names from string fragments at runtime.

## Source, issues, and contributing

The full source — including the Catalogue web app, the UI parity-check tooling (`tools/verify-ui-parity.ps1`), and contribution guidelines — is maintained by [PING Works](https://ping-works.com.au) on GitHub at [https://github.com/PINGWorks-AU/SitecoreBlok.BlazorUI](https://github.com/PINGWorks-AU/SitecoreBlok.BlazorUI).

Bug reports and feature requests are welcome via [GitHub Issues](https://github.com/PINGWorks-AU/SitecoreBlok.BlazorUI/issues).

## License

Licensed under the [Apache License 2.0](https://www.apache.org/licenses/LICENSE-2.0). Third-party notices for Sitecore Blok, shadcn/ui, and Tailwind CSS are included in the package under `ThirdPartyNotices/`.
