using UnityEngine;
using UnityEditor;

public class TilePickerWindow : EditorWindow
{
    [MenuItem("Window/Tile-map Editor/Tile Picker")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow<TilePickerWindow>();
    }

    private float zoom;
    private Vector2 scrollPosition;

    private void OnEnable()
    {
        zoom = 1f;
    }

    private void OnGUI()
    {
        GameObject tileMapObject = Selection.activeGameObject;
        if (tileMapObject)
        {
            TileMap tileMap = tileMapObject.GetComponent<TileMap>();
            if (tileMap)
            {
                RenderSpriteAtlas(tileMap);
            }
            else
            {
                GUIUtility.ErrorLabel("You must select a tile map object.");
            }
        }
        else
        {
            GUIUtility.ErrorLabel("You must select a tile map object.");
        }
    }

    private void OnInspectorUpdate()
    {
        Repaint();
    }

    private void RenderSpriteAtlas(TileMap tileMap)
    {
        zoom = EditorGUILayout.Slider("Zoom", zoom, 0.5f, 2f);

        using (var scrollView = new EditorGUILayout.ScrollViewScope(scrollPosition))
        {
            Rect textureRect = EditorGUILayout.GetControlRect();
            textureRect.width = 320f * zoom;
            textureRect.height = 320f * zoom;
            EditorGUI.DrawTextureTransparent(textureRect, tileMap.TextureAtlas, ScaleMode.ScaleToFit);
        }
    }
}
