using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Tile
{
	public readonly TileRenderData RenderData;
	public readonly Cell Cell;

	public Int32 X
	{
		get => Cell.X;
		set => Cell.X = value;
	}
	public Int32 Y
	{
		get => Cell.Y;
		set => Cell.Y = value;
	}

    public Tile(TileRenderData renderData, Cell cell)
	{
		RenderData = renderData;
        Cell = cell;
	}

	public static Boolean operator ==(Tile lhs, Tile rhs)
	{
		if (lhs == null && rhs == null)
			return true;

		if (lhs == null || rhs == null)
			return false;

		return lhs.RenderData.Sprite == rhs.RenderData.Sprite && lhs.Cell == rhs.Cell;
	}

	public static Boolean operator !=(Tile lhs, Tile rhs)
	{
		return !(lhs == rhs);
	}
}