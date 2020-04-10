using UnityEngine;

public class ProbabilityTile : TileData
{
	public ProbabilityTile()
	{
		name = "Probability";
	}

	public override void OnSet(Chunk chunk, int x, int y, bool bossRoom = false)
	{
		if (Random.Range(0, 2) == 0)
			chunk.SetTile(x, y, TileType.CaveWall);
		else chunk.SetTile(x, y, TileType.Wall);
	}
}
