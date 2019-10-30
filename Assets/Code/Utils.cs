//
// When We Fell
//

using UnityEngine;
using System.Collections.Generic;

public static class Utils
{
	// Converts a chunk position into a world (tile) position.
	public static Vector2Int ChunkToWorldP(int cX, int cY)
		=> new Vector2Int(cX, cY) * Chunk.Size;

	public static int WorldToChunkP(int v)
		=> Mathf.FloorToInt(v / (float)Chunk.Size);

	// Converts a world (tile) position into a chunk position.
	public static Vector2Int WorldToChunkP(int wX, int wY)
		=> new Vector2Int(WorldToChunkP(wX), WorldToChunkP(wY));

	public static Vector2Int WorldToChunkP(Vector3 wP)
		=> WorldToChunkP(Mathf.FloorToInt(wP.x), Mathf.FloorToInt(wP.y));

	// Converts a world (tile) position to a position relative
	// to the chunk (between 0 and Chunk.Size - 1).
	public static Vector2Int WorldToRelP(int wX, int wY)
		=> new Vector2Int(wX % Chunk.Size, wY % Chunk.Size);

	public static float Square(float v)
		=> v * v;

	public static Vector2Int CeilToInt(Vector2 v)
		=> new Vector2Int(Mathf.CeilToInt(v.x), Mathf.CeilToInt(v.y));

	public static Vector2Int TilePos(Vector2 pos)
		=> new Vector2Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y));
}
