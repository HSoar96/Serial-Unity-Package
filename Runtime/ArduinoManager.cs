using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using UnityEngine;

using System.Management;
using System.Linq;

#if UNITY_ARDUINO_API_SET
using System.IO.Ports;
#endif

public class ArduinoManager : MonoBehaviour
{
    #region Serial Communication Variables
    #if UNITY_ARDUINO_API_SET
    private const int BAUD_RATE = 115200;
    private const int DATA_BITS = 8;
    private const int READ_TIMEOUT = 1;
    private SerialPort serialPort = new SerialPort();
#endif
    #endregion

    #region Serial Communication Methods
#if UNITY_ARDUINO_API_SET
    /// <summary>
    /// Gets port names that have the VID and PID stated connected. 
    /// </summary>
    /// <param name="VID">Vendor ID to look for</param>
    /// <param name="PID">Product ID to look for</param>
    /// <returns>All COM Ports that have the specified device connected.</returns>
    private Dictionary<string, string> ComPortNames(string VID, string PID)
    {
        // Make a new empty dictionary.
        var comPorts = new Dictionary<string, string>();

        // This is platform dependant and only works on windows.
        // It will format the string and initialise a new string list for comports.
        string pattern = string.Format("^VID_{0}.PID_{1}", VID, PID);
        Regex _rx = new Regex(pattern, RegexOptions.IgnoreCase);

        // Get to the base location of the registry where HID data is stored.
        RegistryKey rk1 = Registry.LocalMachine;
        RegistryKey rk2 = rk1.OpenSubKey("SYSTEM\\CurrentControlSet\\Enum");

        // Serach through each subkey and its subkey and so on.
        foreach (string s3 in rk2.GetSubKeyNames())
        {
            RegistryKey rk3 = rk2.OpenSubKey(s3);
            foreach (string s in rk3.GetSubKeyNames())
            {
                // Try and match the VID and PID 
                if (_rx.Match(s).Success)
                {
                    RegistryKey rk4 = rk3.OpenSubKey(s);
                    foreach (string s2 in rk4.GetSubKeyNames())
                    {
                        // Here you can get all the data from the devices with those VIDs and PIDs.
                        RegistryKey rk5 = rk4.OpenSubKey(s2);
                        // This gives data in the form of "Port_#000X.Hub_#000Y
                        string location = (string)rk5.GetValue("LocationInformation");

                        // Usually along the lines of "USB Serial Device (COMX)"
                        string friendlyName = (string)rk5.GetValue("FriendlyName");

                        // Open the key where the important data for our use is.
                        RegistryKey rk6 = rk5.OpenSubKey("Device Parameters");

                        // Port name is formatted as "COMX"
                        string portName = (string)rk6.GetValue("PortName");

                        // Add to our dictionary.
                        if (!string.IsNullOrEmpty(portName))
                            comPorts.Add(portName, friendlyName);
                    }
                }
            }
        }
        return comPorts;
    }

    /// <summary>
    /// Gets all USB Serial devices that are stored in the registry, 
    /// and adds them to a dictionary.
    /// </summary>
    /// <returns>All USB Com Ports stored in the registry</returns>
    private Dictionary<string, string> ComPortNames()
    {
        // Make a new empty dictionary.
        var comPorts = new Dictionary<string, string>();

        // Get to the base location of the registry where HID data is stored.
        RegistryKey rk1 = Registry.LocalMachine;
        RegistryKey rk2 = rk1.OpenSubKey("SYSTEM\\CurrentControlSet\\Enum");

        // Serach through each subkey and its subkey and so on.
        foreach (string s3 in rk2.GetSubKeyNames())
        {
            // Search only for USB Serial Devices.
            if (s3 == "USB")
            {
                RegistryKey rk3 = rk2.OpenSubKey(s3);
                foreach (string s in rk3.GetSubKeyNames())
                {
                    RegistryKey rk4 = rk3.OpenSubKey(s);
                    foreach (string s2 in rk4.GetSubKeyNames())
                    {
                        // Here you can get all the data from the devices with those VIDs and PIDs.
                        RegistryKey rk5 = rk4.OpenSubKey(s2);

                        // Usually along the lines of "USB Serial Device (COMX)"
                        string friendlyName = (string)rk5.GetValue("FriendlyName");

                        // Open the key where the important data for our use is.
                        RegistryKey rk6 = rk5.OpenSubKey("Device Parameters");
                        string portName;

                        // TODO: I dont like this, there must be a cleaner way to do it,
                        // look into implementing WMI?
                        try
                        {
                            portName = (string)rk6.GetValue("PortName");
                        }
                        catch(Exception e)
                        {
                            portName = null;
                        }

                        // Add to our Lists.
                        if (!string.IsNullOrEmpty(portName))
                        {
                            comPorts.Add(portName,friendlyName);
                        }
                    }
                }
            }
        }
        return comPorts;
    }

    private SerialPort SetupSerialPort(string portID)
    {
        if (!portID.StartsWith("COM"))
            throw new System.Exception("Port ID must begin with COM");
        
        SerialPort port = new SerialPort(portID);

        port.BaudRate = BAUD_RATE;
        port.Parity = Parity.None;
        port.StopBits = StopBits.One;
        port.DataBits = DATA_BITS;
        port.Handshake = Handshake.None;
        port.ReadTimeout = READ_TIMEOUT;

        return port;
    }
    public void BeginSerialCommuniation(string VID, string PID)
    {
        // Gets a list of COM Ports that exist with the specified VID and PID.
        var portNames = ComPortNames(VID, PID);
        foreach (var com in portNames)
        {
            Console.WriteLine($"Device with VID_{VID} and PID_{PID} detected at: {com}");
            Console.WriteLine();
        }

        if (portNames.Count != 0)
        {
            serialPort = SetupSerialPort(portNames[0]);

            if (!serialPort.IsOpen)
            {
                // When a serial device is connected its COM port is saved in the registry.
                // This will not change unless the registry is cleared, meaning even if the device
                // is not plugged in it will be there, therefore the program will try and connect
                // to a currently unplugged board. 
                //TODO: Look into https://docs.microsoft.com/en-us/dotnet/api/system.io.ports.serialport.pinchanged
                // and https://docs.microsoft.com/en-us/dotnet/api/system.io.ports.serialpinchange (DsrChanged)
                // to see if this can help determine if a device is currently connected before trying to open communication.
                try
                {
                    serialPort.Open();
                    Debug.Log($"Port Opened at {serialPort.PortName}");
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
        }
        else
        {
            Debug.LogWarning("No ports exist.");
        }
    }

    #endif
    #endregion
}
