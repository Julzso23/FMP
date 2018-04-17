using UnityEngine;
using UnityEditor;
using System;
using System.IO;

// Static functions for saving and loading levels
public static class LevelFileSystem
{
    [Serializable]
    private class Tile
    {
        public Sprite sprite;
        public int sortingOrder;
        public Vector2 position;
    }

    [Serializable]
    private class Layer
    {
        public bool enabled;
        public string name;
        public Tile[] tiles;
    }

    // Can't convert to JSON without a container struct
    [Serializable]
    private class Container
    {
        public Layer[] layers;
    }

    public static void SaveLevel(Transform tileMap, string fileName)
    {
        Container container = SerializeTileMap(tileMap);
        string jsonString = JsonUtility.ToJson(container);

        string path = "Assets/" + fileName + ".json";
        File.WriteAllText(path, jsonString);

        AssetDatabase.Refresh();
        TextAsset asset = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
        EditorGUIUtility.PingObject(asset);
    }

    public static void LoadLevel(Transform tileMap, string fileName)
    {
        string jsonString = File.ReadAllText("Assets/" + fileName + ".json");
        Container container = JsonUtility.FromJson<Container>(jsonString);
        DeSerializeTileMap(container, tileMap);
    }

    private static Container SerializeTileMap(Transform tileMap)
    {
        Layer[] layers = new Layer[tileMap.childCount];
        for (int i = 0; i < tileMap.childCount; i++)
        {
            layers[i] = SerializeLayer(tileMap.GetChild(i));
        }

        return new Container()
        {
            layers = layers
        };
    }

    private static Layer SerializeLayer(Transform layer)
    {
        Tile[] tiles = new Tile[layer.childCount];
        for (int i = 0; i < layer.childCount; i++)
        {
            tiles[i] = SerializeTile(layer.GetChild(i));
        }

        return new Layer()
        {
            enabled = layer.gameObject.activeSelf,
            name = layer.name,
            tiles = tiles
        };
    }

    private static Tile SerializeTile(Transform tile)
    {
        return new Tile()
        {
            sprite = tile.GetComponent<SpriteRenderer>().sprite,
            sortingOrder = tile.GetComponent<SpriteRenderer>().sortingOrder,
            position = tile.position
        };
    }

    private static void DeSerializeTileMap(Container container, Transform tileMap)
    {
        while (tileMap.childCount != 0)
        {
            UnityEngine.Object.DestroyImmediate(tileMap.GetChild(0).gameObject);
        }

        foreach (Layer layer in container.layers)
        {
            DeSerializeLayer(tileMap, layer);
        }
    }

    private static void DeSerializeLayer(Transform tileMap, Layer layer)
    {
        GameObject layerObject = new GameObject(layer.name);
        layerObject.transform.SetParent(tileMap);
        layerObject.SetActive(layer.enabled);

        foreach (Tile tile in layer.tiles)
        {
            DeSerializeTile(layerObject.transform, tile);
        }
    }

    private static void DeSerializeTile(Transform layer, Tile tile)
    {
        GameObject tileObject = new GameObject("Tile");
        tileObject.transform.SetParent(layer);
        tileObject.transform.position = tile.position;
        tileObject.AddComponent<SpriteRenderer>().sprite = tile.sprite;
        tileObject.GetComponent<SpriteRenderer>().sortingOrder = tile.sortingOrder;
    }
}
