using UnityEngine;

public class CaveWall : TileData
{
	public CaveWall()
	{
		name = "Cave Wall";
		passable = true;
		sprite = GameAssets.Instance.sprites.GetSprite("CaveWall");
	}
}
