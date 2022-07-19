using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Device
{
    public string Port;
    public string FriendlyName;
    public string VID;
    public string PID;
    public bool Chosen = false;
    public Device(string port, string friendlyName, string[] hwInfo)
    {
        Port = port;
        FriendlyName = friendlyName;

        // HWINFO comes through like this.
        // Take the first line and split it, then assign VID and PID correctly
        // USB\VID_16C0&PID_0483&REV_0277
        // USB\VID_16C0 & PID_0483

        char[] delimiters = { '\\', '&', '_' };
        string[] splitInfo = hwInfo[0].Split(delimiters);
        VID = splitInfo[2];
        PID = splitInfo[4];
    }
    public Device(string port, string friendlyName, string vID, string pID)
    {
        Port = port;
        FriendlyName = friendlyName;
        VID = vID;
        PID = pID;
    }

}
