using UnityEngine;

public class ProcGen
{
	private GameObject player;
	private GameObject[] mobs;
	private int mobCap = 2;

	// Width and height of the level in rooms.
	private int levelWidth = 4;
	private int levelHeight = 4;

	// A flattened 2D array representing the room types at each location.
	private int[] level;

	public ProcGen()
	{
		mobs = Resources.LoadAll<GameObject>("Mobs");
		player = GameObject.FindWithTag("Player");

		level = new int[levelWidth * levelHeight];
	}

	public RectInt Generate(World world, int seed = -1)
	{
		if (seed == -1)
			seed = Random.Range(int.MinValue, int.MaxValue);

		Random.InitState(seed);
		Debug.Log("Seed: " + seed);

		Vector2Int startRoom = makeSolutionPath();
		bool pSpawned = false;

		// This variable controls the number of unique rooms of each room type.
		int numTypes = 5;
		int numRooms = 4;

		TextAsset[] rooms = new TextAsset[numTypes * numRooms];

		//get all rooms/chunks
		for (int i = 0; i < numTypes; i++)
		{
			TextAsset[] temp = Resources.LoadAll<TextAsset>("RoomData/type" + i);

			for (int j = 0; j < numRooms; j++)
				rooms[j * numTypes + i] = temp[j];
		}

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
				room = Random.Range(0, numRooms);

				chunk = new Chunk(roomX, roomY, rooms[room * numTypes + type].text);
				world.SetChunk(roomX, roomY, chunk);

				//spawn the player in a safe space if it's the starting room
				if (roomX == startRoom.x && pSpawned == false)
				{
					int playerX = 8, playerY = 8;

					int direct = 0;
					int turns = 0;
					int cDist = 0, mDist = 1;
					while (!pSpawned)
					{
						if (isSpawnable(chunk, playerX, playerY))
						{
							player.transform.position = new Vector2(16 * roomX + playerX + 0.5f, 16 * roomY + playerY + 0.05f);
							pSpawned = true;
							EventManager.Instance.SignalEvent(GameEvent.PlayerSpawned, null);
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
				}

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

						if (isSpawnable(chunk, tileX, tileY) && mobTot <= mobCap && willSpawn < 5)
						{
							int randMob = Random.Range(0, mobs.GetLength(0));
							SpawnEnemy(randMob, roomX, roomY, tileX, tileY);
							mobTot++;

						}
					}
				}
			}

			mobCap++;
		}

		return new RectInt(0, 0, Chunk.Size * 4, Chunk.Size * 4);
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
	
	//returns the position of the starting room
	private Vector2Int makeSolutionPath()
	{
		int roomX = Random.Range(0, 4);
		int roomY = levelHeight - 1;

		int direction = 0;
		SetRoomType(roomX, roomY, 1);

		while (roomY >= 0)
		{
			direction = Random.Range(0, 5);

			if (direction == 0 || direction == 1)
				goLeft(ref roomX, ref roomY);
			else if (direction == 2 || direction == 3)
				goRight(ref roomX, ref roomY);
			else goDown(ref roomX, ref roomY);
		}

		return new Vector2Int(roomX, 0);
	}

	private void goDown(ref int roomX, ref int roomY)
	{
		int belowType = GetRoomType(roomX, roomY - 1);

		//if trying to go down out of the level, the path is complete
		if (roomY == levelHeight - 1)
		{
			//if the level[roomX, roomY-1]ious movement was down, make the current room type 3.
			if (belowType == 2 || belowType == 4)
				SetRoomType(roomX, roomY, 3);

			roomY--;
			return;
		}

		if (roomY > 0 && (belowType == 2 || belowType == 4))
			SetRoomType(roomX, roomY, 4);
		else SetRoomType(roomX, roomY, 2);

		roomY--;
	}

	private void goLeft(ref int roomX, ref int roomY)
	{
		int leftType = GetRoomType(roomX - 1, roomY);

		if (roomX == 0)
		{
			SetRoomType(roomX, roomY, 1);
			goDown(ref roomX, ref roomY);
			return;
		}

		if (roomY > 0 && (leftType == 2 || leftType == 4))
			SetRoomType(roomX, roomY, 3);
		else if (GetRoomType(roomX, roomY) == 3)
		{
			//3 type rooms already have side exits
			//do nothing so the top exit gets preserved    
		}
		else SetRoomType(roomX, roomY, 1);

		roomX--;
	}

	private void goRight(ref int roomX, ref int roomY)
	{
		int rightType = GetRoomType(roomX + 1, roomY);

		if (roomX == levelWidth - 1)
		{
			SetRoomType(roomX, roomY, 1);
			goDown(ref roomX, ref roomY);
			return;
		}

		if (roomY > 0 && (rightType == 2 || rightType == 4))
			SetRoomType(roomX, roomY, 3);
		else if (GetRoomType(roomX, roomY) == 3)
		{
			//3 type rooms already have side exits
			//do nothing so the top exit gets preserved    
		}
		else SetRoomType(roomX, roomY, 1);

		roomX++;
	}

	private bool isSpawnable(Chunk chunk, int tileX, int tileY)
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

	private void SpawnEnemy(int num, int roomX, int row, int tileX, int tileY)
	{
		Entity entity = Object.Instantiate(mobs[num]).GetComponent<Entity>();
		float yOffset = entity.useCenterPivot ? 0.55f : 0.05f;

		// Room position * Chunk.Size gets the world position of the room's corner. The tile position 
		// determines the offset into the room. yOffset prevents clipping into walls on spawn.
		Object.Instantiate(mobs[num], new Vector2(roomX * Chunk.Size + tileX + 0.5f, row * 16 + tileY + yOffset), Quaternion.identity);
	}
}
