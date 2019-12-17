using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MapFiller
{
	public TileGenerator EmptyTileGenerator;
	public TileGenerator WallTileGenerator;

	public void FillMap(Map map)
	{
		for (var x = 0; x < map.Width; x++)
			for (var y = 0; y < map.Height; y++)
				map.SetTile(x, y, EmptyTileGenerator.Generate());

		for (var x = 10; x < 13; x++)
			map.SetTile(x, 12, WallTileGenerator.Generate());
	}
}
