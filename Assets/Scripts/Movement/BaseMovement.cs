using System;
using UnityEngine;

[Serializable]
public class BaseMovement : IMoving
{
	private readonly Transform _transform;

	public BaseMovement(Transform transform)
	{
		_transform = transform;
	}

	public virtual void HandleMovement(Map map, Entity entity, EDirection direction)
	{
		var moveTo = direction.ToVector3();
		entity.CurrentPosition += direction.ToVector2Int();
		_transform.position += moveTo;
	}

	public virtual Boolean CanMove(Map map, Entity entity, EDirection direction)
	{
		return map.CanMove(entity.CurrentPosition, direction);
	}
}