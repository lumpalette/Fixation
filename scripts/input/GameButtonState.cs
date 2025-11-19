namespace Fixation.Input;

/// <summary>
/// The states in which a game button can be during a frame.
/// </summary>
public enum GameButtonState
{
	/// <summary>
	/// The button is up.
	/// </summary>
	Up,

	/// <summary>
	/// The button was pressed in this frame.
	/// </summary>
	Pressed,

	/// <summary>
	/// The button is down.
	/// </summary>
	Down,

	/// <summary>
	/// The button was released in this frame.
	/// </summary>
	Released
}
