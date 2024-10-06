using Godot;
using System;

public partial class SpawnTiles : Node2D
{
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		String path = "res://scenes/FirstMap.tscn";
		PackedScene preparedScene = GD.Load<PackedScene>(path);
		// PackedScene greenBlock = ResourceLoader.Load("res://blocks/green_block.tscn") as PackedScene;
		// Node2D greenBlockNode = greenBlock. as Node2D;
		// greenBlockNode.Position = new Vector2();
		

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
