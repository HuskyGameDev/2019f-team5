//
// When We Fell
//

using UnityEngine;
using UnityEditor;
using System.IO;

public class TextureBuilder : EditorWindow
{
	private int width, height;
	private new string name;
	private Color color;

	[MenuItem("Extra/Texture Builder")]
	public static void Open()
		=> GetWindow(typeof(TextureBuilder));

	private void OnGUI()
	{
		name = EditorGUILayout.TextField("Name", name);

		EditorGUILayout.BeginHorizontal();
		width = EditorGUILayout.IntField("Width", width);
		height = EditorGUILayout.IntField("Height", height);
		EditorGUILayout.EndHorizontal();

		color = EditorGUILayout.ColorField("Color", color);

		if (GUILayout.Button("Create"))
		{
			Texture2D tex = new Texture2D(width, height);
			tex.filterMode = FilterMode.Point;
			
			for (int y = 0; y < height; ++y)
			{
				for (int x = 0; x < width; ++x)
					tex.SetPixel(x, y, color);
			}

			if (!AssetDatabase.IsValidFolder("Assets/Sprites"))
				AssetDatabase.CreateFolder("Assets", "Sprites");

			string path = Application.dataPath + "/Sprites/" + name + ".png";
			File.WriteAllBytes(path, tex.EncodeToPNG());
			AssetDatabase.Refresh();
		}
	}
}
