using System;

namespace Fixation.Input;

/// <summary>
/// Extension methods for types defined in the input system.
/// </summary>
public static class Extensions
{
	/// <summary>
	/// Returns whether the given game button is a real button or not.
	/// </summary>
	/// <param name="button">The game button to check.</param>
	/// <returns><see langword="true"/> if <paramref name="button"/> is a real button; <see langword="false"/> if it's a meta button.</returns>
	public static bool IsReal(this GameButton button)
	{
		return (button >= GameButton.Accept) && (button <= GameButton.Right);
	}

	/// <summary>
	/// Returns whether the given game button is a meta button or not.
	/// </summary>
	/// <param name="button">The game button to check.</param>
	/// <returns><see langword="true"/> if <paramref name="button"/> is a meta button; <see langword="false"/> if it's a real button.</returns>
	public static bool IsMeta(this GameButton button)
	{
		return (button >= GameButton.Count) && (button <= GameButton.None);
	}

	/// <summary>
	/// Asserts that the given game button is a real button; if it's not, throws an exception.
	/// </summary>
	/// <param name="button">The game button to check.</param>
	/// <param name="paramName">Optional parameter name when calling this extension from another method.</param>
	/// <exception cref="ArgumentException">Thrown if <paramref name="button"/> is not a real button.</exception>
	public static void EnsureIsReal(this GameButton button, string paramName = "button")
	{
		if (button.IsMeta())
		{
			throw new ArgumentException($"Button '{button}' is not a real button", paramName);
		}
	}
}
