using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoomEntitySpawner : MonoBehaviour, IEntitySpawner
{
	public GameObject EntitiesHolder;

    //TODO: Show Tuple in inspector
    //public List<Tuple<Entity, Single>>
	public List<Entity> EntitiesToSpawn;

	[Range(0, 1)]
	public Single ChanceToSpawn;
	public Int32 NumberOfTries;

	public List<Single> SpawnChance;
	public Int32 minEntities;
	public Int32 maxEntities;

	public List<Entity> SpawnedEntities;

	private List<RectInt> rooms;

	public void SetRooms(List<RectInt> rooms)
	{
		this.rooms = rooms;
	}

	public void SpawnEntity(Entity entity, Vector2Int position)
	{
		var go = new GameObject(entity.EntityTileData.InitialTileRenderData.Name);
		go.transform.position = new Vector3(position.x, 0, position.y);
		go.transform.SetParent(EntitiesHolder.transform);

		var renderer = go.AddComponent<BaseRenderer>();
		renderer.SetSpriteRendererSettings(
			entity.EntityTileData.InitialTileRenderData.Sprite, 
			entity.EntityTileData.InitialTileRenderData.OrderInSortingLayer,
			entity.EntityTileData.InitialTileRenderData.SortingLayerId);

        var newEntity = go.AddComponent<Entity>();
		newEntity.CurrentPosition = position;
		newEntity.FovRadius = entity.FovRadius;
		newEntity.EntityTileData = entity.EntityTileData;

		EntitiesHolder.GetComponent<EntitiesHolder>().Entities.Add(newEntity);
    }

    public void SpawnEntities()
	{
		var sparePositions = new List<Vector2Int>();
        foreach (var rectInt in rooms)
		{
			sparePositions.Clear();

            foreach (var sparePosition in rectInt.allPositionsWithin)
				sparePositions.Add(sparePosition);

			for (var i = 0; i < NumberOfTries; i++)
			{
				if (sparePositions.Count == 0)
					continue;

				var position = sparePositions[Random.Range(0, sparePositions.Count)];
				sparePositions.Remove(position);

                var entity = EntitiesToSpawn[Random.Range(0, EntitiesToSpawn.Count)];
				if (Random.value < ChanceToSpawn)
				{
					SpawnEntity(entity, position);
					break;
				}
            }
        }
	}
}
