//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class ProcGen
{
    public void generate(World world)
    {
        //2d array of 2d arrays
        //first two dimensions are coords of metatiles/rooms
        //second two dimensions are coords within each metatile/room
        int[,,,] level = new int[4,4,16,16];

        
    }

    private void solutionPath(int[,,,] level)
    {
        int roomX = Random.Range(0, 4);
        int roomY = 0;

        int direction = 0;
        int prevRoom = 0;

        while (roomY < 4)
        {
            direction = Random.Range(0,5);
            level[roomX, roomY, 0, 0] = 1;

            if (direction == 0 || direction == 1)
            {
                //go left
                goLeft(level, roomX, roomY);

            } else if (direction == 2 || direction == 3)
            {
                //go right
                goRight(level, roomX, roomY);

            } else
            {
                //go down
                goDown(level, roomX, roomY);
            }
        }

        
    }

    private void goLeft(int[,,,] level, ref int roomX, ref int roomY)
    {

    }

    private void goRight(int[,,,] level, ref int roomX, ref int roomY)
    {

    }

    private void goDown(int[,,,] level, ref int roomX, ref int roomY)
    {

    }
}
