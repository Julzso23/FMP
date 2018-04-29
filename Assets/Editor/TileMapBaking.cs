using UnityEngine;

public static class TileMapBaking
{
    public static Texture2D BakeLayerTexture(Transform layer, out Vector2 offset)
    {
        // Layer outer bounds
        Rect rect = new Rect(Vector2.zero, Vector2.zero);

        foreach (Transform tile in layer)
        {
            Sprite sprite = tile.GetComponent<SpriteRenderer>().sprite;

            // If the tile is outside the bounds of the level, expand the bounds
            if (tile.position.x * sprite.rect.width < rect.xMin)
            {
                rect.xMin = tile.position.x * sprite.rect.width;
            }
            if (tile.position.y * sprite.rect.height > rect.yMax)
            {
                rect.yMax = tile.position.y * sprite.rect.height;
            }

            if ((tile.position.x + 1f) * sprite.rect.width > rect.xMax)
            {
                rect.xMax = (tile.position.x + 1f) * sprite.rect.width;
            }
            if ((tile.position.y - 1f) * sprite.rect.height < rect.yMin)
            {
                rect.yMin = (tile.position.y - 1f) * sprite.rect.height;
            }
        }

        rect.x = rect.xMin;
        rect.y = rect.yMax;
        offset = rect.position / 32f;

        // Create a new texture for the level
        Texture2D texture = new Texture2D(Mathf.FloorToInt(rect.width), Mathf.FloorToInt(rect.height), TextureFormat.ARGB32, false);
        texture.filterMode = FilterMode.Point;

        // Set the background to transparent
        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                texture.SetPixel(x, y, Color.clear);
            }
        }

        // Apply the new pixels to the texture
        texture.Apply();

        foreach (Transform tile in layer)
        {
            // Get tile sprite
            Sprite sprite = tile.GetComponent<SpriteRenderer>().sprite;

            const int element = 0;
            const int mip = 0;

            // Convert the tile's world position into texture coordinates
            Vector2 destinationPosition = new Vector2(
                tile.position.x * sprite.rect.width - rect.xMin,
                texture.height + ((tile.position.y - 1f) * sprite.rect.height - rect.y)
            );

            // Copy the tile sprite into the layer texture
            Graphics.CopyTexture(
                sprite.texture,                            // Source texture
                element,                                   // Source element
                mip,                                       // Source mip level
                Mathf.FloorToInt(sprite.rect.xMin),        // Source X
                Mathf.FloorToInt(sprite.rect.yMin),        // Source Y
                Mathf.FloorToInt(sprite.rect.width),       // Source width
                Mathf.FloorToInt(sprite.rect.height),      // Source height
                texture,                                   // Destination texture
                element,                                   // Destination element
                mip,                                       // Destination mip level
                Mathf.FloorToInt(destinationPosition.x),   // Destination X
                Mathf.FloorToInt(destinationPosition.y)    // Destination Y
            );
        }

        return texture;
    }
}
