using Godot;

namespace Fixation.UI.Text;

/// <summary>
/// A collection of formatting attributes that can be applied to <see cref="Label"/> objects. This class cannot be inherited.
/// </summary>
[GlobalClass, Tool]
public sealed partial class LabelStyle : Resource
{
	/// <summary>
	/// The text font resource.
	/// </summary>
	[Export]
	public Font Font { get; set; }

	/// <summary>
	/// The font size of the text, in pixels.
	/// </summary>
	[Export]
	public int FontSize { get; set; }

	/// <summary>
	/// The font color of the text.
	/// </summary>
	[Export]
	public Color Color { get; set; } = Colors.White;

	/// <summary>
	/// The spacing value used for glyphs and lines, represented as a 2D vector.
	/// </summary>
	/// <remarks>
	/// These values work as multipliers, relative to <see cref="FontSize"/>.
	/// </remarks>
	[Export]
	public Vector2 Spacing { get; set; } = Vector2.One;

	/// <summary>
	/// Horizontal text alignment. Supports left, center and right alignment.
	/// </summary>
	[Export]
	public HorizontalAlignment HorizontalAlignment { get; set; }

	/// <summary>
	/// Vertical text alignment. Supports top, center and bottom alignment.
	/// </summary>
	[Export]
	public VerticalAlignment VerticalAlignment { get; set; }
}
