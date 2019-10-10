//
// When We Fell
//

using UnityEngine;

public class SampleRoomLoader
{
	public void Generate(World world)
	{
		const int RoomCount = 10;

		TextAsset[] rooms = Resources.LoadAll<TextAsset>("RoomData");

		for (int i = 0; i < RoomCount; ++i)
		{
			int room = Random.Range(0, rooms.Length);
			Chunk chunk = new Chunk(i, 0, rooms[room].text);

			world.SetChunk(i, 0, chunk);
		}
	}
}
