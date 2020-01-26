//
// When We Fell
//

using UnityEngine;

public class SampleGenerator
{
	public void Generate(World world)
	{
		const int FloorLength = 100;

		for (int x = 0; x < FloorLength; ++x)
			world.SetTile(x, 0, TileType.Floor);

		for (int y = 1; y < 15; ++y)
		{
			world.SetTile(0, y, TileType.Wall);
			world.SetTile(FloorLength - 1, y, TileType.Wall);
		}

		int platformX = 1;

		while (platformX < FloorLength)
		{
			int platformLength = Random.Range(3, 5);
			int platformEnd = platformX + platformLength;

			if (platformEnd >= FloorLength - 1)
				break;

			int platformY = Random.Range(2, 4);

			for (int x = platformX; x <= platformEnd; ++x)
				world.SetTile(x, platformY, TileType.Platform);

			platformX += Random.Range(6, 9);
		}
	}
}
