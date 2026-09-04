namespace PINGWorks.SitecoreBlok.BlazorUI;

// Multi-purpose *******************************************************************************************
public enum Size { Default, Xs3, Xs2, Xs, Sm, Md, Lg, Xl, Xl2, Xl3, Xl4, Xl5, Xl6, Xl7, Xl8, Full }

// Accordion ***********************************************************************************************
public enum AccordionType { Single, Multiple }

// Alert ***************************************************************************************************
public enum AlertVariant { Default, Primary, Danger, Warning, Success }

// Badge ***************************************************************************************************
public enum BadgeColor { Neutral, Primary, Danger, Success, Warning, Yellow, Teal, Cyan, Blue, Pink }
public enum BadgeSize { Sm, Md, Lg }
public enum BadgeVariant { Default, Bold }

// Button **************************************************************************************************
public enum ButtonColor { Primary, Ai, Danger, Success, Neutral }
public enum ButtonSize { Default, Xs, Sm, Lg, Icon, IconLg, IconSm, IconXs }
public enum ButtonVariant { Default, Outline, Ghost, Link }

// Card ****************************************************************************************************
public enum CardElevation { Base, None, Xs, Sm, Md, Lg, Xl }
public enum CardStyle { Flat, Outline, Filled }
public enum CardPadding { Sm, Md, Lg }

// Avatar **************************************************************************************************
public enum AvatarImageStatus { Idle, Loading, Loaded, Errored }

// Icon ****************************************************************************************************
public enum IconSize { Sm, Md, Default, Lg, Xl, Xxl, Custom }
public enum IconVariant { Default, Subtle, Filled }
public enum IconColorScheme { Primary, Neutral, Success, Danger, Warning, Yellow, Teal, Cyan, Blue, Purple, Pink }
public enum AiGradient { G50, G100, G200, G300, G400, G500, G600, G700, G800, G900 }

// Separator ***********************************************************************************************
public enum SeparatorDirection { Horizontal, Vertical }

// Spinner *************************************************************************************************
public enum SpinnerVariant { Default, Primary }

// Switch **************************************************************************************************
public enum SwitchVariant { Primary, Danger, Success }

// Toggle **************************************************************************************************
public enum ToggleVariant { Default, Outline, Square, Rounded }
public enum ToggleSize { Default, Sm, Xs }
public enum ToggleGroupType { Single, Multiple }

// Field ****************************************************************************************************
public enum FieldLegendVariant { Legend, Label }
public enum FieldOrientation { Vertical, Horizontal }

// InputGroup **********************************************************************************************
public enum InputGroupAlign { InlineStart, InlineEnd, BlockStart, BlockEnd }
public enum InputGroupButtonSize { Xs, Sm, IconXs, IconSm }

// Filter *************************************************************************************************
public enum FilterDisplayMode { Text, Badge }
public enum FilterBarDirection { Horizontal, Vertical }

// Tabs ****************************************************************************************************
public enum TabsVariant { Line, SoftRounded }

// Dialog **************************************************************************************************
public enum DialogSize { Sm, Md, Lg, Xl, Full }

// Sheet ***************************************************************************************************
public enum SheetSide { Top, Right, Bottom, Left }

// ActionBar ***********************************************************************************************
public enum ActionBarAlign { Left, Center, Right }

// Carousel ************************************************************************************************
public enum CarouselOrientation { Horizontal, Vertical }

// Tooltip *************************************************************************************************
public enum TooltipSide { Top, Bottom, Left, Right }

// HoverCard ***********************************************************************************************
public enum HoverCardSide { Top, Right, Bottom, Left }
public enum HoverCardAlign { Start, Center, End }

// Sidebar *************************************************************************************************
public enum SidebarSide { Left, Right }
public enum SidebarVariant { Sidebar, Floating, Inset }
public enum SidebarCollapsible { Offcanvas, Icon, None }
public enum SidebarMenuButtonSize { Default, Sm, Lg }
public enum SidebarMenuSubButtonSize { Sm, Md }

// ContextMenu *********************************************************************************************
public enum ContextMenuItemVariant { Default, Destructive }

// DropdownMenu ********************************************************************************************
public enum DropdownMenuItemVariant { Default, Destructive }
public enum DropdownMenuSide { Top, Right, Bottom, Left }
public enum DropdownMenuAlign { Start, Center, End }

// Menubar *************************************************************************************************
public enum MenubarItemVariant { Default, Destructive }

// Toast ***************************************************************************************************
public enum ToastVariant { Default, Success, Error, Warning, Info }
public enum ToastPosition { TopLeft, TopCenter, TopRight, BottomLeft, BottomCenter, BottomRight }

// CodeViewer **********************************************************************************************
public enum CodeLanguage
{
	Markup, HTML, XML, SVG, MathML, SSML, Atom, RSS, CSS, CLike, JavaScript, AspNet,
	Bash, Shell, CSharp, CssExtras, CSV, Diff, GraphQL, Handlebars, Json, Json5, Less, Markdown,
	MarkupTemplating, MongoDB, PlSql, PowerShell, Python, Razor, SQL, Sass, TypeScript, XmlDoc, YAML
}

// Editable ************************************************************************************************
public enum EditableSize { Sm, Md, Lg }
public enum EditableActivationMode { Click, Dblclick }

// ErrorState **********************************************************************************************
public enum ErrorStateVariant { Generic, Http400, Http401, Http403, Http404, Http500, Http503 }

// Table ***************************************************************************************************
public enum TableSize { Sm, Md, Lg }

// Stepper *************************************************************************************************
public enum StepperOrientation { Horizontal, Vertical }
public record StepperStep( string Label, string? Description = null );

// Timeline ************************************************************************************************
public enum TimelineSize { Sm, Md, Lg }
public enum TimelineIndicatorVariant { Solid, Outline, Subtle, Plain }
public enum TimelineIndicatorSize { Sm, Md, Lg }
public enum TimelineConnectorVariant { Solid, Dashed, Dotted }

// Theme ***************************************************************************************************
public enum DisplayMode { Light, Dark }

// TreeView ************************************************************************************************
public enum TreeSelectionMode { None, Single, Multiple }

// Resizable ***********************************************************************************************
public enum ResizableDirection { Horizontal, Vertical }

// StackNavigation *****************************************************************************************
public enum StackNavigationOrientation { Vertical, Horizontal }
public enum StackNavigationColorScheme { Neutral, Primary }