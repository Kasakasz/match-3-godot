using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Grid : Node2D
{
    [Export] public int width;
    [Export] public int height;
    [Export] public int x_start;
    [Export] public int y_start;
    [Export] public int offset;

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
        SpawnPieces();
    }

    private List<List<Block>> Make2dArray() {
        List<List<Block>> initialList = new();
        for (int i = 0; i < width; i++) {
            initialList.Add(new List<Block>());
            for (int j = 0; j < height; j++) {
                List<Block> tempObj = initialList.ElementAt(i);
                tempObj.Add(null);
            }
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
                grid.ElementAt(i).Insert(j, piece);
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
        GD.Print("column = " + column + " row = " + row);

        if (row > 0) {
            Block oneBelow = grid.ElementAt(column).ElementAt(row - 1);
            if (oneBelow != null) {
                ProcessGravity(grid.ElementAt(column).ElementAt(row),
                column, row
                );
            }
        }
    }

    private void ProcessGravity(Block piece, int column, int row) {
        Vector2 currentPos = piece.Position;
        piece.Position = Grid2pixel(column, row);
        grid.ElementAt(column).Insert(row, piece);
    }
}
