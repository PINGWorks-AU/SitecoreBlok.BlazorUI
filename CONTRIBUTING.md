# Contributing to PINGWorks.SitecoreBlok.BlazorUI

Thank you for your interest in contributing to the PINGWorks.SitecoreBlok.BlazorUI library. This is a Blazor port of the Sitecore Blok design system, providing idiomatic Blazor components for building martech applications. We welcome contributions of all kinds, including bug reports, feature suggestions, documentation improvements, and code submissions.

## Repository

This project is hosted on GitHub: [https://github.com/PINGWorks-AU/SitecoreBlok.BlazorUI](https://github.com/PINGWorks-AU/SitecoreBlok.BlazorUI)

## How to Contribute

- **Report Bugs** — Found a problem? [Open an issue](https://github.com/PINGWorks-AU/SitecoreBlok.BlazorUI/issues/new) with a clear description, steps to reproduce, and expected vs actual behavior.
- **Suggest Features** — Have an idea for a new component or improvement? [Open an issue](https://github.com/PINGWorks-AU/SitecoreBlok.BlazorUI/issues/new) describing the use case and proposed solution.
- **Improve Documentation** — Help improve component examples, catalogue pages, or inline documentation.
- **Submit Code** — Fix bugs, add components, or improve existing implementations via [pull requests](https://github.com/PINGWorks-AU/SitecoreBlok.BlazorUI/pulls).

## Code of Conduct

All contributors are expected to follow our [Code of Conduct](CODE_OF_CONDUCT.md). Please be respectful, professional, and constructive in all interactions.

## Development Workflow

1. **Fork** the repository on GitHub: [https://github.com/PINGWorks-AU/SitecoreBlok.BlazorUI/fork](https://github.com/PINGWorks-AU/SitecoreBlok.BlazorUI/fork)
2. **Clone your fork** locally: `git clone https://github.com/YOUR-USERNAME/SitecoreBlok.BlazorUI.git`
3. **Create a branch** from `main` for your changes (e.g. `feature/add-tooltip` or `fix/button-hover`).
4. **Install prerequisites** — [.NET 10 SDK](https://dotnet.microsoft.com/download). The Tailwind CSS CLI is downloaded automatically to the repo root on first build (`DownloadTailwindCLI` MSBuild target); no manual install required.
5. **Make your changes** following the established component patterns (see below).
6. **Build** the solution to verify there are no errors: `dotnet build` from the solution root.
7. **Test visually** by running the Catalogue project and verifying your component renders correctly.
8. **Push to your fork** and **submit a Pull Request** against `main` with a clear description of your changes.

### Deployment Flow

1. [Pull request](https://github.com/PINGWorks-AU/SitecoreBlok.BlazorUI/pulls) submitted targeting `main`
2. Code review by a maintainer
3. Merge to `main` after approval; releases are cut from `main`

## Component Patterns

When creating or modifying components, follow these established patterns:

- **CssClassBuilder** — Use `CssClassBuilder.Start(...).With(class, condition).Build()` for Tailwind class composition.
- **Never compose Tailwind class names from fragments at runtime.** The Tailwind CLI scans source files for full, literal class-name strings. A class assembled from variables (e.g. `$"text-{color}-{shade}"`, `$"bg-{variant}"`) will not be found by the scanner and the corresponding CSS will not be generated, even if the runtime value happens to be a valid utility. Always write the full utility string as a literal. If you need conditional classes, use `.With(class, condition)` on `CssClassBuilder`, or a ternary between two literal strings: `$"{(isActive ? "text-primary" : "text-muted-foreground")} {ClassName}"`.
- **Composable sub-components** — Each exported element in the ShadCN source should be a separate Blazor component (e.g. `CardHeader.razor`, `CardContent.razor`), not a RenderFragment parameter on the parent.
- **Parameters** — Only add `ClassName` if the source component accepts `className`. Only add `ChildContent` if the source renders children. Add `AdditionalAttributes` on leaf elements that forward `{...props}`.
- **Enums** — Define variant/size/color enums in `Enums.cs`, one per component concept.
- **data-slot** — Add `data-slot` attributes matching the ShadCN source.
- **Review step** — After creating a component, compare it side-by-side with the Blok registry JSON (`https://blok.sitecore.com/r/{name}.json`) to verify all features are correctly mapped.
- **Catalogue page** — Add a page to the Catalogue project for every new component. Register it in NavMenu, the Primitives index, and the Home page.
- **Run the parity harness** — After adding or changing a component, run `pwsh ./tools/verify-ui-parity.ps1 -Component {Name}`. It checks that every Tailwind class you reference is generated in the compiled CSS, flags any runtime-composed class names, and diffs your class strings against the Blok source. The PR must not introduce new findings.

## Commit Message Guidelines

- Use the imperative mood (e.g. "Add tooltip component" not "Added tooltip component")
- Keep the subject line concise (under 72 characters)
- Include a body with additional detail when the change is non-trivial
- Reference related issues where applicable (e.g. "Fixes #42")

## Reporting Issues and Requesting Features

When [opening an issue](https://github.com/PINGWorks-AU/SitecoreBlok.BlazorUI/issues/new), please include:

- A clear, descriptive title
- Steps to reproduce (for bugs) or a use case description (for features)
- Expected and actual behavior (for bugs)
- Screenshots or code samples where helpful

## Review and Feedback

Pull requests are typically reviewed within a few business days. Maintainers may request changes or suggest improvements. Please engage constructively with feedback and update your PR accordingly.

## Acknowledgements

Thank you for helping improve this library. Every contribution, no matter how small, is valued and appreciated.
