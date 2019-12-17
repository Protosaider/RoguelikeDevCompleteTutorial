using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map
{
	public MapRenderer MapRenderer;

	public event Action<Tile> OnTileSettled;

    public Tile[] Tiles { get; private set; }
	public Int32 Width { get; }
	public Int32 Height { get; }

	public Int32 Index(Int32 x, Int32 y) => y * Width + x;

	public Map(Int32 width, Int32 height)
	{
		Width = width;
		Height = height;
		Tiles = new Tile[width * height];
	}

	public Boolean IsOutside(Int32 x, Int32 y) => x < 0 || x >= Width || y < 0 || y >= Height;

	public Boolean SetTile(Int32 x, Int32 y, Tile tile)
	{
		if (IsOutside(x, y))
			return false;

		tile.Cell.Position = new Vector2Int(x, y);
		Tiles[Index(x, y)] = tile;

		OnTileSettled?.Invoke(tile);

        return true;
	}

	//public Boolean SetCellProperties(Vector2Int position, Cell cell)
	//{
	//	var x = position.x;
	//	var y = position.y;

 //       if (IsOutside(x, y))
	//		return false;

	//	Tiles[Index(x, y)].Cell.IsWalkable = cell.IsWalkable;
	//	Tiles[Index(x, y)].Cell.IsTransparent = cell.IsTransparent;

	//	return true;
	//}

    public Tile GetTile(Int32 x, Int32 y) => IsOutside(x, y) ? null : Tiles[Index(x, y)];

	public Boolean CanMove(Vector2Int position, EDirection direction)
	{
		var x = position.x + direction.ToVector2Int().x;
		var y = position.y + direction.ToVector2Int().y;

		if (IsOutside(x, y))
			return false;

		return Tiles[Index(x, y)].Cell.IsWalkable;
	}
}
