using UnityEngine;
using UnityEditor;

public class TilePickerWindow : EditorWindow
{
    [MenuItem("Window/Tile Picker")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow<TilePickerWindow>();
    }

    private void OnGUI()
    {
        GameObject tileMapObject = Selection.activeGameObject;
        if (tileMapObject)
        {
            TileMap tileMap = tileMapObject.GetComponent<TileMap>();
            if (tileMap)
            {
                GUILayout.Label("Tile map selected.");
            }
            else
            {
                GUILayout.Label("No tile map selected.");
            }
        }
        else
        {
            GUILayout.Label("No tile map selected.");
        }
    }

    private void OnInspectorUpdate()
    {
        Repaint();
    }
}
