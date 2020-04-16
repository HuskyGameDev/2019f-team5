//
// When We Fell
//

using UnityEngine;

public enum TileOverlapType
{
	None,
	Climb,
	Swim,
	Trigger
}

public class TileData
{
	public Sprite sprite { get; protected set; }
	public int sortingOrder { get; protected set; }
	public bool visible { get; protected set; } = true;
	public float alpha { get; protected set; } = 1.0f;
	public bool passable { get; protected set; }
	public TileOverlapType overlapType { get; protected set; }
	public string name { get; protected set; }
	public OverTimeDamage otDamage;

	// Called whenever a tile is set into the world. Allows per-tile
	// functionality in this regard.
	public virtual void OnSet(Chunk chunk, int x, int y, bool bossRoom = false) { }

	// Called whenever a tile is deleted.
	public virtual void OnDelete(Chunk chunk, int x, int y) { }
}
