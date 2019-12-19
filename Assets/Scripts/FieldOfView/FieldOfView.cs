using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FovMap : Grid<Boolean>
{
	public FovMap(Int32 width, Int32 height) : base(width, height) { }
}

public class FieldOfView : MonoBehaviour
{
	public FovMap Map;

	public FieldOfView(Int32 width, Int32 height)
	{
		Map = new FovMap(width, height);
	}

	public void Clear(Vector2Int origin, Int32 fovRadius) => Clear(origin.x, origin.y, fovRadius);

	public void Clear(Int32 originX, Int32 originY, Int32 fovRadius)
	{
		Map.SetItem(originX, originY, false);

		if (fovRadius <= 0)
			return;

		RectInt fovArea = new RectInt(0, 0, Map.Width - 1, Map.Height - 1);

		fovArea = fovArea.Intersection(
			new RectInt(originX - fovRadius - 1, originY - fovRadius - 1, fovRadius * 2 + 3, fovRadius * 2 + 3)
		);

		for (var y = fovArea.yMin; y <= fovArea.yMax; y++)
			for (var x = fovArea.xMin; x <= fovArea.xMax; x++)
				Map.SetItem(x, y, false);
	}

	public void Recompute(TileMap map, Vector2Int origin, Int32 fovRadius) =>
		Recompute(map, origin.x, origin.y, fovRadius);

	public void Recompute(TileMap map, Int32 originX, Int32 originY, Int32 fovRadius)
	{
		Map.SetItem(originX, originY, true);

		if (fovRadius <= 0)
			return;

		RectInt fovArea = new RectInt(0, 0, Map.Width - 1, Map.Height - 1);

		fovArea = fovArea.Intersection(
			new RectInt(originX - fovRadius, originY - fovRadius, fovRadius * 2 + 1, fovRadius * 2 + 1)
		);

		for (var x = fovArea.xMin; x <= fovArea.xMax; x++) // cast rays towards the top and bottom of the area
		{
			BresenhamLine(map, Map, originX, originY, x, fovArea.yMax, Distances.ManhattanNormalized, fovRadius, false);
			BresenhamLine(map, Map, originX, originY, x, fovArea.yMin, Distances.ManhattanNormalized, fovRadius, false);
		}
		for (var y = fovArea.yMin; y <= fovArea.yMax; y++) // and to the left and right
		{
			BresenhamLine(map, Map, originX, originY, fovArea.xMax, y, Distances.ManhattanNormalized, fovRadius, false);
			BresenhamLine(map, Map, originX, originY, fovArea.xMin, y, Distances.ManhattanNormalized, fovRadius, false);
		}
	}

	private void Swap<T>(ref T lhs, ref T rhs)
	{
		T temp = lhs;
		lhs = rhs;
		rhs = temp;
	}

	private void BresenhamLine(TileMap map, FovMap fovMap, Int32 fromX, Int32 fromY, Int32 toX, Int32 toY,
		Func<Vector2Int, Vector2Int, Int32> getDistance, Int32 fovRange, Boolean isObstacleLighted) =>
		BresenhamLine(map, fovMap, fromX, fromY, toX, toY,
			(x0, y0, x1, y1) => getDistance(new Vector2Int(x0, y0), new Vector2Int(x1, y1)), fovRange,
			isObstacleLighted);


    private void BresenhamLine(TileMap map, FovMap fovMap, Int32 fromX, Int32 fromY, Int32 toX, Int32 toY, Func<Int32, Int32, Int32, Int32, Int32> getDistance, Int32 fovRange, Boolean isObstacleLighted)
    {
		Int32 xDiff = toX - fromX, 
			yDiff = toY - fromY, 
			xLen = Math.Abs(xDiff), 
			yLen = Math.Abs(yDiff);

		Int32 deltaX = Math.Sign(xDiff), 
			deltaY = Math.Sign(yDiff) << 16, 
			index = (fromY << 16) + fromX;

		if (xLen < yLen) // make sure we walk along the long axis
		{
			Swap(ref xLen, ref yLen);
			Swap(ref deltaX, ref deltaY);
		}

		Int32 deltaError = yLen * 2, 
			error = -xLen, 
			errorReset = xLen * 2;

		while (--xLen >= 0) // skip the first point (the origin) since it's always visible and should never stop rays
		{
			index += deltaX; // advance down the long axis (could be X or Y)
			error += deltaError;

			if (error > 0)
			{
				error -= errorReset;
				index += deltaY;
			}

			Int32 x = index & 0xFFFF, 
				y = index >> 16;

			if (getDistance(fromX, fromY, x, y) > fovRange)
				return;
			var tile = map.GetItem(x, y);

			if (!tile.Cell.IsTransparent)
			{
				if (isObstacleLighted)
					fovMap.SetItem(x, y, true);
				return;
			}
			fovMap.SetItem(x, y, true);
		}

    }


}
