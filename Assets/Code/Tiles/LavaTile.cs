using UnityEngine;
using System.Collections.Generic;

public class LavaTile : TileData
{
	// Lava light prefab reference.
	private GameObject lavaLight;

	// Tracks all active lights.
	private Dictionary<Vector2Int, GameObject> lights = new Dictionary<Vector2Int, GameObject>();

	public LavaTile()
	{
		name = "Lava";
		passable = true;
		overlapType = TileOverlapType.Swim;
		sprite = GameAssets.Instance.sprites.GetSprite("Lava");
		otDamage = new OverTimeDamage(TileType.Lava, 0.5f, 2);
	}

	public override void OnSet(Chunk chunk, int x, int y, bool bossRoom = false)
	{
		if (lavaLight == null)
			lavaLight = Resources.Load<GameObject>("Prefabs/LavaLight");

		Vector2Int wP = new Vector2Int(chunk.wPos.x + x, chunk.wPos.y + y);
		GameObject light = Object.Instantiate(lavaLight, new Vector3(wP.x + 0.5f, wP.y + 0.5f, 1.0f), Quaternion.identity);

		lights.Add(wP, light);
	}

	public override void OnDelete(Chunk chunk, int x, int y)
	{
		Vector2Int wP = new Vector2Int(chunk.wPos.x + x, chunk.wPos.y + y);

		if (lights.TryGetValue(wP, out GameObject light))
		{
			Object.Destroy(light);
			lights.Remove(wP);
		}
	}
}
