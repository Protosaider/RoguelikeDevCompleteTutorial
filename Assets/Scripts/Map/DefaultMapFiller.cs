using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

[Serializable]
public class DefaultMapFiller : IMapFiller
{
	public TileGenerator WallTileGenerator;

	public void FillMap(TileMap map)
	{
		for (var y = 0; y < map.Height; y++)
			for (var x = 0; x < map.Width; x++)
				map.SetItem(x, y, WallTileGenerator.Generate(x, y));
	}
}