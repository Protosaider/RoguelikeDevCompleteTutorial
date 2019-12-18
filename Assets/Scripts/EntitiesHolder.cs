using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitiesHolder : MonoBehaviour, IEnumerable<Entity>
{
	public List<Entity> Entities;
	public Entity PlayerEntity;

	//public EntitiesHolder(List<Entity> entities, Entity playerEntity)
	//{
	//	Entities = entities;
	//	PlayerEntity = playerEntity;
	//}

	public IEnumerator<Entity> GetEnumerator()
	{
		for (var i = 0; i < Entities.Count; i++)
			yield return Entities[i];
	}
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public IEnumerable<Entity> GetEnumerable => this;

}
