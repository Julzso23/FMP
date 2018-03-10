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
}
