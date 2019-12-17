using UnityEngine;

[CreateAssetMenu(fileName = "New Tile Data", menuName = "Tiles/Tile Data")]
public class TileData : ScriptableObject
{
	public TileRenderData InitialTileRenderData;
	public Cell InitialCellData;
}