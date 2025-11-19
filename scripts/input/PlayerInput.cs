using Godot;
using System;
using System.Collections.ObjectModel;
using GDInput = Godot.Input;

namespace Fixation.Input;

/// <summary>
/// Representation of a player in the input system. This class cannot be inherited.
/// </summary>
public sealed partial class PlayerInput : Node
{
	/// <summary>
	/// The ID of the input device assigned to the player.
	/// </summary>
	/// <exception cref="ArgumentException">Thrown when set to an invalid device ID.</exception>
	public int? DeviceId
	{
		get => _deviceId;
		set
		{
			if (value.HasValue)
			{
				int deviceId = value.Value;

				bool isKeyboard = deviceId == -1;
				bool isJoypad = GDInput.GetConnectedJoypads().Contains(deviceId);
				
				if ((!isKeyboard) && (!isJoypad))
				{
					throw new ArgumentException($"Device ID ({deviceId}) does not refer to a valid input device");
				}
			}
			
			_deviceId = value;
		}
	}

	/// <summary>
	/// The type of input device assigned to the player. This property is read-only.
	/// </summary>
	public DeviceType DeviceType
	{
		get
		{
			if (DeviceId == -1)
			{
				return DeviceType.Keyboard;
			}
			if (DeviceId >= 0)
			{
				return DeviceType.Joypad;
			}
			return DeviceType.Unknown;
		}
	}

	/// <summary>
	/// A read-only collection containing the primary (0) and secondary (1) keyboard event maps. This property is read-only.
	/// </summary>
	public ReadOnlyCollection<InputEventMap> KeyEventMaps { get; }

	/// <summary>
	/// A read-only collection containing the primary (0) and secondary (1) joypad event maps. This property is read-only.
	/// </summary>
	public ReadOnlyCollection<InputEventMap> JoyEventMaps { get; }
	
	/// <summary>
	/// A value between 0 and 1 representing the current joypad deadzone value.
	/// </summary>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when set to a value outside the valid range (0-1).</exception>
	public float Deadzone
	{
		get => _deadzone;
		set
		{
			if ((value < 0f) || (value > 1f))
			{
				throw new ArgumentOutOfRangeException(nameof(value), value, "Deadzone must be a value from 0.0 to 1.0");
			}

			_deadzone = value;
		}
	}

	/// <summary>
	/// Indicates whether or not the player has an input device connected. This property is read-only.
	/// </summary>
	public bool IsDeviceConnected => _deviceId is not null;

	/// <summary>
	/// Returns whether a game button is currently held down.
	/// </summary>
	/// <param name="button">The game button to check.</param>
	/// <returns><see langword="true"/> if <paramref name="button"/> is down; <see langword="false"/> otherwise.</returns>
	public bool IsDown(GameButton button)
	{
		static bool Predicate(GameButtonState state)
		{
			return (state == GameButtonState.Pressed) || (state == GameButtonState.Down);
		}

		return ButtonSatisfiesPredicate(button, Predicate);
	}

	/// <summary>
	/// Returns whether a game button was pressed in the current frame.
	/// </summary>
	/// <param name="button">The game button to check.</param>
	/// <returns><see langword="true"/> if <paramref name="button"/> was pressed in this frame; <see langword="false"/> otherwise.</returns>
	public bool IsPressed(GameButton button)
	{
		static bool Predicate(GameButtonState state)
		{
			return state == GameButtonState.Pressed;
		}

		return ButtonSatisfiesPredicate(button, Predicate);
	}

	/// <summary>
	/// Returns whether a game button was released in the current frame.
	/// </summary>
	/// <param name="button">The game button to check.</param>
	/// <returns><see langword="true"/> if <paramref name="button"/> was released in this frame; <see langword="false"/> otherwise.</returns>
	public bool IsReleased(GameButton button)
	{
		static bool Predicate(GameButtonState state)
		{
			return state == GameButtonState.Released;
		}

		return ButtonSatisfiesPredicate(button, Predicate);
	}

	/// <summary>
	/// Gets the current press state of the given button.
	/// </summary>
	/// <param name="button">The game button to query. The button must be real.</param>
	/// <returns>A <see cref="GameButtonState"/> representing the state of <paramref name="button"/>.</returns>
	/// <exception cref="ArgumentException">Thrown when <paramref name="button"/> is not a real button.</exception>
	public GameButtonState GetButtonState(GameButton button)
	{
		ValidateButtonIsReal(button);
		return GetButtonStateUnsafe((int)button);
	}

	/// <summary>
	/// Simulates the action of pressing a game button.
	/// </summary>
	/// <remarks>
	/// Calling this method when a button is already down does nothing.
	/// </remarks>
	/// <param name="button">The game button to press. The button must be real.</param>
	/// <exception cref="ArgumentException">Thrown when <paramref name="button"/> is not a real button.</exception>
	public void PressButton(GameButton button)
	{
		ValidateButtonIsReal(button);
		PressButtonUnsafe((int)button, Engine.IsInPhysicsFrame());
	}

	/// <summary>
	/// Simulates the action of releasing a game button.
	/// </summary>
	/// <remarks>
	/// Calling this method when a button is already up does nothing.
	/// </remarks>
	/// <param name="button">The game button to release. The button must be real.</param>
	/// <exception cref="ArgumentException">Thrown when <paramref name="button"/> is not a real button.</exception>
	public void ReleaseButton(GameButton button)
	{
		ValidateButtonIsReal(button);
		ReleaseButtonUnsafe((int)button, Engine.IsInPhysicsFrame());
	}

	/// <summary>
	/// Resets both keyboard and joypad event maps to their default values.
	/// </summary>
	public void ResetEventMaps()
	{
		InputEventMap[] keyMaps = InputService.DefaultKeyEventMaps;
		InputEventMap[] joyMaps = InputService.DefaultJoyEventMaps;

		for (int i = 0; i < 2; i++)
		{
			_keyMap[i].Copy(keyMaps[i]);
			_joyMap[i].Copy(joyMaps[i]);
		}
	}

	public override void _Process(double delta)
	{
		if (IsDeviceConnected)
		{
			switch (DeviceType)
			{
				case DeviceType.Keyboard:
					ProcessKeyboardInput();
					break;
				case DeviceType.Joypad:
					ProcessJoypadInput();
					break;
			}
		}
	}

	private PlayerInput()
	{
		_deviceId = null;
		_deadzone = InputService.DefaultDeadzone;

		_buttonStates = new ButtonState[(int)GameButton.Count];
		for (int i = 0; i < (int)GameButton.Count; i++)
		{
			_buttonStates[i] = new ButtonState();
		}

		_keyMap = InputService.DefaultKeyEventMaps;
		_joyMap = InputService.DefaultJoyEventMaps;

		KeyEventMaps = Array.AsReadOnly(_keyMap);
		JoyEventMaps = Array.AsReadOnly(_joyMap);
	}

	private static void ValidateButtonIsReal(GameButton button)
	{
		int buttonIndex = (int)button;
		if ((buttonIndex < 0) || (buttonIndex >= (int)GameButton.Count))
		{
			throw new ArgumentException($"Button '{button}' is not a real button", nameof(button));
		}
	}

	private GameButtonState GetButtonStateUnsafe(int buttonIndex)
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

	private void PressButtonUnsafe(int buttonIndex, bool inPhysicsFrame)
	{
		ButtonState state = _buttonStates[buttonIndex];
		if (!state.Down)
		{
			state.Down = true;
			state.PressedProcessFrame = Engine.GetProcessFrames();
			state.PressedPhysicsFrame = Engine.GetPhysicsFrames();

			if (inPhysicsFrame)
			{
				state.PressedPhysicsFrame++;
			}
		}
	}

	private void ReleaseButtonUnsafe(int buttonIndex, bool inPhysicsFrame)
	{
		ButtonState state = _buttonStates[buttonIndex];
		if (state.Down)
		{
			state.Down = false;
			state.ReleasedProcessFrame = Engine.GetProcessFrames();
			state.ReleasedPhysicsFrame = Engine.GetPhysicsFrames();

			if (inPhysicsFrame)
			{
				state.ReleasedPhysicsFrame++;
			}
		}
	}

	private void ProcessKeyboardInput()
	{
		static bool IsEventActive(InputEvent e)
		{
			if (e is InputEventKey key)
			{
				return GDInput.IsPhysicalKeyPressed(key.PhysicalKeycode);
			}
			if (e is InputEventMouseButton mbutton)
			{
				return GDInput.IsMouseButtonPressed(mbutton.ButtonIndex);
			}

			return false;
		}

		for (int i = 0; i < (int)GameButton.Count; i++)
		{
			var button = (GameButton)i;
			if (IsEventActive(_keyMap[0][button]) || IsEventActive(_keyMap[1][button]))
			{
				PressButtonUnsafe(i, false);
			}
			else
			{
				ReleaseButtonUnsafe(i, false);
			}
		}
	}

	private void ProcessJoypadInput()
	{
		static bool IsEventActive(InputEvent e, int deviceId, float deadzone)
		{
			if (e is InputEventJoypadButton jbutton)
			{
				return GDInput.IsJoyButtonPressed(deviceId, jbutton.ButtonIndex);
			}
			if (e is InputEventJoypadMotion jmotion)
			{
				float value = GDInput.GetJoyAxis(deviceId, jmotion.Axis);
				bool outsideDeadzone = MathF.Abs(value) >= deadzone;
				bool sameDirection = (value > 0f) == (jmotion.AxisValue > 0f);

				return outsideDeadzone && sameDirection;
			}

			return false;
		}

		for (int i = 0; i < (int)GameButton.Count; i++)
		{
			var deviceId = DeviceId!.Value;
			var button = (GameButton)i;
			if (IsEventActive(_joyMap[0][button], deviceId, Deadzone) || IsEventActive(_joyMap[1][button], deviceId, Deadzone))
			{
				PressButtonUnsafe(i, false);
			}
			else
			{
				ReleaseButtonUnsafe(i, false);
			}
		}
	}

	private bool ButtonSatisfiesPredicate(GameButton button, Predicate<GameButtonState> predicate)
	{
		if ((button == GameButton.Any) || (button == GameButton.None))
		{
			for (int i = 0; i < (int)GameButton.Count; i++)
			{
				if (predicate(GetButtonState((GameButton)i)))
				{
					return button == GameButton.Any;
				}
			}

			return button == GameButton.None;
		}

		return predicate(GetButtonState(button));
	}

	private int? _deviceId;
	private float _deadzone;
	private readonly ButtonState[] _buttonStates;
	private readonly InputEventMap[] _keyMap;
	private readonly InputEventMap[] _joyMap;
	
	// This class defines 4 timestamps to record when the player presses or releases a game button for the two game loops of the engine.
	// This is because InputManager.IsPressed/Released() can be called in _Process() and _PhysicsProcess(). The way these methods work
	// is by comparing if the current frame matches the recorded frame. Since the process and physics loops can go at different speeds,
	// their respective timestamps will be different from each other, which means InputManager.IsPressed/Released() will work when called
	// in the loop going faster, but not in the other.
	private class ButtonState
	{
		public bool Down;

		public ulong PressedProcessFrame;

		public ulong PressedPhysicsFrame;
		
		public ulong ReleasedProcessFrame = ulong.MaxValue;

		public ulong ReleasedPhysicsFrame = ulong.MaxValue;
	}
}
