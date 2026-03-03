using Godot;
using System;
using System.Collections.Generic;

public partial class HighScores : Node2D
{
	private HighScoreManager scoreManager;
	private Label endlessLabel;
	private Label moves10Label;
	private Label moves20Label;
	private Label moves50Label;
	private Label time60Label;

	public override void _Ready()
	{
		scoreManager = GetNode<HighScoreManager>("/root/HighScoreManager");
		LoadHighScores();
	}

	private void LoadHighScores()
	{
		endlessLabel = GetNode<Label>("EndlessPanel/ScoreLabel");
		moves10Label = GetNode<Label>("Moves10Panel/ScoreLabel");
		moves20Label = GetNode<Label>("Moves20Panel/ScoreLabel");
		moves50Label = GetNode<Label>("Moves50Panel/ScoreLabel");
		time60Label = GetNode<Label>("Time60Panel/ScoreLabel");

		SetScoresForMode(GameMode.Endless, endlessLabel);
		SetScoresForMode(GameMode.Moves10, moves10Label);
		SetScoresForMode(GameMode.Moves20, moves20Label);
		SetScoresForMode(GameMode.Moves50, moves50Label);
		SetScoresForMode(GameMode.Time60, time60Label);
	}

	private void SetScoresForMode(GameMode mode, Label label)
	{
		var scores = scoreManager.GetTopScores(mode);
		if (scores.Count == 0)
		{
			label.Text = "No scores yet";
		}
		else
		{
			string text = "";
			for (int i = 0; i < scores.Count; i++)
			{
				text += (i + 1) + ". " + scores[i] + "\n";
			}
			label.Text = text;
		}
	}

	public void BackButtonPressed()
	{
		String path = "res://scenes/Menu.tscn";
		PackedScene preparedScene = GD.Load<PackedScene>(path);
		GetTree().ChangeSceneToPacked(preparedScene);
	}
}
