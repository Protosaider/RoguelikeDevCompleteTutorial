using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Distances
{
    //! Manhattan distance
    public static Single GetManhattanDistance(Vector2 a, Vector2 b)
    {
        Single distanceX = Mathf.Abs(a.x - b.x);
        Single distanceZ = Mathf.Abs(a.y - b.y);

        return distanceX > distanceZ ? 4 * distanceZ + 10 * distanceX : 4 * distanceX + 10 * distanceZ;
    }

	public static Int32 GetManhattanDistance(Vector2Int a, Vector2Int b)
	{
		var distanceX = Mathf.Abs(a.x - b.x);
		var distanceZ = Mathf.Abs(a.y - b.y);

		return distanceX > distanceZ ? (4 * distanceZ + 10 * distanceX) : (4 * distanceX + 10 * distanceZ);
	}

	public static Int32 ManhattanNormalized(Vector2Int a, Vector2Int b)
	{
		Single distanceX = Mathf.Abs(a.x - b.x);
		Single distanceZ = Mathf.Abs(a.y - b.y);

		return distanceX > distanceZ ? (Int32)(0.4f * distanceZ + distanceX) : (Int32)(0.4f * distanceX + distanceZ);
	}
}
