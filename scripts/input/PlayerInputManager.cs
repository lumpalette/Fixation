using Godot;
using System;
using System.Collections.ObjectModel;

namespace Fixation.Input;

/// <summary>
/// A global node responsible for managing the lifecycle and state of all players in the game. This class cannot be inherited.
/// </summary>
public sealed partial class PlayerInputManager : Node
{
	/// <summary>
	/// Maximum number of players that can be connected.
	/// </summary>
	public const int MaxPlayerCount = 4;

	/// <summary>
	/// A read-only collection of all players registered in the system. This property is read-only.
	/// </summary>
	/// <remarks>
	/// The collection always contains 4 elements, corresponding to each player index (0-3). Returns <see langword="null"/> for unassigned player indices.
	/// </remarks>
	public static ReadOnlyCollection<PlayerInput> Players => s_instance._playersReadOnly;

	/// <summary>
	/// The current number of registed players in the system. This property is read-only.
	/// </summary>
	/// <remarks>
	/// This property includes all registered players, including disabled players.
	/// </remarks>
	public static int PlayerCount => s_instance._playerCount;

	/// <summary>
	/// Creates a new player at the specified player index, and assigns it an input device.
	/// </summary>
	/// <param name="playerIndex">A zero-based index to assign the new player (0-3).</param>
	/// <param name="deviceId">The ID of the input device to assign.</param>
	/// <returns>The created <see cref="PlayerInput"/>, if more configuration is required.</returns>
	/// <exception cref="ArgumentException">Thrown when <paramref name="deviceId"/> doesn't correspond to a valid device.</exception>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="playerIndex"/> is outside the valid range (0-3).</exception>
	/// <exception cref="InvalidOperationException">Thrown when a player already exists at <paramref name="playerIndex"/>.</exception>
	public static PlayerInput Create(int playerIndex, int deviceId)
	{
		if (Exists(playerIndex))
		{
			throw new InvalidOperationException($"Cannot create player for index {playerIndex} because a player already exist");
		}

		var player = (PlayerInput)Activator.CreateInstance(typeof(PlayerInput), nonPublic: true);
		player.Name = "Player_" + playerIndex;
		player.DeviceId = deviceId;

		s_instance._players[playerIndex] = player;
		s_instance.AddChild(player);

		s_instance._playerCount++;
		return player;
	}

	/// <summary>
	/// Determines whether a player is assigned to the specified player index.
	/// </summary>
	/// <param name="playerIndex">A zero-based index of the player to target (0-3).</param>
	/// <returns><see langword="true"/> if there is a player assigned to <paramref name="playerIndex"/>; <see langword="false"/> otherwise.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="playerIndex"/> is outside the valid range (0-3).</exception>
	public static bool Exists(int playerIndex)
	{
		ValidatePlayerIndex(playerIndex);
		return s_instance._players[playerIndex] is not null;
	}

	/// <summary>
	/// Determines if a player has input processing currently enabled.
	/// </summary>
	/// <param name="playerIndex">A zero-based index of the player to target (0-3).</param>
	/// <returns><see langword="true"/> if input processing is enabled; <see langword="false"/> otherwise.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="playerIndex"/> is outside the valid range (0-3).</exception>
	/// <exception cref="InvalidOperationException">Thrown when the player at <paramref name="playerIndex"/> doesn't exist.</exception>
	public static bool IsEnabled(int playerIndex)
	{
		ValidatePlayerIndex(playerIndex);
		ValidatePlayerExists(playerIndex);

		return s_instance._players[playerIndex].IsInsideTree();
	}

	/// <summary>
	/// Enables or disables input processing for the specified player.
	/// </summary>
	/// <remarks>
	/// Disabling a player's input processing will immediately release all currently pressed buttons.
	/// </remarks>
	/// <param name="playerIndex">A zero-based index of the player to target (0-3).</param>
	/// <param name="enabled">Whether player input should be enabled (true) or disabled (false).</param>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="playerIndex"/> is outside the valid range (0-3).</exception>
	/// <exception cref="InvalidOperationException">Thrown when the player at <paramref name="playerIndex"/> doesn't exist.</exception>
	public static void SetEnabled(int playerIndex, bool enabled)
	{
		// The player is already enabled/disabled, so ignore method call.
		if (IsEnabled(playerIndex) == enabled)
		{
			return;
		}

		PlayerInput player = s_instance._players[playerIndex];
		if (enabled)
		{
			s_instance.AddChild(player);
		}
		else
		{
			for (int i = 0; i < (int)GameButton.Count; i++)
			{
				player.ReleaseButton((GameButton)i);
			}

			s_instance.RemoveChild(player);
		}
	}

	/// <summary>
	/// Destroys a player assigned to the specified player index.
	/// </summary>
	/// <remarks>
	/// It's always safe to call this method, even when there's no player assigned; in that case, the method call will be ignored.
	/// </remarks>
	/// <param name="playerIndex">A zero-based index of the player to target (0-3).</param>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="playerIndex"/> is outside the valid range (0-3).</exception>
	public static void Destroy(int playerIndex)
	{
		if (!Exists(playerIndex))
		{
			return;
		}

		s_instance._players[playerIndex].QueueFree();
		s_instance._players[playerIndex] = null;

		s_instance._playerCount--;
	}

	public override void _Ready()
	{
		// The default player (player 0) is assigned an input device ID of -1 (the keyboard) by default at the start of the game.
		Create(playerIndex: 0, deviceId: -1);
	}

	private PlayerInputManager()
	{
		if (s_instance is not null)
		{
			QueueFree();
			return;
		}

		s_instance = this;

		_players = new PlayerInput[4];
		_playersReadOnly = Array.AsReadOnly(_players);
	}

	private static void ValidatePlayerIndex(int playerIndex)
	{
		if ((playerIndex < 0) || (playerIndex >= MaxPlayerCount))
		{
			throw new ArgumentOutOfRangeException(nameof(playerIndex), playerIndex, "Player index is outside the valid range (0-3)");
		}
	}

	private static void ValidatePlayerExists(int playerIndex)
	{
		if (s_instance._players[playerIndex] is null)
		{
			throw new InvalidOperationException($"Player at index {playerIndex} does not exist");
		}
	}

	private static PlayerInputManager s_instance;
	private int _playerCount;
	private readonly PlayerInput[] _players;
	private readonly ReadOnlyCollection<PlayerInput> _playersReadOnly;
}