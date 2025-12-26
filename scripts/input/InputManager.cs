using Godot;
using System;

namespace Fixation.Input;

/// <summary>
/// A node that manages the input state of all players and provides an interface to query that state. This class cannot be inherited.
/// </summary>
public sealed partial class InputManager : Node
{
	private int _playerCount;
	private readonly PlayerInput[] _playerSlots;
	
	private InputManager()
	{
		_playerSlots = new PlayerInput[Game.MaxPlayerCount];
	}

	/// <summary>
	/// Gets the player connected at the specified slot.
	/// </summary>
	/// <param name="slot">The slot to access.</param>
	/// <returns>The <see cref="PlayerInput"/> connected to the <paramref name="slot"/>, or <see langword="null"/> if no player is connected.</returns>
	public PlayerInput this[PlayerSlot slot] => _playerSlots[slot];

	public override void _Ready()
	{
		// Slot 0 always has a player connected, regardless of whether the actual player exists or not.
		// The initial input device is decided by the device hotswapping "system" (see below).
		_playerSlots[0] = new PlayerInput();
		_playerCount = 1;

		Game.Party.MemberAdded += AddPlayerInput;
		Game.Party.MemberRemoved += RemovePlayerInput;

		Godot.Input.JoyConnectionChanged += UpdatePlayerRemovedDevices;
	}

	public override void _Input(InputEvent e) // yes here
	{
		// Device hotswapping is only available on singleplayer.
		if (_playerCount > 1)
		{
			return;
		}

		// Hotswap between keyboard to joypad and viceversa.
		Device? currentDevice = _playerSlots[0].Device;
		switch (e)
		{
			case InputEventKey or InputEventMouseButton:
				if (currentDevice?.Id != -1)
				{
					_playerSlots[0].Device = Device.CreateKeyboard();
				}
				break;
			case InputEventJoypadButton jbutton:
				if (currentDevice?.Id != jbutton.Device)
				{
					_playerSlots[0].Device = Device.CreateJoypad(jbutton.Device);
				}
				break;
			case InputEventJoypadMotion jmotion:
				bool outsideDeadzone = MathF.Abs(jmotion.AxisValue) >= _playerSlots[0].Deadzone;
				if (outsideDeadzone && (currentDevice?.Id != jmotion.Device))
				{
					_playerSlots[0].Device = Device.CreateJoypad(jmotion.Device);
				}
				break;
		}
	}

	public override void _Process(double delta)
	{
		for (int i = 0; i < Game.MaxPlayerCount; i++)
		{
			_playerSlots[i]?.Update();
		}
	}

	/// <summary>
	/// Returns whether a player is holding down the specified game button.
	/// </summary>
	/// <param name="button">The game button to check.</param>
	/// <param name="slot">The slot to check. If left unspecified, slot 0 is used.</param>
	/// <returns><see langword="true"/> if the <paramref name="button"/> is down; otherwise, <see langword="false"/>.</returns>
	/// <exception cref="InvalidOperationException">Thrown if there is no player connected to the <paramref name="slot"/>.</exception>
	public bool IsDown(GameButton button, PlayerSlot slot = default)
	{
		unsafe
		{
			PlayerInput player = GetValidatedPlayer(slot);
			return PlayerButtonSatisfiesPredicate(player, button, &StatePredicate);
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
	/// <param name="slot">The slot to check. If left unspecified, slot 0 is used.</param>
	/// <returns><see langword="true"/> if the <paramref name="button"/> was pressed; otherwise, <see langword="false"/>.</returns>
	/// <exception cref="InvalidOperationException">Thrown if there is no player connected to the <paramref name="slot"/>.</exception>
	public bool IsPressed(GameButton button, PlayerSlot slot = default)
	{
		unsafe
		{
			PlayerInput player = GetValidatedPlayer(slot);
			return PlayerButtonSatisfiesPredicate(player, button, &StatePredicate);
		}

		static bool StatePredicate(GameButtonState state)
		{
			return state == GameButtonState.Pressed;
		}
	}

	/// <summary>
	/// Returns whether a player released the specified game button in the current frame (released).
	/// </summary>
	/// <param name="button">The game button to check.</param>
	/// <param name="slot">The slot to check. If left unspecified, slot 0 is used.</param>
	/// <returns><see langword="true"/> if the <paramref name="button"/> was released; otherwise, <see langword="false"/>.</returns>
	/// <exception cref="InvalidOperationException">Thrown if there is no player connected to the <paramref name="slot"/>.</exception>
	public bool IsReleased(GameButton button, PlayerSlot slot = default)
	{
		unsafe
		{
			PlayerInput player = GetValidatedPlayer(slot);
			return PlayerButtonSatisfiesPredicate(player, button, &StatePredicate);
		}

		static bool StatePredicate(GameButtonState state)
		{
			return state == GameButtonState.Released;
		}
	}

	/// <summary>
	/// Creates a 2D vector that represents a player's current directional input.
	/// </summary>
	/// <param name="slot">The slot to check. If left unspecified, slot 0 is used.</param>
	/// <returns>A normalized <see cref="Vector2"/>, where X = (R - L) and Y = (D - U).</returns>
	/// <exception cref="InvalidOperationException">Thrown if there is no player connected to the <paramref name="slot"/>.</exception>
	public Vector2 GetVector(PlayerSlot slot = default)
	{
		PlayerInput player = GetValidatedPlayer(slot);

		float x = ((player.GetButtonState(GameButton.Right) == GameButtonState.Down) ? 1f : 0f)
				- ((player.GetButtonState(GameButton.Left) == GameButtonState.Down) ? 1f : 0f);
		float y = ((player.GetButtonState(GameButton.Down) == GameButtonState.Down) ? 1f : 0f)
				- ((player.GetButtonState(GameButton.Up) == GameButtonState.Down) ? 1f : 0f);

		return new Vector2(x, y).Normalized();
	}

	private PlayerInput GetValidatedPlayer(int slotIndex)
	{
		if (_playerSlots[slotIndex] is null)
		{
			throw new InvalidOperationException($"Player in slot {slotIndex} does not exist");
		}

		return _playerSlots[slotIndex];
	}

	private void AddPlayerInput(object sender, Party.PartyMemberEventArgs e)
	{
		// Slot 0 always has a player connected; ignore method call if player 0 was added to the party.
		if (e.Slot != 0)
		{
			_playerSlots[e.Slot] = new PlayerInput();
			_playerCount++;
		}
	}

	private void RemovePlayerInput(object sender, Party.PartyMemberEventArgs e)
	{
		// Slot 0 always has a player connected; ignore method call if player 0 was removed from the party.
		if (e.Slot != 0)
		{
			_playerSlots[e.Slot] = null;
			_playerCount--;
		}
	}

	private void UpdatePlayerRemovedDevices(long device, bool connected)
	{
		if (connected)
		{
			return;
		}

		foreach (PlayerSlot slot in Game.Party.GetOccupiedSlots())
		{
			if (this[slot].Device?.Id == device)
			{
				// We don't stop here because multiple players can share the same device (they shouldn't, but they can [qué les pasa enfermos]).
				this[slot].Device = null;
			}
		}

		// TODO: trigger an event that notifies that one or more players were left without an input device
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
