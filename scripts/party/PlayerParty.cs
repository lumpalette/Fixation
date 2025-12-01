using Godot;
using System;
using System.Collections.Generic;

namespace Fixation.Party;

/// <summary>
/// A node that represents the current player party. This class cannot be inherited.
/// </summary>
public sealed partial class PlayerParty : Node
{
	private readonly Player[] _members;

	private PlayerParty()
	{
		_members = new Player[Game.MaxPlayerCount];
	}

	/// <summary>
	/// Occurs when a player is added to the party.
	/// </summary>
	public event EventHandler<PartyMemberEventArgs> MemberAdded;

	/// <summary>
	/// Occurs when a player is removed from the party.
	/// </summary>
	public event EventHandler<PartyMemberEventArgs> MemberRemoved;

	/// <summary>
	/// Gets the player assigned to the specified slot.
	/// </summary>
	/// <param name="slot">The slot to access.</param>
	/// <returns>The <see cref="Player"/> assigned to the <paramref name="slot"/>, or <see langword="null"/> is the slot is unoccupied.</returns>
	public Player this[PlayerSlot slot] => _members[slot];

	/// <summary>
	/// The current number of party members. This property is read-only.
	/// </summary>
	public int Size { get; private set; }

	/// <summary>
	/// Assigns a player to the specified slot in the party.
	/// </summary>
	/// <remarks>
	/// If the player is successfully added, the <see cref="MemberAdded"/> event is raised.
	/// </remarks>
	/// <param name="slot">The slot to assign the player to.</param>
	/// <param name="player">The player to be assigned.</param>
	/// <exception cref="InvalidOperationException">Thrown if the <paramref name="slot"/> is already occupied by another player.</exception>
	public void AssignMember(PlayerSlot slot, Player player)
	{
		if (IsSlotOccupied(slot))
		{
			throw new InvalidOperationException($"Party slot {slot} is already occupied by player '{_members[slot].Name}'");
		}

		_members[slot] = player;
		Size++;
		
		MemberAdded?.Invoke(this, new PartyMemberEventArgs()
		{
			Player = player,
			Slot = slot
		});
	}

	/// <summary>
	/// Removes the player from the specified slot in the party.
	/// </summary>
	/// <remarks>
	/// <para>
	/// If the specified slot is already empty, the method call is ignored.
	/// </para>
	/// <para>
	/// If the player is successfully removed, the <see cref="MemberRemoved"/> event is raised.
	/// </para>
	/// </remarks>
	/// <param name="slot">The slot from which to remove the player.</param>
	public void RemoveMember(PlayerSlot slot)
	{
		Player member = _members[slot];
		if (member is null)
		{
			return;
		}

		_members[slot] = null;
		Size--;

		MemberRemoved?.Invoke(this, new PartyMemberEventArgs()
		{
			Player = member,
			Slot = slot
		});
	}

	/// <summary>
	/// Returns whether or not a player is assigned at the specified slot.
	/// </summary>
	/// <param name="slot">The slot to check.</param>
	/// <returns><see langword="true"/> if the <paramref name="slot"/> is occupied; otherwise, <see langword="false"/>.</returns>
	public bool IsSlotOccupied(PlayerSlot slot)
	{
		return _members[slot] is not null;
	}

	/// <summary>
	/// Gets a sequence of all occupied player slots in the party.
	/// </summary>
	/// <returns>A sequence of occupied player slots.</returns>
	public IEnumerable<PlayerSlot> GetOccupiedSlots()
	{
		for (byte i = 0; i < Game.MaxPlayerCount; i++)
		{
			if (IsSlotOccupied(i))
			{
				yield return i;
			}
		}
	}
}
