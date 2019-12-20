using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public static class EDirectionHelper
{
	private static readonly EDirection[] _directions;

	static EDirectionHelper()
	{
		var directionNames = Enum.GetNames(typeof(EDirection));
		_directions = new EDirection[directionNames.Length];

		for (var i = 0; i < directionNames.Length; i++)
		{
			var directionName = directionNames[i];
			Enum.TryParse(directionName, out _directions[i]);
		}
	}

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

			case EDirection.UpRight:
				vector = Vector3.forward + Vector3.right;
				break;

			case EDirection.DownRight:
				vector = Vector3.back + Vector3.right;
				break;

			case EDirection.DownLeft:
				vector = Vector3.back + Vector3.left;
				break;

			case EDirection.UpLeft:
				vector = Vector3.forward + Vector3.left;
				break;

			case EDirection.Diagonals:
			case EDirection.Cardinals:
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

			case EDirection.UpRight:
				vector = Vector2Int.up + Vector2Int.right;
				break;

			case EDirection.DownRight:
				vector = Vector2Int.down + Vector2Int.right;
				break;

			case EDirection.DownLeft:
				vector = Vector2Int.down + Vector2Int.left;
				break;

			case EDirection.UpLeft:
				vector = Vector2Int.up + Vector2Int.left;
				break;

			case EDirection.Diagonals:
			case EDirection.Cardinals:
			case EDirection.None:
				break;
        }

		return vector;
    }

	public static IEnumerable<EDirection> GetFlags(EDirection a)
	{
		for (var i = 0; i < _directions.Length; i++)
		{
			var direction = _directions[i];

			if (a.HasFlag(direction))
				yield return direction;
		}
	}


	public static EDirection SetFlag(EDirection a, EDirection b) => a | b;
	public static EDirection UnsetFlag(EDirection a, EDirection b) => a & ~b;
	// Works with "None" as well
	public static Boolean HasFlag(EDirection a, EDirection b) => (a & b) == b;
	public static EDirection ToggleFlag(EDirection a, EDirection b) => a ^ b;
}