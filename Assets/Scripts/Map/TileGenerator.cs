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
}
