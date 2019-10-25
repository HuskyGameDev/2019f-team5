//
// When We Fell
//

public class AirTile : TileData
{
    public AirTile()
	{
		name = "Air";
		passable = true;
		sprite = GameAssets.Instance.sprites.GetSprite("Air");
	}
}
