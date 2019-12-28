using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
	[SerializeField]
	private Int32 Width = 20;
	[SerializeField]
	private Int32 Height = 20;
	[Space]
	[SerializeField]
	public MapRenderer MapRenderer;
    [Header("Map Fillers")]
	[SerializeField]
	private DefaultMapFiller DefaultMapFiller;
	[SerializeField]
	private SquareRoomMapFiller RoomMapFiller;
	[SerializeField]
	public RoomEntitySpawner RoomEntitySpawner;

	public Vector2Int PlayerSpawnPoint;

    public TileMap CurrentMap;

	public World()
	{
		CurrentMap = new TileMap(Width, Height);
	}

	public void Awake()
	{
		MapRenderer.SetMap(CurrentMap);

		DefaultMapFiller.FillMap(CurrentMap);
		RoomMapFiller.FillMap(CurrentMap);

		RoomEntitySpawner.SetRooms(RoomMapFiller.rooms);
		RoomEntitySpawner.SpawnEntities();

        PlayerSpawnPoint = RoomMapFiller.GetSpawnPoint();
	}
}
