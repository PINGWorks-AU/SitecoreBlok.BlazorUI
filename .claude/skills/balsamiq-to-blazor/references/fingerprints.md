# Blok UI Kit — Balsamiq fingerprint catalogue

Signatures of every rendering in the "Blok UI Kit - Template" Balsamiq project. Match clusters top-down: shells/pages → chunks → primitives → native controls. Colours are the primary signal; structure confirms.

## Palette semantics (hex → token → meaning)

| Hex | Token | Signals |
|---|---|---|
| `#6E3FFF` | primary-500 | Solid primary fill: buttons, active dots, solid Badge, switch track |
| `#5319E0` | primary-600 | Header bands, links, emphasised icons/active nav text |
| `#4715AF` | primary-700 | Text on primary-100 chips/rows |
| `#EAE7FF` | primary-100 | Ghost button bg, chips, active/selected rows, icon chips |
| `#D9D4FF` | primary-200 | Chip/tint borders, subtitle text on purple bands |
| `#B8A9FF` | primary-300 | Upload-zone borders, done-step connectors |
| `#F7F6FF` | primary-50 | Tinted bands: info Alert, BulkActionBar, Hero, StandaloneShell header |
| `#D92739` | danger-500 | Destructive buttons/icons/text |
| `#FFF5F4` / `#FFE4E2` / `#FFCCC8` / `#92001F` | danger ramp | Danger Alert / Badge bg / border / text |
| `#007F66` / `#006450` | success-500/600 | Success icons & text |
| `#E8FCF5` / `#BEF6E3` / `#8BEBD0` | success ramp | Success Alert/Badge bg + borders |
| `#E26E00` / `#BA5200` | warning | Warning icons |
| `#FFF6E7` / `#FDD291` / `#953D00` | warning ramp | Warning Alert/Badge bg / border / text |
| `#212121` | gray-900 | Headings; ActionBar floating pill fill |
| `#3B3B3B` | gray-700 | Tooltip chip fill |
| `#282828` | gray-800 | CodeViewer bg, FullScreenShell top bar |
| `#535353` / `#717171` / `#8E8E8E` | gray-600/500/400 | Body / muted / caption text |
| `#B5B5B5` | gray-300 | Disabled text, input borders, dotted placeholder borders |
| `#D8D8D8` | gray-200 | Card borders, separators, dialog backdrops |
| `#E9E9E9` | gray-100 | Dividers, skeleton blocks, inactive step chips |
| `#F7F7F7` | gray-50 | Subtle bg: sidebars, addons, footers, Kbd |

## Native Balsamiq controls → components

| Read `type` | Component | Notes |
|---|---|---|
| `Text Input` | `Input` / `TextField` | Bold small label above → the `TextField` wrapper; `- text -` = placeholder; dots = `PasswordField` (esp. with fa-eye) |
| `Search Box` | `SearchInput` / `SearchField` | |
| `ComboBox` | `Select` / `SelectField` | Open list shows items; with chips + filter search-box → `Combobox`/`ComboboxField` |
| `Checkbox` | `Checkbox` / `CheckboxField` | `choiceState`: normal/selected/indeterminate/disabled. Inside a `#F7F6FF` bar with "N selected" → part of `BulkActionBar`, not a Checkbox |
| `RadioButton` | `RadioGroup` item / `RadioGroupField` | |
| `Switch` | `Switch` / `SwitchField` | `isOn`; renderer draws green regardless of colour — ignore fill |
| `HSlider` | `Slider` / `SliderField` | |
| `Progress Bar` | `Progress` | Thin style exists |
| `DataGrid` | `Table` | Header `^`/`v` = sortable; first col `[ ]`/`[x]` = selection column; coloured status cells = Badge-like cell content |
| `ButtonBar` | `Tabs` / `ToggleGroup` | Text labels + panel below → `Tabs`; icon glyphs → `ToggleGroup`/`ToggleGroupField`; small icon pair in a toolbar → view switcher inside `DataToolbar` |
| `Button` | `Button` | Fill `#6E3FFF` → Default(Primary); white fill + grey text → Ghost/Outline(Neutral); white + `#5319E0` text → Outline(Primary); white + `#D92739` text → Ghost/Outline(Danger); `#D92739` fill → Default(Danger); `#EAE7FF` fill + `#4715AF` text → Ghost(Primary); `#E9E9E9` fill + `#8E8E8E` text → Disabled |
| `Date Chooser` | `DatePicker` / `DateField` | |
| `Calendar` | `Calendar` | |
| `Icon` | `Icon` | Template uses Font Awesome; code uses MDI — map by meaning, not name |
| `Sticky Note`, `Arrow` | — | Annotations: intent, flow, forced identification (`Blok: X`) |

## Composed primitives

| Component | Fingerprint |
|---|---|
| Badge | Small rounded rect (~h25) + 11px text; tone pairs from palette ramps; solid `#6E3FFF`+white = solid variant |
| Avatar | `fa-circle-user` icon; or `#6E3FFF` rounded rect + 2-letter bold white initials; grey circle shape = image placeholder |
| Card | White rect, roundedSolid, border `#D8D8D8`: bold 14 title + 11 grey desc + divider + buttons |
| Textarea | Solid-border rect (`#B5B5B5`) with top-left grey placeholder text |
| Label / Field | Bold 11-12px label; red `*` = required; label + input + 10px help = Field (red border + red help = error state) |
| InputGroup | Input flanked by `#F7F7F7` bordered addon rects ("https://", "$", ".com") |
| Kbd | Tiny `#F7F7F7` rect, border `#B5B5B5`, short key text |
| CopyableToken | `#F7F7F7` rounded rect: token text + `fa-copy`; dark "Copied!" chip nearby |
| CodeViewer | `#282828` rounded rect with multi-colour code text lines |
| LocalTime | Plain datetime text + format-token caption (weak — context or sticky) |
| Carousel | chevron-left + large grey slide rect + partial next rect + chevron-right + dot row (one `#6E3FFF`) |
| AspectRatio | roundedDotted rect with ratio text ("16 : 9") |
| Alert | Full-width tinted rounded rect: tone icon + bold 12 title + 11 body (tone ramps above); trailing x = closeable |
| CircularProgress | `fa-circle-notch` `#6E3FFF` + bold % text |
| Spinner | `fa-spinner` icon(s) + "Loading…" |
| Skeleton | Borderless `#E9E9E9` bars/circles |
| Stepper | Row: `fa-circle-check` `#6E3FFF` + purple/grey numbered chips + labels + short hline connectors |
| Timeline | Column: 8-10px circles (purple = latest) + bold event + grey time, short vlines between |
| EmptyState | Centered `fa-inbox` `#B5B5B5` + bold title + grey text + button |
| ErrorState | Centered `fa-triangle-exclamation` `#D92739` (or big red "404") + title + retry/home button |
| Toaster / Sonner | White rounded card: status icon + bold title + grey text + x; shrinking stacked rects below = Sonner stack |
| Breadcrumb | Grey text with `›` separators, last segment dark (`BreadcrumbBar` when leading `fa-house`) |
| Pagination | Small button row `‹ 1 2 3 … n ›`, active solid `#6E3FFF` |
| Menubar | Bordered bar of File/Edit/View texts + dropdown panel with shortcut hints |
| NavigationMenu | Nav texts with chevron-down + flyout panel of icon+title+desc rows |
| Sidebar | `#F7F7F7` tall rect: brand row + `#EAE7FF` active item + grey icon items + footer user row |
| StackNavigation | Rail of icon-above-tiny-label items (vertical or horizontal), active purple |
| Navigation (Side) | ALL-CAPS 9px grey group captions + items + `#EAE7FF` active row (no brand) |
| Navigation (Stack) | Bordered list: `#F7F7F7` header row (chevron-left + bold title) + rows with trailing chevron-right |
| Dialog / AlertDialog | `#D8D8D8` backdrop rect containing white rounded card: title + x, body, right-aligned buttons; AlertDialog = no inputs + destructive confirm |
| Sheet | Grey content rect + white bordered edge panel: title + x, content, Close |
| Popover | Trigger button + white bordered rounded panel adjacent |
| Tooltip | `#3B3B3B` rounded chip + white 11px text beside a control |
| HoverCard | Underlined purple @handle + white card: avatar + name + bio + stats |
| DropdownMenu / ContextMenu | White bordered rounded panel of icon+label rows, divider, red destructive row; ContextMenu adds shortcut hints + dotted "Right-click area" rect; on a table row via `fa-ellipsis-vertical` → `RowActions` |
| Accordion | Bold question rows + chevron-up/down + body text, `#E9E9E9` dividers |
| Collapsible | Single chevron-down/right + bold label + indented content |
| ActionBar | Dark `#212121` floating pill: white "N selected" + white icons + `#535353` vlines |
| Resizable | Two panes + 14-16px `#E9E9E9` grip rect with `fa-grip-lines-vertical` |
| ScrollArea | Bordered rect with right-edge `#B5B5B5` thumb bar |
| Stack | Diagram of identical `#EAE7FF` rects in row/column (layout helper — usually becomes flex markup, not a component) |
| TreeView | Indented rows (≈20px steps): chevron + folder/file icons, `#EAE7FF` selected row |
| ThemeToggle | `fa-sun` + switch + `fa-moon` |

## Composed chunks

| Component | Fingerprint |
|---|---|
| ActionCard | Card row: leading icon + title/desc + trailing `fa-arrow-right` `#5319E0` |
| Callout | Single tinted aside with `fa-lightbulb`-style icon + short tip (vs Alert: see disambiguation) |
| CardGrid | Grid of empty grey rounded rects |
| Container | Grey outer rect + centered dotted `#EAE7FF` "MaxWidth" rect |
| ContentSection | Bold 16 heading + grey sub + body slot |
| SectionHeader | Bold 14 + count badge + trailing ghost button + hline |
| ElevatedCard | Empty white rects labelled Outline / Elevation |
| Empty/Error/Loading + Panel/View | Panel: grey rounded rect with inbox/warning/spinner content; View: mini browser frame variant |
| FeatureCard | Card with `#EAE7FF` icon chip top + bold title + grey desc |
| Hero | `#F7F6FF` wide band: 24px bold headline + sub + button pair + media rect |
| KpiTile | Grey 10-11px label + 24-28px bold number + green/red delta text |
| MetricGroup | 2+ KpiTiles separated by `#E9E9E9` vlines |
| MediaCard | Card with top image-area rect (`fa-image`) + title + action buttons |
| PageContent | Stacked dotted rects labelled PageHeader/ContentSection |
| SkeletonCard | Card of grey blocks |
| StatCard | Card row: number block + mini bar-sparkline rects in purple ramp |
| BulkActionBar | `#F7F6FF` inline band, border `#D9D4FF`: selected Checkbox "N selected" + right-aligned action buttons |
| DataToolbar | Row: search-box + closed ComboBox filters + icon ButtonBar view switch + primary button |
| DataPagination | "Showing X–Y of Z" + pager buttons |
| EmptyTable | DataGrid header row + bordered empty rect with inbox icon + clear link |
| FilterChip / FilterChipBar | `#EAE7FF` chip "Label: value ×"; bar = chips + dotted "+ Add filter" chip + "Clear all" link |
| KvList | Rows: fixed-width grey label + value, faint dividers |
| ResultsList | Bordered list, `#EAE7FF` selected row (bold title + meta) |
| SettingsPage | Title + left tab column (`#EAE7FF` active) + vline + form content |
| DetailPage | BackLink + title + status badge + actions; main slot + aside Details/Activity cards |
| DataPage | PageHeader + DataToolbar + BulkActionBar + DataGrid + DataPagination |
| Field wrappers (TextField, SelectField, DateField, TimeField, SearchField, PasswordField, CheckboxField, SwitchField, ToggleField, RadioGroupField, SliderField, ToggleGroupField, ComboboxField) | Bold 11-12px label above the matching native control (+ optional 10px help); see native table |
| FormLabel | Standalone label styles: plain / red `*` / grey "(optional)" |
| FormGrid | 2-column grid of label+input pairs |
| FormSection | Bold 14 title + grey desc + stacked fields |
| FormActions | `#F7F7F7` strip: "Last saved…" + Cancel / Save draft (`#EAE7FF`) / Publish (`#6E3FFF`) |
| InlineForm | Single row: flex-grow input + primary button |
| LoginForm | Card: "Sign in" + email + password + full-width primary button + centre link |
| ConfirmDialog | Backdrop dialog: warn icon + question title + Cancel + destructive confirm |
| FileUpload | Dotted `#B8A9FF` drop zone: `fa-cloud-arrow-up` + "Drag & drop … browse" + file row with thin progress |
| SearchBar | search-box + filter ComboBox + "N results" text |
| AppHeader | Bar: `fa-cubes` brand + nav texts (one purple) + search-box + `fa-bell` + avatar |
| AnnouncementBar | Solid `#5319E0` band: white icon + message + underlined "Learn more" + x |
| SubHeader | `#F7F7F7` strip: env ComboBox + vline + breadcrumb text + right meta |
| PageHeader | Breadcrumb line + 20px bold title + grey desc + right button pair |
| Toolbar | Bordered strip: format icon groups + vlines + trailing ComboBox |
| AppBrand | `#6E3FFF` logo chip (`fa-cube`) + bold name + `#F7F7F7` version chip |
| AccountMenu | Avatar trigger + menu with name/email header row + items + red sign-out |
| AppSidebar | Sidebar + ALL-CAPS group captions + brand + footer (vs plain Sidebar) |
| BackLink | `fa-arrow-left` + bold purple text |
| NavGroup | ALL-CAPS caption + chevron + indented items |
| NavList / NavListItem | Rows icon + label + optional solid-purple count badge, `#EAE7FF` active |
| NavRail | Icon-only `#F7F7F7` rail, `#EAE7FF` active pill |
| TabBar | Text tabs (active purple, counts in labels) + 3px `#6E3FFF` underline rect + hline |

## Shells & pages (macro-structure)

| Shell | Structure |
|---|---|
| AppShell | Header bar + left sidebar + content area (+ bottom-right toast) |
| PageShell | Breadcrumb/title header + dotted Body + dotted Aside column + dotted Footer strip |
| SplitShell / ListDetailShell | Two panes + grip divider; list w/ selected row + detail pane = ListDetail |
| CenteredShell | Grey full-bleed frame + centered logo + card column |
| BlankShell | Bare frame + single dotted ChildContent slot |
| DialogShell / SheetShell | Backdrop + dialog (or edge panel) whose header/body/footer are labelled slots |
| FormShell | Title + FormSections + `#F7F7F7` sticky footer bar |
| WizardShell | Title + Stepper + step fields + Back / "Step n of m" / Next footer |
| ContextPanelShell | Left panel (title + slot) + grey "Page Builder canvas" |
| CustomFieldShell | Backdrop dialog: "Edit field" + slot + Cancel/Save |
| DashboardWidgetShell | Widget card: title + meta + ellipsis + chart/body |
| FullScreenShell | `#282828` top bar (`fa-cubes` + "XM Cloud") + full dotted iframe slot |
| StandaloneShell | `#F7F6FF` portal bar (`fa-cloud` + "Sitecore Cloud Portal") + dotted slot |

## Disambiguation

- **Badge vs FilterChip vs Combobox chip:** FilterChip text is "Label: value" + trailing `fa-xmark`; Combobox chips sit in/near an input with a filter search-box; otherwise Badge.
- **Alert vs Callout vs Toast:** full-width tinted band in page flow = Alert; tip-style aside (lightbulb) = Callout; white floating card with status icon (often stacked) = Toast/Sonner.
- **ActionBar vs BulkActionBar:** dark `#212121` pill = ActionBar (or `BulkActionBar Popup=true`); tinted `#F7F6FF` inline band with count = BulkActionBar.
- **Tabs vs TabBar vs ToggleGroup:** ButtonBar with text labels (+ panel) = Tabs; plain text row + purple underline = TabBar; ButtonBar with icons = ToggleGroup; icon pair in a toolbar = DataToolbar view switch.
- **Sidebar vs AppSidebar vs Navigation (Side) vs NavList:** brand+footer = Sidebar; + group captions = AppSidebar; captions only = Navigation (Side); bare rows = NavList.
- **Card family:** trailing arrow = ActionCard; icon chip top = FeatureCard; image header = MediaCard; big number = StatCard/KpiTile; otherwise Card.
- A cluster matching a page fingerprint (DataPage, FormShell…) generates the chunk composition, not its parts individually.
