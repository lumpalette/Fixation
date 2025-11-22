namespace Fixation.Input;

/// <summary>
/// A wrapper around an input device identifier. This structure is read-only.
/// </summary>
public readonly struct Device
{
	/// <summary>
	/// A unique, numerical identifier for this input device. This property is read-only.
	/// </summary>
	public required int Id { get; init; }

	/// <summary>
	/// The type of this input device. This property is read-only.
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
}
