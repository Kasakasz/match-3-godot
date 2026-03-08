using Godot;
using System;
using System.Collections.Generic;

public partial class HighScores : Node2D
{
	public enum ScoreDisplayMode
	{
		Moves10,
		Moves20,
		Moves50,
		Time60
	}
	
	private HighScoreManager scoreManager;
	private ScoreDisplayMode currentMode = ScoreDisplayMode.Moves10;
	private VBoxContainer tableContainer;
	private Label modeTitleLabel;

	public override void _Ready()
	{
		scoreManager = GetNode<HighScoreManager>("/root/HighScoreManager");
		tableContainer = GetNode<VBoxContainer>("ScorePanel/VBoxContainer");
		modeTitleLabel = GetNode<Label>("ScorePanel/ModeTitleLabel");
		LoadHighScores();
	}

	private void LoadHighScores()
	{
		GameMode mode = currentMode switch
		{
			ScoreDisplayMode.Moves10 => GameMode.Moves10,
			ScoreDisplayMode.Moves20 => GameMode.Moves20,
			ScoreDisplayMode.Moves50 => GameMode.Moves50,
			ScoreDisplayMode.Time60 => GameMode.Time60,
			_ => GameMode.Moves10
		};
		
		string modeName = currentMode switch
		{
			ScoreDisplayMode.Moves10 => "10 Moves",
			ScoreDisplayMode.Moves20 => "20 Moves",
			ScoreDisplayMode.Moves50 => "50 Moves",
			ScoreDisplayMode.Time60 => "60 Seconds",
			_ => "10 Moves"
		};
		
		modeTitleLabel.Text = modeName;
		
		foreach (Node child in tableContainer.GetChildren())
		{
			child.QueueFree();
		}
		
		var scores = scoreManager.GetTopScores(mode);
		
		HBoxContainer headerRow = CreateRow(true);
		CreateCell(headerRow, "#", 0.3f);
		CreateCell(headerRow, "SCORE", 1.5f);
		CreateCell(headerRow, "DATE", 2.0f);
		CreateCell(headerRow, "MOVES", 1.2f);
		CreateCell(headerRow, "COMBO", 1.2f);
		CreateCell(headerRow, "AVG/MOV", 1.3f);
		tableContainer.AddChild(headerRow);
		
		if (scores.Count == 0)
		{
			Label noScores = new Label
			{
				Text = "No scores yet",
				HorizontalAlignment = HorizontalAlignment.Center,
				SizeFlagsHorizontal = Control.SizeFlags.Fill,
				CustomMinimumSize = new Vector2(0, 100)
			};
			tableContainer.AddChild(noScores);
		}
		else
		{
			for (int i = 0; i < scores.Count; i++)
			{
				var entry = scores[i];
				float avg = entry.GetAverageScorePerMove();
				
				HBoxContainer row = CreateRow(false);
				CreateCell(row, (i + 1).ToString(), 0.3f);
				CreateCell(row, entry.score.ToString(), 1.5f);
				CreateCell(row, entry.dateTime, 2.0f);
				CreateCell(row, entry.movesUsed.ToString(), 1.2f);
				CreateCell(row, entry.maxCombo.ToString() + "x", 1.2f);
				CreateCell(row, avg.ToString("F1"), 1.3f);
				tableContainer.AddChild(row);
			}
		}
	}

	private HBoxContainer CreateRow(bool isHeader)
	{
		HBoxContainer row = new HBoxContainer();
		row.SizeFlagsHorizontal = Control.SizeFlags.Expand | Control.SizeFlags.Fill;
		row.CustomMinimumSize = new Vector2(0, isHeader ? 40 : 35);
		return row;
	}

	private void CreateCell(HBoxContainer parent, string text, float stretchRatio = 1.0f)
	{
		Label cell = new Label
		{
			Text = text,
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
			SizeFlagsHorizontal = Control.SizeFlags.Expand | Control.SizeFlags.Fill,
			SizeFlagsStretchRatio = stretchRatio
		};
		
		parent.AddChild(cell);
	}

	public void ShowMoves10()
	{
		currentMode = ScoreDisplayMode.Moves10;
		LoadHighScores();
	}

	public void ShowMoves20()
	{
		currentMode = ScoreDisplayMode.Moves20;
		LoadHighScores();
	}

	public void ShowMoves50()
	{
		currentMode = ScoreDisplayMode.Moves50;
		LoadHighScores();
	}

	public void ShowTime60()
	{
		currentMode = ScoreDisplayMode.Time60;
		LoadHighScores();
	}

	public void BackButtonPressed()
	{
		String path = "res://scenes/Menu.tscn";
		PackedScene preparedScene = GD.Load<PackedScene>(path);
		GetTree().ChangeSceneToPacked(preparedScene);
	}
}
