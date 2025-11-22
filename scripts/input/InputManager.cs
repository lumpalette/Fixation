using Godot;
using System;
using System.Collections.ObjectModel;

namespace Fixation.Input;

/// <summary>
/// A node that manages the input state of all players and provides an interface to query that state. This class cannot be inherited.
/// </summary>
public sealed partial class InputManager : Node
{
	private readonly PlayerInput[] _players;

	private InputManager()
	{
		_players = new PlayerInput[4];
		Players = Array.AsReadOnly(_players);
	}

	public override void _Ready()
	{
		Game.Player.PartyMemberAdded += AddPlayerInput;
		Game.Player.PartyMemberRemoved += RemovePlayerInput;
	}

	public override void _Process(double delta)
	{
		for (int i = 0; i < 4; i++)
		{
			_players[i]?.Update();
		}
	}

	/// <summary>
	/// A read-only collection of all player input controllers registered.
	/// </summary>
	/// <remarks>
	/// The collection contains 4 elements, accessed by player index (0-3). Unassigned player slots return <see langword="null"/>.
	/// </remarks>
	public ReadOnlyCollection<PlayerInput> Players { get; }

	/// <summary>
	/// Returns whether a player is holding down the specified game button.
	/// </summary>
	/// <param name="button">The game button to check.</param>
	/// <param name="playerIndex">The player to target (0-3). If left unspecified, player 0 is used.</param>
	/// <returns><see langword="true"/> if the <paramref name="button"/> is down; otherwise, <see langword="false"/>.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="playerIndex"/> is outside the valid range (0-3).</exception>
	/// <exception cref="InvalidOperationException">Thrown if the player at <paramref name="playerIndex"/> doesn't exist.</exception>
	public bool IsDown(GameButton button, int playerIndex = 0)
	{
		ValidatePlayerSlot(playerIndex);
		unsafe
		{
			return PlayerButtonSatisfiesPredicate(_players[playerIndex], button, &StatePredicate);
		}

		static bool StatePredicate(GameButtonState state)
		{
			return (state == GameButtonState.Pressed) || (state == GameButtonState.Down);
		}
	}

	/// <summary>
	/// Returns whether a player pressed the specified game button in the current frame.
	/// </summary>
	/// <param name="button">The game button to check.</param>
	/// <param name="playerIndex">The player to target (0-3). If left unspecified, player 0 is used.</param>
	/// <returns><see langword="true"/> if the <paramref name="button"/> was pressed; otherwise, <see langword="false"/>.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="playerIndex"/> is outside the valid range (0-3).</exception>
	/// <exception cref="InvalidOperationException">Thrown if the player at <paramref name="playerIndex"/> doesn't exist.</exception>
	public bool IsPressed(GameButton button, int playerIndex = 0)
	{
		ValidatePlayerSlot(playerIndex);
		unsafe
		{
			return PlayerButtonSatisfiesPredicate(_players[playerIndex], button, &StatePredicate);
		}

		static bool StatePredicate(GameButtonState state)
		{
			return state == GameButtonState.Pressed;
		}
	}

	/// <summary>
	/// Returns whether a player released the specified game button in the current frame.
	/// </summary>
	/// <param name="button">The game button to check.</param>
	/// <param name="playerIndex">The player to target (0-3). If left unspecified, player 0 is used.</param>
	/// <returns><see langword="true"/> if the <paramref name="button"/> was released; otherwise, <see langword="false"/>.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="playerIndex"/> is outside the valid range (0-3).</exception>
	/// <exception cref="InvalidOperationException">Thrown if the player at <paramref name="playerIndex"/> doesn't exist.</exception>
	public bool IsReleased(GameButton button, int playerIndex = 0)
	{
		ValidatePlayerSlot(playerIndex);
		unsafe
		{
			return PlayerButtonSatisfiesPredicate(_players[playerIndex], button, &StatePredicate);
		}

		static bool StatePredicate(GameButtonState state)
		{
			return state == GameButtonState.Released;
		}
	}

	/// <summary>
	/// Creates a 2D vector that represents the current directional input.
	/// </summary>
	/// <param name="playerIndex">The player to target (0-3). If left unspecified, player 0 is used.</param>
	/// <returns>A normalized <see cref="Vector2"/>, where X = (R - L) and Y = (D - U).</returns>
	/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="playerIndex"/> is outside the valid range (0-3).</exception>
	public Vector2 GetVector(int playerIndex = 0)
	{
		return new Vector2
		{
			X = (IsDown(GameButton.Right, playerIndex) ? 1f : 0f) - (IsDown(GameButton.Left, playerIndex) ? 1f : 0f),
			Y = (IsDown(GameButton.Down, playerIndex) ? 1f : 0f) - (IsDown(GameButton.Up, playerIndex) ? 1f : 0f)
		}.Normalized();
	}

	private void ValidatePlayerSlot(int playerIndex)
	{
		if ((playerIndex < 0) || (playerIndex >= 4))
		{
			throw new ArgumentOutOfRangeException(nameof(playerIndex), playerIndex, "Player index is outside the valid range (0-3)");
		}

		if (_players[playerIndex] is null)
		{
			throw new InvalidOperationException($"Player at index {playerIndex} does not exist");
		}
	}

	private void AddPlayerInput(object sender, PlayerPartyEventArgs e)
	{
		_players[e.Index] = new PlayerInput();
	}

	private void RemovePlayerInput(object sender, PlayerPartyEventArgs e)
	{
		_players[e.Index] = null;
	}

	private static unsafe bool PlayerButtonSatisfiesPredicate(PlayerInput player, GameButton button, delegate* managed<GameButtonState, bool> predicate)
	{
		if ((button == GameButton.Any) || (button == GameButton.None))
		{
			for (int i = 0; i < (int)GameButton.Count; i++)
			{
				if (predicate(player.GetButtonState((GameButton)i)))
				{
					return button == GameButton.Any;
				}
			}

			return button == GameButton.None;
		}

		return predicate(player.GetButtonState(button));
	}
}
