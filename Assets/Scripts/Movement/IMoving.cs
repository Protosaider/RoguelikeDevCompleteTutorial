using System;

public interface IMoving
{
	void HandleMovement(Map map, Entity entity, EDirection direction);
	Boolean CanMove(Map map, Entity entity, EDirection direction);
}