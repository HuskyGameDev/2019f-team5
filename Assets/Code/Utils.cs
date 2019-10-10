//
// When We Fell
//

using UnityEngine;

public static class Utils
{
	// Converts a chunk position into a world (tile) position.
	public static Vector2Int ChunkToWorldP(int cX, int cY)
		=> new Vector2Int(cX, cY) * Chunk.Size;

	// Converts a world (tile) position into a chunk position.
	public static Vector2Int WorldToChunkP(int wX, int wY)
		=> new Vector2Int(wX / Chunk.Size, wY / Chunk.Size);

	public static Vector2Int WorldToChunkP(Vector3 wP)
		=> WorldToChunkP((int)wP.x, (int)wP.y);

	// Converts a world (tile) position to a position relative
	// to the chunk (between 0 and Chunk.Size - 1).
	public static Vector2Int WorldToRelP(int wX, int wY)
		=> new Vector2Int(wX % Chunk.Size, wY % Chunk.Size);
}
