//
// When We Fell
//

// Note: Do not reorder these. It will mess up the saved room data.
// We could try a more robust system, but it probably won't matter
// for this project.

public enum TileType
{
	Air,
	Obstacle,
	Wall,
	Platform,
	CaveWall,
	Probability,
	Ladder,
	EndLevelTile,
	Spawn,
	Powerup,
	Item,
	BossSpawn,
	Waterdrop,
	Count
}

// Stores tile data as a lookup table. Tiles are represented
// by an index. Associated data can be found for a given tile
// type by getting it from here.
public static class TileManager
{
	// Note: this list must match the exact order of the TileType
	// enumeration above.
	private static TileData[] data =
	{
		new AirTile(),
		new ObstacleTile(),
		new WallTile(),
		new PlatformTile(),
		new CaveWall(),
		new ProbabilityTile(),
		new LadderTile(),
		new EndLevelTile(),
		new SpawnTile(),
		new PowerupTile(),
		new ItemTile(),
		new BossSpawn(),
		new WaterdropTile(),
	};

	public static TileData GetData(TileType type)
		=> data[(int)type];

	public static TileData GetData(Tile tile)
		=> GetData(tile.type);
}
