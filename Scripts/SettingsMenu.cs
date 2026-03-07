using Godot;
using System;

public partial class SettingsMenu : Node2D
{
	private CheckButton _musicCheck;
	private Label _musicLabel;

	public override void _Ready()
	{
		_musicCheck = GetNode<CheckButton>("MusicCheck");
		_musicLabel = GetNode<Label>("MusicLabel");
		
		MusicManager musicManager = GetNode<MusicManager>("/root/MusicManager");
		if (musicManager != null)
		{
			_musicCheck.ButtonPressed = musicManager.IsMusicEnabled();
		}
		
		_musicCheck.Toggled += OnMusicToggled;
	}

	private void OnMusicToggled(bool toggledOn)
	{
		MusicManager musicManager = GetNode<MusicManager>("/root/MusicManager");
		if (musicManager != null)
		{
			musicManager.SetMusicEnabled(toggledOn);
		}
	}

	public void BackButtonPressed()
	{
		String path = "res://scenes/Menu.tscn";
		PackedScene preparedScene = GD.Load<PackedScene>(path);
		GetTree().ChangeSceneToPacked(preparedScene);
	}
}
