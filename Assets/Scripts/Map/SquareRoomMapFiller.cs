using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class SquareRoomMapFiller : IMapFiller
{
	[SerializeField]
	private Int32 maxRooms;
	[SerializeField]
	private Int32 roomMinSize;
	[SerializeField]
	private Int32 roomMaxSize;

	public List<RectInt> rooms = new List<RectInt>();

    public TileGenerator EmptyTileGenerator;

	public Vector2Int GetSpawnPoint() => _playerSpawnPoint;

	private Vector2Int _playerSpawnPoint;

    public void FillMap(TileMap map)
	{
        //TODO: Random seed?
		rooms.Clear();

		for (Int32 i = 0; i < maxRooms; i++)
		{
			var w = Random.Range(roomMinSize, roomMaxSize);
			var h = Random.Range(roomMinSize, roomMaxSize);

			var x = Random.Range(1, map.Width - w);
			var y = Random.Range(1, map.Height - h);

			var newRoom = new RectInt(x, y, w, h);

			if (!newRoom.IntersectAny(rooms))
			{
				CreateRoom(newRoom, map);
				Vector2Int newRoomCenter = newRoom.Center();

				if (rooms.Count == 0)
				{
					_playerSpawnPoint = newRoomCenter;
                }
                else
				{
					Vector2Int prevRoomCenter = rooms[rooms.Count - 1].Center();
					if (Random.value < 0.5)
					{
						CreateHorizontalTunnel(prevRoomCenter.x, newRoomCenter.x, prevRoomCenter.y, map);
						CreateVerticalTunnel(prevRoomCenter.y, newRoomCenter.y, newRoomCenter.x, map);
					}
					else
					{
						CreateVerticalTunnel(prevRoomCenter.y, newRoomCenter.y, prevRoomCenter.x, map);
						CreateHorizontalTunnel(prevRoomCenter.x, newRoomCenter.x, newRoomCenter.y, map);
					}
				}
				rooms.Add(newRoom);
			}
		}
    }

	private void CreateRoom(RectInt area, TileMap map)
	{
		for (var y = area.yMin; y < area.yMax; y++)
			for (var x = area.xMin; x < area.xMax; x++)
				map.SetItem(x, y, EmptyTileGenerator.Generate(x, y));
	}

	private void CreateHorizontalTunnel(Int32 firstRoomCenterX, Int32 secondRoomCenterX, Int32 firstRoomCenterY, TileMap map)
	{
		var xMax = Mathf.Max(firstRoomCenterX, secondRoomCenterX);

        for (var x = Mathf.Min(firstRoomCenterX, secondRoomCenterX); x <= xMax; x++)
			map.SetItem(x, firstRoomCenterY, EmptyTileGenerator.Generate(x, firstRoomCenterY));
	}

	private void CreateVerticalTunnel(Int32 firstRoomCenterY, Int32 secondRoomCenterY, Int32 firstRoomCenterX, TileMap map)
	{
		var yMax = Mathf.Max(firstRoomCenterY, secondRoomCenterY);

        for (var y = Mathf.Min(firstRoomCenterY, secondRoomCenterY); y <= yMax; y++)
			map.SetItem(firstRoomCenterX, y, EmptyTileGenerator.Generate(firstRoomCenterX, y));
	}
}