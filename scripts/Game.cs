using Fixation.Input;
using Fixation.Party;
using Godot;
using System;

namespace Fixation;

/// <summary>
/// A singleton node that provides global access to the game's subsystems. This class cannot be inherited.
/// </summary>
public sealed partial class Game : Node
{
	/// <summary>
	/// The maximum number of players supported by the game.
	/// </summary>
	public const int MaxPlayerCount = 4;

	[Export]
	private InputManager _input;
	[Export]
	private PlayerParty _party;

	private Game()
	{
		if (s_instance is not null)
		{
			QueueFree();
			return;
		}

		s_instance = this;
	}

	public override void _Ready()
	{
		if (Input is null)
		{
			throw new InvalidOperationException($"'{nameof(Input)}' node is null");
		}
		if (Party is null)
		{
			throw new InvalidOperationException($"'{nameof(Party)}' node is null");
		}
	}

	private static Game s_instance;

	/// <summary>
	/// The input manager. Provides information about player input. This property is read-only.
	/// </summary>
	public static InputManager Input => s_instance._input;

	/// <summary>
	/// The current player party. Contains general information about all players. This property is read-only.
	/// </summary>
	public static PlayerParty Party => s_instance._party;
}
