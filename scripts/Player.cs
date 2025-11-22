using Godot;

namespace Fixation;

/// <summary>
/// Representation of a unique player in the game.
/// </summary>
public class Player
{
	/// <summary>
	/// The player's name.
	/// </summary>
	public string Name { get; set; } = "Prisma";

	/// <summary>
	/// The color contained by this player.
	/// </summary>
	public Color Color { get; set; } = new Color(0x27F5B7);
}
