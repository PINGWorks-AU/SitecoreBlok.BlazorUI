---
description: Migrate, update, audit, or verify Sitecore Blok components in BlazorUI
argument-hint: [migrate|update|audit|catalogue|verify] [component|all]
---

Invoke the `blok-migration` skill with the Skill tool, passing these arguments verbatim:

$ARGUMENTS

`.claude/skills/blok-migration/SKILL.md` is the single source of truth for this workflow. Follow its phases as written — do not improvise a shorter path, and do not skip the parity harness or the verify flow.

Argument forms:

- `migrate <component>` — port a new primitive from the Blok registry (Phases 1-4, then the verify flow)
- `update <component>` — re-audit an existing component against current Blok source
- `audit` — diff `MIGRATION_STATUS.md` against Blok `main`; report changed rows and new upstream primitives
- `catalogue <component>` — create or update the Catalogue page only
- `verify <component>` — run `tools/verify-ui-parity.ps1` scoped to one component, then fix findings
- `verify all` — run the harness across every primitive, in the batches the skill defines

With no arguments, run the `audit` flow.
