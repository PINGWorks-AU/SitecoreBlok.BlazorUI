# .claude/

Claude Code assets that ship with this repo.

```
.claude/
├── install-skills.ps1    # Install skills into ~/.claude/skills/ on Windows / PowerShell 7+
├── install-skills.sh     # Install skills into ~/.claude/skills/ on macOS / Linux
└── skills/
    └── blok-migration/   # Component migration, update, audit & UI parity verification skill
        └── SKILL.md
```

> Claude Code also reads a per-project `.claude/settings.local.json` for personal preferences (permissions, model overrides, etc.) if you create one. That file is gitignored and is not shipped by this repo or installed by the scripts below — create it locally if you need it.

## Installing the skills

From the repo root:

```powershell
# Windows (PowerShell 7+) — also works on macOS/Linux with PowerShell installed
pwsh ./.claude/install-skills.ps1
```

```bash
# macOS / Linux (bash)
./.claude/install-skills.sh
```

Each script copies every directory under `.claude/skills/` in this repo to `~/.claude/skills/` on your machine. Existing skills of the same name are overwritten. Restart your Claude Code session after running.

## What's included

### `blok-migration`

Migrate, update, audit, or verify Sitecore Blok design system components in this library. See the [AI Assisted Component Migration](../README.md#ai-assisted-component-migration) section of the main README for usage, trigger phrases, and the recommended contributor workflow.

Invokes the parity harness at `tools/verify-ui-parity.ps1` as part of its verification flow. The harness runs independently of the skill — you can invoke it directly from PowerShell if you don't want to go through Claude Code.
