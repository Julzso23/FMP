using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TileMap))]
public class TileMapEditor : Editor
{
    private void OnSceneGUI()
    {
        if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
        {
            CreateTile(((TileMap)target).SpriteSelection);
            Event.current.Use();
        }
        if (Event.current.type == EventType.MouseDrag && Event.current.button == 0)
        {
            CreateTile(((TileMap)target).SpriteSelection);
            Event.current.Use();
        }

        if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
        {
            RemoveTile();
            Event.current.Use();
        }
        if (Event.current.type == EventType.MouseDrag && Event.current.button == 1)
        {
            RemoveTile();
            Event.current.Use();
        }

        if (Event.current.type == EventType.Layout)
        {
            HandleUtility.AddDefaultControl(0);
        }
    }

    private void CreateTile(Sprite sprite)
    {
        TileMap tileMap = (TileMap)target;

        Vector3 position = GetNewTilePosition();

        Transform oldTile = FindTile(tileMap, position);
        if (oldTile != null)
        {
            oldTile.GetComponent<SpriteRenderer>().sprite = sprite;
            return;
        }

        GameObject tile = new GameObject("Tile");
        tile.transform.SetParent(tileMap.transform);
        tile.transform.position = position;
        tile.AddComponent<SpriteRenderer>().sprite = sprite;
    }

    private void RemoveTile()
    {
        Transform tile = FindTile((TileMap)target, GetNewTilePosition());
        if (tile != null)
        {
            DestroyImmediate(tile.gameObject);
        }
    }

    private Transform FindTile(TileMap tileMap, Vector3 position)
    {
        for (int i = 0; i < tileMap.transform.childCount; i++)
        {
            if (tileMap.transform.GetChild(i).position == position)
            {
                return tileMap.transform.GetChild(i);
            }
        }
        return null;
    }

    private Vector3 GetNewTilePosition()
    {
        Vector3 position = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin;
        position.x = Mathf.Floor(position.x);
        position.y = Mathf.Ceil(position.y);
        position.z = 0f;
        return position;
    }
}
