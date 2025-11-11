using Fixation.Input;
using Godot;

namespace Fixation.Tests;

public partial class InputTest : Sprite2D
{
	public override void _Ready()
	{
		if (!PlayerInputManager.Exists(_playerIndex))
		{
			PlayerInputManager.Create(_playerIndex, -1);
		}

		PlayerInputManager.Players[0].KeyEventMaps[0].Clear();
		PlayerInputManager.Players[1].KeyEventMaps[1].Clear();
	}

	public override void _Process(double delta)
	{
		if (InputService.IsPressed(GameButton.Context, _playerIndex))
		{
			GetTree().Quit();
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		Position += 2 * InputService.GetVector(_playerIndex);
	}

	[Export] private int _playerIndex;
}