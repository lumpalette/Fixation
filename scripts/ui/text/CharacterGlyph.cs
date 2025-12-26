using Godot;

namespace Fixation.UI.Text;

public class CharacterGlyph : Glyph
{
	public string Character { get; set; }

	public Font Font { get; set; }

	public int FontSize { get; set; }

	public override void Draw(CanvasItem context, int column)
	{
		context.DrawChar(Font, Position, Character, FontSize, Color);
	}
}
