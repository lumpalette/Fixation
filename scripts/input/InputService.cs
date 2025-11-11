using Godot;
using System;

namespace Fixation.Input;

/// <summary>
/// Provides a simple, static interface for querying information about the input system. This class cannot be inherited.
/// </summary>
public static class InputService
{
	/// <summary>
	/// The default deadzone value for all joypads.
	/// </summary>
	public const float DefaultDeadzone = 0.2f;

	/// <summary>
	/// An array containing the default primary (0) and secondary (1) event maps for keyboard input. This property is read-only.
	/// </summary>
	public static InputEventMap[] DefaultKeyEventMaps
	{
		get
		{
			// Primary mapping.
			InputEventMap m1 = new();
			m1[GameButton.Accept] = new InputEventKey() { PhysicalKeycode = Key.Z };
			m1[GameButton.Decline] = new InputEventKey() { PhysicalKeycode = Key.X };
			m1[GameButton.Context] = new InputEventKey() { PhysicalKeycode = Key.C };
			m1[GameButton.Up] = new InputEventKey() { PhysicalKeycode = Key.Up };
			m1[GameButton.Down] = new InputEventKey() { PhysicalKeycode = Key.Down };
			m1[GameButton.Left] = new InputEventKey() { PhysicalKeycode = Key.Left };
			m1[GameButton.Right] = new InputEventKey() { PhysicalKeycode = Key.Right };

			// Secondary mapping.
			InputEventMap m2 = new();
			m2[GameButton.Accept] = new InputEventKey() { PhysicalKeycode = Key.Enter };
			m2[GameButton.Decline] = new InputEventKey() { PhysicalKeycode = Key.Shift };
			m2[GameButton.Context] = new InputEventKey() { PhysicalKeycode = Key.Ctrl };
			m2[GameButton.Up] = new InputEventKey() { PhysicalKeycode = Key.W };
			m2[GameButton.Down] = new InputEventKey() { PhysicalKeycode = Key.S };
			m2[GameButton.Left] = new InputEventKey() { PhysicalKeycode = Key.A };
			m2[GameButton.Right] = new InputEventKey() { PhysicalKeycode = Key.D };

			return [m1, m2];
		}
	}

	/// <summary>
	/// An array containing the default primary (0) and secondary (1) event maps for joypad input. This property is read-only.
	/// </summary>
	public static InputEventMap[] DefaultJoyEventMaps
	{
		get
		{
			// Primary mapping.
			InputEventMap m1 = new();
			m1[GameButton.Accept] = new InputEventJoypadButton() { ButtonIndex = JoyButton.B };
			m1[GameButton.Decline] = new InputEventJoypadButton() { ButtonIndex = JoyButton.Y };
			m1[GameButton.Context] = new InputEventJoypadButton() { ButtonIndex = JoyButton.X };
			m1[GameButton.Up] = new InputEventJoypadMotion() { Axis = JoyAxis.LeftY, AxisValue = -1f };
			m1[GameButton.Down] = new InputEventJoypadMotion() { Axis = JoyAxis.LeftY, AxisValue = 1f };
			m1[GameButton.Left] = new InputEventJoypadMotion() { Axis = JoyAxis.LeftX, AxisValue = -1f };
			m1[GameButton.Right] = new InputEventJoypadMotion() { Axis = JoyAxis.LeftX, AxisValue = 1f };

			// Secondary mapping.
			InputEventMap m2 = new();
			m2[GameButton.Accept] = new InputEventJoypadButton() { ButtonIndex = JoyButton.RightShoulder };
			m2[GameButton.Decline] = new InputEventJoypadButton() { ButtonIndex = JoyButton.LeftShoulder };
			m2[GameButton.Context] = new InputEventJoypadButton() { ButtonIndex = JoyButton.Start };
			m2[GameButton.Up] = new InputEventJoypadButton() { ButtonIndex = JoyButton.DpadUp };
			m2[GameButton.Down] = new InputEventJoypadButton() { ButtonIndex = JoyButton.DpadDown };
			m2[GameButton.Left] = new InputEventJoypadButton() { ButtonIndex = JoyButton.DpadLeft };
			m2[GameButton.Right] = new InputEventJoypadButton() { ButtonIndex = JoyButton.DpadRight };

			return [m1, m2];
		}
	}

	/// <summary>
	/// Returns whether a game button is currently held down
	/// </summary>
	/// <param name="button">The game button to check..</param>
	/// <param name="playerIndex">A zero-based index of the player to target (0-3). If left unspecified, player 0 is used.</param>
	/// <returns><see langword="true"/> if <paramref name="button"/> is down; <see langword="false"/> otherwise.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="playerIndex"/> is outside the valid range (0-3).</exception>
	public static bool IsDown(GameButton button, int playerIndex = 0)
	{
		static bool Predicate(GameButtonState state)
		{
			return (state == GameButtonState.Pressed) || (state == GameButtonState.Down);
		}
		
		return PlayerButtonSatisfiesPredicate(PlayerInputManager.Players[playerIndex], button, Predicate);
	}

	/// <summary>
	/// Returns whether a game button was pressed in the current frame.
	/// </summary>
	/// <param name="button">The game button to check.</param>
	/// <param name="playerIndex">A zero-based index of the player to target (0-3). If left unspecified, player 0 is used.</param>
	/// <returns><see langword="true"/> if <paramref name="button"/> was pressed in this frame; <see langword="false"/> otherwise.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="playerIndex"/> is outside the valid range (0-3).</exception>
	public static bool IsPressed(GameButton button, int playerIndex = 0)
	{
		static bool Predicate(GameButtonState state)
		{
			return state == GameButtonState.Pressed;
		}

		return PlayerButtonSatisfiesPredicate(PlayerInputManager.Players[playerIndex], button, Predicate);
	}

	/// <summary>
	/// Returns whether a game button was released in the current frame.
	/// </summary>
	/// <param name="button">The game button to check.</param>
	/// <param name="playerIndex">A zero-based index of the player to target (0-3). If left unspecified, player 0 is used.</param>
	/// <returns><see langword="true"/> if <paramref name="button"/> was released in this frame; <see langword="false"/> otherwise.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="playerIndex"/> is outside the valid range (0-3).</exception>
	public static bool IsReleased(GameButton button, int playerIndex = 0)
	{
		static bool Predicate(GameButtonState state)
		{
			return state == GameButtonState.Released;
		}

		return PlayerButtonSatisfiesPredicate(PlayerInputManager.Players[playerIndex], button, Predicate);
	}

	/// <summary>
	/// Creates a 2D vector that represents the current directional input.
	/// </summary>
	/// <param name="playerIndex">A zero-based index of the player to target (0-3). If left unspecified, player 0 is used.</param>
	/// <returns>A normalized <see cref="Vector2"/>, where X = (R - L) and Y = (D - U).</returns>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="playerIndex"/> is outside the valid range (0-3).</exception>
	public static Vector2 GetVector(int playerIndex = 0)
	{
		return new Vector2
		{
			X = (IsDown(GameButton.Right, playerIndex) ? 1f : 0f) - (IsDown(GameButton.Left, playerIndex) ? 1f : 0f),
			Y = (IsDown(GameButton.Down, playerIndex) ? 1f : 0f) - (IsDown(GameButton.Up, playerIndex) ? 1f : 0f)
		}.Normalized();
	}

	/// <summary>
	/// Determines which device type corresponds to the specified device ID.
	/// </summary>
	/// <param name="deviceId">The ID of the input device to check.</param>
	/// <returns>One of the <see cref="DeviceType"/> values.</returns>
	public static DeviceType GetDeviceType(int? deviceId)
	{
		if (deviceId == -1)
		{
			return DeviceType.Keyboard;
		}
		if (deviceId >= 0)
		{
			return DeviceType.Joypad;
		}

		return DeviceType.Unknown;
	}

	private static bool PlayerButtonSatisfiesPredicate(PlayerInput player, GameButton button, Predicate<GameButtonState> predicate)
	{
		if (player is null)
		{
			return false;
		}

		if (button == GameButton.Any)
		{
			for (int i = 0; i < (int)GameButton.Count; i++)
			{
				if (predicate(player.GetButtonState((GameButton)i)))
				{
					return true;
				}
			}

			return false;
		}
		if (button == GameButton.None)
		{
			for (int i = 0; i < (int)GameButton.Count; i++)
			{
				if (predicate(player.GetButtonState((GameButton)i)))
				{
					return false;
				}
			}

			return true;
		}
		
		return predicate(player.GetButtonState(button));
	}
}