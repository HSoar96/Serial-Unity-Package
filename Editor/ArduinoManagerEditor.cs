using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ArduinoManager))]
public class ArduinoManagerEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }

    private Color HexToColour(string hex)
    {
        Color color = new Color();
        ColorUtility.TryParseHtmlString(hex, out color);
        return color;
    }
}
