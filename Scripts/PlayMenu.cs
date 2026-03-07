using Godot;
using System;

public partial class PlayMenu : Node2D
{
	public void PlayEndlessPressed() {
		Menu.SelectedMode = GameMode.Endless;
		LoadGame();
	}

	public void PlayMoves10Pressed() {
		Menu.SelectedMode = GameMode.Moves10;
		LoadGame();
	}

	public void PlayMoves20Pressed() {
		Menu.SelectedMode = GameMode.Moves20;
		LoadGame();
	}

	public void PlayMoves50Pressed() {
		Menu.SelectedMode = GameMode.Moves50;
		LoadGame();
	}

	public void PlayTime60Pressed() {
		Menu.SelectedMode = GameMode.Time60;
		LoadGame();
	}

	public void BackButtonPressed() {
		String path = "res://scenes/Menu.tscn";
		PackedScene preparedScene = GD.Load<PackedScene>(path);
		GetTree().ChangeSceneToPacked(preparedScene);
	}

	private void LoadGame() {
		String path = "res://scenes/FirstMap.tscn";
		PackedScene preparedScene = GD.Load<PackedScene>(path);
		GetTree().ChangeSceneToPacked(preparedScene);
	}
}
