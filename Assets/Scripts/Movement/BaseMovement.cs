using System;
using UnityEngine;

[Serializable]
public class BaseMovement
{
	private readonly Transform _transform;
	private readonly Entity _entity;

	public BaseMovement(Transform transform, Entity entity)
	{
		_transform = transform;
		_entity = entity;
	}

	public virtual void HandleMovement(EDirection direction)
	{
		var moveTo = direction.ToVector3();
		_entity.CurrentPosition += direction.ToVector2Int();
		_transform.position += moveTo;
	}

}