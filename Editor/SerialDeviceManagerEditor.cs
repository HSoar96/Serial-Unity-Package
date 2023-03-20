using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(SerialDeviceManager)),CanEditMultipleObjects]
public class SerialDeviceManagerEditor : Editor
{
    private BuildTargetGroup buildTargetGroup;
    private SerialDeviceManager serialManager;

    public override void OnInspectorGUI()
    {
        // Build target group is being depreceated
        // however no unity method exists at the moment to replace it.
        // TODO: Make a new method that uses NamedBuildTarget.
        buildTargetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
        serialManager = target as SerialDeviceManager;

        base.OnInspectorGUI();

        // Display text and button to change the API level.
        SetAPILevel();

#if SUP_API_SET
        // Display button to begin serial communication with selected device.
        ConnectedDevicesButton();
        DisplayDevices();
#endif
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

        #if !SUP_API_SET
            // TODO: Work out a neater way to produce.
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
                SetGlobalDefine("SUP_API_SET");
            }
            GUILayout.EndVertical();
        #endif

        #if SUP_API_SET
            GUILayout.BeginVertical();
            GUI.backgroundColor = Color.green;

            GUILayout.TextArea(
                "\nAPI compatibility level is set correctly to .NET 4.6\n\n" +
                "Unity Arduino should work as expected.\n");

            GUILayout.EndVertical();
        #endif
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

#if SUP_API_SET
    /// <summary>
    /// Displays a button that on press updates currently connected devices.
    /// </summary>
    private void ConnectedDevicesButton()
    {
        GUI.backgroundColor = HexToColour("#0078FF");
        GUILayout.BeginVertical();       
        // Gets connected ports converts them to a list of strings
        // and shows them in the inspector.
        if (GUILayout.Button("Get Currently Connected Devices"))
        {
            serialManager.GetConnectedDevices();
        }
        GUILayout.EndVertical();
    }

    /// <summary>
    /// Displays a list of all connected devices with a button for users
    /// to choose the device they want to use 
    /// </summary>
    private void DisplayDevices()
    {
        if (serialManager.connectedDevices == null)
            return;

        bool deviceChosen = false;
        for (int i = 0; i < serialManager.connectedDevices.Count; i++)
        {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.TextArea(serialManager.connectedDevices[i].FriendlyName);
            GUILayout.TextArea(serialManager.connectedDevices[i].Port);
            serialManager.connectedDevices[i].Chosen = GUILayout.Toggle(serialManager.connectedDevices[i].Chosen, "Use This Device");
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            // If a device is chosen define the device to use and set others to false.
            if (serialManager.connectedDevices[i].Chosen)
            {
                deviceChosen = true;
                for (int j = 0; j < serialManager.connectedDevices.Count; j++)
                {
                    if (j != i)
                    {
                        serialManager.connectedDevices[j].Chosen = false;
                    }
                }
                serialManager.deviceChosen = serialManager.connectedDevices[i];
            }
        }
        if (!deviceChosen)
            serialManager.deviceChosen = null;
    }
#endif

    /// <summary>
    /// Trys to parse a hex value into a unity Color and returns it.
    /// </summary>
    /// <param name="hex">Hex string to parse</param>
    /// <returns>A <c>Unity Color</c> that corrosponds to parsed Hex</returns>
    private Color HexToColour(string hex)
    {
        if (!hex.Contains("#"))
        {
            hex = "#" + hex;
        }
        ColorUtility.TryParseHtmlString(hex, out Color color);
        return color;
    }
}