using Godot;
using Godot.Collections;

namespace Fixation.Tests;

internal partial class GlyphTest : Control
{
	private const string TodoHablaDeTi = "Pero tú no estás";

	[Export]
	private Font _font;

	private TextServer _server;
	private Rid _shapedText;
	private Array<Dictionary> _glyphs;

	public override void _Ready()
	{
		_server = TextServerManager.GetPrimaryInterface();

		_shapedText = _server.CreateShapedText(TextServer.Direction.Ltr);

		_server.ShapedTextAddString(_shapedText, TodoHablaDeTi, _font.GetRids(), 8);
		_server.ShapedTextShape(_shapedText);

		_glyphs = _server.ShapedTextGetGlyphs(_shapedText);

		GD.Print(_server.FontGetGlyphSize(_font.GetRids()[0], new Vector2I(8, 8), (long)_glyphs[0]["index"]));
	}

	public override void _PhysicsProcess(double delta)
	{
		QueueRedraw();
	}

	public override void _Draw()
	{
		for (int i = 0; i < TodoHablaDeTi.Length; i++)
		{
			// 0.34 ms (i will go with this one)
			_server.FontDrawGlyph((Rid)_glyphs[i]["font_rid"], GetCanvasItem(), (long)_glyphs[i]["font_size"], new Vector2(i * 8, (float)_server.FontGetAscent((Rid)_glyphs[i]["font_rid"], (long)_glyphs[i]["font_size"])), (long)_glyphs[i]["index"]);
			
			// 0.36 ms
			//DrawChar(_font, new Vector2(i * 8, (float)_server.FontGetAscent((Rid)_glyphs[i]["font_rid"], (long)_glyphs[i]["font_size"])), TodoHablaDeTi[i].ToString(), 8);
		}

		// surprisingly, this takes ~0.36 ms too
		//_server.ShapedTextDraw(_shapedText, GetCanvasItem(), Vector2.Zero);
	}
}
