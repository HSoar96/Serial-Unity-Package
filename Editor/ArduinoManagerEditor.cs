using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ArduinoManager))]
public class ArduinoManagerEditor : Editor
{
    private BuildTargetGroup buildTargetGroup;

    public override void OnInspectorGUI()
    {   
        // Build target group is being depreceated
        // however no unity method exists at the moment to replace it.
        // TODO: Make a new method that uses NamedBuildTarget.
        buildTargetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);

        base.OnInspectorGUI();
        SetAPILevel();
    }

    /// <summary>
    /// Sets global define to avoid calls to System.IO.Ports, 
    /// when API level is not set correctly.
    /// </summary>
    private void SetGlobalDefine(string define)
    {
        string currentDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);

        if (currentDefines.Contains(define))
        {
            Debug.LogWarning($"<b>{define}</b> Already exists in scripting defines for group <b>{buildTargetGroup}</b>");
            return;
        }

        // Defines are seperated by a ; in unity so add ;define onto the
        // string of current defines to add a new one.
        PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, currentDefines + ";" + define);
    }

    /// <summary>
    /// Creates UI elements that check API level 
    /// and allow the user to press a button to change it if required.
    /// </summary>
    private void SetAPILevel()
    {
        // Check if the current project has the correct build settings.
        // If it does tell the user everything is fine,
        // else provide a prompt and a button to alert them.

        // Could use #if UNITY_ARDUINO_API_SET here but this will be easier
        // to read down the line and performance doesnt matter to much in editor scripts.
        if (PlayerSettings.GetApiCompatibilityLevel(buildTargetGroup) != ApiCompatibilityLevel.NET_4_6)
        {
            // TODO: Work out a neater way to produce 
            GUILayout.BeginVertical();
            GUI.backgroundColor = Color.red;

            GUILayout.TextArea(
                "\n Unity Arduino manager requires API Compatibility level of 4.X.\n \n" +
                "Please change this in your project settings or press the button below.\n");

            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("Change API Level to .NET 4.6"))
            {
                // Directly changes the users API level in their project settings,
                // because using anything other than .NET 4.X will not allow the use of System.IO.Ports
                PlayerSettings.SetApiCompatibilityLevel(buildTargetGroup, ApiCompatibilityLevel.NET_4_6);
                SetGlobalDefine("UNITY_ARDUINO_API_SET");
            }
            GUILayout.EndVertical();
        }
        else
        {
            GUILayout.BeginVertical();
            GUI.backgroundColor = Color.green;

            GUILayout.TextArea(
                "\nAPI compatibility level is set correctly to .NET 4.6\n\n" +
                "Unity Arduino should work as expected.\n");

            GUILayout.EndVertical();
        }
    }

    /// <summary>
    /// Trys to parse a hex value into a unity Color and returns it.
    /// </summary>
    /// <param name="hex">Hex string to parse</param>
    /// <returns>A <c>Unity Color</c> that corrosponds to parsed Hex</returns>
    private Color HexToColour(string hex)
    {
        Color color = new Color();
        ColorUtility.TryParseHtmlString(hex, out color);
        return color;
    }
}
