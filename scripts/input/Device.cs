using System;

namespace Fixation.Input;

/// <summary>
/// A wrapper around a 32-bit signed integer that represents an input device identifier. This structure is read-only.
/// </summary>
public readonly record struct Device
{
	/// <summary>
	/// A unique, numerical identifier for this input device. This property is read-only.
	/// </summary>
	public required int Id { get; init; }

	/// <summary>
	/// The type of input device this structure represents.
	/// </summary>
	public DeviceType Type
	{
		get
		{
			if (Id == -1)
			{
				return DeviceType.Keyboard;
			}
			if (Id >= 0)
			{
				return DeviceType.Joypad;
			}

			return DeviceType.Unknown;
		}
	}

	/// <summary>
	/// Returns whether this input device is considered valid, i.e., whether it represents a connected device.
	/// </summary>
	/// <returns><see langword="true"/> if the structure is valid; otherwise, <see langword="false"/>.</returns>
	public bool IsValid()
	{
		return (Id == -1) || Godot.Input.GetConnectedJoypads().Contains(Id);
	}

	/// <summary>
	/// Creates a device that represents the system keyboard.
	/// </summary>
	/// <returns>A <see cref="Device"/> representing the keyboard.</returns>
	public static Device CreateKeyboard()
	{
		return new Device() { Id = -1 };
	}

	/// <summary>
	/// Creates a joypad device for a native joypad identifier.
	/// </summary>
	/// <param name="id">The joypad identifier to create a device for.</param>
	/// <returns>A <see cref="Device"/> representing a joypad.</returns>
	/// <exception cref="ArgumentException">Thrown if <paramref name="id"/> does not refer to a valid joypad.</exception>
	public static Device CreateJoypad(int id)
	{
		Device device = new() { Id = id };
		if (!device.IsValid())
		{
			throw new ArgumentException($"Invalid joypad ID ({id})", nameof(id));
		}

		return device;
	}
}
