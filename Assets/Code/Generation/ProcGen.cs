//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ProcGen
{
	public GameObject player;

	private void goDown(int[,] level, ref int roomX, ref int roomY)
	{
		//if trying to go down out of the level, the path is complete
		if (roomY == level.GetLength(1) - 1)
		{
			//if the level[roomX, roomY-1]ious movement was down, make the current room type 3.
			if (level[roomX, roomY - 1] == 2 || level[roomX, roomY - 1] == 4)
			{
				level[roomX, roomY] = 3;
			}
			roomY++;
			return;
		}

		if (roomY > 0 && (level[roomX, roomY - 1] == 2 || level[roomX, roomY - 1] == 4))
		{
			level[roomX, roomY] = 4;
		}
		else
		{
			level[roomX, roomY] = 2;
		}
		roomY++;
	}

	private void goLeft(int[,] level, ref int roomX, ref int roomY)
	{
		if (roomX == 0)
		{
			level[roomX, roomY] = 1;
			goDown(level, ref roomX, ref roomY);
			return;
		}

		if (roomY > 0 && (level[roomX, roomY - 1] == 2 || level[roomX, roomY - 1] == 4))
		{
			level[roomX, roomY] = 3;
		}
		else if (level[roomX, roomY] == 3)
		{
			//3 type rooms already have side exits
			//do nothing so the top exit gets preserved    
		}
		else
		{
			level[roomX, roomY] = 1;
		}
		roomX--;
	}

	private void goRight(int[,] level, ref int roomX, ref int roomY)
	{
		if (roomX == level.GetLength(0) - 1)
		{
			level[roomX, roomY] = 1;
			goDown(level, ref roomX, ref roomY);
			return;
		}

		if (roomY > 0 && (level[roomX, roomY - 1] == 2 || level[roomX, roomY - 1] == 4))
		{
			level[roomX, roomY] = 3;
		}
		else if (level[roomX, roomY] == 3)
		{
			//3 type rooms already have side exits
			//do nothing so the top exit gets preserved    
		}
		else
		{
			level[roomX, roomY] = 1;
		}
		roomX++;
	}

	//returns the x value of the starting room
	//the y value isn't needed as the starting room is always at the top
	private int makeSolutionPath(int[,] level)
	{
		int roomX = Random.Range(0, 4);
		int roomY = 0;

		// Spawn the player in the bottom-left corner of the starting room
		// player = GameObject.Find("Player");
		// player.transform.position = new Vector2(16 * roomX + (float)1.5, 49);

		int direction = 0;
		level[roomX, roomY] = 1;

		while (roomY < level.GetLength(1))
		{
			direction = Random.Range(0, 5);

			if (direction == 0 || direction == 1)
			{
				//go left
				goLeft(level, ref roomX, ref roomY);

			}
			else if (direction == 2 || direction == 3)
			{
				//go right
				goRight(level, ref roomX, ref roomY);

			}
			else
			{
				//go down
				goDown(level, ref roomX, ref roomY);
			}
		}

		return roomX;
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

	public RectInt Generate(World world, int seed = -1)
	{
		player = GameObject.FindWithTag("Player");

		if (seed == -1)
			seed = Random.Range(int.MinValue, int.MaxValue);

		Random.InitState(seed);
		Debug.Log("Seed: " + seed);

		//2d array of rooms
		int[,] level = new int[4, 4];
		int sRoomX = makeSolutionPath(level);
		bool pSpawned = false;

		for (int y = 0; y < level.GetLength(1); y++)
		{
			string output = "";
			for (int x = 0; x < level.GetLength(0); x++)
			{
				output += level[x, y];
			}
		}

		// This variable controls the number of unique rooms of each room type.
		int numRooms = 4;
		TextAsset[,] rooms = new TextAsset[5, numRooms];
		//get all rooms/chunks
		for (int i = 0; i < rooms.GetLength(0); i++)
		{
			TextAsset[] temp = Resources.LoadAll<TextAsset>("RoomData/type" + i);
			for (int j = 0; j < rooms.GetLength(1); j++)
			{
				rooms[i, j] = temp[j];
			}
		}
		//get all enemy prefabs
		GameObject[] mobs = Resources.LoadAll<GameObject>("Mobs");
		int mobCap = 2;

		//fill in level with rooms of appropriate types
		int type;
		int room;
		int row = 3;
		Chunk chunk;
		for (int y = 0; y < level.GetLength(0); y++)
		{
			for (int x = 0; x < level.GetLength(1); x++)
			{
				//generate the room
				type = level[x, y];
				room = Random.Range(0, rooms.GetLength(1));

				chunk = new Chunk(x, row, rooms[type, room].text);
				world.SetChunk(x, row, chunk);

				//spawn the player in a safe space if it's the starting room
				if (x == sRoomX && pSpawned == false)
				{
					int playerX = 8, playerY = 8;

					int direct = 0;
					int turns = 0;
					int cDist = 0, mDist = 1;
					while (!pSpawned)
					{
						if (isSpawnable(chunk, playerX, playerY))
						{
							player.transform.position = new Vector2(16 * x + playerX + 0.5f, 16 * row + playerY + 0.05f);
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

				//generate actors in the room
				int mobTot = 0;
				for (int tileY = 0; tileY < 16; tileY++)
				{
					for (int tileX = 0; tileX < 16; tileX++)
					{
						//stop spawning mobs if the cap is reached
						if (mobTot >= mobCap) { break; }
						//probability a mob spawns in a given space
						int willSpawn = Random.Range(0, 100);
						if (isSpawnable(chunk, tileX, tileY) && mobTot <= mobCap && willSpawn < 5)
						{
							//Debug.Log("test?");
							int randMob = Random.Range(0, mobs.GetLength(0));
							GameObject.Instantiate(mobs[randMob],
								new Vector2(x * 16 + tileX + .5f, row * 16 + tileY + 0.05f), Quaternion.identity);
							mobTot++;

						}
					}
				}

			}
			row--;
			mobCap++;
		}

		return new RectInt(0, 0, Chunk.Size * 4, Chunk.Size * 4);
	}
}
