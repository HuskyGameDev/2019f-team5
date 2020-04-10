//
// When We Fell
//

using UnityEngine;

public class ObstacleTile : TileData
{
	public ObstacleTile()
	{
		name = "Obstacle";
		sprite = GameAssets.Instance.sprites.GetSprite("Obstacle");
		overlapType = TileOverlapType.Climb;
		passable = true;
	}

	public override void OnSet(Chunk chunk, int x, int y, bool bossRoom = false)
	{
		if (Random.Range(0, 2) == 0)
		{
			TextAsset obstacle = ProcGen.GetRandomObstacle();
			chunk.SetObstacleBlock(x, y, obstacle.text);
		}
		else chunk.SetTile(x, y, TileType.Wall);
	}
}
