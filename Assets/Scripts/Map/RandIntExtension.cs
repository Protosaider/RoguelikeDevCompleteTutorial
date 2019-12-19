﻿using System;
using System.Collections.Generic;
using UnityEngine;

public static class RandIntExtension
{
	public static Boolean Intersect(this RectInt rect, RectInt other) =>
		rect.xMin <= other.xMax && rect.xMax >= other.xMin &&
		rect.yMin <= other.yMax && rect.yMax >= other.yMin;

	public static RectInt Intersection(this RectInt rect, RectInt other)
	{
		if (!rect.Intersect(other))
			return default;

		var xMin = Mathf.Max(rect.xMin, other.xMin);
		var width = Mathf.Min(rect.xMax, other.xMax) - xMin;
		var yMin = Mathf.Max(rect.yMin, other.yMin);
		var height = Mathf.Min(rect.yMax, other.yMax) - yMin;

		return new RectInt(xMin, yMin, width, height);
	}

	public static Boolean IntersectAny(this RectInt rect, IEnumerable<RectInt> others)
	{
		foreach (RectInt other in others)
			if (rect.Intersect(other))
				return true;
		return false;
	}

	public static Vector2Int Center(this RectInt rect) =>
		new Vector2Int((Int32)Mathf.Floor(rect.center.x), (Int32)Mathf.Floor(rect.center.y));
}