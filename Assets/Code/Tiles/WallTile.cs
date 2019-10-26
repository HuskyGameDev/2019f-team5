//
// When We Fell
//

using UnityEngine;

public class WallTile : TileData
{
	public WallTile()
	{
		name = "Wall";
		sprite = GameAssets.Instance.sprites.GetSprite("Wall");
	}
}
