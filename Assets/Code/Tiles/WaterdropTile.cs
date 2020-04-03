using UnityEngine;

public class WaterdropTile : TileData
{
	private static GameObject[] Waterdrop;

	public WaterdropTile()
		=> name = "Waterdrop";

	public override void OnSet(Chunk chunk, int x, int y)
	{
		if (Waterdrop == null)
			Waterdrop = Resources.LoadAll<GameObject>("Enviorment Particles");

		if (Random.value <= 0.5f)
		{
			Vector2Int wP = chunk.wPos;

			GameObject Waterdrops = Waterdrop[Random.Range(0, Waterdrop.Length)];
			Object.Instantiate(Waterdrops, new Vector2(wP.x + x + 0.5f, wP.y + y + 0.25f), Quaternion.identity);
		}

		chunk.SetTile(x, y, TileType.CaveWall);
	}
}
