//
// When We Fell
//

public class EndLevelTile : TileData
{
    public EndLevelTile()
	{
		name = "EndLevelTile";
		passable = true;
		sprite = GameAssets.Instance.sprites.GetSprite("EndLevelTile");
		overlapType = TileOverlapType.Trigger;
	}

	public override void OnSet(Chunk chunk, int x, int y, bool bossRoom = false)
	{
		if (TemplateGenerator.BossActive)
		{
			TemplateGenerator.AddPendingTile(chunk, x, y, TileType.EndLevelTile);
			chunk.SetTile(x, y, TileType.CaveWall);
		}
	}
}
