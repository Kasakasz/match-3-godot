using Godot;
using System;

public partial class Block : Node2D
{

	[Export] public int Speed { get; set; } = 10;
	[Export] public String colour;
	[Export] public bool selected;
	
	private Sprite2D sprite;

	public override void _Ready()
	{
		sprite = GetNode<Sprite2D>("Sprite2D");
	}

	public override void _Process(double delta)
	{
		if (selected && sprite != null) {
			sprite.Modulate = new Color(1.5f, 1.5f, 1.5f);
		} else if (sprite != null) {
			sprite.Modulate = new Color(1f, 1f, 1f);
		}
	}
	
	public void SetSelected(bool isSelected) {
		selected = isSelected;
	}

	public void moveDown() {
		var move = new Vector2();
		move.Y = Speed;
		Position += move;
	}
	
}
