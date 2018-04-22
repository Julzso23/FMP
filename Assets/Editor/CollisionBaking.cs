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

    private static List<List<Vector2>> BakeLayer(Transform layer)
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

        if (corners.Count == 0)
        {
            return null;
        }

        List<List<Vector2>> paths = new List<List<Vector2>>();

        int currentPath = 0;

        while (corners.Count != 0)
        {
            if (currentPath + 1 > paths.Count)
            {
                paths.Add(new List<Vector2>());
            }

            int currentCorner = 0;
            paths[currentPath].Add(corners[0]);
            corners.Remove(corners[0]);

            while (!IsConnected(paths[currentPath][0], paths[currentPath][paths[currentPath].Count - 1], layer))
            {
                foreach (Vector2 corner in corners)
                {
                    if (IsConnected(corner, paths[currentPath][currentCorner], layer))
                    {
                        paths[currentPath].Add(corner);
                        currentCorner++;
                        corners.Remove(corner);
                        break;
                    }
                }
            }

            currentPath++;
        }

        return paths;
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
        if (position1 == position2)
        {
            return false;
        }

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
