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

    private const int outlineSize = 3; // Must be odd
    private Color outlineColor = Color.green;

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

        using (EditorGUILayout.ScrollViewScope scrollView = new EditorGUILayout.ScrollViewScope(scrollPosition))
        {
            scrollPosition = scrollView.scrollPosition;

            Sprite[] sprites = tileMap.TextureAtlas.GetSprites();

            EditorGUILayout.BeginHorizontal();

            foreach (Sprite sprite in sprites)
            {
                Rect spriteRect = new Rect(
                    sprite.rect.x / sprite.texture.width,
                    sprite.rect.y / sprite.texture.height,
                    sprite.rect.width / sprite.texture.width,
                    sprite.rect.height / sprite.texture.height
                );

                Rect rect = GUILayoutUtility.GetRect(
                    sprite.rect.width * zoom + (float)outlineSize * 2f,
                    sprite.rect.height * zoom + (float)outlineSize * 2f
                );

                if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && rect.Contains(Event.current.mousePosition))
                {
                    tileMap.SpriteSelection = sprite;
                    Event.current.Use();
                }

                if (sprite == tileMap.SpriteSelection)
                {
                    EditorGUI.DrawRect(rect, outlineColor);
                    EditorGUI.DrawRect(new Rect(
                        rect.position + new Vector2((float)outlineSize - 1f, (float)outlineSize - 1f),
                        rect.size - new Vector2((float)outlineSize * 2f - 2f, (float)outlineSize * 2f - 2f)
                    ), Color.white);
                }

                GUI.DrawTextureWithTexCoords(new Rect(
                    rect.position + new Vector2((float)outlineSize, (float)outlineSize),
                    rect.size - new Vector2((float)outlineSize * 2f, (float)outlineSize * 2f)
                ), tileMap.TextureAtlas, spriteRect);
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }
    }
}
