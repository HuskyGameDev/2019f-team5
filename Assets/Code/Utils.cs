//
// When We Fell
//

using UnityEngine;

public static class Utils
{
	public static Vector2Int ChunkToWorldP(int cX, int cY)
		=> new Vector2Int(cX, cY) * Chunk.Size;

	public static Vector2Int WorldToChunkP(int wX, int wY)
		=> new Vector2Int(wX >> Chunk.Shift, wY >> Chunk.Shift);

	public static Vector2Int WorldToChunkP(Vector3 wP)
		=> WorldToChunkP((int)wP.x, (int)wP.y);

	public static Vector2Int WorldToRelP(int wX, int wY)
		=> new Vector2Int(wX & Chunk.Mask, wY & Chunk.Mask);
}
