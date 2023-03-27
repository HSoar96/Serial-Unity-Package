using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

#if SUP_API_SET
using System.IO.Ports;
#endif

public class SerialDeviceManager : MonoBehaviour
{
    [HideInInspector]
    public Device deviceChosen = null;
    private Device deviceToUse = null;
    [HideInInspector]
    public List<Device> connectedDevices = new List<Device>();

#if SUP_API_SET
    #region Serial Communication Variables
    private const int BAUD_RATE = 115200;
    private const int DATA_BITS = 8;
    private const int READ_TIMEOUT = 10;
    public SerialPort serialPort = null;
    #endregion

    private void OnApplicationQuit()
    {
        // Ensures the port is closed when the program closes.
        CloseSerialPort();
    }

    private void OnSceneUnloaded(Scene currentScene)
    {
        // Ensures the port is closed when the scene is unloaded.
        CloseSerialPort();
    }

    private void Awake()
    {
        EditorJsonUtility.FromJsonOverwrite(EditorPrefs.GetString(GetDeviceChosen()), this);
        SceneManager.sceneUnloaded += OnSceneUnloaded;
        BeginSerialCommuniation();
    }

    /// <summary>
    /// Closes a currently open serial port.
    /// </summary>
    private void CloseSerialPort()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
            Debug.LogWarning($"Serial communcation ended with {deviceToUse.FriendlyName} @ {serialPort.PortName}");
        }
    }
#endif

    private string GetDeviceChosen()
    {
        return deviceChosen + GetInstanceID().ToString();
    }

    public void SetDeviceChosen(Device chosenDevice) {
        deviceChosen = chosenDevice;
        EditorPrefs.SetString(GetDeviceChosen(), EditorJsonUtility.ToJson(this));
    }

#if SUP_API_SET
    /// <summary>
    /// Gets every USB serial device and then tries to connect.
    /// </summary>
    /// <returns>A list of currently connected devices</returns>
    public void GetConnectedDevices()
    {
        connectedDevices = PollDevices(GetAllComDevices());
    }

    private List<Device> PollDevices(List<Device> devicesToPoll)
    {
        var liveDevices = new List<Device>();

        foreach (Device device in devicesToPoll)
        {
            SerialPort port = new SerialPort(device.Port);
            try
            {
                // If it opens immediatly close,
                // log it and add to list.
                if (!port.IsOpen)
                {
                    port.Open();
                }
                port.Close();
                liveDevices.Add(device);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"{device.FriendlyName} not currently connected at {device.Port} \n" + e);
            }

        }

        return liveDevices;
    }

    /// <summary>
    /// Updates the device to use, sets up the serial port,
    /// and trys to open serial communication.
    /// </summary>
    /// <exception cref="NullReferenceException"></exception>
    private void BeginSerialCommuniation()
    {
        UpdateDeviceToUse(deviceChosen);

        if (deviceToUse == null)
            throw new NullReferenceException("Chosen device cannot be null. Are you sure you chose a device in Serial Device Manager?");

        serialPort = SetupSerialPort(deviceToUse.Port);
        if (!serialPort.IsOpen)
        {
            // When a serial device is connected its COM port is saved in the registry.
            // This will not change unless the registry is cleared, meaning even if the device
            // is not plugged in it will be there, therefore the program will try and connect
            // to a currently unplugged board. 
            // TODO: Look into https://docs.microsoft.com/en-us/dotnet/api/system.io.ports.serialport.pinchanged
            // and https://docs.microsoft.com/en-us/dotnet/api/system.io.ports.serialpinchange (DsrChanged)
            // to see if this can help determine if a device is currently connected before trying to open communication.
            try
            {
                serialPort.Open();
                Debug.LogWarning($"Serial communcation begun with {deviceToUse.FriendlyName} @ {serialPort.PortName} @ {serialPort.BaudRate} BAUD.");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }

    /// <summary>
    /// Sets up a USB Serial Port.
    /// </summary>
    /// <param name="portID">The COM ID to use</param>
    /// <returns>A correclty setup Serial Port</returns>
    /// <exception cref="Exception"></exception>
    private SerialPort SetupSerialPort(string portID)
    {
#if UNITY_EDITOR_WIN
        if (!portID.StartsWith("COM"))
            throw new Exception("Port ID must begin with COM");
#endif

        SerialPort port = new SerialPort(portID);

        port.BaudRate = BAUD_RATE;
        port.Parity = Parity.None;
        port.StopBits = StopBits.One;
        port.DataBits = DATA_BITS;
        port.Handshake = Handshake.None;
        port.ReadTimeout = READ_TIMEOUT;

        return port;
    }

    /// <summary>
    /// Updates the device to be used by searching 
    /// for the VID and PID that the chosen device has.
    /// </summary>
    /// <param name="chosenDevice">Device whos VID and PID you will search for </param>
    /// <exception cref="Exception"></exception>
    private void UpdateDeviceToUse(Device chosenDevice)
    {
        if (chosenDevice == null)
            throw new Exception("Device cannot be null");

#if UNITY_EDITOR_WIN
        // This is platform dependant and only works on windows.
        // It will format the string and initialise a new string list for comports.
        string pattern = string.Format("^VID_{0}.PID_{1}", chosenDevice.VID, chosenDevice.PID);
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

                        // Usually along the lines of "USB Serial Device (COMX)"
                        string friendlyName = (string)rk5.GetValue("FriendlyName");

                        // Gets VID and PID but data needs to be parsed.
                        string[] hardwareInfo = (string[])rk5.GetValue("HardwareID");

                        // Open the key where the important data for our use is.
                        RegistryKey rk6 = rk5.OpenSubKey("Device Parameters");

                        // Port name is formatted as "COMX"
                        string portName = (string)rk6.GetValue("PortName");

                        // Create a new device that will be the first device with that VID and PID found.
                        if (!string.IsNullOrEmpty(portName))
                        {
                            // TODO: Clean this up.
                            Device d = new Device(portName, friendlyName, hardwareInfo);
                            SerialPort port = new SerialPort(d.Port);
                            try
                            {
                                // If it opens immediatly close,
                                // set that as device to use and return.
                                if (!port.IsOpen)
                                {
                                    port.Open();
                                }
                                port.Close();
                                deviceToUse = d;
                                return;
                            }
                            catch (Exception e)
                            {
                            }
                        }
                    }
                }
            }
        }
#endif

#if UNITY_EDITOR_OSX
        foreach (String portName in SerialPort.GetPortNames())
        {
            String[] hardwareInfo = { "NULL_NULL_NULL_NULL_NULL" };
            if (portName.Equals(chosenDevice.Port))
                deviceToUse = new Device(portName, portName, hardwareInfo);
        }
#endif
    }

    /// <summary>
    /// Gets all USB Serial devices that are stored in the registry, 
    /// and adds them to a list.
    /// </summary>
    /// <returns>All USB Com Devices stored in the registry</returns>
    private List<Device> GetAllComDevices()
    {
        var devices = new List<Device>();

#if UNITY_EDITOR_WIN
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

                        // Gets VID and PID but data needs to be parsed.
                        string[] hardwareInfo = (string[])rk5.GetValue("HardwareID");

                        // Open the key where the important data for our use is.
                        RegistryKey rk6 = rk5.OpenSubKey("Device Parameters");
                        string portName;

                        // TODO: I dont like this, there must be a cleaner way to do it,
                        // look into implementing WMI?
                        try
                        {
                            portName = (string)rk6.GetValue("PortName");
                        }
                        catch (Exception e)
                        {
                            portName = null;
                        }

                        // Add to our Lists.
                        if (!string.IsNullOrEmpty(portName))
                        {

                            devices.Add(new Device(portName, friendlyName, hardwareInfo));
                        }
                    }
                }
            }
        }
#endif

#if UNITY_EDITOR_OSX
        foreach (String portName in SerialPort.GetPortNames())
        {
            // Example: /dev/tty.usbmodem1101
            String[] hardwareInfo = {"NULL_NULL_NULL_NULL_NULL"};
            Match m = Regex.Match(portName, "/dev/tty.usb*");
            if (m.Success)
                devices.Add(new Device(portName, portName, hardwareInfo));
        }
#endif
        return devices;
    }

#endif
}
