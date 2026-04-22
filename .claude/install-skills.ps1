<#
.SYNOPSIS
    Installs the repo's Claude Code skills into the current user's skills directory.

.DESCRIPTION
    Copies every skill under .claude/skills/ in this repo to ~/.claude/skills/ on the
    local machine, so Claude Code picks them up on the next session. Existing skill
    directories of the same name are overwritten.

    Cross-platform (PowerShell 7+). On Windows, ~/.claude resolves to
    %USERPROFILE%\.claude. On macOS/Linux it resolves to $HOME/.claude.

.EXAMPLE
    pwsh ./.claude/install-skills.ps1
#>

[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'

$repoRoot      = Split-Path $PSScriptRoot -Parent
$repoSkillsDir = Join-Path $repoRoot '.claude/skills'
$userSkillsDir = Join-Path $HOME '.claude/skills'

if (-not (Test-Path $repoSkillsDir)) {
    Write-Error "No skills found at $repoSkillsDir"
    exit 1
}

if (-not (Test-Path $userSkillsDir)) {
    New-Item -ItemType Directory -Path $userSkillsDir -Force | Out-Null
}

$installed = @()
Get-ChildItem -Path $repoSkillsDir -Directory | ForEach-Object {
    $source = $_.FullName
    $target = Join-Path $userSkillsDir $_.Name

    if (Test-Path $target) { Remove-Item -Path $target -Recurse -Force }

    Copy-Item -Path $source -Destination $target -Recurse
    $installed += $_.Name
    Write-Host "Installed skill: $($_.Name) -> $target" -ForegroundColor Green
}

if ($installed.Count -eq 0) {
    Write-Host "No skill directories found under $repoSkillsDir" -ForegroundColor Yellow
} else {
    Write-Host ""
    Write-Host "Installed $($installed.Count) skill(s). Restart your Claude Code session to pick them up." -ForegroundColor Cyan
}
