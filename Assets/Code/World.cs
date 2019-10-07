//
// When We Fell
// 

using UnityEngine;
using System.Collections.Generic;

public class World : MonoBehaviour
{
	private Dictionary<Vector2Int, Chunk> chunks = new Dictionary<Vector2Int, Chunk>();

	private void Start()
	{
		SampleGenerator generator = new SampleGenerator();
		generator.Generate(this);
	}

	public Chunk GetChunk(int cX, int cY)
	{
		if (chunks.TryGetValue(new Vector2Int(cX, cY), out Chunk chunk))
			return chunk;

		return null;
	}

	public Chunk GetChunk(Vector2Int cP)
		=> GetChunk(cP.x, cP.y);

	public Tile GetTile(int wX, int wY)
	{
		Vector2Int cP = Utils.WorldToChunkP(wX, wY);
		Chunk chunk = GetChunk(cP);

		if (chunk == null)
			return TileType.Air;

		Vector2Int rel = Utils.WorldToRelP(wX, wY);
		return chunk.GetTile(rel.x, rel.y);
	}

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
}
