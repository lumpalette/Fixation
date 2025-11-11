using Godot;
using System;

namespace Fixation.Input;

/// <summary>
/// A map that binds each game button to an input event. This class cannot be inherited.
/// </summary>
public sealed class InputEventMap
{
	/// <summary>
	/// Creates an empty <see cref="InputEventMap"/> object.
	/// </summary>
	public InputEventMap()
	{
		_events = new InputEvent[(int)GameButton.Count];
	}

	/// <summary>
	/// Gets or sets the input event associated to the given game button.
	/// </summary>
	/// <param name="button">The game button to access.</param>
	/// <returns>The <see cref="InputEvent"/> associated to <paramref name="button"/>, or <see langword="null"/> if no event is assigned.</returns>
	/// <exception cref="ArgumentException">Thrown when <paramref name="button"/> is not a real button.</exception>
	public InputEvent this[GameButton button]
	{
		get
		{
			ValidateButtonIsReal(button);
			return _events[(int)button];
		}
		set
		{
			ValidateButtonIsReal(button);
			_events[(int)button] = value;
		}
	}

	/// <summary>
	/// Determines whether a game button is associated to the given input event.
	/// </summary>
	/// <param name="button">The game button to query.</param>
	/// <param name="e">The input event to check.</param>
	/// <returns><see langword="true"/> if <paramref name="button"/> is associated with <paramref name="e"/>; <see langword="false"/> otherwise.</returns>
	/// <exception cref="ArgumentException">Thrown when <paramref name="button"/> is not a real button.</exception>
	public bool ButtonHasEvent(GameButton button, InputEvent e)
	{
		ValidateButtonIsReal(button);
		return ButtonHasEventUnsafe(button, e);
	}

	/// <summary>
	/// Finds the game button associated to the given input event.
	/// </summary>
	/// <param name="e">The input event to check.</param>
	/// <returns>The <see cref="GameButton"/> associated with <paramref name="e"/>, or <see langword="null"/> if no action was found.</returns>
	public GameButton? FindButtonForEvent(InputEvent e)
	{
		for (int i = 0; i < (int)GameButton.Count; i++)
		{
			var current = (GameButton)i;
			if (ButtonHasEventUnsafe(current, e))
			{
				return current;
			}
		}

		return null;
	}

	/// <summary>
	/// Copies the input events from another event map to this map.
	/// </summary>
	/// <param name="source">The event map to copy.</param>
	/// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> is <see langword="null"/>.</exception>
	public void Copy(InputEventMap source)
	{
		if (source is null)
		{
			throw new ArgumentNullException(nameof(source), "Cannot copy from a null input event map");
		}

		Array.Copy(source._events, _events, (int)GameButton.Count);
	}

	/// <summary>
	/// Removes all input events defined in this map.
	/// </summary>
	public void Clear()
	{
		Array.Clear(_events);
	}

	private static void ValidateButtonIsReal(GameButton button)
	{
		int buttonIndex = (int)button;
		if ((buttonIndex < 0) || (buttonIndex >= (int)GameButton.Count))
		{
			throw new ArgumentException($"Button '{button}' is not a real button", nameof(button));
		}
	}

	private bool ButtonHasEventUnsafe(GameButton button, InputEvent e)
	{
		InputEvent current = _events[(int)button];
		if ((e is null) || (current is null))
		{
			return false;
		}

		return current.IsMatch(e, current is InputEventJoypadMotion);
	}

	private readonly InputEvent[] _events;
}