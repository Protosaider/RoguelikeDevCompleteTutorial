﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
	public Map Map;

    public BaseMovement BaseMovement;
	public BaseRenderer BaseRenderer;

	public TileData EntityTileData;

	public Vector2Int CurrentPosition;

	private void Awake()
	{
		BaseMovement = new BaseMovement(GetComponent<Transform>());
		BaseRenderer = GetComponent<BaseRenderer>();
	}
}
