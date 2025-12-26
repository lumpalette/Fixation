using Fixation.Input;
using Godot;

namespace Fixation.Tests;

public partial class InputTest : Sprite2D
{
	[Export]
	private byte _playerSlot;
	
	[Export]
	private string _playerName;
	
	[Export]
	private Color _playerColor;
	
	[Export]
	private int _deviceId;

	public override void _Ready()
	{
		if (!Game.Party.IsSlotOccupied(_playerSlot))
		{
			Game.Party.AssignMember(_playerSlot, new Player() { Name = _playerName, Color = _playerColor });
		}
		
		Game.Input[_playerSlot].Device = new Device() { Id = _deviceId };

		Modulate = _playerColor;
	}

	public override void _Process(double delta)
	{
		if (Game.Input.IsReleased(GameButton.Accept, _playerSlot))
		{
			GetTree().Quit();
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		Position += 2f * Game.Input.GetVector(_playerSlot);
	}
}
