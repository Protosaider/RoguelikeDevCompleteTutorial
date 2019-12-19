using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMap : Grid<Tile>
{
	public MapRenderer MapRenderer;

	public TileMap(Int32 width, Int32 height) : base(width, height) { }

	public Boolean CanMove(Vector2Int position, EDirection direction)
	{
		var x = position.x + direction.ToVector2Int().x;
		var y = position.y + direction.ToVector2Int().y;

		if (IsOutside(x, y))
			return false;

		return Items[Index(x, y)].Cell.IsWalkable;
	}

}
