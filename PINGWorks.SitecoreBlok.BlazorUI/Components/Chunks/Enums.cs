namespace PINGWorks.SitecoreBlok.BlazorUI;

// Chunks-shared enums. Lives separately from the project-root Enums.cs so the
// latter stays canonical to Blok primitives. Both files share this namespace,
// so consumer code resolves either file's enums identically.

public enum Position { Top, Right, Bottom, Left }
public enum Orientation { Horizontal, Vertical }
public enum Placement { Left, Right, None }
public enum Alignment { Start, Center, End, Stretch }

public enum Tone { Info, Success, Warning, Danger, Neutral }
public enum Density { Comfortable, Compact }
public enum Trend { Up, Down, Neutral }
public enum Columns { One, Two, Three, Four }
