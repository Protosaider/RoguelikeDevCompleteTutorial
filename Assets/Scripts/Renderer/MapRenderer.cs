using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapRenderer : MonoBehaviour
{
	private TileMap _map;
	private Grid<BaseRenderer> _tileGameObjects;

	public void SetMap(TileMap map)
	{
		_map = map;
		_map.OnItemSettled += CreateOrUpdateRenderer;

		_tileGameObjects = new Grid<BaseRenderer>(_map.Width, _map.Height);
    }

	public void CreateOrUpdateRenderer(Tile tile)
	{
		if (_tileGameObjects.GetItem(tile.X, tile.Y) == null)
		{
			var go = new GameObject("Tile (" + tile.X + ", " + tile.Y + ")");
			go.transform.SetParent(transform);
			go.transform.position = new Vector3(tile.X, 0, tile.Y);

			var baseRenderer = go.AddComponent<BaseRenderer>();
			baseRenderer.SetSpriteRendererSettings(tile.RenderData.Sprite, tile.RenderData.OrderInSortingLayer, tile.RenderData.SortingLayerId);
			_tileGameObjects.SetItem(tile.X, tile.Y, baseRenderer);
		}
		else
		{
			_tileGameObjects.GetItem(tile.X, tile.Y).SetSpriteRendererSettings(tile.RenderData.Sprite, tile.RenderData.OrderInSortingLayer, tile.RenderData.SortingLayerId);
        }
	}

	public void RefreshVisibility(FovMap fovMap)
	{
		for (var y = 0; y < fovMap.Height; y++)
		{
			for (var x = 0; x < fovMap.Width; x++)
			{
				_tileGameObjects.GetItem(x, y).FovTest(fovMap.GetItem(x, y));
			}
		}

	}
}
