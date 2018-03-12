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
        zoom = EditorGUILayout.Slider("Zoom", zoom, 0.5f, 4f);

        using (var scrollView = new EditorGUILayout.ScrollViewScope(scrollPosition))
        {
            Sprite[] sprites = tileMap.TextureAtlas.GetSprites();
            tileMap.SpriteSelection = sprites[0];

            EditorGUILayout.BeginHorizontal();

            foreach (Sprite sprite in sprites)
            {
                Rect spriteRect = new Rect(
                    sprite.rect.x / sprite.texture.width,
                    sprite.rect.y / sprite.texture.height,
                    sprite.rect.width / sprite.texture.width,
                    sprite.rect.height / sprite.texture.height
                );
                Rect rect = GUILayoutUtility.GetRect(sprite.rect.width * zoom + 4f, sprite.rect.height * zoom + 4f);
                if (sprite == tileMap.SpriteSelection)
                {
                    EditorGUI.DrawRect(rect, Color.green);
                    EditorGUI.DrawRect(new Rect(rect.position + new Vector2(1f, 1f), rect.size - new Vector2(2f, 2f)), Color.white);
                }
                GUI.DrawTextureWithTexCoords(new Rect(rect.position + new Vector2(2f, 2f), rect.size - new Vector2(4f, 4f)), tileMap.TextureAtlas, spriteRect);
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }
    }
}
