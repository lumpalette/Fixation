using Godot;
using System.Collections.Generic;
using System.Text;

namespace Fixation.UI.Text;

/// <summary>
/// A control that renders text with support for formatting &lt;tags&gt;. This class cannot be inherited.
/// </summary>
[Tool]
public sealed partial class Label : Control
{
	private const string DefaultStylePath = "res://assets/text/styles/default.tres";

	private string _textField;
	private LabelStyle _styleField;

	/// <summary>
	/// The text string to display. It may contain formatting tags that controls the text's rendering.
	/// </summary>
	[Export(PropertyHint.MultilineText)]
	public string Text
	{
		get => _textField;
		set
		{
			if (_textField != value)
			{
				if (value is not null)
				{
					// TODO: check if this doesn't suck (too much) for performance
					_textField = value
						.Replace("\\n", "\n")
						.Replace("\\r", "\r")
						.Replace("\\t", "\t")
						.Replace("\\f", "\f")
						.Replace("\\0", "\0");
				}
				else
				{
					_textField = string.Empty;
				}

				RegenerateText();
			}
		}
	}

	/// <summary>
	/// The default formatting attributes currently used by the label.
	/// </summary>
	[Export]
	public LabelStyle Style
	{
		get => _styleField;
		set
		{
			if (_styleField != value)
			{
				_styleField = value;
				
				if (!Engine.IsEditorHint())
				{
					RegenerateText();
				}
			}
		}
	}

	/// <summary>
	/// . This property is read-only.
	/// </summary>
	public Glyph[] Glyphs { get; private set; }

	/// <summary>
	/// The value of <see cref="Text"/> without formatting tags.
	/// </summary>
	public string ParsedText { get; private set; }

	public override void _Process(double delta)
	{
		QueueRedraw();
	}

	public override void _Draw()
	{
		if (Glyphs is null)
		{
			return;
		}
		
		for (int i = 0; i < Glyphs.Length; i++)
		{
			Glyphs[i].Draw(this, i);
		}
	}

	private void RegenerateText()
	{
		var writer = new Typewriter(Style ?? GD.Load<LabelStyle>(DefaultStylePath), Size.X);
		var parsedText = new StringBuilder();

		// Analyze the text character by character.
		foreach (char c in Text)
		{
			switch (c)
			{
				// End of text.
				case '\0':
					break;

				// Formatting tags.
				case '<':
					continue;
				case '>':
					continue;

				// Newlines and carriage modifiers.
				case '\n':
					writer.CarriageReturn();
					writer.LineFeed();
					break;
				case '\r':
					writer.CarriageReturn();
					break;
				case ' ':
					writer.Space(1);
					break;
				case '\t':
					writer.Space(4);
					break;

				// Non-breaking space.
				case '\u00A0':
					writer.Append(' ');
					break;

				// Printable character.
				default:
					if (!char.IsControl(c))
					{
						writer.Append(c);
					}
					break;
			}
			
			parsedText.Append(c);
		}

		Glyphs = writer.FormFeed();
		ParsedText = parsedText.ToString();
	}
}

partial class Label
{
	// A funny little helper class for the Label.RegenerateText() method.
	// Innacurate with how actual typewriters work but whatever.
	private class Typewriter
	{
		private LabelStyle _style;
		private int _lineWidth;
		private int _lineHeight;
		private Vector2I _carriage;
		private readonly List<Glyph> _textLine;
		private readonly List<Glyph> _textPage;
		private readonly float _pageWidth;
		private readonly StringBuilder _wordBuilder;

		public Typewriter(LabelStyle style, float pageWidth)
		{
			_style = (LabelStyle)style.Duplicate();
			_pageWidth = pageWidth;
			_textPage = [];
			_textLine = [];
			_wordBuilder = new StringBuilder();
		}

		// Enqueues the specified character into the word buffer.
		public void Append(char character)
		{
			_wordBuilder.Append(character);
		}

		// Adds the specified amount of spaces to the current line.
		public void Space(int n)
		{
			Advance(_style.GetHorizontalAdvance() * n);
		}

		public void Advance(float pixels)
		{

		}

		// Resets the carriage position back to the start of the line.
		public void CarriageReturn()
		{
			
		}

		// Resets the carriage position and inserts a new line.
		public void LineFeed()
		{

		}

		// Returns all lines of text written as a continuous array of glyphs.
		public Glyph[] FormFeed()
		{
			
		}
	}
}
