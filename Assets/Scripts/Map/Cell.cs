using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

[Serializable]
public class Cell
{
	public Vector2Int Position;
	public Boolean IsTransparent;
	public Boolean IsWalkable;
    //public Boolean IsExplored;
    //public Boolean IsInFov;

	public Int32 X
	{
		get => Position.x;
		set => Position.x = value;
	}
	public Int32 Y
	{
		get => Position.y;
		set => Position.y = value;
	}

	public Cell() { }

	public Cell(Vector2Int position, Boolean isTransparent, Boolean isWalkable)
	{
		Position = position;
		IsTransparent = isTransparent;
		IsWalkable = isWalkable;
	}

	public Cell Duplicate()
	{
		var cell = new Cell
		{
			Position = Position,
			IsTransparent = IsTransparent,
			IsWalkable = IsWalkable,
		};

		return cell;
	}

	public static Boolean operator==(Cell lhs, Cell rhs)
	{
		if (Object.ReferenceEquals(lhs, null) && Object.ReferenceEquals(rhs, null))
			return true;

		if (Object.ReferenceEquals(lhs, null) || Object.ReferenceEquals(rhs, null))
			return false;

		return lhs.Position == rhs.Position && lhs.IsWalkable == rhs.IsWalkable &&
			   lhs.IsTransparent == rhs.IsTransparent;
	}

	public static Boolean operator !=(Cell lhs, Cell rhs)
	{
		return !(lhs == rhs);
	}
}
