using Godot;
using System;

public partial class Menu : Node2D
{
	public static GameMode SelectedMode = GameMode.Endless;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void PlayButtonPressed() {
		String path = "res://scenes/PlayMenu.tscn";
		PackedScene preparedScene = GD.Load<PackedScene>(path);
		GetTree().ChangeSceneToPacked(preparedScene);
	}

	public void QuitButtonPressed() {
		GetTree().Quit();
	}

	public void PlayEndlessPressed() {
		SelectedMode = GameMode.Endless;
		LoadGame();
	}

	public void PlayMoves10Pressed() {
		SelectedMode = GameMode.Moves10;
		LoadGame();
	}

	public void PlayMoves20Pressed() {
		SelectedMode = GameMode.Moves20;
		LoadGame();
	}

	public void PlayMoves50Pressed() {
		SelectedMode = GameMode.Moves50;
		LoadGame();
	}

	public void PlayTime60Pressed() {
		SelectedMode = GameMode.Time60;
		LoadGame();
	}

	public void HighScoresButtonPressed() {
		String path = "res://scenes/HighScores.tscn";
		PackedScene preparedScene = GD.Load<PackedScene>(path);
		GetTree().ChangeSceneToPacked(preparedScene);
	}

	public void SettingsButtonPressed() {
		String path = "res://scenes/SettingsMenu.tscn";
		PackedScene preparedScene = GD.Load<PackedScene>(path);
		GetTree().ChangeSceneToPacked(preparedScene);
	}

	private void LoadGame() {
		String path = "res://scenes/FirstMap.tscn";
		PackedScene preparedScene = GD.Load<PackedScene>(path);
		GetTree().ChangeSceneToPacked(preparedScene);
	}
}
