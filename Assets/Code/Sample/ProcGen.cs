//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ProcGen
{
    private void goDown(int[,] level, ref int roomX, ref int roomY, ref int prev)
    {
        if (roomY == level.GetLength(1) - 1) {
            roomY++;
            return;
        }

        if (prev == 2) {
            level[roomX, roomY] = 4;
            prev = 4;
        } else {
            level[roomX, roomY] = 2;
            prev = 2;
        }
        roomY++;
    }

    private void goLeft(int[,] level, ref int roomX, ref int roomY, ref int prev)
    {
        if (roomX == 0) {
            goDown(level, ref roomX, ref roomY, ref prev);
            return;
        }

        if (prev == 2) {
            level[roomX, roomY] = 3;
            prev = 3;
        } else {
            level[roomX, roomY] = 1;
            prev = 1;
        }
        roomX--;
    }

    private void goRight(int[,] level, ref int roomX, ref int roomY, ref int prev)
    {
        if (roomX == level.GetLength(0) - 1) {
            goDown(level, ref roomX, ref roomY, ref prev);
            return;
        }

        if (prev == 2) {
            level[roomX, roomY] = 3;
            prev = 3;
        } else {
            level[roomX, roomY] = 1;
            prev = 1;
        }
        roomX++;
    }

    private void makeSolutionPath(int[,] level)
    {
        int roomX = Random.Range(0, 4);
        int roomY = 0;

        int direction = 0;
        int prevRoom = 0;
        level[roomX, roomY] = 1;

        while (roomY < level.GetLength(1))
        {
            direction = Random.Range(0,5);

            if (direction == 0 || direction == 1)
            {
                //go left
                goLeft(level, ref roomX, ref roomY, ref prevRoom);

            } else if (direction == 2 || direction == 3)
            {
                //go right
                goRight(level, ref roomX, ref roomY, ref prevRoom);

            } else
            {
                //go down
                goDown(level, ref roomX, ref roomY, ref prevRoom);
            }
        }
    }

    public void Generate(World world)
    {
        //2d array of 2d arrays
        //first two dimensions are coords of metatiles/rooms
        //second two dimensions are coords within each metatile/room
        int[,] level = new int[4,4];
        makeSolutionPath(level);

        TextAsset[,] rooms = new TextAsset[6,1];

        for (int i = 0; i < rooms.GetLength(0); i++) {
            TextAsset[] temp = Resources.LoadAll<TextAsset>("RoomData/type" + i);
            for (int j = 0; j < rooms.GetLength(1); j++) {
                rooms[i, j] = temp[j];
            }
        }

        int type;
        int room;
        Chunk chunk;
        for (int i = 0; i < level.GetLength(0); i++) {
            for (int j = 0; j < level.GetLength(1); j++) {
                type = level[i, j];
                room = Random.Range(0, rooms.GetLength(1));

                chunk = new Chunk(i, j, rooms[type, room].text);
                world.SetChunk(i, j, chunk);
            }
        }
        
    }

    
}
