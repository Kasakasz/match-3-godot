using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public enum GameMode
{
	Endless,
	Moves10,
	Moves20,
	Moves50,
	Time60
}

public partial class Grid : Node2D
{
	[Export] public int width;
	[Export] public int height;
	[Export] public int x_start;
	[Export] public int y_start;
	[Export] public int offset;

	[Export] public GameMode gameMode = GameMode.Endless;

	private Block selectedBlock;
	private Vector2 selectedGridPos;
	private int score;
	private int movesLeft;
	private float timeLeft;
	private bool gameOver = false;
	private Label scoreLabel;
	private Label movesLabel;
	private Label timeLabel;
	private Timer gameTimer;
	private Control gameOverPanel;
	private Dictionary<string, Texture2D> gemTextures = new();

	public List<PackedScene> possiblePieces = new()
	{
		GD.Load<PackedScene>("res://blocks/blueblock.tscn"),
		GD.Load<PackedScene>("res://blocks/redblock.tscn"),
		GD.Load<PackedScene>("res://blocks/yellowblock.tscn"),
		GD.Load<PackedScene>("res://blocks/brownblock.tscn"),
		GD.Load<PackedScene>("res://blocks/greenblock.tscn"),
		GD.Load<PackedScene>("res://blocks/indykblock.tscn"),
	};

	private List<List<Block>> grid = new();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {

		GD.Randomize();
		grid = Make2dArray();
		LoadTextures();
		SpawnPieces();
		
		scoreLabel = GetNode<Label>("../ScoreLabel");
		movesLabel = GetNode<Label>("../MovesLabel");
		timeLabel = GetNode<Label>("../TimeLabel");
		gameOverPanel = GetNode<Control>("../GameOverPanel");
		gameTimer = GetNode<Timer>("../GameTimer");
		
		UpdateScoreLabel();
	}

	public void InitializeGame() {
		InitializeGameMode();
	}

	private void InitializeGameMode() {
		gameOver = false;
		score = 0;
		
		switch (gameMode) {
			case GameMode.Endless:
				movesLeft = -1;
				timeLeft = -1;
				if (movesLabel != null) movesLabel.Visible = false;
				if (timeLabel != null) timeLabel.Visible = false;
				if (gameTimer != null) gameTimer.Stop();
				break;
			case GameMode.Moves10:
				movesLeft = 10;
				timeLeft = -1;
				if (movesLabel != null) {
					movesLabel.Visible = true;
					UpdateMovesLabel();
				}
				if (timeLabel != null) timeLabel.Visible = false;
				if (gameTimer != null) gameTimer.Stop();
				break;
			case GameMode.Moves20:
				movesLeft = 20;
				timeLeft = -1;
				if (movesLabel != null) {
					movesLabel.Visible = true;
					UpdateMovesLabel();
				}
				if (timeLabel != null) timeLabel.Visible = false;
				if (gameTimer != null) gameTimer.Stop();
				break;
			case GameMode.Moves50:
				movesLeft = 50;
				timeLeft = -1;
				if (movesLabel != null) {
					movesLabel.Visible = true;
					UpdateMovesLabel();
				}
				if (timeLabel != null) timeLabel.Visible = false;
				if (gameTimer != null) gameTimer.Stop();
				break;
				case GameMode.Time60:
				movesLeft = -1;
				timeLeft = 60f;
				if (movesLabel != null) movesLabel.Visible = false;
				if (timeLabel != null) {
					timeLabel.Visible = true;
					UpdateTimeLabel();
				}
				if (gameTimer != null) {
					gameTimer.Stop();
					if (!gameTimer.IsConnected("timeout", Callable.From(OnGameTimerTimeout))) {
						gameTimer.Connect("timeout", Callable.From(OnGameTimerTimeout));
					}
					gameTimer.Start(1f);
				}
				break;
		}
		
		if (gameOverPanel != null) {
			gameOverPanel.Visible = false;
		}
	}

	private void OnGameTimerTimeout() {
		if (gameOver) return;
		
		timeLeft -= 1f;
		UpdateTimeLabel();
		
		if (timeLeft <= 0) {
			timeLeft = 0;
			UpdateTimeLabel();
			EndGame();
		}
	}

	private void UpdateMovesLabel() {
		if (movesLabel != null) {
			movesLabel.Text = "Moves: " + movesLeft;
		}
	}

	private void UpdateTimeLabel() {
		if (timeLabel != null) {
			int seconds = (int)Math.Ceiling(timeLeft);
			timeLabel.Text = "Time: " + seconds + "s";
		}
	}

	private void UpdateScoreLabel() {
		if (scoreLabel != null) {
			scoreLabel.Text = "Score: " + score;
		}
	}

	private void LoadTextures() {
		gemTextures["blue"] = GD.Load<Texture2D>("res://art/dblue.png");
		gemTextures["red"] = GD.Load<Texture2D>("res://art/dred.png");
		gemTextures["yellow"] = GD.Load<Texture2D>("res://art/dyellow.png");
		gemTextures["brown"] = GD.Load<Texture2D>("res://art/dorange.png");
		gemTextures["green"] = GD.Load<Texture2D>("res://art/dgreen.png");
		gemTextures["indyk"] = GD.Load<Texture2D>("res://art/dpurple.png");
	}

	private List<List<Block>> Make2dArray() {
		List<List<Block>> initialList = new();
		for (int i = 0; i < width; i++) {
			List<Block> column = new();
			for (int j = 0; j < height; j++) {
				column.Add(null);
			}
			initialList.Add(column);
		}
		return initialList;
	}

	private Vector2 Grid2pixel(int column, int row)
	{
		int newx = x_start + offset * column;
		int newy = y_start + (-offset) * row;
		return new Vector2(newx, newy);
	}

	private void SpawnPieces() {
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				int random = new RandomNumberGenerator().RandiRange(0, possiblePieces.Count - 1);
				Block piece = possiblePieces.ElementAt(random).Instantiate() as Block;
				while (MatchAt(i, j, piece.colour))
				{
					random = new RandomNumberGenerator().RandiRange(0, possiblePieces.Count - 1);
					piece = possiblePieces.ElementAt(random).Instantiate() as Block;
				}
				AddChild(piece);
				piece.Position = Grid2pixel(i, j);
				grid[i][j] = piece;
			}
		}
	}

	private Boolean MatchAt(int column, int row, String colour) {

		if (row > 1) {
			Block oneBelow = grid.ElementAt(column).ElementAt(row - 1);
			Block twoBelow = grid.ElementAt(column).ElementAt(row - 2);

			if (oneBelow != null && twoBelow != null) {
				if (oneBelow.colour == colour && twoBelow.colour == colour) {
					return true;
				}
			}
		}
		if (column > 1) {
			Block oneLeft = grid.ElementAt(column - 1).ElementAt(row);
			Block twoLeft = grid.ElementAt(column - 2).ElementAt(row);

			if (oneLeft != null && twoLeft != null) {
				if (oneLeft.colour == colour && twoLeft.colour == colour) {
					return true;
				}
			}
		}

		return false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				MoveDown(i, j);   
			} 
		}
	}

	private void MoveDown(int column, int row) {
		if (row > 0) {
			Block currentPiece = grid.ElementAt(column).ElementAt(row);
			Block oneBelow = grid.ElementAt(column).ElementAt(row - 1);
			if (oneBelow == null && currentPiece != null) {
				ProcessGravity(currentPiece, column, row);
			}
		}
	}

	private void ProcessGravity(Block piece, int column, int row) {
		piece.Position = Grid2pixel(column, row - 1);
		grid.ElementAt(column).RemoveAt(row);
		grid.ElementAt(column).Insert(row - 1, piece);
	}

	private void RespawnBlock(int column) {
		int row = height - 1;
		int random = new RandomNumberGenerator().RandiRange(0, possiblePieces.Count - 1);
		Block piece = possiblePieces[random].Instantiate() as Block;
		while (MatchAt(column, row, piece.colour))
		{
			random = new RandomNumberGenerator().RandiRange(0, possiblePieces.Count - 1);
			piece = possiblePieces[random].Instantiate() as Block;
		}
		grid[column][row] = piece;
		AddChild(piece);
		piece.Position = Grid2pixel(column, row);
	}

	public override void _Input(InputEvent @event) {
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left) {
			Vector2 mousePos = GetGlobalMousePosition();
			HandleClick(mousePos);
		}
	}

	private void HandleClick(Vector2 mousePos) {
		int column = (int)Math.Round((mousePos.X - x_start) / (float)offset);
		int row = (int)Math.Round((y_start - mousePos.Y) / (float)offset);

		if (column < 0 || column >= width || row < 0 || row >= height) {
			return;
		}

		Block clickedBlock = grid.ElementAt(column).ElementAt(row);

		if (clickedBlock == null) {
			return;
		}

		if (selectedBlock == null) {
			selectedBlock = clickedBlock;
			selectedGridPos = new Vector2(column, row);
			clickedBlock.SetSelected(true);
		} else {
			int selColumn = (int)selectedGridPos.X;
			int selRow = (int)selectedGridPos.Y;

			selectedBlock.SetSelected(false);

			if (Math.Abs(column - selColumn) + Math.Abs(row - selRow) == 1) {
				SwapBlocks(selColumn, selRow, column, row);
			}

			selectedBlock = null;
		}
	}

	private void SwapBlocks(int col1, int row1, int col2, int row2) {
		if (gameOver) return;
		
		Block block1 = grid[col1][row1];
		Block block2 = grid[col2][row2];

		grid[col1][row1] = block2;
		grid[col2][row2] = block1;

		block1.Position = Grid2pixel(col2, row2);
		block2.Position = Grid2pixel(col1, row1);

		CheckMatches();
		
		if (gameMode != GameMode.Endless && gameMode != GameMode.Time60) {
			if (movesLeft > 0) {
				movesLeft--;
				UpdateMovesLabel();
			}
			
			if (movesLeft <= 0) {
				GetTree().CreateTimer(0.5f);
				CallDeferred(nameof(EndGame));
			}
		}
	}

	private List<List<Block>> FindMatchGroups() {
		List<List<Block>> groups = new();
		HashSet<Block> used = new();

		for (int col = 0; col < width; col++) {
			for (int row = 0; row < height - 2; row++) {
				Block b1 = grid[col][row];
				Block b2 = grid[col][row + 1];
				Block b3 = grid[col][row + 2];
				if (b1 != null && b2 != null && b3 != null &&
					b1.colour == b2.colour && b2.colour == b3.colour) {
					List<Block> group = new() { b1, b2, b3 };
					int r = row + 3;
					while (r < height && grid[col][r] != null && grid[col][r].colour == b1.colour) {
						group.Add(grid[col][r]);
						r++;
					}
					if (!used.Contains(b1)) {
						groups.Add(group);
						foreach (var b in group) used.Add(b);
					}
				}
			}
		}

		for (int row = 0; row < height; row++) {
			for (int col = 0; col < width - 2; col++) {
				Block b1 = grid[col][row];
				Block b2 = grid[col + 1][row];
				Block b3 = grid[col + 2][row];
				if (b1 != null && b2 != null && b3 != null &&
					b1.colour == b2.colour && b2.colour == b3.colour) {
					List<Block> group = new() { b1, b2, b3 };
					int c = col + 3;
					while (c < width && grid[c][row] != null && grid[c][row].colour == b1.colour) {
						group.Add(grid[c][row]);
						c++;
					}
					if (!used.Contains(b1)) {
						groups.Add(group);
						foreach (var b in group) used.Add(b);
					}
				}
			}
		}

		return groups;
	}

	private void CheckMatches() {
		List<List<Block>> matchGroups = FindMatchGroups();
		
		if (matchGroups.Count > 0) {
			int totalScore = 0;
			foreach (var group in matchGroups) {
				int groupSize = group.Count;
				if (groupSize == 3) totalScore += 1;
				else if (groupSize == 4) totalScore += 2;
				else if (groupSize >= 5) totalScore += 3;
			}
			score += totalScore;
			UpdateScoreLabel();
			
			float explosionScale = 1f + (matchGroups[0].Count - 3) * 0.3f;
			float explosionDuration = 0.6f + (matchGroups[0].Count - 3) * 0.15f;
			
			HashSet<Block> allMatches = new();
			foreach (var group in matchGroups) {
				foreach (Block block in group) {
					allMatches.Add(block);
				}
			}
			
			foreach (Block block in allMatches) {
				Vector2 blockPos = block.Position;
				string colour = block.colour;
				
				for (int col = 0; col < width; col++) {
					for (int row = 0; row < height; row++) {
						if (grid[col][row] == block) {
							grid[col][row] = null;
							CreateExplosion(blockPos, colour, explosionScale, explosionDuration);
							block.QueueFree();
							break;
						}
					}
				}
			}

			GetTree().CreateTimer(explosionDuration);
			CallDeferred(nameof(ContinueAfterExplosion));
			return;
		}
	}

	private void CreateExplosion(Vector2 position, string colour, float scale, float duration) {
		if (!gemTextures.ContainsKey(colour)) return;
		
		Texture2D texture = gemTextures[colour];
		int particleCount = (int)(8 * scale);
		float speed = 150f * scale;
		
		for (int i = 0; i < particleCount; i++) {
			Sprite2D particle = new Sprite2D {
				Texture = texture,
				Position = position,
				Scale = new Vector2(0.1f, 0.1f)
			};
			AddChild(particle);
			
			float angle = i * Mathf.Pi * 2f / particleCount + new RandomNumberGenerator().Randf() * 0.5f;
			float actualSpeed = speed + new RandomNumberGenerator().Randf() * 100f;
			Vector2 velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * actualSpeed;
			
			CreateTween().TweenProperty(particle, "position", position + velocity, duration)
				.SetTrans(Tween.TransitionType.Expo)
				.SetEase(Tween.EaseType.Out);
			CreateTween().TweenProperty(particle, "modulate:a", 0f, duration)
				.SetTrans(Tween.TransitionType.Expo)
				.SetEase(Tween.EaseType.Out)
				.Finished += () => particle.QueueFree();
		}
	}

	private void ContinueAfterExplosion() {
		float fallDuration = 0.5f;
		ApplyGravity();
		FillEmptySpaces();
		GetTree().CreateTimer(fallDuration);
		CallDeferred(nameof(CheckMatches));
	}

	private void ApplyGravity() {
		for (int col = 0; col < width; col++) {
			for (int row = 0; row < height; row++) {
				if (grid[col][row] == null) {
					for (int above = row + 1; above < height; above++) {
						if (grid[col][above] != null) {
							grid[col][row] = grid[col][above];
							grid[col][above] = null;
							
							Vector2 targetPos = Grid2pixel(col, row);
							float fallDistance = Mathf.Abs(grid[col][row].Position.Y - targetPos.Y);
							float fallDuration = fallDistance / 400f;
							
							CreateTween().TweenProperty(grid[col][row], "position", targetPos, fallDuration)
								.SetTrans(Tween.TransitionType.Expo)
								.SetEase(Tween.EaseType.In);
							break;
						}
					}
				}
			}
		}
	}

	private void FillEmptySpaces() {
		for (int col = 0; col < width; col++) {
			for (int row = 0; row < height; row++) {
				if (grid[col][row] == null) {
					int random = new RandomNumberGenerator().RandiRange(0, possiblePieces.Count - 1);
					Block piece = possiblePieces[random].Instantiate() as Block;
					AddChild(piece);
					
					Vector2 targetPos = Grid2pixel(col, row);
					piece.Position = new Vector2(targetPos.X, targetPos.Y - offset * height);
					grid[col][row] = piece;
					
					float fallDistance = targetPos.Y - piece.Position.Y;
					float fallDuration = fallDistance / 400f;
					
					CreateTween().TweenProperty(piece, "position", targetPos, fallDuration)
						.SetTrans(Tween.TransitionType.Expo)
						.SetEase(Tween.EaseType.In);
				}
			}
		}
	}

	private void EndGame() {
		if (gameOver) return;
		gameOver = true;
		
		if (gameTimer != null) {
			gameTimer.Stop();
		}
		
		HighScoreManager scoreManager = GetNode<HighScoreManager>("/root/HighScoreManager");
		if (scoreManager != null) {
			scoreManager.AddScore(gameMode, score);
		}
		
		if (gameOverPanel != null) {
			gameOverPanel.Visible = true;
			Label finalScoreLabel = gameOverPanel.GetNode<Label>("FinalScoreLabel");
			if (finalScoreLabel != null) {
				finalScoreLabel.Text = "Final Score: " + score;
			}
		}
	}
}
