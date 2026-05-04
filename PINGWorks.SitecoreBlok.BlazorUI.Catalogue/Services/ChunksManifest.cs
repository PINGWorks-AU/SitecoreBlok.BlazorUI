using PINGWorks.SitecoreBlok.BlazorUI.Catalogue.Components.Shared;

namespace PINGWorks.SitecoreBlok.BlazorUI.Catalogue.Services;

/// <summary>
/// Single source of truth for Chunk metadata in the Catalogue.
/// Chunks are not tracked in MIGRATION_STATUS.md (per spec §5.9), so this manifest
/// drives both the /chunks Index page and the ChunksNavMenu left-nav.
/// </summary>
public static class ChunksManifest
{
	public sealed record ChunkEntry(
		string Family,
		string Name,
		string Slug,
		string Description,
		ComponentInteractivity Interactivity );

	private const string LayoutsFamily = "Layouts";
	private const string HeadersFamily = "Headers";
	private const string NavFamily = "Navigation";
	private const string ContentFamily = "Content";
	private const string FormsFamily = "Forms";
	private const string DataFamily = "Data";
	private const string MarketplaceFamily = "Marketplace";

	public static readonly ChunkEntry[] All =
	[
		new( LayoutsFamily, "AppShell",        "app-shell",         "Outer dark-mode + popover/toaster mount",                  ComponentInteractivity.Interactive ),
		new( LayoutsFamily, "BlankShell",      "blank-shell",       "No-chrome envelope; sets background and font only",        ComponentInteractivity.Ssr ),
		new( LayoutsFamily, "CenteredShell",   "centered-shell",    "Single centered column for auth / error / empty states",   ComponentInteractivity.Ssr ),
		new( LayoutsFamily, "DialogShell",     "dialog-shell",      "Opinionated wrapper around the Dialog primitive",          ComponentInteractivity.Interactive ),
		new( LayoutsFamily, "ListDetailShell", "list-detail-shell", "Opinionated SplitShell with selection state",              ComponentInteractivity.Interactive ),
		new( LayoutsFamily, "PageShell",       "page-shell",        "In-app page envelope with header/body/aside/footer slots", ComponentInteractivity.Ssr ),
		new( LayoutsFamily, "SheetShell",      "sheet-shell",       "Edge-sliding panel; wraps Sheet primitive",                ComponentInteractivity.Interactive ),
		new( LayoutsFamily, "SplitShell",      "split-shell",       "Two-pane (master/detail) layout, optional resizable",      ComponentInteractivity.Ssr ),

		new( HeadersFamily, "AnnouncementBar", "announcement-bar", "Top-of-app dismissible banner — Message + Tone + optional Action",      ComponentInteractivity.Interactive ),
		new( HeadersFamily, "AppBrand",        "app-brand",        "Logo + product name + optional version chip",                           ComponentInteractivity.Ssr ),
		new( HeadersFamily, "AppHeader",       "app-header",       "Sticky top bar with backdrop blur — slots: Brand, Nav, Actions",        ComponentInteractivity.Ssr ),
		new( HeadersFamily, "PageHeader",      "page-header",      "Top of an in-page area: Title, Description, Breadcrumbs, Actions",      ComponentInteractivity.Ssr ),
		new( HeadersFamily, "SectionHeader",   "section-header",   "Smaller heading band inside a ContentSection",                          ComponentInteractivity.Ssr ),
		new( HeadersFamily, "SubHeader",       "sub-header",       "Context strip below AppHeader (env switcher, ambient breadcrumbs)",     ComponentInteractivity.Ssr ),
		new( HeadersFamily, "Toolbar",         "toolbar",          "Horizontal action strip — slots: Start, Center, End. Density enum",     ComponentInteractivity.Ssr ),

		new( NavFamily, "AccountMenu",   "account-menu",   "Avatar trigger + DropdownMenu shell — Name, Email, AvatarUrl, Initials", ComponentInteractivity.Interactive ),
		new( NavFamily, "AppSidebar",    "app-sidebar",    "Pre-wired sidebar with Brand / Nav / Footer slots",                      ComponentInteractivity.Ssr ),
		new( NavFamily, "BackLink",      "back-link",      "Single \"← Back to X\" element — Href, Label",                           ComponentInteractivity.Ssr ),
		new( NavFamily, "BreadcrumbBar", "breadcrumb-bar", "Declarative breadcrumb — Items: IList<BreadcrumbItem>",                  ComponentInteractivity.Ssr ),
		new( NavFamily, "NavGroup",      "nav-group",      "Labelled grouping inside a NavList — Label, Collapsible, DefaultOpen",   ComponentInteractivity.Interactive ),
		new( NavFamily, "NavList",       "nav-list",       "Vertical link list — ChildContent of NavListItem / NavGroup",            ComponentInteractivity.Ssr ),
		new( NavFamily, "NavListItem",   "nav-list-item",  "Single link row — Href, IconSvg, Label, Active, Badge, OnClick",         ComponentInteractivity.Ssr ),
		new( NavFamily, "NavRail",       "nav-rail",       "Narrow icon-only nav rail — Items: IList<NavRailItem>",                  ComponentInteractivity.Ssr ),
		new( NavFamily, "TabBar",        "tab-bar",        "Top-of-page tab navigation — Items: IList<TabDefinition>",               ComponentInteractivity.Interactive ),

		new( ContentFamily, "ActionCard",       "action-card",        "Card with click target + trailing arrow — Href / OnClick",         ComponentInteractivity.Ssr ),
		new( ContentFamily, "Callout",          "callout",            "Visually distinct aside — Title, Tone, IconSvg, ChildContent",     ComponentInteractivity.Ssr ),
		new( ContentFamily, "CardGrid",         "card-grid",          "Responsive grid of cards — Columns, Gap",                          ComponentInteractivity.Ssr ),
		new( ContentFamily, "Container",        "container",          "Max-width content centerer — MaxWidth: Size",                      ComponentInteractivity.Ssr ),
		new( ContentFamily, "ContentSection",   "content-section",    "Section header + body in standard padding/spacing",                ComponentInteractivity.Ssr ),
		new( ContentFamily, "ElevatedCard",     "elevated-card",      "Card primitive wrapper — Style / Elevation / HoverElevation",      ComponentInteractivity.Ssr ),
		new( ContentFamily, "EmptyStatePanel",  "empty-state-panel",  "EmptyState wrapped in section-level chrome",                       ComponentInteractivity.Ssr ),
		new( ContentFamily, "EmptyView",        "empty-view",         "Full-page empty state",                                            ComponentInteractivity.Ssr ),
		new( ContentFamily, "ErrorStatePanel",  "error-state-panel",  "ErrorState wrapped in section chrome — adds Status (e.g. 404)",    ComponentInteractivity.Ssr ),
		new( ContentFamily, "ErrorView",        "error-view",         "Full-page error",                                                  ComponentInteractivity.Ssr ),
		new( ContentFamily, "FeatureCard",      "feature-card",       "Icon + Title + Description card; for landing/onboarding",          ComponentInteractivity.Ssr ),
		new( ContentFamily, "Hero",             "hero",               "Landing-style intro band — Title, Subtitle, Actions, Media",       ComponentInteractivity.Ssr ),
		new( ContentFamily, "KpiTile",          "kpi-tile",           "Single big-number stat tile — Label, Value, Delta, Trend",         ComponentInteractivity.Ssr ),
		new( ContentFamily, "LoadingPanel",     "loading-panel",      "Spinner + optional message centered in min-height block",          ComponentInteractivity.Ssr ),
		new( ContentFamily, "LoadingView",      "loading-view",       "Full-page loading",                                                ComponentInteractivity.Ssr ),
		new( ContentFamily, "MediaCard",        "media-card",         "Thumbnail-first card — Image + Title/Description + Actions",       ComponentInteractivity.Ssr ),
		new( ContentFamily, "MetricGroup",      "metric-group",       "Horizontal arrangement of KpiTiles with dividers",                 ComponentInteractivity.Ssr ),
		new( ContentFamily, "PageContent",      "page-content",       "Vertical stack of sections with consistent gap",                   ComponentInteractivity.Ssr ),
		new( ContentFamily, "SkeletonCard",     "skeleton-card",      "Skeleton-of-a-card preset for grid loading",                       ComponentInteractivity.Ssr ),
		new( ContentFamily, "StatCard",         "stat-card",          "Card-based KPI with Sparkline + Actions slots",                    ComponentInteractivity.Ssr ),

		new( FormsFamily, "CheckboxField",    "checkbox-field",     "Inline checkbox + label-right field wrapper",                       ComponentInteractivity.Interactive ),
		new( FormsFamily, "ComboboxField",    "combobox-field",     "Combobox wrapper — Items, Value, Placeholder",                      ComponentInteractivity.Interactive ),
		new( FormsFamily, "ConfirmDialog",    "confirm-dialog",     "Dialog-based confirm-action wrapper — Tone, ConfirmLabel",          ComponentInteractivity.Interactive ),
		new( FormsFamily, "DateField",        "date-field",         "DatePicker wrapper — DateTime? Value",                              ComponentInteractivity.Interactive ),
		new( FormsFamily, "FileUpload",       "file-upload",        "Drop-zone file picker — click or drag-and-drop, IBrowserFile",      ComponentInteractivity.Interactive ),
		new( FormsFamily, "FilterBar",        "filter-bar",         "Horizontal filter chip row — Filters, ClearAction",                 ComponentInteractivity.Ssr ),
		new( FormsFamily, "FormActions",      "form-actions",       "Sticky-bottom action row — Start, End slots",                       ComponentInteractivity.Interactive ),
		new( FormsFamily, "FormGrid",         "form-grid",          "Multi-column field layout — Columns, Gap",                          ComponentInteractivity.Ssr ),
		new( FormsFamily, "FormLabel",        "form-label",         "Form-context wrapper around the Label primitive",                   ComponentInteractivity.Ssr ),
		new( FormsFamily, "FormSection",      "form-section",       "Labelled grouping of fields — Title, Description, ChildContent",    ComponentInteractivity.Ssr ),
		new( FormsFamily, "FormShell",        "form-shell",         "Page-level form envelope: PageHeader + Sections + FormActions",     ComponentInteractivity.Interactive ),
		new( FormsFamily, "InlineForm",       "inline-form",        "Single-row form for search-and-go / subscribe / quick-add",         ComponentInteractivity.Interactive ),
		new( FormsFamily, "LoginForm",        "login-form",         "Opinionated login template — Email/Password + Footer slot",         ComponentInteractivity.Interactive ),
		new( FormsFamily, "PasswordField",    "password-field",     "Password Input + show/hide toggle",                                 ComponentInteractivity.Interactive ),
		new( FormsFamily, "RadioGroupField",  "radio-group-field",  "Vertical radio group with label-above",                             ComponentInteractivity.Interactive ),
		new( FormsFamily, "SearchBar",        "search-bar",         "Toolbar pattern: SearchInput + filter slot + result-count",         ComponentInteractivity.Interactive ),
		new( FormsFamily, "SearchField",      "search-field",       "SearchInput wrapper — Query, Placeholder",                          ComponentInteractivity.Interactive ),
		new( FormsFamily, "SelectField",      "select-field",       "Select wrapper — Items, Value, Placeholder",                        ComponentInteractivity.Interactive ),
		new( FormsFamily, "SliderField",      "slider-field",       "Slider wrapper — Min, Max, Step, Value",                            ComponentInteractivity.Interactive ),
		new( FormsFamily, "SwitchField",      "switch-field",       "Inline switch + label-right field wrapper",                         ComponentInteractivity.Interactive ),
		new( FormsFamily, "TextAreaField",    "text-area-field",    "Textarea wrapper — Rows, Value",                                    ComponentInteractivity.Interactive ),
		new( FormsFamily, "TextField",        "text-field",         "Input wrapper — Type (Text/Email/Number/Tel/Url), Value",           ComponentInteractivity.Interactive ),
		new( FormsFamily, "TimeField",        "time-field",         "TimePicker wrapper — TimeSpan? Value",                              ComponentInteractivity.Interactive ),
		new( FormsFamily, "ToggleField",      "toggle-field",       "Inline toggle + label-right field wrapper",                         ComponentInteractivity.Interactive ),
		new( FormsFamily, "ToggleGroupField", "toggle-group-field", "Horizontal multi-button toggle group — Options",                    ComponentInteractivity.Interactive ),
		new( FormsFamily, "WizardShell",      "wizard-shell",       "Stepper + per-step body + Back/Next footer",                        ComponentInteractivity.Interactive ),

		new( DataFamily, "BulkActionBar",  "bulk-action-bar",  "Selection-driven action bar above a table",                           ComponentInteractivity.Interactive ),
		new( DataFamily, "DataPage",       "data-page",        "Full data-table page envelope — Toolbar, Table, Pagination slots",    ComponentInteractivity.Interactive ),
		new( DataFamily, "DataPagination", "data-pagination",  "Pagination row with item count + page nav",                           ComponentInteractivity.Interactive ),
		new( DataFamily, "DataToolbar",    "data-toolbar",     "Search + filters + view-switcher + actions row above a table",        ComponentInteractivity.Interactive ),
		new( DataFamily, "DetailPage",     "detail-page",      "Record detail layout — Title, Description, Main / Aside / Footer",    ComponentInteractivity.Ssr ),
		new( DataFamily, "EmptyTable",     "empty-table",      "Empty state inside a Table aware of column count",                    ComponentInteractivity.Ssr ),
		new( DataFamily, "FilterChip",     "filter-chip",      "Single removable filter chip — Label, OnRemove, IconSvg",             ComponentInteractivity.Interactive ),
		new( DataFamily, "KvList",         "kv-list",          "Label/value definition list — ChildContent of KvListItem",            ComponentInteractivity.Ssr ),
		new( DataFamily, "ResultsList",    "results-list",     "Generic vertical list with selection, optional pagination, Empty",    ComponentInteractivity.Interactive ),
		new( DataFamily, "RowActions",     "row-actions",      "Table-row dropdown action menu — Items slot",                         ComponentInteractivity.Interactive ),
		new( DataFamily, "SettingsPage",   "settings-page",    "Side-tabs + content area — Tabs, SelectedTab, Content slot",          ComponentInteractivity.Interactive ),

		new( MarketplaceFamily, "ContextPanelShell",    "context-panel-shell",    "XMC Page Builder left context panel shell",              ComponentInteractivity.Ssr ),
		new( MarketplaceFamily, "CustomFieldShell",     "custom-field-shell",     "XMC Page Builder Custom Field dialog shell",             ComponentInteractivity.Ssr ),
		new( MarketplaceFamily, "DashboardWidgetShell", "dashboard-widget-shell", "XMC Dashboard widget shell — Title, Header, Body",       ComponentInteractivity.Ssr ),
		new( MarketplaceFamily, "FullScreenShell",      "full-screen-shell",      "XMC Sites full-screen iframe shell — ChildContent",      ComponentInteractivity.Ssr ),
		new( MarketplaceFamily, "StandaloneShell",      "standalone-shell",       "Cloud Portal homepage standalone shell — ChildContent",  ComponentInteractivity.Ssr ),
	];

	public static IEnumerable<ChunkEntry> ByFamily( string family )
		=> All.Where( e => string.Equals( e.Family, family, StringComparison.OrdinalIgnoreCase ) );

	public static IEnumerable<string> Families
		=> All.Select( e => e.Family ).Distinct();
}
