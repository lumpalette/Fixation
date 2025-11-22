using Godot;
using System;
using System.Collections.ObjectModel;

namespace Fixation.Input;

/// <summary>
/// A collection of default values for the input system. This class cannot be inherited.
/// </summary>
public static class InputDefaults
{
	/// <summary>
	/// The default deadzone value for all joypads.
	/// </summary>
	public const float Deadzone = 0.2f;

	/// <summary>
	/// A read-only collection containing the default primary (0) and secondary (1) event maps for keyboard input.
	/// </summary>
	public static ReadOnlyCollection<InputEventMap> KeyEventMaps { get; }

	/// <summary>
	/// A read-only collection containing the default primary (0) and secondary (1) event maps for joypad input.
	/// </summary>
	public static ReadOnlyCollection<InputEventMap> JoyEventMaps { get; }

	static InputDefaults()
	{
		#region Input mapping

		// Keyboard mappings.
		InputEventMap k1 = new();
		k1[GameButton.Accept] = new InputEventKey() { PhysicalKeycode = Key.Z };
		k1[GameButton.Decline] = new InputEventKey() { PhysicalKeycode = Key.X };
		k1[GameButton.Context] = new InputEventKey() { PhysicalKeycode = Key.C };
		k1[GameButton.Up] = new InputEventKey() { PhysicalKeycode = Key.Up };
		k1[GameButton.Down] = new InputEventKey() { PhysicalKeycode = Key.Down };
		k1[GameButton.Left] = new InputEventKey() { PhysicalKeycode = Key.Left };
		k1[GameButton.Right] = new InputEventKey() { PhysicalKeycode = Key.Right };

		InputEventMap k2 = new();
		k2[GameButton.Accept] = new InputEventKey() { PhysicalKeycode = Key.Enter };
		k2[GameButton.Decline] = new InputEventKey() { PhysicalKeycode = Key.Shift };
		k2[GameButton.Context] = new InputEventKey() { PhysicalKeycode = Key.Ctrl };
		k2[GameButton.Up] = new InputEventKey() { PhysicalKeycode = Key.W };
		k2[GameButton.Down] = new InputEventKey() { PhysicalKeycode = Key.S };
		k2[GameButton.Left] = new InputEventKey() { PhysicalKeycode = Key.A };
		k2[GameButton.Right] = new InputEventKey() { PhysicalKeycode = Key.D };

		KeyEventMaps = Array.AsReadOnly([k1, k2]);

		// Joypad mappings.
		InputEventMap j1 = new();
		j1[GameButton.Accept] = new InputEventJoypadButton() { ButtonIndex = JoyButton.B };
		j1[GameButton.Decline] = new InputEventJoypadButton() { ButtonIndex = JoyButton.Y };
		j1[GameButton.Context] = new InputEventJoypadButton() { ButtonIndex = JoyButton.X };
		j1[GameButton.Up] = new InputEventJoypadMotion() { Axis = JoyAxis.LeftY, AxisValue = -1f };
		j1[GameButton.Down] = new InputEventJoypadMotion() { Axis = JoyAxis.LeftY, AxisValue = 1f };
		j1[GameButton.Left] = new InputEventJoypadMotion() { Axis = JoyAxis.LeftX, AxisValue = -1f };
		j1[GameButton.Right] = new InputEventJoypadMotion() { Axis = JoyAxis.LeftX, AxisValue = 1f };

		InputEventMap j2 = new();
		j2[GameButton.Accept] = new InputEventJoypadButton() { ButtonIndex = JoyButton.RightShoulder };
		j2[GameButton.Decline] = new InputEventJoypadButton() { ButtonIndex = JoyButton.LeftShoulder };
		j2[GameButton.Context] = new InputEventJoypadButton() { ButtonIndex = JoyButton.Start };
		j2[GameButton.Up] = new InputEventJoypadButton() { ButtonIndex = JoyButton.DpadUp };
		j2[GameButton.Down] = new InputEventJoypadButton() { ButtonIndex = JoyButton.DpadDown };
		j2[GameButton.Left] = new InputEventJoypadButton() { ButtonIndex = JoyButton.DpadLeft };
		j2[GameButton.Right] = new InputEventJoypadButton() { ButtonIndex = JoyButton.DpadRight };

		JoyEventMaps = Array.AsReadOnly([j1, j2]);

		#endregion
	}
}
