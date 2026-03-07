using Godot;
using System;

public partial class MusicManager : Node
{
	private AudioStreamPlayer _musicPlayer;
	private bool _musicEnabled = true;
	private const string SETTINGS_FILE = "user://settings.txt";

	public override void _Ready()
	{
		_musicPlayer = new AudioStreamPlayer();
		AddChild(_musicPlayer);
		_musicPlayer.Bus = "Master";
		
		LoadSettings();
		UpdateMusicState();
	}

	private void LoadSettings()
	{
		try
		{
			if (FileAccess.FileExists(SETTINGS_FILE))
			{
				using var file = FileAccess.Open(SETTINGS_FILE, FileAccess.ModeFlags.Read);
				if (file != null)
				{
					string content = file.GetAsText();
					_musicEnabled = content.Trim() != "0";
				}
			}
		}
		catch (Exception e)
		{
			GD.PrintErr("Failed to load settings: " + e.Message);
		}
	}

	public void SaveSettings()
	{
		try
		{
			using var file = FileAccess.Open(SETTINGS_FILE, FileAccess.ModeFlags.Write);
			if (file != null)
			{
				file.StoreString(_musicEnabled ? "1" : "0");
			}
		}
		catch (Exception e)
		{
			GD.PrintErr("Failed to save settings: " + e.Message);
		}
	}

	public bool IsMusicEnabled()
	{
		return _musicEnabled;
	}

	public void ToggleMusic()
	{
		_musicEnabled = !_musicEnabled;
		SaveSettings();
		UpdateMusicState();
	}

	public void SetMusicEnabled(bool enabled)
	{
		_musicEnabled = enabled;
		SaveSettings();
		UpdateMusicState();
	}

	private void UpdateMusicState()
	{
		if (_musicEnabled)
		{
			PlayMusic();
		}
		else
		{
			StopMusic();
		}
	}

	public void PlayMusic()
	{
		if (_musicEnabled && _musicPlayer != null && !_musicPlayer.Playing)
		{
			AudioStream music = GD.Load<AudioStream>("res://music/powerful-energy-upbeat-rock-advertising-music-245728.mp3");
			_musicPlayer.Stream = music;
			_musicPlayer.Play();
		}
	}

	public void StopMusic()
	{
		if (_musicPlayer != null && _musicPlayer.Playing)
		{
			_musicPlayer.Stop();
		}
	}
}
