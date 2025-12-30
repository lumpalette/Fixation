using Godot;

namespace Fixation.UI;

/// <summary>
/// A control that renders a formatted string over multiple pages of text. This class cannot be inherited.
/// </summary>
[Tool]
public sealed partial class Text : Control
{
	/// <summary>
	/// The formatted text string to parse and display.
	/// </summary>
	public string String { get; set; }
}
