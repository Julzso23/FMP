using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class TileMapLayers : ScriptableObject
{
    [System.Serializable]
    public struct Layer
    {
        public bool enabled;
        public string name;
    }

    public List<Layer> layers = new List<Layer>();
}
