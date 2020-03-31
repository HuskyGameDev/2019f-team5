using UnityEngine;
using System.Collections.Generic;

// Notes on room types:
// 0 = arbitrary
// 1 = exits to the left and right.
// 2 = exits to the left, right, and down.
// 3 = exits to the left, right, and up.
// 4 = exits in all four directions.
// 5 - Spawn room (guaranteed to exit right).
// 6 - End level room (guaranteed to left/right/up).

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
	private int mobCap = 2;

	private GameObject[] mobs;
	private GameObject[] bosses;

	private List<(int, Vector2)> enemiesToSpawn = new List<(int, Vector2)>();

	public RectInt Generate(World world, int seed = -1)
	{
		if (mobs == null)
			mobs = Resources.LoadAll<GameObject>("Mobs");

		if (bosses == null)
			bosses = Resources.LoadAll<GameObject>("Boss");

		player = GameObject.FindWithTag("Player");
		int levelID = PlayerPrefs.HasKey("Level") ? PlayerPrefs.GetInt("Level") : 0;

		if (seed == -1)
			seed = Random.Range(int.MinValue, int.MaxValue);

		Random.InitState(seed);
		Debug.Log("Seed: " + seed);

		// Every third level, generate a boss.
		if ((levelID + 1) % 3 == 0)
			return GenerateBossRoom(world);
		else return GenerateCave(world);
	}

	private void GenerateRoom(World world, int roomX, int roomY, string data, bool spawnEnemies)
	{
		Chunk chunk = new Chunk(roomX, roomY, data);
		world.SetChunk(roomX, roomY, chunk);

		// Spawn enemies.
		if (spawnEnemies)
		{
			int mobTot = 0;

			for (int tileY = 0; tileY < Chunk.Size; tileY++)
			{
				for (int tileX = 0; tileX < Chunk.Size; tileX++)
				{
					//stop spawning mobs if the cap is reached
					if (mobTot >= mobCap)
						break;

					//probability a mob spawns in a given space
					int willSpawn = Random.Range(0, 100);

					if (IsSpawnable(chunk, tileX, tileY) && mobTot <= mobCap && willSpawn < 5)
					{
						int randMob = Random.Range(0, mobs.GetLength(0));

						// Room position * Chunk.Size gets the world position of the room's corner. The tile position
						// determines the offset into the room. yOffset prevents clipping into walls on spawn.
						Vector2 spawnP = new Vector2(roomX * Chunk.Size + tileX, roomY * Chunk.Size + tileY);

						// Don't spawn enemies yet until the entire level is generaated.
						// Instead, add them to a list to be spawned in at the end.
						enemiesToSpawn.Add((randMob, spawnP));
						mobTot++;

					}
				}
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
				// Pick a random room from the available types as specified by the template.
				int type = template.GetRoomType(x, y);
				string data = roomData[type][Random.Range(0, roomData[type].Length)].text;

				bool spawnEnemies = new Vector2Int(x, y) != template.spawn;
				GenerateRoom(world, x, y, data, spawnEnemies);
			}
		}

		SpawnPlayer(world.GetChunk(template.spawn), template.spawn.x, template.spawn.y);

		for (int i = 0; i < enemiesToSpawn.Count; ++i)
		{
			(int, Vector2) toSpawn = enemiesToSpawn[i];
			SpawnEntity(toSpawn.Item1, toSpawn.Item2);
		}

		AddSolidPerimeter(world, template.width, template.height);

		return new RectInt(0, 0, Chunk.Size * template.width, Chunk.Size * template.height);
	}

	private RectInt GenerateBossRoom(World world)
	{
		int boss = Random.Range(0, bosses.Length);
		string name = bosses[boss].name.ToLower();

		TextAsset[] templateList = Resources.LoadAll<TextAsset>("BossTemplates/" + name);

		int templateNum = Random.Range(0, templateList.Length);

		LevelTemplate template = JsonUtility.FromJson<LevelTemplate>(templateList[templateNum].text);

		Object.Instantiate(bosses[boss], new Vector3(template.width * Chunk.Size - 8.0f, 3.0f), Quaternion.identity);

		TextAsset[][] roomData = new TextAsset[template.roomTypes][];

		// Load room data.
		for (int i = 0; i < template.roomTypes; i++)
			roomData[i] = Resources.LoadAll<TextAsset>("RoomData/boss/type" + i);

		for (int y = 0; y < template.height; ++y)
		{
			for (int x = 0; x < template.width; ++x)
			{
				// Pick a random room from the available types as specified by the template.
				int type = template.GetRoomType(x, y);
				string data = roomData[type][Random.Range(0, roomData[type].Length)].text;

				GenerateRoom(world, x, y, data, false);
			}
		}

		SpawnPlayer(world.GetChunk(template.spawn), template.spawn.x, template.spawn.y);
		AddSolidPerimeter(world, template.width, template.height);

		return new RectInt(0, 0, Chunk.Size * template.width, Chunk.Size * template.height);
	}

	private void SpawnEntity(int num, Vector2 spawnP)
	{
		Entity entity = Object.Instantiate(mobs[num]).GetComponent<Entity>();
		float yOffset = entity.useCenterPivot ? 0.55f : 0.05f;

		spawnP.x += 0.5f;
		spawnP.y += yOffset;

		entity.transform.position = spawnP;
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

	private void SpawnPlayer(Chunk chunk, int roomX, int roomY)
	{
		bool spawned = false;
		Vector2Int spawnP = new Vector2Int(8, 8);

		// Try to find the spawn tile in here. If multiple are found,
		// pick one randomly (TODO).
		for (int y = 0; y < Chunk.Size; ++y)
		{
			for (int x = 0; x < Chunk.Size; ++x)
			{
				if (chunk.GetTile(x, y) == TileType.Spawn)
				{
					spawnP = new Vector2Int(x, y);
					spawned = true;
					chunk.SetTile(x, y, TileType.CaveWall);
					break;
				}
			}
		}

		// We did not find a spawn tile, so spawn at the first surface
		// found, beginning the search at the center.
		if (!spawned)
		{
			int playerX = 8, playerY = 8;

			int direct = 0;
			int turns = 0;
			int cDist = 0, mDist = 1;

			for (int i = 0; i < Chunk.Size * Chunk.Size; ++i)
			{
				if (IsSpawnable(chunk, playerX, playerY))
				{
					spawnP = new Vector2Int(playerX, playerY);
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
		}

		// If spawned is false, then we found no surface to spawn on, 
		// so we'll just throw the player into the center and hope for the best.

		player.transform.position = new Vector2(Chunk.Size * roomX + spawnP.x + 0.5f, Chunk.Size * roomY + spawnP.y + 0.05f);
		EventManager.Instance.SignalEvent(GameEvent.PlayerSpawned, null);
	}

	// Adds a solid-filled room around the outside of the map.
	private void AddSolidPerimeter(World world, int width, int height)
	{
		TextAsset data = Resources.Load<TextAsset>("RoomData/Solid");

		for (int y = -1; y < height + 1; ++y)
		{
			Chunk left = new Chunk(-1, y, data.text);
			Chunk right = new Chunk(width, y, data.text);

			world.SetChunk(-1, y, left);
			world.SetChunk(width, y, right);
		}

		for (int x = 0; x < width; ++x)
		{
			Chunk bottom = new Chunk(x, -1, data.text);
			Chunk top = new Chunk(x, height, data.text);

			world.SetChunk(x, -1, bottom);
			world.SetChunk(x, height, top);
		}
	}
}
