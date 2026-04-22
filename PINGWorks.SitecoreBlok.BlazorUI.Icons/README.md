# README

This is a companion library for the Blazor port of the Sitecore Blok UI kit.
There are over 7,500 SVG images in this library making it quite large. This is not a problem for a SSR project but if you're
intending to deliver this library to a phone app or WASM then it is preferable to include only the images that
you actually need, as we have done in the `PINGWorks.SitecoreBlok.BlazorUI` library.

You can review the full library in the Catalogue project.

## Installation

Add the package to your Blazor project via NuGet.

Add the following to the `_Imports.razor` file in your Blazor project:

```razor
@using PINGWorks.SitecoreBlok.BlazorUI.Icons
```
