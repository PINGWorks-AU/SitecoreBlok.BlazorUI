<#
.SYNOPSIS
    Generates icon-metadata.json and icon-categories.json for the Catalogue's
    icon search feature.

.DESCRIPTION
    Reads the full Material Design Icons meta file (published by Pictogrammers
    for every MDI release) and emits two JSON files into the Catalogue's
    wwwroot folder:

      1. icon-metadata.json   — a flat object keyed by the C# property name
         used in PINGWorks.SitecoreBlok.BlazorUI.Icons.IconSvg. Each entry
         preserves the original kebab-case MDI name, the "al" array as
         "seeAlso", and the "st" array as "categories" so the search can
         match against synonyms and tag-like terms rather than property names
         alone.

      2. icon-categories.json — the distinct list of every category that
         appears across the icon set, each with a count of how many icons
         carry that category. Sorted by count descending (alphabetical
         tiebreak) so the UI can surface the most populated categories
         first for filtering.

    The MDI source JSON is NOT committed — it's several MB and changes only
    when we bump the MDI version. Download the meta JSON for the version
    matching IconSvg.cs (currently 7.4.47) from the Pictogrammers MDI package
    or GitHub release and drop it at the default Source path (or pass -Source
    explicitly).

.PARAMETER Source
    Path to the MDI meta JSON (the file with a top-level "i" array of icons).
    Defaults to <script folder>\mdi-source.json.

.PARAMETER Destination
    Path to write the per-icon metadata. Defaults to the Catalogue project's
    wwwroot so the file is served as a static asset.

.PARAMETER CategoriesDestination
    Path to write the category rollup. Defaults to icon-categories.json in
    the same folder as Destination.

.EXAMPLE
    ./build-icon-metadata.ps1

.EXAMPLE
    ./build-icon-metadata.ps1 -Source C:\Temp\mdi-7.4.47.json
#>

param(
    [string]$Source = (Join-Path $PSScriptRoot 'mdi-source.json'),
    [string]$Destination = (Join-Path $PSScriptRoot '..\PINGWorks.SitecoreBlok.BlazorUI.Catalogue\wwwroot\icon-metadata.json'),
    [string]$CategoriesDestination = (Join-Path $PSScriptRoot '..\PINGWorks.SitecoreBlok.BlazorUI.Catalogue\wwwroot\icon-categories.json')
)

$ErrorActionPreference = 'Stop'

if (-not (Test-Path $Source)) {
    Write-Error @"
MDI source JSON not found at: $Source

Download the MDI meta JSON (matching the version used in IconSvg.cs) from
https://pictogrammers.com/ or the @mdi/svg npm package's meta.json, then
either place it at the path above or re-run with -Source <path>.
"@
    exit 1
}

function ConvertTo-PascalCase {
    param([string]$KebabName)
    $sb = [System.Text.StringBuilder]::new()
    foreach ($part in ($KebabName -split '-')) {
        if ($part.Length -eq 0) { continue }
        [void]$sb.Append([char]::ToUpperInvariant($part[0]))
        if ($part.Length -gt 1) { [void]$sb.Append($part.Substring(1)) }
    }
    return $sb.ToString()
}

Write-Host "Reading $Source..."
$data = Get-Content -Path $Source -Raw | ConvertFrom-Json

Write-Host "Found $($data.i.Count) icons. Building map..."
$result = [ordered]@{}
$categoryCounts = @{}
$dupes = 0
foreach ($icon in $data.i) {
    $key = ConvertTo-PascalCase -KebabName $icon.n
    if ($result.Contains($key)) {
        $dupes++
        Write-Warning "Duplicate key '$key' for '$($icon.n)' — skipping."
        continue
    }
    $categories = @($icon.st)
    $result[$key] = [ordered]@{
        mdi        = $icon.n
        seeAlso    = @($icon.al)
        categories = $categories
    }

    # Dedupe within a single icon so an accidental repeat can't inflate counts.
    foreach ($cat in ($categories | Select-Object -Unique)) {
        if ($categoryCounts.ContainsKey($cat)) {
            $categoryCounts[$cat]++
        } else {
            $categoryCounts[$cat] = 1
        }
    }
}

# Sort: count desc, then name asc. Emit as array of { name, count } so the
# order is preserved on the wire and the UI can render directly.
$categoriesOutput = @(
    $categoryCounts.GetEnumerator() |
        Sort-Object @{ Expression = 'Value'; Descending = $true }, @{ Expression = 'Key'; Descending = $false } |
        ForEach-Object { [ordered]@{ name = $_.Key; count = $_.Value } }
)

foreach ($path in @($Destination, $CategoriesDestination)) {
    $dir = Split-Path -Parent $path
    if (-not (Test-Path $dir)) { New-Item -ItemType Directory -Path $dir -Force | Out-Null }
}

Write-Host "Writing $Destination..."
$metaJson = $result | ConvertTo-Json -Depth 5
[System.IO.File]::WriteAllText($Destination, $metaJson, [System.Text.UTF8Encoding]::new($false))

Write-Host "Writing $CategoriesDestination..."
$categoriesJson = ConvertTo-Json -InputObject $categoriesOutput -Depth 3
[System.IO.File]::WriteAllText($CategoriesDestination, $categoriesJson, [System.Text.UTF8Encoding]::new($false))

$metaKb = [math]::Round((Get-Item $Destination).Length / 1KB, 1)
$catsKb = [math]::Round((Get-Item $CategoriesDestination).Length / 1KB, 1)
Write-Host "Done. Icons: $($result.Count) ($metaKb KB). Categories: $($categoriesOutput.Count) ($catsKb KB). Duplicates skipped: $dupes"
