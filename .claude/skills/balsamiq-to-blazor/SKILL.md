---
name: balsamiq-to-blazor
description: Use when reading a Balsamiq board or wireframe (via the Balsamiq MCP) to identify components or generate Blazor/Razor code — prototypes composed from the "Blok UI Kit - Template" project using PINGWorks.SitecoreBlok.BlazorUI. Triggers: "generate code from my board", "read my Balsamiq prototype", "board to Blazor", "what components are on this board", Blok wireframe mapping.
---

# Balsamiq → Blazor (Blok UI Kit)

## Overview

Prototypes are composed in Balsamiq by copying renderings from the **"Blok UI Kit - Template"** project. Component/symbol names do **not** survive the MCP read — identification works by **fingerprint**: every template rendering uses the Blok token palette and a known structure. The full catalogue is in [references/fingerprints.md](references/fingerprints.md).

## Workflow

1. **Fetch.** `list_balsamiq_projects` → `get_balsamiq_project_toc` → `get_balsamiq_board_content`; keep calling with `cursor` until no `nextCursor`. Use `get_balsamiq_board_preview` as a visual cross-check.
2. **Segment.** Controls whose ids share a first segment (`7-0`, `7-1-2`) are one pasted component instance. Plain ids are loose controls — segment by containment (`children`), then proximity.
3. **Identify — highest altitude first.** Match each cluster against **chunks and shells before primitives** (fingerprints reference). If your pick needs several "wireframe vs runtime mismatch" explanations, you picked too low — re-match one level up.
4. **Extract.** Text = user content overrides (labels, ChildContent). Colour = variant/tone via the palette table. Control state (`choiceState`, `isOn`, `selectedIndex`) = component state.
5. **Verify APIs, then generate.** Read the component source (`PINGWorks.SitecoreBlok.BlazorUI/Components/**`, `Enums.cs`) or the packaged manifest (`components.json`, `llms.txt`); Catalogue pages show usage. This kit extends Blok — e.g. `Button` adds `Title`, `OnClick`, `StartIconSvg`, nullable `ColorScheme` with variant-dependent defaults. Never emit a parameter you have not seen in source or manifest.
6. **Bind, don't hard-code.** Placeholder text like "0 selected" or "24,532" becomes a binding. Flag drawn states the real component would not render (e.g. `BulkActionBar` hides itself when `SelectedCount` is 0).

## Reading rules

| Signal in board JSON | Meaning |
|---|---|
| Sticky notes (esp. `Blok: X`) and arrows | Author intent / navigation flow — obey them, never render them |
| Dotted rect labelled "… slot" | ChildContent placeholder |
| White card + bold 15px name + 11px grey desc + `#E9E9E9` divider | Template sheet chrome accidentally copied — treat the name as a directive, not UI |
| Off-token colour (e.g. `#733FFB`) | User recolour — snap to the nearest Blok token |
| Sketch geometry, 11–13px fonts | Noise — never infer `Size` from pixels alone |

## Example (verified against a real board)

`#F7F6FF` band + border `#D9D4FF` + selected Checkbox " 0 selected" + Publish/Move/Delete buttons →

```razor
<BulkActionBar SelectedCount="@Selected.Count" OnClear="ClearSelection">
	<Actions>
		<Button Size="ButtonSize.Sm" Title="Publish" OnClick="PublishSelected" />
		<Button Variant="ButtonVariant.Ghost" Size="ButtonSize.Sm" Title="Move" OnClick="MoveSelected" />
		<Button Variant="ButtonVariant.Ghost" ColorScheme="ButtonColor.Danger" Size="ButtonSize.Sm" Title="Delete" OnClick="DeleteSelected" />
	</Actions>
</BulkActionBar>
```

The drawn checkbox is not a `Checkbox` — it is the chunk's count affordance. The dark `#212121` floating pill is the `ActionBar` primitive instead; the tinted inline band is always `BulkActionBar`.

## Common mistakes

- Mapping to a primitive when a chunk matches the whole cluster (`ActionBar` vs `BulkActionBar`).
- Hard-coding override text that should be a binding.
- Emitting parameters from memory instead of reading source/manifest.
- Rendering sticky notes, arrows, or template chrome as UI.
- Stopping at page 1 of `get_balsamiq_board_content` when `nextCursor` is present.
