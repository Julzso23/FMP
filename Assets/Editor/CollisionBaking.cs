using UnityEngine;
using System.Collections.Generic;

public static class CollisionBaking
{
    public static void BakeLevel(Transform level)
    {
        foreach (Transform layer in level)
        {
            BakeLayer(layer);
        }
    }

    private static void BakeLayer(Transform layer)
    {
        List<Vector2> corners = new List<Vector2>();

        foreach (Transform tile in layer)
        {
            Vector2[] positions = new Vector2[]
            {
                tile.position,
                (Vector2)tile.position + Vector2.right,
                (Vector2)tile.position + Vector2.down,
                (Vector2)tile.position + new Vector2(1f, -1f)
            };

            foreach (Vector2 position in positions)
            {
                if (IsCorner(position, layer))
                {
                    corners.Add(position);
                }
            }
        }
    }

    private static bool IsCorner(Vector2 position, Transform layer)
    {
        int adjacentCount = GetAdjacentTileCount(position, layer);

        if (adjacentCount == 1 || adjacentCount == 3)
        {
            return true;
        }

        return false;
    }

    private static int GetAdjacentTileCount(Vector2 position, Transform layer)
    {
        int adjacentCount = 0;

        if (HasTile(layer, position + new Vector2(-1f, 1f))) adjacentCount++;
        if (HasTile(layer, position + Vector2.up)) adjacentCount++;
        if (HasTile(layer, position + Vector2.left)) adjacentCount++;
        if (HasTile(layer, position)) adjacentCount++;

        return adjacentCount;
    }

    private static bool IsConnected(Vector2 position1, Vector2 position2, Transform layer)
    {
        if ((position1.x != position2.x) && (position1.y == position2.y))
        {
            if (position1.x > position2.x)
            {
                Vector2 temp = position2;
                position2 = position1;
                position1 = temp;
            }

            for (float x = position1.x + 1; x < position2.x; x++)
            {
                if (GetAdjacentTileCount(new Vector2(x, position1.y), layer) != 2)
                {
                    return false;
                }
            }

            return true;
        }

        if ((position1.x == position2.x) && (position1.y != position2.y))
        {
            if (position1.y > position2.y)
            {
                Vector2 temp = position2;
                position2 = position1;
                position1 = temp;
            }

            for (float y = position1.y + 1; y < position2.y; y++)
            {
                if (GetAdjacentTileCount(new Vector2(position1.x, y), layer) != 2)
                {
                    return false;
                }
            }

            return true;
        }

        return false;
    }

    private static bool HasTile(Transform layer, Vector2 position)
    {
        foreach (Transform child in layer)
        {
            if ((Vector2)child.position == position)
            {
                return true;
            }
        }
        return false;
    }
}
