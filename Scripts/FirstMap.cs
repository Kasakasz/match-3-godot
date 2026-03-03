using Godot;
using System;

public partial class FirstMap : Node2D
{
	private Grid grid;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		grid = GetNode<Grid>("Grid");
		if (grid != null) {
			grid.gameMode = Menu.SelectedMode;
			grid.InitializeGame();
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void EndGameButtonClicked() {
		String path = "res://scenes/Menu.tscn";
		PackedScene preparedScene = GD.Load<PackedScene>(path);
		GetTree().ChangeSceneToPacked(preparedScene);
	}

	public void RestartButtonClicked() {
		GetTree().ReloadCurrentScene();
	}
}
