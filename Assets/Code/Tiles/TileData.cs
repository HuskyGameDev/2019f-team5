//
// When We Fell
//

using UnityEngine;

public class TileData
{
	public Sprite sprite { get; protected set; }
	public int sortingOrder { get; protected set; }
	public bool visible { get; protected set; } = true;
	public float alpha { get; protected set; } = 1.0f;
	public bool passable { get; protected set; }
	public string name { get; protected set; }
}
