using Godot;

namespace Fixation.Dialogue;

/// <summary>
/// A collection of formatting attributes for <see cref="Text"/> objects. This class cannot be inherited.
/// </summary>
[GlobalClass]
public sealed partial class TextStyle : Resource
{
	/// <summary>
	/// The text font.
	/// </summary>
	[Export] public Font Font { get; set; }

	/// <summary>
	/// The size of the text font, in pixels.
	/// </summary>
	[Export] public int FontSize { get; set; }

	/// <summary>
	/// The text font color.
	/// </summary>
	[Export] public Color Color { get; set; }

	/// <summary>
	/// The speed at which the glyphs are drawn, in seconds.
	/// </summary>
	[Export] public float WriteSpeed { get; set; }
}
