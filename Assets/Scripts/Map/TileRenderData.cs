using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New TileRenderData", menuName = "Tiles/Tile Render Data")]
public class TileRenderData : ScriptableObject
{
	public String Name;
	public String Description;
	public Sprite Sprite;

	public Int32 OrderInSortingLayer;
	public Int32 SortingLayerId;
	//public String SortingLayerName;
}