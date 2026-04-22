namespace PINGWorks.SitecoreBlok.BlazorUI.Catalogue.Services;

// Describes one element (Blazor component) within a component family shown on a Catalogue page.
// Name:        Element name as written in markup, e.g. "Accordion", "AccordionItem".
// Description: Short prose describing the element's purpose.
// Depth:       Indent level in the summary table. 0 = root, 1 = child, 2 = grandchild.
// Properties:  Public parameters exposed by the element.
public sealed record ApiElement(
	string Name,
	string Description,
	int Depth,
	ApiProperty[] Properties
);

// Describes one public [Parameter] on an element.
// Name:        Property name as written in markup, e.g. "Variant", "ClassName".
// Type:        Type signature as it should appear in docs, e.g. "ButtonVariant", "string?".
// IsRequired:  True if the parameter is [EditorRequired] or must be set for the element to work.
// Description: Prose description. Include the default value inline when present.
public sealed record ApiProperty(
	string Name,
	string Type,
	bool IsRequired,
	string Description
);
