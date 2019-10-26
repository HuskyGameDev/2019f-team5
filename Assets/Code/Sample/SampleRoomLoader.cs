//
// When We Fell
//

using UnityEngine;

public class SampleRoomLoader
{
	public void Generate(World world)
	{
		const int RoomCount = 20;

		TextAsset[] rooms = Resources.LoadAll<TextAsset>("RoomData");

		if (rooms.Length == 0)
			return;

		for (int i = 0; i < RoomCount; ++i)
		{
			int room = Random.Range(0, rooms.Length);
			Chunk chunk = new Chunk(i, 0, rooms[room].text);
			world.SetChunk(i, 0, chunk);
		}
	}
}
