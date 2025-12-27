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
		
	}

	// A funny little helper class for RegenerateText() that manages glyph writing and layout.
	// Innacurate with how actual typewriters work but whatever.
	private class Typewriter
	{
		private LabelStyle _currentStyle;
		private Vector2I _carriage;
		private readonly float _pageWidth;
		private readonly List<Glyph> _textPage;
		private readonly List<Glyph> _textLine;
		private readonly StringBuilder _wordBuffer;

		public Typewriter(float pageWidth, LabelStyle style)
		{
			SetStyle(style);
			_pageWidth = pageWidth;

			_textPage = [];
			_textLine = [];

			_wordBuffer = new StringBuilder();
		}

		// Sets the current text style to a copy of the given style.
		public void SetStyle(LabelStyle style)
		{
			_currentStyle = (LabelStyle)style.Duplicate();
		}

		// Writes the specified character into the word buffer.
		public void Write(char character)
		{
			_wordBuffer.Append(character);
		}

		// Moves the carriage by inserting the specified amount of spaces.
		public void Space(int n)
		{
			SubmitWord();
			_carriage.X += n * (int)(_currentStyle.FontSize * _currentStyle.Spacing.X);
		}

		private void SubmitWord()
		{
			// Add a new line if the word doesn't fit in the current one.
			int wordWidth = _wordBuffer.Length * _currentStyle.FontSize;
			if (_carriage.X + wordWidth >= _pageWidth)
			{

			}

			// Append the word at the end of the line.
			foreach (char c in _wordBuffer.ToString())
			{
				WriteGlyph(new CharacterGlyph()
				{
					Character = c.ToString(),
					Font = _currentStyle.Font,
					FontSize = _currentStyle.FontSize,
					Position = _carriage,
					Color = _currentStyle.Color
				});
			}

			_wordBuffer.Clear();
		}

		private void WriteGlyph(Glyph glyph)
		{

		}
	}
}
