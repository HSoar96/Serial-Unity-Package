using System.Collections;
using System.Collections.Generic;


public class Device
{
    public Device(string port, string friendlyName, string[] hwInfo)
    {
        Port = port;
        FriendlyName = friendlyName;
        HWInfo = hwInfo;
    }

    public string Port { get;}
    public string FriendlyName { get;}
    public string[] HWInfo { get;}
}
