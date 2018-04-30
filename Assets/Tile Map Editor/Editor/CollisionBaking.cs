using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;

public static class CollisionBaking
{
    public static List<List<Vector2>> BakeLayer(Transform layer)
    {
        List<Vector2> corners = new List<Vector2>();

        foreach (Transform tile in layer)
        {
            Vector2[] positions = new Vector2[]
            {
                tile.position, // Top-left
                (Vector2)tile.position + Vector2.right, // Top-right
                (Vector2)tile.position + Vector2.down, // Bottom-left
                (Vector2)tile.position + new Vector2(1f, -1f) // Bottom-right
            };

            // Check if any of the tile's corners are corners of the level as a whole
            foreach (Vector2 position in positions)
            {
                if (IsCorner(position, layer) && !corners.Contains(position))
                {
                    corners.Add(position);
                }
            }
        }

        // Don't try to bake the collision if there aren't any corners
        if (corners.Count == 0)
        {
            return null;
        }

        List<List<Vector2>> paths = new List<List<Vector2>>();

        int currentPath = 0;

        while (corners.Count != 0)
        {
            // Add a new path
            if (currentPath + 1 > paths.Count)
            {
                paths.Add(new List<Vector2>());
            }

            // Add the first corner
            int currentCorner = 0;
            paths[currentPath].Add(corners[0]);
            corners.Remove(corners[0]);

            Stopwatch watch = Stopwatch.StartNew();

            // End the path when the two ends connect
            while ((paths[currentPath].Count < 3) || !IsConnected(paths[currentPath][0], paths[currentPath][paths[currentPath].Count - 1], layer))
            {
                // Find the next corner that connects to the path
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

                // Add 2 second timeout to stop freezing if baking fails
                if (watch.ElapsedMilliseconds > 2000)
                {
                    watch.Stop();
                    break;
                }
            }

            currentPath++;
        }

        return paths;
    }

    private static bool IsCorner(Vector2 position, Transform layer)
    {
        int adjacentCount = GetAdjacentTileCount(position, layer);

        // A corner point will always have either 1 or 3 adjacent tiles
        if (adjacentCount == 1 || adjacentCount == 3)
        {
            return true;
        }

        return false;
    }

    private static int GetAdjacentTileCount(Vector2 position, Transform layer)
    {
        int adjacentCount = 0;

        if (HasTile(layer, position + new Vector2(-1f, 1f))) adjacentCount++; // Top-left
        if (HasTile(layer, position + Vector2.up)) adjacentCount++; // Top-right
        if (HasTile(layer, position + Vector2.left)) adjacentCount++; // Bottom-left
        if (HasTile(layer, position)) adjacentCount++; // Bottom-right

        return adjacentCount;
    }

    private static bool IsConnected(Vector2 position1, Vector2 position2, Transform layer)
    {
        if (position1 == position2)
        {
            return false;
        }

        // x-axis connection
        if ((position1.x != position2.x) && (position1.y == position2.y))
        {
            // Make sure the smallest point is first
            if (position1.x > position2.x)
            {
                Vector2 temp = position2;
                position2 = position1;
                position1 = temp;
            }

            // Check for tile collision
            if (HasTile(layer, new Vector2(position1.x, position1.y)) &&
                HasTile(layer, new Vector2(position1.x, position1.y + 1f)))
            {
                return false;
            }

            for (float x = position1.x + 1; x < position2.x; x++)
            {
                // Check for tiles forming an edge
                if (GetAdjacentTileCount(new Vector2(x, position1.y), layer) != 2)
                {
                    return false;
                }
            }

            return true;
        }

        // y-axis connection
        if ((position1.x == position2.x) && (position1.y != position2.y))
        {
            // Make sure the smallest point is first
            if (position1.y > position2.y)
            {
                Vector2 temp = position2;
                position2 = position1;
                position1 = temp;
            }

            // Check for tile collision
            if (HasTile(layer, new Vector2(position2.x, position2.y)) &&
                HasTile(layer, new Vector2(position2.x - 1f, position2.y)))
            {
                return false;
            }

            for (float y = position1.y + 1; y < position2.y; y++)
            {
                // Check for tiles forming an edge
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
        // Look for tiles at this position
        foreach (Transform tile in layer)
        {
            if ((Vector2)tile.position == position)
            {
                return true;
            }
        }
        return false;
    }
}
