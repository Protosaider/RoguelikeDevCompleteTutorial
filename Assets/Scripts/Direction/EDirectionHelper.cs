using UnityEngine;

public static class EDirectionHelper
{
	public static Vector3 ToVector3(this EDirection direction)
	{
		var vector = Vector3.zero;

		switch (direction) {
			case EDirection.Right:
				vector = Vector3.right;
				break;

			case EDirection.Left:
				vector = Vector3.left;
				break;

			case EDirection.Up:
				vector = Vector3.forward;
				break;

			case EDirection.Down:
				vector = Vector3.back;
				break;

			case EDirection.None:
				break;
		}

		return vector;
	}

	public static Vector2Int ToVector2Int(this EDirection direction)
	{
		var vector = Vector2Int.zero;

		switch (direction)
		{
			case EDirection.Right:
				vector = Vector2Int.right;
				break;

			case EDirection.Left:
				vector = Vector2Int.left;
				break;

			case EDirection.Up:
				vector = Vector2Int.up;
				break;

			case EDirection.Down:
				vector = Vector2Int.down;
				break;

			case EDirection.None:
				break;
		}

		return vector;
	}
}