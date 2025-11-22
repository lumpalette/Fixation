using Fixation.Input;
using Godot;

namespace Fixation;

/// <summary>
/// A node that provides global access to the game's subsystems. This class cannot be inherited.
/// </summary>
public sealed partial class Game : Node
{
	[Export] private InputManager _input;
	[Export] private PlayerManager _player;

	private Game()
	{
		if (s_instance is not null)
		{
			QueueFree();
			return;
		}

		s_instance = this;
	}

	private static Game s_instance;

	/// <summary>
	/// The input manager. Provides information about player input. This property is read-only.
	/// </summary>
	public static InputManager Input => s_instance._input;

	/// <summary>
	/// The player manager. Contains general information about all players. This property is read-only.
	/// </summary>
	public static PlayerManager Player => s_instance._player;
}
