using Godot;
using System;
using System.Collections.ObjectModel;

namespace Fixation.Input;

/// <summary>
/// Representation of a player in the input system. Contains an input device, mappings and other settings. This class cannot be inherited.
/// </summary>
public sealed partial class PlayerInput
{
	private float _deadzoneField;
	private readonly ButtonState[] _buttonStates;
	private readonly InputEventMap[] _keyMaps;
	private readonly InputEventMap[] _joyMaps;

	/// <summary>
	/// Creates a new <see cref="PlayerInput"/> with no input device assigned.
	/// </summary>
	public PlayerInput()
	{
		_deadzoneField = InputDefaults.Deadzone;

		_buttonStates = new ButtonState[(int)GameButton.Count];
		for (int i = 0; i < _buttonStates.Length; i++)
		{
			_buttonStates[i] = new ButtonState();
		}

		_keyMaps = [.. InputDefaults.KeyEventMaps];
		_joyMaps = [.. InputDefaults.JoyEventMaps];

		KeyEventMaps = Array.AsReadOnly(_keyMaps);
		JoyEventMaps = Array.AsReadOnly(_joyMaps);
	}

	/// <summary>
	/// The input device assigned to the player.
	/// </summary>
	public Device? Device { get; set; }
	
	/// <summary>
	/// The current deadzone value, normalized.
	/// </summary>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when it's set to a value outside the valid range (0-1).</exception>
	public float Deadzone
	{
		get => _deadzoneField;
		set
		{
			if ((value < 0f) || (value > 1f))
			{
				throw new ArgumentOutOfRangeException(nameof(value), value, "Deadzone must be normalized");
			}

			_deadzoneField = value;
		}
	}
	
	/// <summary>
	/// A read-only collection containing the primary (0) and secondary (1) event maps for keyboard input.
	/// </summary>
	public ReadOnlyCollection<InputEventMap> KeyEventMaps { get; }

	/// <summary>
	/// A read-only collection containing the primary (0) and secondary (1) event maps for joypad input.
	/// </summary>
	public ReadOnlyCollection<InputEventMap> JoyEventMaps { get; }

	/// <summary>
	/// Updates the input state of the player.
	/// </summary>
	public void Update()
	{
		if (Device?.IsValid() != true)
		{
			return;
		}

		unsafe
		{
			switch (Device.Value.Type)
			{
				case DeviceType.Keyboard:
					UpdateButtons(_keyMaps, &IsKeyEventActive);
					break;
				case DeviceType.Joypad:
					UpdateButtons(_joyMaps, &IsJoyEventActive);
					break;
			}
		}
	}

	/// <summary>
	/// Gets the current state of the given button.
	/// </summary>
	/// <param name="button">The game button to query. The button must be real.</param>
	/// <returns>A <see cref="GameButtonState"/> representing the state of the <paramref name="button"/>.</returns>
	/// <exception cref="ArgumentException">Thrown if <paramref name="button"/> is not a real button.</exception>
	public GameButtonState GetButtonState(GameButton button)
	{
		button.EnsureIsReal();
		return GetButtonStateInternal((int)button);
	}

	/// <summary>
	/// Simulates a game button press.
	/// </summary>
	/// <remarks>
	/// Calling this method when a button is already down does nothing.
	/// </remarks>
	/// <param name="button">The game button to press. The button must be real.</param>
	/// <exception cref="ArgumentException">Thrown if <paramref name="button"/> is not a real button.</exception>
	public void PressButton(GameButton button)
	{
		button.EnsureIsReal();
		PressButtonInternal((int)button, Engine.IsInPhysicsFrame());
	}

	/// <summary>
	/// Simulates a game button release.
	/// </summary>
	/// <remarks>
	/// Calling this method when a button is already up does nothing.
	/// </remarks>
	/// <param name="button">The game button to release. The button must be real.</param>
	/// <exception cref="ArgumentException">Thrown if <paramref name="button"/> is not a real button.</exception>
	public void ReleaseButton(GameButton button)
	{
		button.EnsureIsReal();
		ReleaseButtonInternal((int)button, Engine.IsInPhysicsFrame());
	}

	/// <summary>
	/// Resets both keyboard and joypad event maps to their default values.
	/// </summary>
	public void ResetEventMaps()
	{
		for (int i = 0; i < 2; i++)
		{
			KeyEventMaps[i].Copy(InputDefaults.KeyEventMaps[i]);
			JoyEventMaps[i].Copy(InputDefaults.JoyEventMaps[i]);
		}
	}

	private unsafe void UpdateButtons(InputEventMap[] maps, delegate* managed<InputEvent, PlayerInput, bool> eventActive)
	{
		for (int i = 0; i < (int)GameButton.Count; i++)
		{
			var button = (GameButton)i;
			if (eventActive(maps[0][button], this) || eventActive(maps[1][button], this))
			{
				PressButtonInternal(i, false);
			}
			else
			{
				ReleaseButtonInternal(i, false);
			}
		}
	}

	private GameButtonState GetButtonStateInternal(int buttonIndex)
	{
		ButtonState state = _buttonStates[buttonIndex];
		if (Engine.IsInPhysicsFrame())
		{
			if (state.Down)
			{
				return (Engine.GetPhysicsFrames() == state.PressedPhysicsFrame) ? GameButtonState.Pressed : GameButtonState.Down;
			}

			return (Engine.GetPhysicsFrames() == state.ReleasedPhysicsFrame) ? GameButtonState.Released : GameButtonState.Up;
		}
		else
		{
			if (state.Down)
			{
				return (Engine.GetProcessFrames() == state.PressedProcessFrame) ? GameButtonState.Pressed : GameButtonState.Down;
			}

			return (Engine.GetProcessFrames() == state.ReleasedProcessFrame) ? GameButtonState.Released : GameButtonState.Up;
		}
	}

	private void PressButtonInternal(int buttonIndex, bool inPhysicsFrame)
	{
		ButtonState button = _buttonStates[buttonIndex];
		if (!button.Down)
		{
			button.Down = true;
			button.PressedProcessFrame = Engine.GetProcessFrames();
			button.PressedPhysicsFrame = Engine.GetPhysicsFrames();

			if (!inPhysicsFrame)
			{
				button.PressedPhysicsFrame++;
			}
		}
	}

	private void ReleaseButtonInternal(int buttonIndex, bool inPhysicsFrame)
	{
		ButtonState button = _buttonStates[buttonIndex];
		if (button.Down)
		{
			button.Down = false;
			button.ReleasedProcessFrame = Engine.GetProcessFrames();
			button.ReleasedPhysicsFrame = Engine.GetPhysicsFrames();

			if (!inPhysicsFrame)
			{
				button.ReleasedPhysicsFrame++;
			}
		}
	}

	// we put a PlayerInput parameter here so the method matches the function pointer signature of UpdateButtons().
	private static bool IsKeyEventActive(InputEvent e, PlayerInput myself)
	{
		return ((e is InputEventKey key) && Godot.Input.IsPhysicalKeyPressed(key.PhysicalKeycode))
			|| ((e is InputEventMouseButton mbutton) && Godot.Input.IsMouseButtonPressed(mbutton.ButtonIndex));
	}

	// we can't make this an instance method because a static reference is required for UpdateButtons().
	private static bool IsJoyEventActive(InputEvent e, PlayerInput myself)
	{
		if (e is InputEventJoypadButton jbutton)
		{
			return Godot.Input.IsJoyButtonPressed(myself.Device!.Value.Id, jbutton.ButtonIndex);
		}
		if (e is InputEventJoypadMotion jmotion)
		{
			float value = Godot.Input.GetJoyAxis(myself.Device!.Value.Id, jmotion.Axis);
			bool outsideDeadzone = MathF.Abs(value) >= myself.Deadzone;
			bool sameDirection = (value > 0f) == (jmotion.AxisValue > 0f);

			return outsideDeadzone && sameDirection;
		}

		return false;
	}
}

partial class PlayerInput
{
	// This class defines 4 timestamps to record when the player presses or releases a game button for the two game loops of the engine.
	// This is because InputManager.IsPressed/Released() can be called in _Process() and _PhysicsProcess(). The way these methods work
	// is by comparing if the current frame matches the recorded frame. Since the process and physics loops can go at different speeds,
	// their respective timestamps will be different from each other, which means InputManager.IsPressed/Released() will work when called
	// in the loop going faster, but not in the other.
	private class ButtonState
	{
		// Whether the button is currently held down or not.
		public bool Down;

		// The process frame in which this button was pressed.
		public ulong PressedProcessFrame;

		// The process frame in which this button was released.
		public ulong PressedPhysicsFrame;

		// The physics frame in which this button was pressed.
		public ulong ReleasedProcessFrame = ulong.MaxValue;

		// The physics frame in which this button was released.
		public ulong ReleasedPhysicsFrame = ulong.MaxValue;
	}
}
