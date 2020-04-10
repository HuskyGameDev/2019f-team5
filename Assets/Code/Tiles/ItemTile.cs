using UnityEngine;

public class ItemTile : TileData
{
	private static GameObject[] items;

	public ItemTile()
		=> name = "Items";

	public override void OnSet(Chunk chunk, int x, int y)
	{
		if (TemplateGenerator.BossActive)
			TemplateGenerator.AddPendingTile(chunk, x, y, TileType.Item);
		else
		{
			if (items == null)
				items = Resources.LoadAll<GameObject>("Items");

			if (Random.value <= 0.5f)
			{
				Vector2Int wP = chunk.wPos;

				GameObject item = items[Random.Range(0, items.Length)];
				Object.Instantiate(item, new Vector2(wP.x + x + 0.5f, wP.y + y + 0.25f), Quaternion.identity);
			}
		}

		chunk.SetTile(x, y, TileType.CaveWall);
	}
}
