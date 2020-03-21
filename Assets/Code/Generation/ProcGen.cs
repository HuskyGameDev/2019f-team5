using UnityEngine;
using System.Collections.Generic;

// NOTE:
// We now use TemplateGenerator as our generator.
// This was the previous generator. This is left
// for reference reasons only.

public class ProcGen
{
	struct PathEntry
	{
		public int x, y;
		public bool[] dirs;

		public PathEntry(int x, int y)
		{
			this.x = x;
			this.y = y;
			dirs = new bool[4];
		}
	}

	private List<(int, Vector2)> enemiesToSpawn = new List<(int, Vector2)>();

	private const int Left = 0, Right = 1, Down = 2, Up = 3;

	private GameObject player;

	private GameObject[] mobs;
	private GameObject[] bosses;

	private int mobCap = 2;

	private const int numTypes = 5;

	// Width and height of the level in rooms.
	private int levelWidth = 4;
	private int levelHeight = 4;

	// A flattened 2D array representing the room types at each location.
	private int[] level;

	private List<PathEntry> solutionPath = new List<PathEntry>();

	// Tracks which rooms have been put in the solution path already.
	// This prevents the path overwriting itself.
	private HashSet<Vector2Int> checkedRooms = new HashSet<Vector2Int>();

	// Provides all options for a given exit direction.
	// For example, the hash set corresponding to index 0
	// contains all rooms that exit to the left.
	private HashSet<int>[] roomOptions = new HashSet<int>[4];

	private static TextAsset[] obstacles;

	public ProcGen()
	{
		mobs = Resources.LoadAll<GameObject>("Mobs");
		bosses = Resources.LoadAll<GameObject>("Boss");

		player = GameObject.FindWithTag("Player");

		roomOptions[Left] = new HashSet<int>() { 1, 2, 3, 4 };
		roomOptions[Right] = new HashSet<int>() { 1, 2, 3, 4 };
		roomOptions[Down] = new HashSet<int>() { 2, 4 };
		roomOptions[Up] = new HashSet<int>() { 3, 4 };
	}

	// Returns a random obstacle from all obstacle blocks
	// saved in the Resources/RoomData/Obstacles folder.
	public static TextAsset GetRandomObstacle()
	{
		if (obstacles == null)
			obstacles = Resources.LoadAll<TextAsset>("RoomData/Obstacles");

		int r = Random.Range(0, obstacles.Length);
		return obstacles[r];
	}

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

	public RectInt Generate(World world, int seed = -1)
	{
		int levelID = PlayerPrefs.HasKey("Level") ? PlayerPrefs.GetInt("Level") : 0;

		if (seed == -1)
			seed = Random.Range(int.MinValue, int.MaxValue);

		Random.InitState(seed);

		// Every third level, generate a boss.
		if ((levelID + 1) % 3 == 0)
			return GenerateBossRoom(world);
		else return GenerateCave(world);
	}

	private RectInt GenerateCave(World world)
	{
		levelWidth = 4;
		levelHeight = 4;

		level = new int[levelWidth * levelHeight];

		Vector2Int startRoom = MakeSolutionPath();
		SetSolutionPathRooms();

		TextAsset[][] roomData = new TextAsset[numTypes][];

		// Load room data.
		for (int i = 0; i < numTypes; i++)
			roomData[i] = Resources.LoadAll<TextAsset>("RoomData/type" + i);

		//fill in level with rooms of appropriate types
		int type;
		int room;
		Chunk chunk;

		for (int roomY = levelWidth - 1; roomY >= 0; roomY--)
		{
			for (int roomX = 0; roomX < levelHeight; roomX++)
			{
				//generate the room
				type = GetRoomType(roomX, roomY);
				room = Random.Range(0, roomData[type].Length);

				chunk = new Chunk(roomX, roomY, roomData[type][room].text);
				world.SetChunk(roomX, roomY, chunk);

				//spawn the player in a safe space if it's the starting room
				if (roomX == startRoom.x && roomY == levelHeight - 1)
					SpawnPlayer(chunk, roomX, roomY);

				if (roomX == startRoom.x && roomY == startRoom.y)
					continue;

				//generate actors in the room
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

			mobCap++;
		}

		AddEndLevelTile(world);
		AddPowerups(world);
		AddSolidPerimeter(world);
		AddItems(world);

		for (int i = 0; i < enemiesToSpawn.Count; ++i)
		{
			(int, Vector2) toSpawn = enemiesToSpawn[i];
			SpawnEntity(toSpawn.Item1, toSpawn.Item2);
		}

		return new RectInt(0, 0, Chunk.Size * levelWidth, Chunk.Size * levelHeight);
	}

	private RectInt GenerateBossRoom(World world)
	{
		TextAsset[] roomData = Resources.LoadAll<TextAsset>("RoomData/boss");

		levelWidth = 2;
		levelHeight = 2;

		level = new int[levelWidth * levelHeight];

		for (int roomY = 0; roomY < levelHeight; ++roomY)
		{
			for (int roomX = 0; roomX < levelHeight; ++roomX)
			{
				int choice = Random.Range(0, roomData.Length);

				Chunk chunk = new Chunk(roomX, roomY, roomData[choice].text);
				world.SetChunk(roomX, roomY, chunk);

				if (roomX == 0 && roomY == 0)
					SpawnPlayer(chunk, roomX, roomY);
			}
		}

		AddSolidPerimeter(world);

		int boss = Random.Range(0, bosses.Length);
		Object.Instantiate(bosses[boss], new Vector3(levelWidth * Chunk.Size - 8.0f, 3.0f), Quaternion.identity);

		return new RectInt(0, 0, Chunk.Size * levelWidth, Chunk.Size * levelHeight);
	}

	// Returns a random tile that is passable but has a solid surface
	// directly under it, in the given chunk. It will randomly
	// sample points up to 'maxTries' number of times, and return
	// null if it fails to find a location. This is not the most
	// efficient way to do this, but should work for now.
	private Vector2Int? RandomSurface(Chunk chunk, int maxTries = 1024)
	{
		bool passable, passableBelow;
		int relX, relY;
		int tries = 0;

		do
		{
			relX = Random.Range(0, Chunk.Size);
			relY = Random.Range(1, Chunk.Size);

			passable = TileManager.GetData(chunk.GetTile(relX, relY)).passable;
			passableBelow = TileManager.GetData(chunk.GetTile(relX, relY - 1)).passable;

			if (++tries == maxTries)
				return null;
		}
		while (!passable || passableBelow);

		return new Vector2Int(relX, relY);
	}

	// Add the end level tile randomly in the final
	// room in the solution path.
	private void AddEndLevelTile(World world)
	{
		PathEntry entry = solutionPath[solutionPath.Count - 1];
		Chunk chunk = world.GetChunk(entry.x, entry.y);

		Vector2Int? randP = RandomSurface(chunk, 4096);

		if (randP == null)
			Debug.LogError("Failed to find a passable surface to place the end level tile in!");
		else
		{
			Vector2Int p = randP.Value;
			chunk.SetTile(p.x, p.y, TileType.EndLevelTile);
		}
	}

	// Add 1-2 powerups somewhere along the solution path.
	private void AddPowerups(World world)
	{
		GameObject[] powerups = Resources.LoadAll<GameObject>("Power Ups");

		int amt = Random.Range(1, 3);

		for (int i = 0; i < amt; ++i)
		{
			// The solution path stores information about every chunk along it.
			// We can grab a random path entry and get the chunk from that.
			// That will be where we generate the powerup.
			PathEntry entry = solutionPath[Random.Range(0, solutionPath.Count)];

			Chunk chunk = world.GetChunk(entry.x, entry.y);
			Vector2Int? randP = RandomSurface(chunk);

			if (randP != null)
			{
				Vector2Int rel = randP.Value;
				GameObject powerup = powerups[Random.Range(0, powerups.Length)];
				Object.Instantiate(powerup, new Vector2(entry.x * Chunk.Size + rel.x + 0.5f, entry.y * Chunk.Size + rel.y + 0.25f), Quaternion.identity);
			}
		}
	}

	private void AddItems(World world)
	{
		GameObject[] items = Resources.LoadAll<GameObject>("Items");

		int amt = Random.Range(0, 6);

		for (int i = 0; i < amt; ++i)
		{
			PathEntry entry = solutionPath[Random.Range(0, solutionPath.Count)];

			Chunk chunk = world.GetChunk(entry.x, entry.y);
			bool passable, passableBelow;

			int relX, relY;
			int tries = 0;

			do
			{
				relX = Random.Range(0, Chunk.Size);
				relY = Random.Range(1, Chunk.Size);

				passable = TileManager.GetData(chunk.GetTile(relX, relY)).passable;
				passableBelow = TileManager.GetData(chunk.GetTile(relX, relY - 1)).passable;

				if (++tries == 1024)
					return;
			}
			while (!passable || passableBelow);

			GameObject item = items[Random.Range(0, items.Length)];
			Object.Instantiate(item, new Vector2(entry.x * Chunk.Size + relX + 0.5f, entry.y * Chunk.Size + relY + 0.25f), Quaternion.identity);
		}
	}

	private int GetRoomType(int roomX, int roomY)
	{
		if (roomX >= 0 && roomX < levelWidth && roomY >= 0 && roomY < levelWidth)
			return level[roomY * levelWidth + roomX];

		return -1;
	}

	private void SetRoomType(int roomX, int roomY, int type)
	{
		if (roomX >= 0 && roomX < levelWidth && roomY >= 0 && roomY < levelWidth)
			level[roomY * levelWidth + roomX] = type;
	}

	// Fills the solutionPath list with a path through the level.
	// It is guaranteed this path will be traversable. Other paths
	// could also generate off the main path by chance.
	private Vector2Int MakeSolutionPath()
	{
		int startRoomX = Random.Range(0, 4);
		int startRoomY = levelHeight - 1;

		int roomX = startRoomX;
		int roomY = startRoomY;

		PathEntry first = new PathEntry(roomX, roomY);
		solutionPath.Add(first);
		checkedRooms.Add(new Vector2Int(roomX, roomY));

		int prevIndex = 0;

		// End once we've reached the bottom row of the level.
		// We use checkedRooms to ensure we don't overwrite
		// locations we've already visited.
		while (roomY > 0)
		{
			PathEntry prev = solutionPath[prevIndex];
			int direction = Random.Range(0, 5);

			if (direction == 0 || direction == 1)
			{
				if (roomX > 0 && !checkedRooms.Contains(new Vector2Int(roomX - 1, roomY)))
					AddLeft();
				else AddDown();
			}
			else if (direction == 2 || direction == 3)
			{
				if (roomX < levelWidth - 1 && !checkedRooms.Contains(new Vector2Int(roomX + 1, roomY)))
					AddRight();
				else AddDown();
			}
			else AddDown();

			solutionPath[prevIndex++] = prev;

			// Local functions for adding a solution path
			// entry in the given direction.

			void AddLeft()
			{
				--roomX;
				prev.dirs[Left] = true;

				PathEntry next = new PathEntry(roomX, roomY);
				next.dirs[Right] = true;

				solutionPath.Add(next);
				checkedRooms.Add(new Vector2Int(roomX, roomY));
			}

			void AddRight()
			{
				++roomX;
				prev.dirs[Right] = true;

				PathEntry next = new PathEntry(roomX, roomY);
				next.dirs[Left] = true;

				solutionPath.Add(next);
				checkedRooms.Add(new Vector2Int(roomX, roomY));
			}

			void AddDown()
			{
				--roomY;
				prev.dirs[Down] = true;

				PathEntry next = new PathEntry(roomX, roomY);
				next.dirs[Up] = true;

				solutionPath.Add(next);
				checkedRooms.Add(new Vector2Int(roomX, roomY));
			}
		}

		return new Vector2Int(startRoomX, startRoomY);
	}

	// Given a filled solution path, sets the room types
	// along the path randomly according to what is possible.
	// This method is not very efficient, though it won't matter
	// for our purposes currently. If it later does, we can optimize
	// it then.
	private void SetSolutionPathRooms()
	{
		for (int i = 0; i < solutionPath.Count; ++i)
		{
			PathEntry entry = solutionPath[i];

			HashSet<int> options = null;

			// Go through each direction and add all types
			// that are supported by all directions we need
			// to support for this room.
			for (int j = 0; j < 4; ++j)
			{
				if (entry.dirs[j])
				{
					if (options == null)
						options = roomOptions[j];
					else options.IntersectWith(roomOptions[j]);
				}
			}

			int[] arr = new int[options.Count];
			options.CopyTo(arr);

			int choice = Random.Range(0, arr.Length);
			SetRoomType(entry.x, entry.y, arr[choice]);
		}
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

	private void SpawnEntity(int num, Vector2 spawnP)
	{
		Entity entity = Object.Instantiate(mobs[num]).GetComponent<Entity>();
		float yOffset = entity.useCenterPivot ? 0.55f : 0.05f;

		spawnP.x += 0.5f;
		spawnP.y += yOffset;

		entity.transform.position = spawnP;
	}

	// Adds a solid-filled room around the outside of the map.
	private void AddSolidPerimeter(World world)
	{
		TextAsset data = Resources.Load<TextAsset>("RoomData/Solid");

		for (int y = -1; y < levelHeight + 1; ++y)
		{
			Chunk left = new Chunk(-1, y, data.text);
			Chunk right = new Chunk(levelWidth, y, data.text);

			world.SetChunk(-1, y, left);
			world.SetChunk(levelWidth, y, right);
		}

		for (int x = 0; x < levelWidth; ++x)
		{
			Chunk bottom = new Chunk(x, -1, data.text);
			Chunk top = new Chunk(x, levelHeight, data.text);

			world.SetChunk(x, -1, bottom);
			world.SetChunk(x, levelHeight, top);
		}
	}
}
