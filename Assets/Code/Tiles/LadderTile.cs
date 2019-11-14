//
// When We Fell
//

public class LadderTile : TileData
{
	public LadderTile()
	{
		name = "Ladder";
		sprite = GameAssets.Instance.sprites.GetSprite("Ladder");
		overlapType = TileOverlapType.Climb;
		passable = true;
	}
}
