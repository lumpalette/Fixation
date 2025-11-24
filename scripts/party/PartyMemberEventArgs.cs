using System;

namespace Fixation.Party;

/// <summary>
/// Provides data for the <see cref="PlayerParty.MemberAdded"/> and <see cref="PlayerParty.MemberRemoved"/> events.
/// </summary>
public class PartyMemberEventArgs : EventArgs
{
	/// <summary>
	/// The player subject of the event. This property is read-only.
	/// </summary>
	public required Player Player { get; init; }

	/// <summary>
	/// The party slot of the player. This property is read-only.
	/// </summary>
	public required PlayerSlot Slot { get; init; }
}
