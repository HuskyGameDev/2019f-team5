//
// When We Fell
//

using UnityEngine;
using System.Collections.Generic;

public class WorldRender : MonoBehaviour
{
	private World world;
	private Queue<SpriteRenderer> spritePool = new Queue<SpriteRenderer>();

	private Camera cam;
	private List<Chunk> visibleChunks = new List<Chunk>();

	private void Start()
	{
		world = GetComponent<World>();
		cam = Camera.main;
	}

	public SpriteRenderer GetSpriteRenderer()
	{
		SpriteRenderer rend;

		if (spritePool.Count > 0)
		{
			rend = spritePool.Dequeue();
			rend.enabled = true;
		}
		else
		{
			GameObject obj = GameAssets.Instance.spritePrefab;
			rend = Instantiate(obj, world.transform).GetComponent<SpriteRenderer>();
		}

		return rend;
	}

	public void ReturnSpriteRenderer(SpriteRenderer rend)
	{
		rend.enabled = false;
		spritePool.Enqueue(rend);
	}

	private void GetVisibleChunks()
	{
		for (int i = 0; i < visibleChunks.Count; ++i)
			visibleChunks[i].pendingClear = true;

		Vector2Int min = Utils.WorldToChunkP(cam.ScreenToWorldPoint(Vector3.zero));
		Vector2Int max = Utils.WorldToChunkP(cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height)));

		for (int y = min.y; y <= max.y; ++y)
		{
			for (int x = min.x; x <= max.x; ++x)
			{
				Chunk chunk = world.GetChunk(x, y);

				if (chunk != null)
				{
					if (visibleChunks.Contains(chunk))
						chunk.pendingClear = false;
					else visibleChunks.Add(chunk);
				}
			}
		}
	}

	public void Update()
	{
		GetVisibleChunks();

		for (int i = visibleChunks.Count - 1; i >= 0; --i)
		{
			Chunk chunk = visibleChunks[i];

			if (chunk.pendingClear)
			{
				chunk.ClearSprites(this);
				chunk.pendingClear = false;
				visibleChunks.RemoveAt(i);
			}
		}

		for (int i = 0; i < visibleChunks.Count; ++i)
		{
			Chunk chunk = visibleChunks[i];
			chunk.Update(this);
		}
	}
}
