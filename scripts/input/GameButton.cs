namespace Fixation.Input;

/// <summary>
/// A list of identifiers representing the main actions of the game.
/// </summary>
/// <remarks>
/// <para>
/// This enumeration considers two types of buttons:
/// </para>
/// 
/// <para>
/// <b>Real buttons:</b> Buttons that describe a physical action inside the game. <br/>
/// The following buttons are considered real:
/// <list type="bullet">
///		<item><see cref="Accept"/></item>
///		<item><see cref="Decline"/></item>
///		<item><see cref="Context"/></item>
///		<item><see cref="Up"/></item>
///		<item><see cref="Down"/></item>
///		<item><see cref="Left"/></item>
///		<item><see cref="Right"/></item>
/// </list>
/// </para>
/// 
/// <para>
/// <b>Meta buttons:</b> Buttons that describe information about the enumeration. <br/>
/// The following buttons are considered meta:
/// <list type="bullet">
///		<item><see cref="Count"/></item>
///		<item><see cref="Any"/></item>
///		<item><see cref="None"/></item>
/// </list>
/// </para>
/// </remarks>
public enum GameButton
{
	/// <summary>
	/// The <c>Accept</c> button. Used to select options, jump in battle and overworld sections, skip dialogue, etc. This is a real button.
	/// </summary>
	/// <remarks>
	/// Default inputs:
	/// <list type="bullet">
	///		<item>
	///			<term>Keyboard</term>
	///			<description><c>[Z]</c> &amp; <c>[Enter]</c></description>
	///		</item>
	///		<item>
	///			<term>Joypad</term>
	///			<description><c>[Right Action]</c> &amp; <c>[Right Shoulder]</c></description>
	///		</item>
	/// </list>
	/// </remarks>
	Accept,

	/// <summary>
	/// The <c>Decline</c> button. Used to go back menus, slow down in battle and overworld sections, cancel selections, etc. This is a real button.
	/// </summary>
	/// <remarks>
	/// Default inputs:
	/// <list type="bullet">
	///		<item>
	///			<term>Keyboard</term>
	///			<description><c>[X]</c> &amp; <c>[Shift]</c></description>
	///		</item>
	///		<item>
	///			<term>Joypad</term>
	///			<description><c>[Top Action]</c> &amp; <c>[Left Shoulder]</c></description>
	///		</item>
	/// </list>
	/// </remarks>
	Decline,

	/// <summary>
	/// The <c>Context</c> button. Used for various purposes that depends on the current game context. This is a real button.
	/// </summary>
	/// <remarks>
	/// Default inputs:
	/// <list type="bullet">
	///		<item>
	///			<term>Keyboard</term>
	///			<description><c>[C]</c> &amp; <c>[Ctrl]</c></description>
	///		</item>
	///		<item>
	///			<term>Joypad</term>
	///			<description><c>[Left Action]</c> &amp; <c>[Start]</c></description>
	///		</item>
	/// </list>
	/// </remarks>
	Context,

	/// <summary>
	/// The <c>Up</c> button. Used for menu navigation, player movement, etc. This is a real button.
	/// </summary>
	/// <remarks>
	/// Default inputs:
	/// <list type="bullet">
	///		<item>
	///			<term>Keyboard</term>
	///			<description><c>[Up]</c> &amp; <c>[W]</c></description>
	///		</item>
	///		<item>
	///			<term>Joypad</term>
	///			<description><c>[Left Stick Up]</c> &amp; <c>[D-Pad Up]</c></description>
	///		</item>
	/// </list>
	/// </remarks>
	Up,

	/// <summary>
	/// The <c>Down</c> button. Used for menu navigation, player movement, etc. This is a real button.
	/// </summary>
	/// <remarks>
	/// Default inputs:
	/// <list type="bullet">
	///		<item>
	///			<term>Keyboard</term>
	///			<description><c>[Down]</c> &amp; <c>[S]</c></description>
	///		</item>
	///		<item>
	///			<term>Joypad</term>
	///			<description><c>[Left Stick Down]</c> &amp; <c>[D-Pad Down]</c></description>
	///		</item>
	/// </list>
	/// </remarks>
	Down,

	/// <summary>
	/// The <c>Left</c> button. Used for menu navigation, player movement, etc. This is a real button.
	/// </summary>
	/// <remarks>
	/// Default inputs:
	/// <list type="bullet">
	///		<item>
	///			<term>Keyboard</term>
	///			<description><c>[Left]</c> &amp; <c>[A]</c></description>
	///		</item>
	///		<item>
	///			<term>Joypad</term>
	///			<description><c>[Left Stick Left]</c> &amp; <c>[D-Pad Left]</c></description>
	///		</item>
	/// </list>
	/// </remarks>
	Left,

	/// <summary>
	/// The <c>Right</c> button. Used for menu navigation, player movement, etc. This is a real button.
	/// </summary>
	/// <remarks>
	/// Default inputs:
	/// <list type="bullet">
	///		<item>
	///			<term>Keyboard</term>
	///			<description><c>[Right]</c> &amp; <c>[D]</c></description>
	///		</item>
	///		<item>
	///			<term>Joypad</term>
	///			<description><c>[Left Stick Right]</c> &amp; <c>[D-Pad Right]</c></description>
	///		</item>
	/// </list>
	/// </remarks>
	Right,

	/// <summary>
	/// Represents the number of buttons defined in this enumeration. This is a meta button.
	/// </summary>
	Count,

	/// <summary>
	/// This is used to represent all game buttons using one value. This is a meta button.
	/// </summary>
	Any,

	/// <summary>
	/// This is used to represent no game button using one value. This is a meta button.
	/// </summary>
	None
}