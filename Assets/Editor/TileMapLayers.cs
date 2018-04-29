using UnityEngine;
using System.Collections.Generic;

// Class to store layer data to be stored in a file
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
