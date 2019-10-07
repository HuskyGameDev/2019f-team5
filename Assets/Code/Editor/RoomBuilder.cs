//
// When We Fell
//

using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;

// Creates an editor window that allows building a
// 16x16 room and saving it to a data file.
public class RoomBuilder : EditorWindow
{
	[Serializable]
	private class RoomData
	{
		public List<int> tiles = new List<int>();
	}

	private const float PPU = 32.0f;

	private TileType[] tiles = new TileType[Chunk.Size2];
	private Texture[] textures = new Texture[(int)TileType.Count];

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

	private void LoadRoomData(string path)
	{
		string json = File.ReadAllText(path);
		RoomData data;

		try
		{
			data = JsonUtility.FromJson<RoomData>(json);
		}
		catch (Exception e)
		{
			Debug.LogWarning("Cannot open that file in the tile editor.");
			return;
		}

		DecodeRLE(data);
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

		Rect hRect = new Rect(gridStart.x, 15.0f, 150.0f, 25.0f);
		EditorGUI.LabelField(hRect, "Selected: " + selectedName);

		hRect.x += 140.0f;
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
				RoomData data = EncodeRLE();
				string json = JsonUtility.ToJson(data);

				if (!AssetDatabase.IsValidFolder("Assets/RoomData"))
					AssetDatabase.CreateFolder("Assets", "RoomData");

				File.WriteAllText(Application.dataPath + "/RoomData/" + fileName + ".dat", json);
				AssetDatabase.Refresh();
			}
		}

		hRect.x += 65.0f;
		
		// Clear tiles in the room.
		if (GUI.Button(hRect, "Clear"))
		{
			Array.Clear(tiles, 0, tiles.Length);
			Repaint();
		}

		EventType e = Event.current.type;

		if (e == EventType.DragExited)
		{
			if (DragAndDrop.paths.Length == 1)
				LoadRoomData(DragAndDrop.paths[0]);
		}

		// Support click and drag editing.
		if (e == EventType.MouseDown || e == EventType.MouseDrag)
		{
			Vector2 mouseP = Event.current.mousePosition;
			Vector2 gridP = (mouseP - gridStart) / PPU;

			int gridX = Mathf.FloorToInt(gridP.x);
			int gridY = Mathf.FloorToInt(gridP.y);

			// Ensure we're within the bounds of the editing area.
			if (gridX >= 0 && gridX < Chunk.Size && gridY >= 0 && gridY < Chunk.Size)
			{
				int index = Chunk.TileIndex(gridX, gridY);

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
		}

		for (int y = 0; y < Chunk.Size; ++y)
		{
			for (int x = 0; x < Chunk.Size; ++x)
			{
				TileType tile = tiles[Chunk.TileIndex(x, y)];

				Rect rect = new Rect(gridStart.x + (x * PPU), gridStart.y + (y * PPU), PPU, PPU);
				EditorGUI.DrawPreviewTexture(rect, TileManager.GetData(tile).sprite.texture);
			}
		}

		EditorGUILayout.EndScrollView();
	}

	// Encode the room tile data using run-length encoding.
	private RoomData EncodeRLE()
	{
		RoomData data = new RoomData();

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

	private void DecodeRLE(RoomData data)
	{
		int i = 0;
		int loc = 0;

		while (i < Chunk.Size2)
		{
			int count = data.tiles[loc++];
			TileType tile = (TileType)data.tiles[loc++];

			for (int j = 0; j < count; ++j)
				tiles[i++] = tile;
		}
	}
}
