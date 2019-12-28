using UnityEngine;

public interface IEntitySpawner
{
	void SpawnEntity(Entity entity, Vector2Int position);
	void SpawnEntities();
}