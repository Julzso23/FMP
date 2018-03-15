using UnityEngine;

public class TileMap : MonoBehaviour
{
    public Sprite SpriteSelection { get; set; }

    [SerializeField] private Texture2D textureAtlas;
    public Texture2D TextureAtlas
    {
        get
        {
            return textureAtlas;
        }
    }

    private void OnDrawGizmos()
    {
        DrawGrid(-1000f, 1000f, 1f);
    }

    private void DrawGrid(float min, float max, float cellSize)
    {
        for (float x = min; x < max; x += cellSize)
        {
            Gizmos.DrawLine(new Vector2(x, min), new Vector2(x, max));
        }

        for (float y = min; y < max; y += cellSize)
        {
            Gizmos.DrawLine(new Vector2(min, y), new Vector2(max, y));
        }
    }
}
