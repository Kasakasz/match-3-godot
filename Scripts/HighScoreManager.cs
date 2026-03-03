using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

public partial class HighScoreManager : Node
{
	private const int MaxScoresPerMode = 5;
	private Dictionary<GameMode, List<int>> highScores = new();
	private string savePath = "user://highscores.json";

	public override void _Ready()
	{
		LoadScores();
	}

	public void AddScore(GameMode mode, int score)
	{
		if (!highScores.ContainsKey(mode))
		{
			highScores[mode] = new List<int>();
		}

		highScores[mode].Add(score);
		highScores[mode] = highScores[mode].OrderByDescending(s => s).Take(MaxScoresPerMode).ToList();
		SaveScores();
	}

	public List<int> GetTopScores(GameMode mode)
	{
		if (!highScores.ContainsKey(mode))
		{
			return new List<int>();
		}
		return highScores[mode].OrderByDescending(s => s).Take(MaxScoresPerMode).ToList();
	}

	public bool IsHighScore(GameMode mode, int score)
	{
		var scores = GetTopScores(mode);
		if (scores.Count < MaxScoresPerMode)
		{
			return true;
		}
		return score > scores.Min();
	}

	private void SaveScores()
	{
		try
		{
			var json = JsonSerializer.Serialize(ToSerializableDict());
			using var file = FileAccess.Open(savePath, FileAccess.ModeFlags.Write);
			file.StoreString(json);
		}
		catch (Exception e)
		{
			GD.PrintErr("Failed to save high scores: " + e.Message);
		}
	}

	private void LoadScores()
	{
		try
		{
			if (!FileAccess.FileExists(savePath))
			{
				return;
			}
			using var file = FileAccess.Open(savePath, FileAccess.ModeFlags.Read);
			string json = file.GetAsText();
			var dict = JsonSerializer.Deserialize<Dictionary<string, List<int>>>(json);
			if (dict != null)
			{
				highScores = new Dictionary<GameMode, List<int>>();
				foreach (var kvp in dict)
				{
					if (Enum.TryParse<GameMode>(kvp.Key, out var mode))
					{
						highScores[mode] = kvp.Value;
					}
				}
			}
		}
		catch (Exception e)
		{
			GD.PrintErr("Failed to load high scores: " + e.Message);
		}
	}

	private Dictionary<string, List<int>> ToSerializableDict()
	{
		var dict = new Dictionary<string, List<int>>();
		foreach (var kvp in highScores)
		{
			dict[kvp.Key.ToString()] = kvp.Value;
		}
		return dict;
	}
}
