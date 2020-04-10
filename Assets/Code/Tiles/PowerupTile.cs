using UnityEngine;

public class PowerupTile : TileData
{
	private static GameObject[] powerups;

	public PowerupTile()
		=> name = "Powerup";

	public override void OnSet(Chunk chunk, int x, int y, bool bossRoom = false)
	{
		if (TemplateGenerator.BossActive)
			TemplateGenerator.AddPendingTile(chunk, x, y, TileType.Powerup);
		else
		{
			if (powerups == null)
				powerups = Resources.LoadAll<GameObject>("Power Ups");

			if (Random.value <= 0.5f || bossRoom)
			{
				Vector2Int wP = chunk.wPos;

				GameObject powerup = powerups[Random.Range(0, powerups.Length)];
				Object.Instantiate(powerup, new Vector2(wP.x + x + 0.5f, wP.y + y + 0.25f), Quaternion.identity);
			}
		}

		chunk.SetTile(x, y, TileType.CaveWall);
	}
}
