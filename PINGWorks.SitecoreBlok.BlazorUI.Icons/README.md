# PINGWorks.SitecoreBlok.BlazorUI.Icons

Companion icon package for [PINGWorks.SitecoreBlok.BlazorUI](https://www.nuget.org/packages/PINGWorks.SitecoreBlok.BlazorUI/) — the Blazor port of the Sitecore Blok UI kit.

This package contains the **full ~7,500-icon Material Design Icon set** as static SVG path constants on `IconSvg`, `IllustrationSvg`, and `FaviconSvg`. It's intended for server-rendered (SSR) apps where bundle size is not a concern.

For Blazor WebAssembly, mobile apps, or any scenario where payload matters, use the curated ~300-icon subset already included in the main `PINGWorks.SitecoreBlok.BlazorUI` package and add only the icons you actually need.

You can browse the full icon set in the Catalogue web app shipped in the [project repository](https://github.com/PINGWorks-AU/SitecoreBlok.BlazorUI).

## Installation

```bash
dotnet add package PINGWorks.SitecoreBlok.BlazorUI.Icons
```

Add the using directive to `_Imports.razor`:

```razor
@using PINGWorks.SitecoreBlok.BlazorUI.Icons
```

Reference an icon via the `<Icon>` component from the main package:

```razor
<Icon Svg="@IconSvg.AccountCircle" />
```

## License

Licensed under the [Apache License 2.0](https://www.apache.org/licenses/LICENSE-2.0). Third-party notices are included under `ThirdPartyNotices/`.
