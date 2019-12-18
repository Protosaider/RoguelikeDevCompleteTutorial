using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
	[SerializeField]
	private List<MapFiller> MapFillers;
	[SerializeField]
	private MapRenderer MapRenderer;

	[SerializeField]
	private Int32 Width = 20, Height = 20;

	public Map CurrentMap;

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
	}
}
