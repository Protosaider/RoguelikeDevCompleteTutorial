using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapRenderer : MonoBehaviour
{
	private Map _map;
	private BaseRenderer[] _tileGameObjects;

	public void SetMap(Map map)
	{
		_map = map;
		_map.OnTileSettled += CreateOrUpdateRenderer;

		_tileGameObjects = new BaseRenderer[_map.Width * _map.Height];
    }

	public void CreateOrUpdateRenderer(Tile tile)
	{
		if (_tileGameObjects[_map.Index(tile.X, tile.Y)] == null)
		{
			var go = new GameObject("Tile (" + tile.X + ", " + tile.Y + ")");
			go.transform.SetParent(transform);
			go.transform.position = new Vector3(tile.X, 0, tile.Y);

			var baseRenderer = go.AddComponent<BaseRenderer>();
			baseRenderer.SetSprite(tile.RenderData.Sprite, tile.RenderData.OrderInSortingLayer, tile.RenderData.SortingLayerId);
			_tileGameObjects[_map.Index(tile.X, tile.Y)] = baseRenderer;
		}
		else
		{
			_tileGameObjects[_map.Index(tile.X, tile.Y)].SetSprite(tile.RenderData.Sprite, tile.RenderData.OrderInSortingLayer, tile.RenderData.SortingLayerId);
        }
	}
}
