using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using UnityEngine;
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
    private List<string> ComPortNames(string VID, string PID)
    {
        string pattern = string.Format("^VID_{0}.PID_{1}", VID, PID);
        Regex _rx = new Regex(pattern, RegexOptions.IgnoreCase);
        List<string> comports = new List<string>();

        RegistryKey rk1 = Registry.LocalMachine;
        RegistryKey rk2 = rk1.OpenSubKey("SYSTEM\\CurrentControlSet\\Enum");

        foreach (string s3 in rk2.GetSubKeyNames())
        {
            RegistryKey rk3 = rk2.OpenSubKey(s3);
            foreach (string s in rk3.GetSubKeyNames())
            {
                if (_rx.Match(s).Success)
                {
                    RegistryKey rk4 = rk3.OpenSubKey(s);
                    foreach (string s2 in rk4.GetSubKeyNames())
                    {
                        RegistryKey rk5 = rk4.OpenSubKey(s2);
                        string location = (string)rk5.GetValue("LocationInformation");
                        RegistryKey rk6 = rk5.OpenSubKey("Device Parameters");
                        string portName = (string)rk6.GetValue("PortName");
                        if (!string.IsNullOrEmpty(portName))
                            comports.Add((string)rk6.GetValue("PortName"));
                    }
                }
            }
        }
        return comports;
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
        // Gets a list of COM Ports that have a Teensy 3.6 connected in serial mode.
        // TODO: Change teensy VID and PID to ensure unique to game.
        var portNames = ComPortNames(VID, PID);

        if(portNames.Count == 0)
        {
            Debug.LogWarning("No ports exist.");
        }

        foreach (var com in portNames)
        {
            Console.WriteLine($"Teensy 3.6 detected at: {com}");
            Console.WriteLine();
        }

        if (portNames.Count != 0)
        {
            serialPort = SetupSerialPort(portNames[0]);

            if (!serialPort.IsOpen)
            {
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
    }
    #endif
    #endregion
}
