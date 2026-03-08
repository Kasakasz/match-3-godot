using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

public class HighScoreEntry
{
	public int score;
	public string dateTime;
	public int movesUsed;
	public int maxCombo;

	public HighScoreEntry() { }

	public HighScoreEntry(int score, int movesUsed, int maxCombo)
	{
		this.score = score;
		this.dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
		this.movesUsed = movesUsed;
		this.maxCombo = maxCombo;
	}

	public float GetAverageScorePerMove()
	{
		if (movesUsed <= 0) return 0;
		return (float)score / movesUsed;
	}
}

public partial class HighScoreManager : Node
{
	private const int MaxScoresPerMode = 10;
	private Dictionary<GameMode, List<HighScoreEntry>> highScores = new();
	private string savePath = "user://highscores.json";

	public override void _Ready()
	{
		LoadScores();
	}

	public void AddScore(GameMode mode, int score, int movesUsed, int maxCombo)
	{
		if (!highScores.ContainsKey(mode))
		{
			highScores[mode] = new List<HighScoreEntry>();
		}

		highScores[mode].Add(new HighScoreEntry(score, movesUsed, maxCombo));
		highScores[mode] = highScores[mode].OrderByDescending(s => s.score).Take(MaxScoresPerMode).ToList();
		SaveScores();
	}

	public List<HighScoreEntry> GetTopScores(GameMode mode)
	{
		if (!highScores.ContainsKey(mode))
		{
			return new List<HighScoreEntry>();
		}
		return highScores[mode].OrderByDescending(s => s.score).Take(MaxScoresPerMode).ToList();
	}

	public bool IsHighScore(GameMode mode, int score)
	{
		var scores = GetTopScores(mode);
		if (scores.Count < MaxScoresPerMode)
		{
			return true;
		}
		return score > scores.Min(s => s.score);
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
			
			var wrapper = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, HighScoreEntryJson>>>(json);
			if (wrapper != null)
			{
				highScores = new Dictionary<GameMode, List<HighScoreEntry>>();
				foreach (var modeKvp in wrapper)
				{
					if (Enum.TryParse<GameMode>(modeKvp.Key, out var mode))
					{
						highScores[mode] = new List<HighScoreEntry>();
						foreach (var entry in modeKvp.Value.Values)
						{
							var entryObj = new HighScoreEntry
							{
								score = entry.score,
								dateTime = entry.dateTime,
								movesUsed = entry.movesUsed,
								maxCombo = entry.maxCombo
							};
							highScores[mode].Add(entryObj);
						}
					}
				}
			}
		}
		catch (Exception e)
		{
			GD.PrintErr("Failed to load high scores: " + e.Message);
		}
	}

	private Dictionary<string, Dictionary<string, HighScoreEntryJson>> ToSerializableDict()
	{
		var dict = new Dictionary<string, Dictionary<string, HighScoreEntryJson>>();
		foreach (var kvp in highScores)
		{
			dict[kvp.Key.ToString()] = new Dictionary<string, HighScoreEntryJson>();
			int index = 0;
			foreach (var entry in kvp.Value)
			{
				dict[kvp.Key.ToString()][index.ToString()] = new HighScoreEntryJson
				{
					score = entry.score,
					dateTime = entry.dateTime,
					movesUsed = entry.movesUsed,
					maxCombo = entry.maxCombo
				};
				index++;
			}
		}
		return dict;
	}

	private class HighScoreEntryJson
	{
		public int score { get; set; }
		public string dateTime { get; set; }
		public int movesUsed { get; set; }
		public int maxCombo { get; set; }
	}
}
