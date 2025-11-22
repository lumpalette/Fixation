using System;

namespace Fixation;

/// <summary>
/// Provides data for the <see cref="PlayerManager.PartyMemberAdded"/> and <see cref="PlayerManager.PartyMemberRemoved"/> events.
/// </summary>
public class PlayerPartyEventArgs : EventArgs
{
	/// <summary>
	/// The player subject of the event. This property is read-only.
	/// </summary>
	public required Player Player { get; init; }

	/// <summary>
	/// The player index of the player. This property is read-only.
	/// </summary>
	public required int Index { get; init; }
}
