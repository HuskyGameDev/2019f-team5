using UnityEngine;

public class ProbabilityTile : TileData
{
	public ProbabilityTile()
	{
		name = "Probability";
		sprite = GameAssets.Instance.sprites.GetSprite("Probability");
	}

	public override void OnSet(Chunk chunk, int x, int y)
	{
		if (Random.Range(0, 2) == 0)
			chunk.SetTile(x, y, TileType.CaveWall);
	}
}
