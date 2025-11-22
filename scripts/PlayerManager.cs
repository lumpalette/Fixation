using Godot;
using System;
using System.Collections.ObjectModel;

namespace Fixation;

/// <summary>
/// A node that manages the player party and provides information about it. This class cannot be inherited.
/// </summary>
public sealed partial class PlayerManager : Node
{
	/// <summary>
	/// Occurs when a player is added to the party.
	/// </summary>
	public event EventHandler<PlayerPartyEventArgs> PartyMemberAdded;

	/// <summary>
	/// Occurs when a player is removed from the party.
	/// </summary>
	public event EventHandler<PlayerPartyEventArgs> PartyMemberRemoved;

	/// <summary>
	/// A read-only collection of all players that make up the current player party.
	/// </summary>
	public ReadOnlyCollection<Player> Party { get; }

	private readonly Player[] _party;
}
