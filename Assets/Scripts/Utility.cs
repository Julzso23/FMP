using UnityEditor;
using UnityEngine;

public static class GUIUtility
{
    public static void ErrorLabel(string text)
    {
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize = 24;
        style.fontStyle = FontStyle.Bold;
        style.alignment = TextAnchor.MiddleCenter;
        style.normal.textColor = Color.red;

        GUILayout.Label(text, style);
    }

    public static Sprite[] GetSprites(this Texture2D texture)
    {
        string path = AssetDatabase.GetAssetPath(texture);
        return AssetDatabase.LoadAllAssetsAtPath(path) as Sprite[];
    }
}
