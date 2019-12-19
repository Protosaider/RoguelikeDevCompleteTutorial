using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TileGenerator
{
	public TileData TileData;

	public Tile Generate()
	{
		if (TileData == null || TileData.InitialTileRenderData == null || TileData.InitialCellData == null)
			return null;

		var tile = new Tile(TileData.InitialTileRenderData, TileData.InitialCellData.Duplicate());

		return tile;
	}

	public Tile Generate(Int32 x, Int32 y)
	{
		if (TileData == null || TileData.InitialTileRenderData == null || TileData.InitialCellData == null)
			return null;

		var tile = new Tile(TileData.InitialTileRenderData, TileData.InitialCellData.Duplicate())
		{
			X = x, Y = y
		};

		return tile;
	}
}
