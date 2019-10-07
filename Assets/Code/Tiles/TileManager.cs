//
// When We Fell
//

public enum TileType
{
	Air,
	Floor,
	Wall,
	Platform,
	Count
}

public static class TileManager
{
	private static TileData[] data =
	{
		new AirTile(),
		new FloorTile(),
		new WallTile(),
		new PlatformTile()
	};

	public static TileData GetData(TileType type)
		=> data[(int)type];
}
