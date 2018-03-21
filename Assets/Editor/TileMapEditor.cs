using UnityEditor;
using UnityEngine;
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
            if (!layers.layers.Exists((TileMapLayers.Layer other) => other.name == newName))
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
        ((TileMap)target).TextureAtlas = (Texture2D)EditorGUILayout.ObjectField("Texture Atlas",
                                                                                ((TileMap)target).TextureAtlas,
                                                                                typeof(Texture2D),
                                                                                true);

        layersList.DoLayoutList();
    }

    private void OnSceneGUI()
    {
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
            return;
        }

        GameObject tile = new GameObject("Tile");
        tile.transform.SetParent(layer);
        tile.transform.position = position;
        tile.AddComponent<SpriteRenderer>().sprite = sprite;
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
        for (int i = 0; i < layer.childCount; i++)
        {
            if (layer.GetChild(i).position == position)
            {
                return layer.GetChild(i);
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
