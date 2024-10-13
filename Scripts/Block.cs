using Godot;
using System;

public partial class Block : Node2D
{

	[Export] public int Speed { get; set; } = 10;
	[Export] public String colour;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//Velocity = inputDirection * Speed;
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

	public void moveDown() {
		var move = new Vector2();
		move.Y = Speed;
		Position += move;
	}
	
}
