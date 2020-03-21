using UnityEngine;

// Notes on room types:
// 0 = arbitrary
// 1 = exits to the left and right.
// 2 = exits to the left, right, and down.
// 3 = exits to the left, right, and up.
// 4 = exits in all four directions.

[System.Serializable]
public class LevelTemplate
{
	public int width, height;
	public int roomTypes;
	public Vector2Int spawn;
	public int[] types;

	public void SetRoomType(int x, int y, int type)
		=> types[y * width + x] = type;

	public int GetRoomType(int x, int y)
		=> types[y * width + x];

	public bool Valid()
		=> types != null && width > 0 && height > 0;
}

public sealed class TemplateGenerator
{
	private GameObject player;

	public RectInt Generate(World world, int seed = -1)
	{
		player = GameObject.FindWithTag("Player");
		int levelID = PlayerPrefs.HasKey("Level") ? PlayerPrefs.GetInt("Level") : 0;

		if (seed == -1)
			seed = Random.Range(int.MinValue, int.MaxValue);

		Random.InitState(seed);

		// Every third level, generate a boss.
		if ((levelID + 1) % 3 == 0)
			return GenerateBossRoom(world);
		else return GenerateCave(world);
	}

	private void GenerateRoom(World world, int roomX, int roomY, string data)
	{
		Chunk chunk = new Chunk(roomX, roomY, data);
		world.SetChunk(roomX, roomY, chunk);

		for (int tileY = 0; tileY < Chunk.Size; tileY++)
		{
			for (int tileX = 0; tileX < Chunk.Size; tileX++)
			{
				// TODO: Spawning things.
			}
		}
	}

	private RectInt GenerateCave(World world)
	{
		TextAsset[] templateList = Resources.LoadAll<TextAsset>("Templates");

		int templateNum = Random.Range(0, templateList.Length);

		LevelTemplate template = JsonUtility.FromJson<LevelTemplate>(templateList[templateNum].text);

		TextAsset[][] roomData = new TextAsset[template.roomTypes][];

		// Load room data.
		for (int i = 0; i < template.roomTypes; i++)
			roomData[i] = Resources.LoadAll<TextAsset>("RoomData/type" + i);

		for (int y = 0; y < template.height; ++y)
		{
			for (int x = 0; x < template.width; ++x)
			{
				int type = template.GetRoomType(x, y);
				string data = roomData[type][Random.Range(0, roomData[type].Length)].text;
				GenerateRoom(world, x, y, data);
			}
		}

		SpawnPlayer(world.GetChunk(template.spawn), template.spawn.x, template.spawn.y);
		AddSolidPerimeter(world, template);

		return new RectInt(0, 0, Chunk.Size * template.width, Chunk.Size * template.height);
	}

	private RectInt GenerateBossRoom(World world)
	{
		return new RectInt(0, 0, 0, 0);
	}

	private bool IsSpawnable(Chunk chunk, int tileX, int tileY)
	{
		// We ensure tileY is larger than 0 for now to ensure the tileY - 1 check doesn't
		// go out of bounds. This prevents enemies from spawning on the bottom row of the room.
		// We can change this fairly easily if we care.
		if (tileY > 0)
		{
			bool passable = TileManager.GetData(chunk.GetTile(tileX, tileY)).passable;
			bool passableBelow = TileManager.GetData(chunk.GetTile(tileX, tileY - 1)).passable;

			if (passable && !passableBelow)
				return true;
		}

		return false;
	}

	// TODO: Use a tile for this.
	private void SpawnPlayer(Chunk chunk, int roomX, int roomY)
	{
		int playerX = 8, playerY = 8;

		int direct = 0;
		int turns = 0;
		int cDist = 0, mDist = 1;

		bool pSpawned = false;

		for (int i = 0; i < Chunk.Size * Chunk.Size; ++i)
		{
			if (IsSpawnable(chunk, playerX, playerY))
			{
				pSpawned = true;
				break;
			}
			else
			{
				//move outward in spiral pattern to find a spawnpoint close to the center
				switch (direct)
				{
					case 0: playerY++; break;
					case 1: playerX++; break;
					case 2: playerY--; break;
					case 3: playerX--; break;
				}

				cDist++;

				if (cDist == mDist)
				{
					cDist = 0;
					//turn "left"
					direct = (direct + 1) % 4;
					turns++;

					if (turns == 2)
					{
						turns = 0;
						mDist++;
					}
				}
			}
		}

		if (!pSpawned)
		{
			playerX = 8;
			playerY = 8;
		}

		player.transform.position = new Vector2(Chunk.Size * roomX + playerX + 0.5f, Chunk.Size * roomY + playerY + 0.05f);
		EventManager.Instance.SignalEvent(GameEvent.PlayerSpawned, null);
	}

	// Adds a solid-filled room around the outside of the map.
	private void AddSolidPerimeter(World world, LevelTemplate template)
	{
		TextAsset data = Resources.Load<TextAsset>("RoomData/Solid");

		for (int y = -1; y < template.height + 1; ++y)
		{
			Chunk left = new Chunk(-1, y, data.text);
			Chunk right = new Chunk(template.width, y, data.text);

			world.SetChunk(-1, y, left);
			world.SetChunk(template.width, y, right);
		}

		for (int x = 0; x < template.width; ++x)
		{
			Chunk bottom = new Chunk(x, -1, data.text);
			Chunk top = new Chunk(x, template.height, data.text);

			world.SetChunk(x, -1, bottom);
			world.SetChunk(x, template.height, top);
		}
	}
}
