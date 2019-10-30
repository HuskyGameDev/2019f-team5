//
// When We Fell
// 

using UnityEngine;
using System.Collections.Generic;

public class World : MonoBehaviour
{
	// Chunks are stored in a hash map for maximum flexibility. This doesn't force 
	// any constraints as to how the world should be.
	// Accessing chunks is slightly slower, but the difference is negligible.
	private Dictionary<Vector2Int, Chunk> chunks = new Dictionary<Vector2Int, Chunk>();

	private void Start()
	{
		//SampleRoomLoader generator = new SampleRoomLoader();
		ProcGen generator = new ProcGen();
		generator.Generate(this);
	}

	// Returns the chunk at the given position, or null if the
	// chunk doesn't exist.
	public Chunk GetChunk(int cX, int cY)
	{
		if (chunks.TryGetValue(new Vector2Int(cX, cY), out Chunk chunk))
			return chunk;

		return null;
	}

	public Chunk GetChunk(Vector2Int cP)
		=> GetChunk(cP.x, cP.y);

	public void SetChunk(int cX, int cY, Chunk chunk)
		=> chunks.Add(new Vector2Int(cX, cY), chunk);

	// Returns the tile at the given world location.
	public Tile GetTile(int wX, int wY)
	{
		Vector2Int cP = Utils.WorldToChunkP(wX, wY);
		Chunk chunk = GetChunk(cP);

		if (chunk == null)
			return TileType.Air;

		Vector2Int rel = Utils.WorldToRelP(wX, wY);
		return chunk.GetTile(rel.x, rel.y);
	}

	// Sets a tile at the given world location. Computes the chunk the tile belongs
	// in, and creates it if it doesn't exist.
	public void SetTile(int wX, int wY, Tile tile)
	{
		Vector2Int cP = Utils.WorldToChunkP(wX, wY);
		Chunk chunk = GetChunk(cP);

		if (chunk == null)
		{
			chunk = new Chunk(cP.x, cP.y);
			chunks.Add(cP, chunk);
		}

		Vector2Int rel = Utils.WorldToRelP(wX, wY);
		chunk.SetTile(rel.x, rel.y, tile);
	}

	public List<Entity> GetOverlappingEntities(AABB bb)
	{
		List<Entity> result = new List<Entity>();

		Vector2Int minChunk = Utils.WorldToChunkP(bb.center - bb.radius);
		Vector2Int maxChunk = Utils.WorldToChunkP(bb.center + bb.radius);

		for (int y = minChunk.y; y <= maxChunk.y; ++y)
		{
			for (int x = minChunk.x; x <= maxChunk.x; ++x)
			{
				Chunk chunk = GetChunk(x, y);

				if (chunk == null)
					continue;

				List<Entity> list = chunk.entities;

				for (int i = 0; i < list.Count; i++)
				{
					Entity targetEntity = list[i];

					if (AABB.TestOverlap(bb, targetEntity.GetBoundingBox()))
						result.Add(targetEntity);
				}
			}
		}

		return result;
	}
}
