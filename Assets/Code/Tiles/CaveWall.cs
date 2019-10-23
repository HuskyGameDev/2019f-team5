using UnityEngine;

public class CaveWall : TileData
{
	public CaveWall()
	{
		name = "Cave Wall";
		sprite = GameAssets.Instance.sprites.GetSprite("CaveWall");
	}
}
