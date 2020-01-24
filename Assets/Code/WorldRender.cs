//
// When We Fell
//

using UnityEngine;
using System.Collections.Generic;

public class WorldRender : MonoBehaviour
{
	private World world;
	private Queue<SpriteRenderer> rectPool = new Queue<SpriteRenderer>();

	private Camera cam;
	private List<Chunk> visibleChunks = new List<Chunk>();

	private void Start()
	{
		world = GetComponent<World>();
		cam = Camera.main;
	}

	// Returns a SpriteRenderer to be used to display
	// tiles in a chunk from our pool. 
	// Pooling is much more efficient than instantiating/destroying
	// objects constantly.
	public SpriteRenderer GetTileRect()
	{
		SpriteRenderer rect;

		if (rectPool.Count > 0)
		{
			rect = rectPool.Dequeue();
			rect.enabled = true;
		}
		else
		{
			GameObject prefab = GameAssets.Instance.tileRectPrefab;
			GameObject obj = Instantiate(prefab, world.transform);
			rect = obj.GetComponent<SpriteRenderer>();
		}

		return rect;
	}

	public void ReturnTileRect(SpriteRenderer tileRect)
	{
		tileRect.enabled = false;
		rectPool.Enqueue(tileRect);
	}

	// Gets all chunks that intersect the viewing area and adds them to a list.
	private void GetVisibleChunks()
	{
		// Assume all chunks are out of view initially.
		for (int i = 0; i < visibleChunks.Count; ++i)
			visibleChunks[i].pendingClear = true;

		// Get the world location at the min and max screen corner and convert those positions to
		// chunk positions to get our range.
		Vector2Int min = Utils.WorldToChunkP(cam.ScreenToWorldPoint(Vector3.zero));
		Vector2Int max = Utils.WorldToChunkP(cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height)));

		for (int y = min.y; y <= max.y; ++y)
		{
			for (int x = min.x; x <= max.x; ++x)
			{
				Chunk chunk = world.GetChunk(x, y);

				if (chunk == null)
				{
					chunk = new Chunk(x, y, true);
					world.SetChunk(x, y, chunk);
				}

				// If a chunk is still in view, it's no longer pending clear.
				// Any chunks still pending clear after this function returns
				// are actually out of view and can be cleared.
				if (visibleChunks.Contains(chunk))
					chunk.pendingClear = false;
				else visibleChunks.Add(chunk);
			}
		}
	}

	public void Update()
	{
		GetVisibleChunks();

		// Remove all chunks that are in the visible list and pending clear.
		// Those chunks are now out of view.
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
