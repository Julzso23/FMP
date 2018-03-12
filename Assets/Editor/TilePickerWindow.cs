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

            Sprite[] sprites = tileMap.TextureAtlas.GetSprites();

            Vector2 position = EditorGUILayout.GetControlRect().position;
            foreach (Sprite sprite in sprites)
            {
                Rect rect = new Rect(
                    sprite.rect.x / sprite.texture.width,
                    sprite.rect.y / sprite.texture.height,
                    sprite.rect.width / sprite.texture.width,
                    sprite.rect.height / sprite.texture.height
                );
                GUI.DrawTextureWithTexCoords(new Rect(position, sprite.rect.size * zoom), tileMap.TextureAtlas, rect);
                position += new Vector2(sprite.rect.size.x * zoom + 2f, 0f);
            }
        }
    }
}
