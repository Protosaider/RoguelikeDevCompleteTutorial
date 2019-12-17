using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
	public List<MapFiller> MapFillers;
	public MapRenderer MapRenderer;

	public Int32 Width = 20, Height = 20;

	public Map CurrentMap;
	public List<Entity> Entities;
	public Entity PlayerEntity;

	public World()
	{
		CurrentMap = new Map(Width, Height);
	}

	public void Awake()
	{
		MapRenderer.SetMap(CurrentMap);

		for (var i = 0; i < MapFillers.Count; i++)
		{
			var mapFiller = MapFillers[i];
			mapFiller.FillMap(CurrentMap);
		}

		PlayerEntity.Map = CurrentMap;

		for (var i = 0; i < Entities.Count; i++)
		{
			var entity = Entities[i];
			entity.Map = CurrentMap;
		}
	}
}
