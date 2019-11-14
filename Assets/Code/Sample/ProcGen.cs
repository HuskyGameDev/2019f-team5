//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ProcGen
{
    public GameObject player;

    private void Start()
    {
        player = GameObject.Find("Player");
    }

    private void goDown(int[,] level, ref int roomX, ref int roomY)
    {
        //if trying to go down out of the level, the path is complete
        if (roomY == level.GetLength(1) - 1) {
            //if the level[roomX, roomY-1]ious movement was down, make the current room type 3.
            if (level[roomX, roomY-1] == 2 || level[roomX, roomY-1] == 4) {
                level[roomX, roomY] = 3;
            }
            roomY++;
            return;
        }

        if (roomY > 0 && (level[roomX, roomY-1] == 2 || level[roomX, roomY-1] == 4)) {
            level[roomX, roomY] = 4;
        } else {
            level[roomX, roomY] = 2;
        }
        roomY++;
    }

    private void goLeft(int[,] level, ref int roomX, ref int roomY)
    {
        if (roomX == 0) {
            level[roomX, roomY] = 1;
            goDown(level, ref roomX, ref roomY);
            return;
        }

        if (roomY > 0 && (level[roomX, roomY-1] == 2 || level[roomX, roomY-1] == 4)) {
            level[roomX, roomY] = 3;
        } else if (level[roomX, roomY] == 3) {
            //3 type rooms already have side exits
            //do nothing so the top exit gets preserved    
        } else {
            level[roomX, roomY] = 1;
        }
        roomX--;
    }

    private void goRight(int[,] level, ref int roomX, ref int roomY)
    {
        if (roomX == level.GetLength(0) - 1) {
            level[roomX, roomY] = 1;
            goDown(level, ref roomX, ref roomY);
            return;
        }

        if (roomY > 0 && (level[roomX, roomY-1] == 2 || level[roomX, roomY-1] == 4)) {
            level[roomX, roomY] = 3;
        } else if (level[roomX, roomY] == 3) {
            //3 type rooms already have side exits
            //do nothing so the top exit gets preserved    
        } else {
            level[roomX, roomY] = 1;
        }
        roomX++;
    }

    private void makeSolutionPath(int[,] level)
    {
        int roomX = Random.Range(0, 4);
        int roomY = 0;

        // Spawn the player in the bottom-left corner of the starting room
        player = GameObject.Find("Player");
        player.transform.position = new Vector2(16 * roomX + (float)1.5, 49);

        int direction = 0;
        level[roomX, roomY] = 1;

        while (roomY < level.GetLength(1))
        {
            direction = Random.Range(0,5);
            Debug.Log(direction);

            if (direction == 0 || direction == 1)
            {
                //go left
                goLeft(level, ref roomX, ref roomY);

            } else if (direction == 2 || direction == 3)
            {
                //go right
                goRight(level, ref roomX, ref roomY);

            } else
            {
                //go down
                goDown(level, ref roomX, ref roomY);
            }
        }
    }

    private bool isSpawnable(Chunk chunk, int tileX, int tileY) {
        if (chunk.GetTile(tileX, tileY).type == TileType.Air && 
        chunk.GetTile(tileX, tileY - 1).type != TileType.Air) {
            return true;
        }
        return false;
    }

    public void Generate(World world)
    {
        //2d array of rooms
        int[,] level = new int[4,4];
        makeSolutionPath(level);

        for (int y = 0; y < level.GetLength(1); y++) {
            string output = "";
            for (int x = 0; x < level.GetLength(0); x++) {
                output += level[x, y];
            }
            Debug.Log(output);
        }

        TextAsset[,] rooms = new TextAsset[5,3];
        //get all rooms/chunks
        for (int i = 0; i < rooms.GetLength(0); i++) {
            TextAsset[] temp = Resources.LoadAll<TextAsset>("RoomData/type" + i);
            for (int j = 0; j < rooms.GetLength(1); j++) {
                rooms[i, j] = temp[j];
            }
        }
        //get all enemy prefabs
        GameObject[] mobs = Resources.LoadAll<GameObject>("Mobs");
        int mobCap = 4;

        //fill in level with rooms of appropriate types
        int type;
        int room;
        int row = 3;
        Chunk chunk;
        for (int y = 0; y < level.GetLength(0); y++) {
            for (int x = 0; x < level.GetLength(1); x++) {
                //generate the room
                type = level[x, y];
                room = Random.Range(0, rooms.GetLength(1));

                chunk = new Chunk(x, row, rooms[type, room].text);
                world.SetChunk(x, row, chunk);

                //generate mobs in the room
                int mobTot = 0;
                for (int tileY = 0; tileY < 16; tileY++) {
                    //stop spawning mobs if the cap is reached
                    if (mobTot >= mobCap) { break; }
                    for (int tileX = 0; tileX < 16; tileX++) {
                        //probability a mob spawns in a given space
                        int willSpawn = Random.Range(0,100);
                        if (isSpawnable(chunk, tileX, tileY) && mobTot <= mobCap && willSpawn < 5) {
                            int randMob = Random.Range(0, mobs.GetLength(0));
                            GameObject.Instantiate(mobs[randMob], new Vector2(x * 16 + tileX, y * 16 + tileY), Quaternion.identity);
                            mobTot++;

                        }
                    }
                }

            }
            row--;
        }

        
        
    }

    
}
