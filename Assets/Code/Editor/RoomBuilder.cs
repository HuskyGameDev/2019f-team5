//
// When We Fell
//

using UnityEditor;
using UnityEngine;
using System;
using System.IO;

// Creates an editor window that allows building a
// 16x16 room and saving it to a data file.
public class RoomBuilder : EditorWindow
{
	private enum EditMode
	{
		Room,
		ObstacleBlock
	}

	private const float PPU = 32.0f;

	// Stores tiles for the room being edited.
	private TileType[] roomTiles = new TileType[Chunk.Size2];

	private Texture[] textures = new Texture[(int)TileType.Count];

	// Stores tiles for the obstacle block being edited.
	private TileType[] blockTiles = new TileType[Chunk.ObstacleBlockWidth * Chunk.ObstacleBlockHeight];

	private EditMode mode = EditMode.Room;

	private TileType tileToSet = TileType.Platform;
	private Vector2 scroll;

	private string fileName;

	[MenuItem("Extra/Room Builder")]
	public static void Open()
		=> GetWindow(typeof(RoomBuilder), false, "Room Builder");

	// Load textures for each tile type from the asset database
	// so that they can be used in the editor.
	private bool LoadTextures()
	{
		string[] names = Enum.GetNames(typeof(TileType));

		for (int i = 0; i < textures.Length; ++i)
		{
			textures[i] = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Sprites/" + names[i] + ".png", typeof(Texture));

			if (textures[i] == null)
			{
				Debug.LogError("Failed to find texture for tile " + names[i]);
				return false;
			}
		}

		return true;
	}

	private bool ValidFileName(string s)
		=> fileName.Length > 0 && fileName.IndexOfAny(Path.GetInvalidFileNameChars()) == -1;

	// Fill all tiles in the room or obstacle block being edited with the current active tile.
	private void Fill()
	{
		TileType[] tiles = mode == EditMode.Room ? roomTiles : blockTiles;

		for (int i = 0; i < tiles.Length; ++i)
			tiles[i] = tileToSet;

		Repaint();
	}

	// Attempts to load in room or obstacle block data from
	// a text asset file on disk.
	private void LoadData(string path, bool room)
	{
		string json = File.ReadAllText(path);
		ChunkData data;

		try
		{
			data = JsonUtility.FromJson<ChunkData>(json);
		}
		catch (Exception)
		{
			Debug.LogWarning("Cannot open that file in the tile editor.");
			return;
		}

		if (room) DecodeRoomRLE(data);
		else DecodeBlockRLE(data);

		// Set the title of the room builder editor window to the name of the file being edited
		// without the full path nor extensions.
		titleContent = new GUIContent(Path.GetFileNameWithoutExtension(path));
	}

	// The GUI for building rooms is flipped by default from how data is represented in
	// the world. This ensures the data will be consistent.
	private int RoomTileIndex(int x, int y)
		=> ((Chunk.Size - 1) - y) * Chunk.Size + x;

	private int BlockTileIndex(int x, int y)
		=> ((Chunk.ObstacleBlockHeight - 1) - y) * Chunk.ObstacleBlockWidth + x;

	private void CreateRoomDataFolder()
	{
		if (!AssetDatabase.IsValidFolder("Assets/Resources/RoomData"))
			AssetDatabase.CreateFolder("Assets/Resources", "RoomData");
	}

	private void CreateObstacleFolder()
	{
		if (!AssetDatabase.IsValidFolder("Assets/Resources/RoomData/Obstacles"))
			AssetDatabase.CreateFolder("Assets/Resources/RoomData", "Obstacles");
	}

	private void OnGUI()
	{
		// Ensures we don't get an ugly icon at the cursor while dragging a file to our window.
		DragAndDrop.visualMode = DragAndDropVisualMode.Generic;

		scroll = EditorGUILayout.BeginScrollView(scroll);
		float tileStart = Chunk.Size * PPU + PPU + 15.0f;

		for (int i = 1; i < textures.Length; ++i)
		{
			if (textures[i] == null)
			{
				if (!LoadTextures())
					return;
			}

			Rect rect = new Rect(tileStart, 10.0f + (i * PPU), PPU, PPU);
			
			if (GUI.Button(rect, textures[i]))
				tileToSet = (TileType)i;
		}

		Vector2 gridStart = new Vector2(PPU, PPU + 15.0f);

		string selectedName = textures[(int)tileToSet].name;

		Rect hRect = new Rect(gridStart.x, 15.0f, 115.0f, 20.0f);

		// Display a button to show the mode we're in, and to toggle modes.
		if (GUI.Button(hRect, "Mode: " + (mode == EditMode.Room ? "Room" : "Obstacle")))
			mode = mode == EditMode.Room ? EditMode.ObstacleBlock : EditMode.Room;

		hRect.x += 125.0f;
		hRect.width = 150.0f;
		hRect.height = 25.0f;
		EditorGUI.LabelField(hRect, "Filename");

		hRect.x += 65.0f;
		hRect.width = 125.0f;
		hRect.height = 20.0f;
		fileName = EditorGUI.TextField(hRect, fileName);

		hRect.x += 130.0f;
		hRect.width = 60.0f;

		// Save the room using the given file name as a data file
		// in the RoomData directory.
		if (GUI.Button(hRect, "Save"))
		{
			if (!ValidFileName(fileName))
				Debug.LogError("Invalid file name.");
			else
			{
				if (mode == EditMode.Room)
				{
					ChunkData data = EncodeRLE(roomTiles);
					string json = JsonUtility.ToJson(data);

					CreateRoomDataFolder();

					File.WriteAllText(Application.dataPath + "/Resources/RoomData/" + fileName + ".txt", json);
				}
				else
				{
					ChunkData data = EncodeRLE(blockTiles);
					string json = JsonUtility.ToJson(data);

					CreateRoomDataFolder();
					CreateObstacleFolder();

					File.WriteAllText(Application.dataPath + "/Resources/RoomData/Obstacles/" + fileName + ".txt", json);
				}
				
				AssetDatabase.Refresh();
				titleContent = new GUIContent(fileName);
			}
		}

		hRect.x += 65.0f;
		
		// Clear tiles in the room.
		if (GUI.Button(hRect, "Clear"))
		{
			if (mode == EditMode.Room)
				Array.Clear(roomTiles, 0, roomTiles.Length);
			else if (mode == EditMode.ObstacleBlock)
				Array.Clear(blockTiles, 0, blockTiles.Length);

			Repaint();
		}

		hRect.x += 65.0f;

		if (GUI.Button(hRect, "Fill"))
			Fill();

		hRect.x += 65.0f;
		hRect.width = 120.0f;

		EditorGUI.LabelField(hRect, "Selected: " + selectedName);

		switch (mode)
		{
			case EditMode.Room:
				RoomEditMode(gridStart);
				break;

			case EditMode.ObstacleBlock:
				ObstacleBlockEditMode(gridStart);
				break;
		}

		EditorGUILayout.EndScrollView();
	}

	private Vector2Int GetMouseGrid(Vector2 gridStart)
	{
		Vector2 mouseP = Event.current.mousePosition;
		Vector2 gridP = (mouseP - gridStart) / PPU;

		int gridX = Mathf.FloorToInt(gridP.x);
		int gridY = Mathf.FloorToInt(gridP.y);

		return new Vector2Int(gridX, gridY);
	}

	private void ProcessClick(TileType[] tiles, int index)
	{
		if (Event.current.button == 0)
		{
			if (tiles[index] != tileToSet)
			{
				tiles[index] = tileToSet;
				Repaint();
			}
		}
		else if (Event.current.button == 1)
		{
			if (tiles[index] != TileType.Air)
			{
				tiles[index] = TileType.Air;
				Repaint();
			}
		}
	}

	// Runs when we're editing a room.
	private void RoomEditMode(Vector2 gridStart)
	{
		EventType e = Event.current.type;

		if (e == EventType.DragExited)
		{
			if (DragAndDrop.paths.Length == 1)
				LoadData(DragAndDrop.paths[0], true);
		}

		// Support click and drag editing.
		if (e == EventType.MouseDown || e == EventType.MouseDrag)
		{
			Vector2Int gridP = GetMouseGrid(gridStart);

			// Ensure we're within the bounds of the editing area.
			if (gridP.x >= 0 && gridP.x < Chunk.Size && gridP.y >= 0 && gridP.y < Chunk.Size)
			{
				int index = RoomTileIndex(gridP.x, gridP.y);
				ProcessClick(roomTiles, index);
			}
		}

		// Draw the tiles making up the current room being edited.
		for (int y = 0; y < Chunk.Size; ++y)
		{
			for (int x = 0; x < Chunk.Size; ++x)
			{
				TileType tile = roomTiles[RoomTileIndex(x, y)];

				Rect rect = new Rect(gridStart.x + (x * PPU), gridStart.y + (y * PPU), PPU, PPU);
				EditorGUI.DrawPreviewTexture(rect, textures[(int)tile]);
			}
		}
	}

	// Called when we're editing an obstacle block.
	private void ObstacleBlockEditMode(Vector2 gridStart)
	{
		EventType e = Event.current.type;

		if (e == EventType.DragExited)
		{
			if (DragAndDrop.paths.Length == 1)
				LoadData(DragAndDrop.paths[0], false);
		}

		// Support click and drag editing.
		if (e == EventType.MouseDown || e == EventType.MouseDrag)
		{
			Vector2Int gridP = GetMouseGrid(gridStart);

			// Ensure we're within the bounds of the editing area.
			if (gridP.x >= 0 && gridP.x < Chunk.ObstacleBlockWidth && gridP.y >= 0 && gridP.y < Chunk.ObstacleBlockHeight)
			{
				int index = BlockTileIndex(gridP.x, gridP.y);
				ProcessClick(blockTiles, index);
			}
		}

		// Draw the tiles making up the obstacle block being edited.
		for (int y = 0; y < Chunk.ObstacleBlockHeight; ++y)
		{
			for (int x = 0; x < Chunk.ObstacleBlockWidth; ++x)
			{
				TileType tile = blockTiles[BlockTileIndex(x, y)];

				Rect rect = new Rect(gridStart.x + (x * PPU), gridStart.y + (y * PPU), PPU, PPU);
				EditorGUI.DrawPreviewTexture(rect, textures[(int)tile]);
			}
		}
	}

	// Encode the room tile data using run-length encoding.
	private ChunkData EncodeRLE(TileType[] tiles)
	{
		ChunkData data = new ChunkData();

		int current = (int)tiles[0];
		int count = 1;

		for (int i = 1; i < tiles.Length; ++i)
		{
			int tile = (int)tiles[i];

			if (tile != current)
			{
				data.tiles.Add(count);
				data.tiles.Add(current);
				count = 1;
				current = tile;
			}
			else ++count;

			if (i == tiles.Length - 1)
			{
				data.tiles.Add(count);
				data.tiles.Add(current);
			}
		}

		return data;
	}

	private void DecodeRoomRLE(ChunkData data)
	{
		int i = 0;
		int loc = 0;

		while (i < Chunk.Size2)
		{
			int count = data.tiles[loc++];
			TileType tile = (TileType)data.tiles[loc++];

			for (int j = 0; j < count; ++j)
				roomTiles[i++] = tile;
		}
	}

	private void DecodeBlockRLE(ChunkData data)
	{
		int i = 0;
		int loc = 0;

		while (i < Chunk.ObstacleBlockWidth * Chunk.ObstacleBlockHeight)
		{
			int count = data.tiles[loc++];
			TileType tile = (TileType)data.tiles[loc++];

			for (int j = 0; j < count; ++j)
				blockTiles[i++] = tile;
		}
	}
}
