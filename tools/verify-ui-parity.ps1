<#
.SYNOPSIS
    Verifies BlazorUI / Blok UI parity across six dimensions:
    (1) every Tailwind class referenced in Razor exists in the compiled CSS,
    (2) no class names are composed at runtime from fragments,
    (3) class strings don't drift wildly from Blok source,
    (4) any theme-aware surface background (bg-background, bg-card, bg-popover,
        bg-muted, bg-accent, bg-primary, bg-secondary, bg-destructive) is paired
        with an explicit text-* token in the same class string. Without the
        pairing, dark-mode text colour relies on inheritance — which silently
        breaks for fixed-positioned and portal-rendered content.
    (5) any hardcoded fixed-shade background (bg-gray-700, bg-black, bg-{color}-500,
        etc.) paired with a flipping text token (text-foreground, text-inverse-text,
        text-*-fg, etc.) is flagged — the text colour flips between modes while
        the bg doesn't, producing invisible text in one mode (Tooltip class of
        bug). Use literal text colours like text-white instead.
    (6) any `--color-X-N` token defined as `blackAlpha-*` (or `whiteAlpha-*`) in
        colors.css must have a corresponding override in globals.css `.dark { }`
        — otherwise an alpha-on-dark token stays invisible in dark mode. Catches
        the Skeleton-class "invisible in dark mode" bug systemically.

.DESCRIPTION
    Run from the repo root. Exits non-zero if any issue is found.

    By default runs all six checks across all components. Use -Component
    to scope a single check (or -Components "Button,Card,Input" to scope a
    subset).

    Writes a machine-readable report to docs/ui-parity-report.md.

.PARAMETER Component
    Optional single component name (without ".razor"). Scopes all checks to
    that component only. Useful from the blok-migration skill.

.PARAMETER Components
    Optional comma-separated list of component names.

.PARAMETER SkipDrift
    Skip Check 3 (Blok source diff). Useful offline.

.PARAMETER ReportPath
    Override the output report path. Defaults to docs/ui-parity-report.md.

.EXAMPLE
    # Check everything
    pwsh ./tools/verify-ui-parity.ps1

.EXAMPLE
    # Check only Button
    pwsh ./tools/verify-ui-parity.ps1 -Component Button

.EXAMPLE
    # Check Button and Input, skip drift (offline)
    pwsh ./tools/verify-ui-parity.ps1 -Components "Button,Input" -SkipDrift

.OUTPUTS
    docs/ui-parity-report.md — markdown report with all findings.
    Exit code 0 if clean, 1 if any issues.
#>

[CmdletBinding()]
param(
    [string]$Component,
    [string]$Components,
    [switch]$SkipDrift,
    [string]$ReportPath = "docs/ui-parity-report.md"
)

$ErrorActionPreference = 'Stop'

# ---- Paths -------------------------------------------------------------------

$repoRoot      = Split-Path $PSScriptRoot -Parent
$componentsDir = Join-Path $repoRoot "PINGWorks.SitecoreBlok.BlazorUI/Components"
$compiledCss   = Join-Path $repoRoot "PINGWorks.SitecoreBlok.BlazorUI/wwwroot/css/sitecore-blok.css"
$fullReportPath = Join-Path $repoRoot $ReportPath

if (-not (Test-Path $componentsDir)) { throw "Components folder not found: $componentsDir" }
if (-not (Test-Path $compiledCss))   { throw "Compiled CSS not found: $compiledCss (has Tailwind CLI run?)" }

# ---- Which components to scan -----------------------------------------------

function Resolve-ComponentFiles([string]$name, [string]$root) {
    $trim = $name.Trim()
    $familyDir = Join-Path $root $trim
    if (Test-Path $familyDir -PathType Container) {
        # New structure: Components/{Family}/*.razor — scan the whole family folder
        return Get-ChildItem -Path $familyDir -Filter *.razor -File -Recurse | ForEach-Object { $_.FullName }
    }
    # Legacy structure: Components/{Name}.razor — single flat file
    return @(Join-Path $root "$trim.razor")
}

$razorFiles = if ($Component) {
    Resolve-ComponentFiles -name $Component -root $componentsDir
} elseif ($Components) {
    $Components.Split(',') | ForEach-Object { Resolve-ComponentFiles -name $_ -root $componentsDir }
} else {
    Get-ChildItem -Path $componentsDir -Filter *.razor -File -Recurse | ForEach-Object { $_.FullName }
}

$razorFiles = $razorFiles | Where-Object { Test-Path $_ }
if ($razorFiles.Count -eq 0) { throw "No Razor files to scan." }

Write-Host "Scanning $($razorFiles.Count) component(s)." -ForegroundColor Cyan

# ---- Helpers -----------------------------------------------------------------

# Extract every quoted string that looks like it contains Tailwind classes.
# Targets:
#   - CssClassBuilder chain calls   .Start("…") / .With("…") / .Reset("…")
#   - class="literal-only"          simple attribute with no interpolation
#   - Inside class="@(…)"           we pull literal string tokens, but only
#                                   from known class-context function calls.
# We deliberately ignore style=, data-slot=, href=, src= and other attributes.
function Get-RazorClassStrings {
    param([string]$Path)

    $content = Get-Content -Path $Path -Raw

    $found = [System.Collections.Generic.List[string]]::new()

    # 1) CssClassBuilder chain — every "literal" passed to .Start/.With/.Reset
    #    (also covers the multi-arg form: .Start("a","b","c") because the
    #     regex matches each "…" individually).
    $chainPattern = '(?:CssClassBuilder\.Start|\bCssClassBuilder\b\.With|(?<=Start\s*\([^)]*)\s*,\s*"[^"]*")|(\.With|\.Reset)\s*\(\s*(?:[^"]*?"([^"]*)")'
    # Simpler: match every `.Start(...)` / `.With(...)` / `.Reset(...)` block
    # and pull strings from inside the parens (scoped to that call).
    $callPattern = '\b(?:Start|With|Reset)\s*\(((?:[^()"]|"[^"]*")*)\)'
    [regex]::Matches($content, $callPattern) | ForEach-Object {
        $argBlock = $_.Groups[1].Value
        [regex]::Matches($argBlock, '"([^"]*)"') | ForEach-Object { $found.Add($_.Groups[1].Value) }
    }

    # 2) Plain class="literal" — only when the value contains no `@` (no interpolation)
    [regex]::Matches($content, 'class\s*=\s*"([^"@]*)"') | ForEach-Object { $found.Add($_.Groups[1].Value) }

    # 3) Interpolated class="@( $"…" )" — pull string literals only from
    #    inside @( ) that are specifically assigned to class. This stops at the
    #    attribute boundary by looking for `)">` or `)"\s+\w+=` (next attribute).
    $interpClassPattern = 'class\s*=\s*"@\((.*?)\)\s*"(?=\s|/?>)'
    [regex]::Matches($content, $interpClassPattern, [System.Text.RegularExpressions.RegexOptions]::Singleline) | ForEach-Object {
        $argBlock = $_.Groups[1].Value
        [regex]::Matches($argBlock, '"([^"]*)"') | ForEach-Object { $found.Add($_.Groups[1].Value) }
    }

    # 4) ClassName="literal" attribute (Razor component parameter, same shape as class=)
    [regex]::Matches($content, 'ClassName\s*=\s*"([^"@]*)"') | ForEach-Object { $found.Add($_.Groups[1].Value) }

    # 5) Switch-expression arms — `=> "literal class string"` in @code blocks.
    #    Captures class strings returned from switch expressions (e.g. TileClass, ColorSchemeClass).
    #    Get-TailwindTokens will filter out any non-Tailwind strings naturally.
    [regex]::Matches($content, '=>\s*"([^"]*)"') | ForEach-Object { $found.Add($_.Groups[1].Value) }

    $found
}

# Extract Tailwind-like tokens from a class string (may contain multiple).
function Get-TailwindTokens {
    param([string]$ClassString)
    if ([string]::IsNullOrWhiteSpace($ClassString)) { return @() }

    # Split on whitespace; keep anything that looks like a utility (contains a letter and at least one of - : [ /)
    $tokens = $ClassString -split '\s+' | Where-Object {
        $_.Length -gt 0 -and $_ -match '[a-z]' -and ($_ -match '[-:\[/]')
    }
    $tokens | Select-Object -Unique
}

# Strip the `size-N:` responsive prefix and `dark:`/`hover:`/etc modifiers
# to get the bare utility (e.g. `dark:hover:bg-primary-bg` → `bg-primary-bg`).
function Get-UtilityBase {
    param([string]$Token)
    $parts = $Token -split ':'
    return $parts[-1]
}

# ---- Check 1 — Compiled-utility coverage ------------------------------------

Write-Host "`n[1/3] Checking compiled-utility coverage…" -ForegroundColor Cyan

$compiledContent = Get-Content -Path $compiledCss -Raw
$missingUtilities = [System.Collections.Generic.List[pscustomobject]]::new()

foreach ($file in $razorFiles) {
    $fileName = Split-Path $file -Leaf
    $classStrings = Get-RazorClassStrings -Path $file
    $tokens       = $classStrings | ForEach-Object { Get-TailwindTokens $_ } | Select-Object -Unique

    foreach ($token in $tokens) {
        # Arbitrary values like `min-h-[4rem]` or `max-h-(--var)` always work (generated inline from source scan).
        if ($token -match '\[.*\]') { continue }
        if ($token -match '\(.*\)') { continue }

        # Skip pseudo-selectors from [&_svg]:.. patterns that aren't real class names
        if ($token -match '^\[') { continue }

        # Named container-query / group / peer variants: `group/name`, `peer/name`, `@container/name`
        # Tailwind generates these lazily; they always work when referenced.
        if ($token -match '^(group|peer|@container)/[a-zA-Z0-9_-]+$') { continue }

        # `parity-*` is the harness's own marker namespace (e.g. `parity-no-text-pair`).
        # These are not real Tailwind utilities — they're suppression hints for Check 4.
        if ($token -match '^parity-') { continue }

        # Tailwind compiles class selectors with CSS escapes for special chars:
        #   gap-1.5                → `.gap-1\.5`          (dot escaped)
        #   hover:bg-primary-hover → `.hover\:bg-primary-hover`
        #   dark:bg-input/30       → `.dark\:bg-input\/30`
        #   dark:hover:bg-input/50 → `.dark\:hover\:bg-input\/50`
        #   !bg-transparent        → `.\!bg-transparent`
        #   size-2.5               → `.size-2\.5`
        # Build the CSS-escaped form of the selector, then regex-escape for matching.
        $cssEscaped = $token `
            -replace '\.', '\.' `
            -replace ':', '\:' `
            -replace '/', '\/' `
            -replace '!', '\!'
        $literalTarget = ".$cssEscaped"
        $regexPattern  = [regex]::Escape($literalTarget)

        # Match followed by any non-identifier char (so `bg-primary` doesn't match `bg-primary-bg`)
        $found = $compiledContent -match "$regexPattern(?![a-zA-Z0-9_-])"

        if (-not $found) {
            $missingUtilities.Add([pscustomobject]@{
                Component = $fileName
                Token     = $token
            })
        }
    }
}

# ---- Check 2 — Runtime-composed class detection ------------------------------

Write-Host "[2/3] Checking for runtime-composed class strings…" -ForegroundColor Cyan

$composedFindings = [System.Collections.Generic.List[pscustomobject]]::new()

# Only flag interpolations glued directly to a Tailwind utility prefix.
# Safe: "hello {ClassName}" (whitespace before brace), "bar {(a ? "foo" : "baz")}" (ternary of literals)
# Dangerous: "text-{shade}" or "bg-{color}-500" (token fragment composition)
$tailwindPrefixes = '(?:bg|text|border|ring|hover|active|focus|dark|placeholder|divide|outline|accent|fill|stroke|shadow|from|to|via|m|p|mx|my|mt|mr|mb|ml|px|py|pt|pr|pb|pl|w|h|min-w|min-h|max-w|max-h|size|gap|grid-cols|grid-rows|col-span|row-span|rounded|opacity|space)'

foreach ($file in $razorFiles) {
    $content = Get-Content -Path $file -Raw

    # Find $"..." blocks THAT are used in a class/ClassName context.
    # We look at the 40 characters preceding the $"…" to see what attribute it's assigned to.
    $interpBlocks = [regex]::Matches($content, '\$"([^"]*)"')
    foreach ($m in $interpBlocks) {
        $str = $m.Groups[1].Value

        # Inspect preceding context to figure out what attribute this is in
        $preceding = if ($m.Index -gt 50) { $content.Substring($m.Index - 50, 50) } else { $content.Substring(0, $m.Index) }
        $isClassContext = $preceding -match 'class\s*=\s*"@\(\s*$' `
                       -or $preceding -match 'ClassName\s*=\s*"?\s*@?\(?\s*$' `
                       -or $preceding -match '(?:\.Start|\.With|\.Reset)\s*\(\s*$'
        if (-not $isClassContext) { continue }

        # Skip if the string looks like a CSS value or inline style (contains `var(`, unit values, etc.)
        if ($str -match 'var\(|px\)|px;|rem\)|rem;|%\)|%;|transform:|position:|top:|left:|display:') { continue }

        # Look for a Tailwind prefix followed by `-` immediately before `{`, OR `}` followed by class-like chars
        $dangerous = [regex]::IsMatch($str, "$tailwindPrefixes(?:-[a-z]+)?-\{")
        $dangerous = $dangerous -or [regex]::IsMatch($str, '\}-(?:\d|[a-z])')

        if ($dangerous) {
            $upToMatch = $content.Substring(0, $m.Index)
            $line = ($upToMatch -split "`n").Length
            $composedFindings.Add([pscustomobject]@{
                Component = Split-Path $file -Leaf
                Line      = $line
                Snippet   = $str.Substring(0, [Math]::Min(120, $str.Length))
            })
        }
    }
}

# ---- Check 3 — Blok class-string drift (optional) ----------------------------

$driftFindings = [System.Collections.Generic.List[pscustomobject]]::new()

if (-not $SkipDrift) {
    Write-Host "[3/3] Checking Blok class-string drift…" -ForegroundColor Cyan

    # Equivalence map — pairs of functionally-identical class names (shadcn/ui ↔ Chakra-semantic
    # naming, or semantic-token-aliases that redirect to the same CSS variable).
    # Both sides map to the same canonical form so "our" and "Blok" classes compare as equal.
    $equivGroups = @(
        @('bg-background', 'bg-body-bg'),
        @('border-border', 'border-border-color'),
        @('text-foreground', 'text-body-text'),
        @('focus-visible:border-primary', 'focus:border-primary'),
        @('focus-visible:ring-primary/50', 'focus:ring-primary'),
        @('text-muted-foreground', 'text-subtle-text'),
        @('text-accent-foreground', 'text-body-text'),
        # Semantic hover/active aliases — `--primary-hover` resolves to `--color-primary-600`
        # in globals.css, so `bg-primary-hover` and `bg-primary-600` paint the same colour.
        # Same for active/700 and the success/danger families.
        @('hover:bg-primary-hover', 'hover:bg-primary-600'),
        @('active:bg-primary-active', 'active:bg-primary-700'),
        @('hover:bg-success-hover', 'hover:bg-success-600'),
        @('active:bg-success-active', 'active:bg-success-700'),
        @('hover:bg-danger-hover', 'hover:bg-danger-600'),
        @('active:bg-danger-active', 'active:bg-danger-700'),
        # Direct-child vs descendant svg selectors — equivalent now that Icon
        # emits the <svg> directly with no wrapper. Documented in Common
        # Pitfalls (Icon section); both forms target the same element.
        @('dark:[&>svg]:text-primary-200', 'dark:[&_svg]:text-primary-200'),
        @('dark:[&>svg]:text-danger-200', 'dark:[&_svg]:text-danger-200'),
        @('dark:[&>svg]:text-warning-200', 'dark:[&_svg]:text-warning-200'),
        @('dark:[&>svg]:text-success-200', 'dark:[&_svg]:text-success-200'),
        @('dark:[&>svg]:text-info-200', 'dark:[&_svg]:text-info-200')
    )
    $canonicalMap = @{}
    foreach ($group in $equivGroups) {
        $canonical = $group[0]
        foreach ($alias in $group) { $canonicalMap[$alias] = $canonical }
    }

    # Track which components we've already audited (aggregated with sub-components)
    $auditedBlokNames = @{}

    foreach ($file in $razorFiles) {
        $fileName = Split-Path $file -Leaf
        $compName = [IO.Path]::GetFileNameWithoutExtension($fileName).ToLowerInvariant()

        # Figure out the Blok base name. Our sub-components use PascalCase splits:
        #   AccordionItem → accordion, BreadcrumbLink → breadcrumb, DialogHeader → dialog
        # Heuristic: try the full name, then progressively strip trailing PascalCase words
        # until we hit a Blok source file.
        $baseName = $compName
        $blokSrc = $null
        $candidate = $baseName
        for ($i = 0; $i -lt 4; $i++) {
            $blokUrl = "https://raw.githubusercontent.com/Sitecore/blok/main/src/components/ui/$candidate.tsx"
            try {
                $blokSrc = Invoke-WebRequest -Uri $blokUrl -UseBasicParsing -ErrorAction Stop -TimeoutSec 10
                $baseName = $candidate
                break
            } catch {
                # Strip trailing PascalCase word from original name
                $candidate = [IO.Path]::GetFileNameWithoutExtension($fileName)
                $m = [regex]::Match($candidate, '^(.+?)[A-Z][a-z]+$')
                if (-not $m.Success) { $candidate = $null; break }
                $candidate = $m.Groups[1].Value.ToLowerInvariant()
                if ($auditedBlokNames.ContainsKey($candidate)) { $candidate = $null; break }
            }
        }

        if (-not $blokSrc) { continue }
        if ($auditedBlokNames.ContainsKey($baseName)) { continue }
        $auditedBlokNames[$baseName] = $true

        # Aggregate our class strings across ALL razor files that map to this Blok component.
        # We search the full Components tree here (not just $razorFiles) because sub-components
        # may be outside the scope explicitly passed via -Component / -Components.
        $relatedFiles = Get-ChildItem -Path $componentsDir -Filter *.razor -File -Recurse |
                        Where-Object {
                            $n = [IO.Path]::GetFileNameWithoutExtension($_.Name).ToLowerInvariant()
                            $n -eq $baseName -or $n -like "$baseName*"
                        } | ForEach-Object { $_.FullName }

        $ourTokens = $relatedFiles |
                     ForEach-Object { Get-RazorClassStrings -Path $_ } |
                     ForEach-Object { Get-TailwindTokens $_ } |
                     Select-Object -Unique

        # Pull Blok class strings from the source (strings containing Tailwind-like content)
        $blokTokens = [regex]::Matches($blokSrc.Content, '"([^"]*(?:bg-|text-|border-|ring-|hover:|active:|focus:|focus-visible:|aria-|data-|dark:|\[&|rounded-|shadow-|h-|w-|size-|px-|py-|gap-|flex|inline-|relative|absolute|fixed|transition)[^"]*)"') |
                       ForEach-Object { $_.Groups[1].Value } |
                       ForEach-Object { Get-TailwindTokens $_ } |
                       Select-Object -Unique

        # Normalize via canonical map
        $normalize = {
            param($t)
            if ($canonicalMap.ContainsKey($t)) { return $canonicalMap[$t] }
            return $t
        }

        $ourSet  = $ourTokens  | ForEach-Object { & $normalize $_ } | Select-Object -Unique
        $blokSet = $blokTokens | ForEach-Object { & $normalize $_ } | Select-Object -Unique

        # Classes in Blok but not in ours (missing) — filter for real drift only.
        # Skip ARBITRARY-VALUE tokens (like `bg-[#ff0000]`, `min-h-[4rem]`) but
        # allow STATE-SELECTOR tokens (like `data-[state=open]:bg-primary` or
        # `dark:data-[state=unchecked]:bg-foreground`). Differentiator: split on
        # `:` and inspect the LAST segment (the actual utility); if that ends
        # in a `[...]` value it's arbitrary, otherwise it's a real utility with
        # variant prefixes that just happen to contain `[...]`.
        $isArbitraryValueToken = {
            param($tok)
            $segments = $tok -split ':'
            $utility = $segments[-1]
            return $utility -match '^[a-z-]+-\[[^\]]+\]$'
        }
        $missingInOurs = $blokSet | Where-Object {
            ($_ -notin $ourSet) `
              -and ($_ -match '^(bg-|text-|border-|ring-|hover:|active:|focus:|dark:|rounded-|shadow-)') `
              -and ($_ -notmatch '[\[\]\(\),\.]$') `
              -and ($_ -notmatch '\(.*\)') `
              -and -not (& $isArbitraryValueToken $_)
        }

        if ($missingInOurs) {
            foreach ($mm in $missingInOurs | Select-Object -First 5) {
                $driftFindings.Add([pscustomobject]@{
                    Component = "$baseName.tsx (Blok) ↔ $($relatedFiles.Count) Razor file(s)"
                    InBlok    = $mm
                    InOurs    = "(missing)"
                })
            }
        }
    }
}

# ---- Check 4 — Surface background without paired text token ------------------
#
# Theme-aware "surface" background utilities (bg-background, bg-card, bg-popover,
# bg-muted, bg-accent, bg-primary, bg-secondary, bg-destructive) flip colour in
# dark mode via CSS variables. The corresponding text colour is supposed to
# inherit from the body's `text-foreground` — but in fixed-positioned or
# portal-rendered content (Dialog, AlertDialog, Sheet, etc.) the cascade is
# unreliable and dark-mode text disappears or stays the wrong colour.
#
# This check flags any class string that contains a surface background utility
# (unprefixed) AND does not contain ANY `text-*` utility (unprefixed or
# prefixed). The fix is to pair the background with the matching text token,
# e.g. `bg-background text-foreground`, `bg-card text-card-foreground`,
# `bg-primary text-white`.

Write-Host "[4/6] Checking surface backgrounds for missing text-* pairing." -ForegroundColor Cyan

$bgTextFindings = [System.Collections.Generic.List[pscustomobject]]::new()

$surfaceBgPattern = '\bbg-(background|card|popover|secondary|muted|accent|primary|destructive)(?![a-z0-9_-])'

foreach ($file in $razorFiles) {
    $fileName = Split-Path $file -Leaf
    $classStrings = Get-RazorClassStrings -Path $file

    foreach ($cs in $classStrings) {
        if ([string]::IsNullOrWhiteSpace($cs)) { continue }

        # Suppression marker for genuinely decorative surfaces (slider tracks,
        # progress fills, indicator dots) where the bg is colour-only and the
        # element has no text descendants. Tailwind ignores the class.
        if ($cs -match '\bparity-no-text-pair\b') { continue }

        # Only consider DEFAULT-state surface bg (no prefix). Prefixed forms
        # like `hover:bg-accent`, `data-[state=checked]:bg-primary` only change
        # bg in a specific state — the default text colour cascade is unaffected.
        $tokens = $cs -split '\s+'
        $surfaceTokens = @()
        foreach ($tok in $tokens) {
            if ($tok -match ':') { continue }
            if ($tok -match $surfaceBgPattern) { $surfaceTokens += $tok }
        }

        if ($surfaceTokens.Count -eq 0) { continue }

        # ANY text-* token (prefixed or not) signals the developer thought about
        # text colour and is enough to clear the flag.
        $hasText = $false
        foreach ($tok in $tokens) {
            $bare = ($tok -split ':')[-1]
            if ($bare -match '^text-[a-z]') { $hasText = $true; break }
        }

        if ($hasText) { continue }

        # Flag — record the first surface token found and a snippet
        $snippet = if ($cs.Length -gt 100) { $cs.Substring(0, 100) + "..." } else { $cs }
        $bgTextFindings.Add([pscustomobject]@{
            Component = $fileName
            Surface   = $surfaceTokens[0]
            Snippet   = $snippet
        })
    }
}

# ---- Check 5 — Fixed-shade bg paired with flipping text token ---------------
#
# Hardcoded fixed-shade backgrounds (bg-gray-700, bg-zinc-900, bg-black, etc.)
# do NOT flip with dark/light mode — they're literal colours. Pairing one with
# a flipping text token (text-foreground, text-inverse-text, text-{thing}-fg)
# means the text colour changes between modes while the bg doesn't. In one mode
# the contrast vanishes and the text becomes invisible.
#
# Tooltip is the canonical example: surface is `bg-gray-700` in both modes;
# original code used `text-inverse-text` which evaluates to dark in dark mode
# → invisible black-on-grey-700 text in dark mode.
#
# Use a literal text colour instead (`text-white`, `text-{shade}-50/100/200`),
# or suppress with `parity-no-text-pair` if the element has no rendered text.

Write-Host "[5/6] Checking fixed-shade backgrounds for flipping text-token mismatches." -ForegroundColor Cyan

$fixedBgFindings = [System.Collections.Generic.List[pscustomobject]]::new()

# Fixed-shade dark surfaces — these never flip in dark mode.
# Includes: bg-{gray|zinc|slate|stone|neutral}-{700-950}, bg-black,
# and bg-{primary|danger|success|warning|info}-{500-900} (the literal palette
# entries, not the semantic -fg/-bg tokens which we treat in Check 4).
$fixedDarkBgPattern = '\bbg-(?:(?:gray|zinc|slate|stone|neutral)-(?:7|8|9)\d{2}|black|(?:primary|danger|success|warning|info)-(?:5|6|7|8|9)\d{2})(?![a-z0-9_-])'

# Flipping text tokens — these resolve to different colours in light vs dark.
# `text-foreground`, `text-inverse-text`, `text-card-foreground`,
# `text-popover-foreground`, `text-{thing}-fg` (alpha-foreground tokens),
# `text-muted-foreground`, `text-accent-foreground`, `text-secondary-foreground`,
# `text-primary-foreground`, `text-destructive-foreground`.
$flippingTextPattern = '\btext-(?:foreground|inverse-text|card-foreground|popover-foreground|muted-foreground|accent-foreground|secondary-foreground|primary-foreground|destructive-foreground|(?:primary|danger|success|warning|info|neutral)-fg)(?![a-z0-9_-])'

foreach ($file in $razorFiles) {
    $fileName = Split-Path $file -Leaf
    $classStrings = Get-RazorClassStrings -Path $file

    foreach ($cs in $classStrings) {
        if ([string]::IsNullOrWhiteSpace($cs)) { continue }

        # Suppression marker (shared with Check 4) — element has no rendered text
        # or text is set in a different chain.
        if ($cs -match '\bparity-no-text-pair\b') { continue }

        # Find unprefixed fixed-shade dark bg in this class string
        $tokens = $cs -split '\s+'
        $fixedBgs = @()
        $flipping = @()
        foreach ($tok in $tokens) {
            if ($tok -match ':') { continue }
            if ($tok -match $fixedDarkBgPattern) { $fixedBgs += $tok }
            if ($tok -match $flippingTextPattern) { $flipping += $tok }
        }

        if ($fixedBgs.Count -eq 0 -or $flipping.Count -eq 0) { continue }

        # Both present — flag the mismatch
        $snippet = if ($cs.Length -gt 100) { $cs.Substring(0, 100) + "..." } else { $cs }
        $fixedBgFindings.Add([pscustomobject]@{
            Component = $fileName
            FixedBg   = $fixedBgs[0]
            FlipText  = $flipping[0]
            Snippet   = $snippet
        })
    }
}

# ---- Check 6 — Token light/dark symmetry ------------------------------------
#
# Tokens defined in colors.css using `var(--color-blackAlpha-N)` are subtle
# dark-on-light shades. They render as nearly-invisible against the dark page
# bg unless `globals.css`'s `.dark { }` block redefines them to use
# `var(--color-whiteAlpha-N)` (subtle light-on-dark equivalent).
#
# Skeleton's `bg-neutral-50` was the trigger: defined as blackAlpha-50 in
# light, never redefined in dark, so the placeholder rendered invisibly
# against the dark page bg.
#
# This check parses both files and flags any `--color-{name}-{N}` token
# defined as `var(--color-blackAlpha-*)` in colors.css that has no
# corresponding non-blackAlpha definition in globals.css's `.dark { }` block.
# Same in reverse for whiteAlpha-* tokens (less common but worth catching).

Write-Host "[6/6] Checking token light/dark symmetry." -ForegroundColor Cyan

$tokenSymmetryFindings = [System.Collections.Generic.List[pscustomobject]]::new()
$colorsCssPath = Join-Path $repoRoot "PINGWorks.SitecoreBlok.BlazorUI/wwwroot/css/blok/colors.css"
$globalsCssPath = Join-Path $repoRoot "PINGWorks.SitecoreBlok.BlazorUI/wwwroot/css/blok/globals.css"

if ((Test-Path $colorsCssPath) -and (Test-Path $globalsCssPath)) {
    $colorsContent = Get-Content -Path $colorsCssPath -Raw
    $globalsContent = Get-Content -Path $globalsCssPath -Raw

    # Extract the `.dark { ... }` block from globals.css.
    $darkBlockMatch = [regex]::Match($globalsContent, '\.dark\s*\{([^{}]|\{[^{}]*\})*\}', [System.Text.RegularExpressions.RegexOptions]::Singleline)
    $darkBlock = if ($darkBlockMatch.Success) { $darkBlockMatch.Value } else { '' }

    # Find every `--color-{name}-{N}: var(--color-{black|white}Alpha-N)` definition in colors.css.
    $alphaTokenPattern = '--color-([a-zA-Z]+-\d+|[a-zA-Z]+):\s*var\(--color-(blackAlpha|whiteAlpha)-(\d+)\)'
    $colorsMatches = [regex]::Matches($colorsContent, $alphaTokenPattern)

    foreach ($m in $colorsMatches) {
        $tokenName = $m.Groups[1].Value          # e.g. "neutral-50"
        $alphaSide = $m.Groups[2].Value          # "blackAlpha" or "whiteAlpha"
        $alphaNum  = $m.Groups[3].Value          # "50"

        # Look for `--color-{tokenName}:` redefinition in the .dark block.
        # Any redefinition counts (doesn't have to be the inverse alpha — could be
        # a non-alpha colour). What we're catching is the ABSENCE of any override.
        $darkOverridePattern = '--color-' + [regex]::Escape($tokenName) + '\s*:'
        if ($darkBlock -match $darkOverridePattern) { continue }

        # No dark override. Check if the token is actually USED in any razor file
        # as a `bg-{tokenName}` / `text-{tokenName}` / `border-{tokenName}`
        # utility — if not, it doesn't matter visually.
        $usagePattern = "\b(?:bg|text|border|ring|fill|stroke|placeholder|outline|accent|divide|caret|decoration)-$([regex]::Escape($tokenName))\b"
        $isUsed = $false
        foreach ($file in $razorFiles) {
            $fileContent = Get-Content -Path $file -Raw -ErrorAction SilentlyContinue
            if ($fileContent -and $fileContent -match $usagePattern) { $isUsed = $true; break }
        }
        if (-not $isUsed) { continue }

        $oppositeSide = if ($alphaSide -eq 'blackAlpha') { 'whiteAlpha' } else { 'blackAlpha' }
        $tokenSymmetryFindings.Add([pscustomobject]@{
            Token         = "--color-$tokenName"
            LightDef      = "var(--color-$alphaSide-$alphaNum)"
            SuggestedDark = "var(--color-$oppositeSide-$alphaNum)"
        })
    }
}

# ---- Report ------------------------------------------------------------------

$reportDir = Split-Path $fullReportPath -Parent
if (-not (Test-Path $reportDir)) { New-Item -ItemType Directory -Path $reportDir -Force | Out-Null }

$sb = [System.Text.StringBuilder]::new()
[void]$sb.AppendLine("# UI Parity Report")
[void]$sb.AppendLine("")
[void]$sb.AppendLine("Generated: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')  ")
[void]$sb.AppendLine("Scope: $($razorFiles.Count) component(s)")
[void]$sb.AppendLine("")
[void]$sb.AppendLine("## Check 1 — Compiled-utility coverage")
[void]$sb.AppendLine("")
if ($missingUtilities.Count -eq 0) {
    [void]$sb.AppendLine("No missing utilities.")
} else {
    [void]$sb.AppendLine("| Component | Token missing from compiled CSS |")
    [void]$sb.AppendLine("|-----------|----------------------------------|")
    foreach ($m in $missingUtilities) { [void]$sb.AppendLine("| $($m.Component) | ``$($m.Token)`` |") }
}
[void]$sb.AppendLine("")
[void]$sb.AppendLine("## Check 2 — Runtime-composed class detection")
[void]$sb.AppendLine("")
if ($composedFindings.Count -eq 0) {
    [void]$sb.AppendLine("No runtime-composed class names detected.")
} else {
    [void]$sb.AppendLine("| Component | Line | Snippet |")
    [void]$sb.AppendLine("|-----------|------|---------|")
    foreach ($c in $composedFindings) { [void]$sb.AppendLine("| $($c.Component) | $($c.Line) | ``$($c.Snippet)`` |") }
}
[void]$sb.AppendLine("")
[void]$sb.AppendLine("## Check 3 — Blok class-string drift")
[void]$sb.AppendLine("")
if ($SkipDrift) {
    [void]$sb.AppendLine("Skipped (--SkipDrift).")
} elseif ($driftFindings.Count -eq 0) {
    [void]$sb.AppendLine("No drift detected.")
} else {
    [void]$sb.AppendLine("| Component | In Blok | In ours |")
    [void]$sb.AppendLine("|-----------|---------|---------|")
    foreach ($d in $driftFindings) { [void]$sb.AppendLine("| $($d.Component) | ``$($d.InBlok)`` | $($d.InOurs) |") }
}
[void]$sb.AppendLine("")
[void]$sb.AppendLine("## Check 4 — Surface background without paired text token")
[void]$sb.AppendLine("")
[void]$sb.AppendLine("Theme-aware surface backgrounds (``bg-background``, ``bg-card``, ``bg-popover``, ``bg-muted``, ``bg-accent``, ``bg-primary``, ``bg-secondary``, ``bg-destructive``) flip colour in dark mode. Without an explicit ``text-*`` token in the same class string the foreground relies on cascade — which silently breaks for fixed-positioned or portal-rendered content. Pair the surface bg with its matching text token (e.g. ``bg-background text-foreground``, ``bg-card text-card-foreground``, ``bg-primary text-white``).")
[void]$sb.AppendLine("")
if ($bgTextFindings.Count -eq 0) {
    [void]$sb.AppendLine("No unpaired surface backgrounds found.")
} else {
    [void]$sb.AppendLine("| Component | Surface bg | Snippet |")
    [void]$sb.AppendLine("|-----------|------------|---------|")
    foreach ($b in $bgTextFindings) { [void]$sb.AppendLine("| $($b.Component) | ``$($b.Surface)`` | ``$($b.Snippet)`` |") }
}
[void]$sb.AppendLine("")
[void]$sb.AppendLine("## Check 5 — Fixed-shade background with flipping text token")
[void]$sb.AppendLine("")
[void]$sb.AppendLine("Hardcoded fixed-shade backgrounds (``bg-gray-700``, ``bg-black``, ``bg-{color}-{500-900}``) do not flip with dark mode. Pairing them with a flipping text token (``text-foreground``, ``text-inverse-text``, ``text-*-fg``) means the text changes colour between modes while the surface stays put — producing invisible text in one mode. Use a literal text colour instead (``text-white``, ``text-{shade}-50/100/200``), or suppress with ``parity-no-text-pair`` if the element has no rendered text.")
[void]$sb.AppendLine("")
if ($fixedBgFindings.Count -eq 0) {
    [void]$sb.AppendLine("No fixed-shade / flipping-text mismatches found.")
} else {
    [void]$sb.AppendLine("| Component | Fixed bg | Flipping text | Snippet |")
    [void]$sb.AppendLine("|-----------|----------|---------------|---------|")
    foreach ($f in $fixedBgFindings) { [void]$sb.AppendLine("| $($f.Component) | ``$($f.FixedBg)`` | ``$($f.FlipText)`` | ``$($f.Snippet)`` |") }
}
[void]$sb.AppendLine("")
[void]$sb.AppendLine("## Check 6 — Token light/dark symmetry")
[void]$sb.AppendLine("")
[void]$sb.AppendLine("Tokens defined as ``var(--color-blackAlpha-N)`` (or ``whiteAlpha-N``) in ``colors.css`` are alpha-based and tied to one theme. They MUST have a corresponding override in ``globals.css`` ``.dark { }`` block that flips them to the opposite alpha (or to a non-alpha colour) — otherwise they render as nearly invisible against the opposite-theme page background. Skeleton's ``bg-neutral-50`` was the trigger.")
[void]$sb.AppendLine("")
if ($tokenSymmetryFindings.Count -eq 0) {
    [void]$sb.AppendLine("No token-symmetry issues found.")
} else {
    [void]$sb.AppendLine("| Token | Light-mode definition | Suggested dark override |")
    [void]$sb.AppendLine("|-------|-----------------------|-------------------------|")
    foreach ($t in $tokenSymmetryFindings) { [void]$sb.AppendLine("| ``$($t.Token)`` | ``$($t.LightDef)`` | ``$($t.SuggestedDark)`` |") }
}
$sb.ToString() | Set-Content -Path $fullReportPath -Encoding UTF8

# ---- Summary to console ------------------------------------------------------

Write-Host ""
Write-Host "=== Summary ===" -ForegroundColor Yellow
Write-Host "Missing utilities         : $($missingUtilities.Count)"
Write-Host "Composed class hits       : $($composedFindings.Count)"
Write-Host "Drift findings            : $(if ($SkipDrift) { 'skipped' } else { $driftFindings.Count })"
Write-Host "Unpaired surface bg/text  : $($bgTextFindings.Count)"
Write-Host "Fixed-bg + flipping-text  : $($fixedBgFindings.Count)"
Write-Host "Token light/dark asymmetry: $($tokenSymmetryFindings.Count)"
Write-Host ""
Write-Host "Full report: $fullReportPath"

$totalIssues = $missingUtilities.Count + $composedFindings.Count + $bgTextFindings.Count + $fixedBgFindings.Count + $tokenSymmetryFindings.Count + $(if ($SkipDrift) { 0 } else { $driftFindings.Count })

if ($totalIssues -gt 0) {
    Write-Host ""
    Write-Host "FAIL — $totalIssues issue(s) found." -ForegroundColor Red
    exit 1
} else {
    Write-Host ""
    Write-Host "PASS — clean." -ForegroundColor Green
    exit 0
}
