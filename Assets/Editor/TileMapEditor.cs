using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEditorInternal;
using System.Collections.Generic;

[CustomEditor(typeof(TileMap))]
public class TileMapEditor : Editor
{
    private TileMapLayers layers;
    private ReorderableList layersList;

    private void OnEnable()
    {
        if (!LoadLayersAsset())
        {
            CreateLayersAsset();
        }

        layersList = new ReorderableList(layers.layers, typeof(string));
        layersList.drawHeaderCallback = (Rect rect) => {
            GUI.Label(rect, "Layers");
        };
        layersList.onAddCallback = AddLayer;
        layersList.drawElementCallback = DrawLayerElement;
        layersList.onRemoveCallback = RemoveLayer;
        layersList.onReorderCallback = ReorderLayers;

        if (layers.layers.Count == 0)
        {
            TileMapLayers.Layer layer = new TileMapLayers.Layer();
            layer.enabled = true;
            layer.name = "Default";
            layers.layers.Add(layer);
        }

        layersList.index = 0;
    }

    private void AddLayer(ReorderableList list)
    {
        TileMapLayers.Layer layer = new TileMapLayers.Layer();
        layer.enabled = true;
        layer.name = "New Layer";
        list.list.Add(layer);
        EditorUtility.SetDirty(layers);
    }

    private void RemoveLayer(ReorderableList list)
    {
        Transform layer = GetLayerTransform((TileMap)target, ((TileMapLayers.Layer)list.list[list.index]).name);
        DestroyImmediate(layer.gameObject);
        list.list.RemoveAt(list.index);

        if (list.index >= list.list.Count)
        {
            list.index--;
        }
    }

    private void ReorderLayers(ReorderableList list)
    {
        for (int i = 0; i < list.list.Count; i++)
        {
            SetLayerSortingOrder(GetLayerTransform((TileMap)target, ((TileMapLayers.Layer)list.list[i]).name), -i);
        }
    }

    private void SetLayerSortingOrder(Transform layer, int sortingOrder)
    {
        foreach (Transform tile in layer)
        {
            tile.GetComponent<SpriteRenderer>().sortingOrder = sortingOrder;
        }
    }

    private void DrawLayerElement(Rect rect, int index, bool isActive, bool isFocused)
    {
        TileMapLayers.Layer layer = (TileMapLayers.Layer)layersList.list[index];

        rect.yMin++;
        rect.height -= 2f;

        bool wasEnabled = layer.enabled;
        layer.enabled = EditorGUI.Toggle(new Rect(rect.position, new Vector2(rect.height, rect.height)), layer.enabled);
        if (layer.enabled != wasEnabled)
        {
            Transform layerObject = ((TileMap)target).transform.Find(layer.name);
            if (layerObject != null)
            {
                layerObject.gameObject.SetActive(layer.enabled);
            }
        }

        rect.xMin += rect.height + 2f;

        string newName = EditorGUI.TextField(rect, layer.name);
        if (layer.name != newName)
        {
            if (!layers.layers.Exists((TileMapLayers.Layer other) => other.name == newName) && newName != "")
            {
                Transform layerObject = ((TileMap)target).transform.Find(layer.name);
                if (layerObject != null)
                {
                    layerObject.name = newName;
                }

                layer.name = newName;
            }
        }

        layersList.list[index] = layer;

        if (GUI.changed)
        {
            EditorUtility.SetDirty(layers);
        }
    }

    private void CreateLayersAsset()
    {
        layers = ScriptableObject.CreateInstance<TileMapLayers>();

        if (!AssetDatabase.IsValidFolder("Assets/Tile Map"))
        {
            AssetDatabase.CreateFolder("Assets", "Tile map");
        }
        AssetDatabase.CreateAsset(layers, "Assets/Tile Map/Layers.asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private bool LoadLayersAsset()
    {
        TileMapLayers asset = AssetDatabase.LoadAssetAtPath<TileMapLayers>("Assets/Tile Map/Layers.asset");

        if (asset == null)
        {
            return false;
        }

        layers = asset;
        return true;
    }

    public override void OnInspectorGUI()
    {
        TileMap tileMap = (TileMap)target;

        tileMap.TextureAtlas =
            (Texture2D)EditorGUILayout.ObjectField(
                "Texture Atlas",
                tileMap.TextureAtlas,
                typeof(Texture2D),
                true
            );

        layersList.DoLayoutList();

        tileMap.Canvas = (Transform)EditorGUILayout.ObjectField("Canvas", tileMap.Canvas, typeof(Transform), true);
        if (GUILayout.Button("Bake Textures"))
        {
            List<Transform> children = new List<Transform>();
            foreach (Transform child in tileMap.Canvas)
            {
                children.Add(child);
            }
            foreach (Transform child in children)
            {
                DestroyImmediate(child.gameObject);
            }

            tileMap.LayerTextures = new Texture2D[layers.layers.Count];
            for (int i = 0; i < layers.layers.Count; i++)
            {
                Debug.Log(i);
                Transform layerTransform = GetLayerTransform(tileMap, layers.layers[i].name);
                tileMap.LayerTextures[i] = BakeLayer(layerTransform);
                GameObject bakedLayer = new GameObject(layers.layers[i].name);
                bakedLayer.transform.SetParent(tileMap.Canvas);
                bakedLayer.AddComponent<RawImage>().texture = tileMap.LayerTextures[i];
            }
        }
    }

    private Texture2D BakeLayer(Transform layer)
    {
        Vector2 bakedPosition = Vector2.zero;
        Vector2 bakedSize = new Vector2(1f, 1f);
        const int tileSize = 32;

        foreach (Transform tile in layer)
        {
            Sprite tileSprite = tile.GetComponent<SpriteRenderer>().sprite;

            if (tile.position.x < bakedPosition.x)
            {
                bakedPosition.x = tile.position.x;
            }
            if (tile.position.y < bakedPosition.y)
            {
                bakedPosition.y = tile.position.y;
            }

            if (tile.position.x + tileSprite.rect.width > bakedPosition.x + bakedSize.x)
            {
                bakedSize.x = tile.position.x + tileSprite.rect.width - bakedPosition.x;
            }
            if (tile.position.y + tileSprite.rect.height > bakedPosition.y + bakedSize.y)
            {
                bakedSize.y = tile.position.y + tileSprite.rect.height - bakedPosition.y;
            }
        }

        Texture2D texture = new Texture2D(Mathf.FloorToInt(bakedSize.x) * tileSize, Mathf.FloorToInt(bakedSize.y) * tileSize);

        foreach (Transform tile in layer)
        {
            Sprite tileSprite = tile.GetComponent<SpriteRenderer>().sprite;

            const int element = 0;
            const int mip = 0;

            Graphics.CopyTexture(
                tileSprite.texture,                                                                 // Source texture
                element,                                                                            // Source element
                mip,                                                                                // Source mip level
                Mathf.FloorToInt(tileSprite.rect.xMin),                                             // Source X
                Mathf.FloorToInt(tileSprite.rect.yMin),                                             // Source Y
                Mathf.FloorToInt(tileSprite.rect.width),                                            // Source width
                Mathf.FloorToInt(tileSprite.rect.height),                                           // Source height
                texture,                                                                            // Destination texture
                element,                                                                            // Destination element
                mip,                                                                                // Destination mip level
                Mathf.FloorToInt((tile.position.x - bakedPosition.x) * tileSize),                   // Destination X
                texture.height - Mathf.FloorToInt((tile.position.y - bakedPosition.y) * tileSize) -
                    Mathf.FloorToInt(tileSprite.rect.height)                                        // Destination Y
            );
        }

        Color[] colours = new Color[texture.width * texture.height];
        for (int i = 0; i < texture.width * texture.height; i++) colours[i] = Color.red;
        texture.SetPixels(0, 0, texture.width, texture.height, colours);

        return texture;
    }

    private void OnSceneGUI()
    {
        if (layersList.index == -1) return;

        Transform layer = GetLayerTransform((TileMap)target, layers.layers[layersList.index].name);

        if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
        {
            CreateTile(((TileMap)target).SpriteSelection, layer);
            Event.current.Use();
        }
        if (Event.current.type == EventType.MouseDrag && Event.current.button == 0)
        {
            CreateTile(((TileMap)target).SpriteSelection, layer);
            Event.current.Use();
        }

        if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
        {
            RemoveTile(layer);
            Event.current.Use();
        }
        if (Event.current.type == EventType.MouseDrag && Event.current.button == 1)
        {
            RemoveTile(layer);
            Event.current.Use();
        }

        if (Event.current.type == EventType.Layout)
        {
            HandleUtility.AddDefaultControl(0);
        }
    }

    private void CreateTile(Sprite sprite, Transform layer)
    {
        TileMap tileMap = (TileMap)target;

        Vector3 position = GetNewTilePosition();

        Transform oldTile = FindTile(layer, position);
        if (oldTile != null)
        {
            oldTile.GetComponent<SpriteRenderer>().sprite = sprite;
            oldTile.GetComponent<SpriteRenderer>().sortingOrder = -layersList.index;
            return;
        }

        GameObject tile = new GameObject("Tile");
        tile.transform.SetParent(layer);
        tile.transform.position = position;
        tile.AddComponent<SpriteRenderer>().sprite = sprite;
        tile.GetComponent<SpriteRenderer>().sortingOrder = -layersList.index;
    }

    private void RemoveTile(Transform layer)
    {
        Transform tile = FindTile(layer, GetNewTilePosition());
        if (tile != null)
        {
            DestroyImmediate(tile.gameObject);
        }
    }

    private Transform GetLayerTransform(TileMap tileMap, string layerName)
    {
        Transform layer = tileMap.transform.Find(layerName);
        if (layer == null)
        {
            GameObject newLayer = new GameObject(layerName);
            layer = newLayer.transform;
            layer.SetParent(tileMap.transform);
            layer.position = Vector2.zero;
        }

        return layer;
    }

    private Transform FindTile(Transform layer, Vector3 position)
    {
        foreach (Transform child in layer)
        {
            if (child.position == position)
            {
                return child;
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
