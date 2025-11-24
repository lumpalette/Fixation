using System;

namespace Fixation;

/// <summary>
/// A wrapper around a 8-bit unsigned integer that represents a player slot in the player party.
/// </summary>
public readonly record struct PlayerSlot
{
	/// <summary>
	/// The maximum player slot allowed.
	/// </summary>
	public const byte MaxValue = Game.MaxPlayerCount - 1;

	/// <summary>
	/// Creates a new <see cref="PlayerSlot"/>, between the allowed range.
	/// </summary>
	/// <param name="value">A value between 0 and <see cref="MaxValue"/> representing the player slot.</param>
	/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is outside the allowed range (0-<see cref="MaxValue"/>).</exception>
	public PlayerSlot(byte value)
	{
		if (!IsValid(value))
		{
			throw new ArgumentOutOfRangeException(nameof(value), value, $"Player slot index is outside the valid range (0-{MaxValue})");
		}

		Value = value;
	}

	/// <summary>
	/// The player slot value. This property is read-only.
	/// </summary>
	public byte Value { get; }

	/// <summary>
	/// Returns whether a value is considered a valid player slot.
	/// </summary>
	/// <param name="value">The value to check.</param>
	/// <returns><see langword="true"/> if <paramref name="value"/> is a valid player slot; otherwise, <see langword="false"/>.</returns>
	public static bool IsValid(byte value)
	{
		return value <= MaxValue;
	}

	/// <summary>
	/// Implicitly converts a <see cref="PlayerSlot"/> to a 8-bit unsigned integer.
	/// </summary>
	/// <param name="value">The <see cref="PlayerSlot"/> to convert.</param>
	public static implicit operator byte(PlayerSlot value) => value.Value;

	/// <summary>
	/// Implicitly converts a 8-bit unsigned integer into a <see cref="PlayerSlot"/>.
	/// </summary>
	/// <param name="value">The value to convert.</param>
	public static implicit operator PlayerSlot(byte value) => new(value);
}
