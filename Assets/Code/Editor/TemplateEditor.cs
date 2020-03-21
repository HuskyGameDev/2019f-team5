using UnityEngine;
using UnityEditor;
using System.IO;

public class TemplateEditor : EditorWindow
{
    private const int MaxWidth = 16;
    private const int MaxHeight = 12;

    private LevelTemplate template;

    private string templateName;

    [MenuItem("Extra/Template Editor")]
    public static void Open()
        => GetWindow(typeof(TemplateEditor), false, "Template Editor");

    private void ResizeTypes()
    {
        if (template.width == 0 || template.height == 0)
            return;

        if (template.types != null)
            System.Array.Resize(ref template.types, template.width * template.height);
    }

    private bool ValidFileName()
        => templateName.Length > 0 && templateName.IndexOfAny(Path.GetInvalidFileNameChars()) == -1;

    private void SaveTemplate()
    {
        if (!ValidFileName())
            Debug.LogError("Invalid file name.");
        else
        {
            string json = JsonUtility.ToJson(template);

            if (!AssetDatabase.IsValidFolder("Assets/Resources/Templates"))
                AssetDatabase.CreateFolder("Assets/Resources", "Templates");

            File.WriteAllText(Application.dataPath + "/Resources/Templates/" + templateName + ".txt", json);

            AssetDatabase.Refresh();
            titleContent = new GUIContent(templateName);
        }
    }

    private void LoadTemplate(string path)
    {
        string json = File.ReadAllText(path);
        
        try
        {
            template = JsonUtility.FromJson<LevelTemplate>(json);
        }
        catch (System.Exception)
        {
            Debug.LogWarning("Cannot open that file in the tile editor.");
            return;
        }

        templateName = Path.GetFileNameWithoutExtension(path);
        titleContent = new GUIContent(templateName);
    }

    private void OnGUI()
    {
        if (template == null)
            template = new LevelTemplate();

        if (template.types != null)
        {
            if (template.types.Length != template.width * template.height)
                ResizeTypes();
        }

        float startX = 10.0f;
        template.roomTypes = Mathf.Max(template.roomTypes, 1);

        DragAndDrop.visualMode = DragAndDropVisualMode.Generic;

        EventType e = Event.current.type;

        if (e == EventType.DragExited)
        {
            if (DragAndDrop.paths.Length == 1)
                LoadTemplate(DragAndDrop.paths[0]);
        }

        Rect rect = new Rect(startX, 10.0f, 70.0f, 22.0f);

        EditorGUI.LabelField(rect, "Level Size: ");

        rect.x += rect.width + 5.0f;
        rect.width = 30.0f;

        EditorGUI.BeginChangeCheck();
        template.width = EditorGUI.IntField(rect, template.width);

        if (EditorGUI.EndChangeCheck())
        {
            template.width = Mathf.Clamp(template.width, 0, MaxWidth);
            ResizeTypes();
        }

        rect.x += rect.width + 5.0f;

        EditorGUI.BeginChangeCheck();
        template.height = EditorGUI.IntField(rect, template.height);

        if (EditorGUI.EndChangeCheck())
        {
            template.height = Mathf.Clamp(template.height, 0, MaxHeight);
            ResizeTypes();
        }

        rect.x += rect.width + 5.0f;
        rect.width = 72.0f;

        EditorGUI.LabelField(rect, "Room Types: ");

        rect.x += rect.width + 5.0f;
        rect.width = 30.0f;
        template.roomTypes = EditorGUI.IntField(rect, template.roomTypes);

        rect.x += rect.width + 5.0f;
        rect.width = 40.0f;

        EditorGUI.LabelField(rect, "Spawn");

        rect.x += rect.width + 5.0f;
        rect.width = 80.0f;
        template.spawn = EditorGUI.Vector2IntField(rect, "", template.spawn);

        template.spawn.x = Mathf.Clamp(template.spawn.x, 0, template.width - 1);
        template.spawn.y = Mathf.Clamp(template.spawn.y, 0, template.height - 1);

        rect.x = startX;
        rect.width = 35.0f;
        rect.y += rect.height + 5.0f;

        EditorGUI.LabelField(rect, "Name");

        rect.x += rect.width + 5.0f;
        rect.width = 100.0f;

        templateName = EditorGUI.TextField(rect, templateName);

        rect.x += rect.width + 5.0f;

        if (GUI.Button(rect, "Save Template"))
            SaveTemplate();

        rect.x = startX;
        rect.y += rect.height + 10.0f;
        rect.width = 30.0f;
        rect.height = 30.0f;

        if (template.Valid())
        {
            // Draw starting at the top first. UI elements are drawn
            // at the top first and going down.
            for (int y = template.height - 1; y >= 0; --y)
            {
                for (int x = 0; x < template.width; ++x)
                {
                    int result = EditorGUI.IntField(rect, template.GetRoomType(x, y));
                    template.SetRoomType(x, y, Mathf.Clamp(result, 0, template.roomTypes - 1));

                    rect.x += rect.width + 5.0f;
                }

                rect.x = startX;
                rect.y += rect.height + 5.0f;
            }
        }
    }
}
