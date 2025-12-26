using Godot;

namespace Fixation.UI.Text;

public abstract class Glyph
{
	public Vector2 Position { get; set; }

	public Color Color { get; set; }

	public bool Visible { get; set; } = true;

	public abstract void Draw(CanvasItem context, int column);
}
