using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_ARDUINO_API_SET
using System.IO.Ports;
#endif

public class SerialCommunicationManager : MonoBehaviour
{
#if UNITY_ARDUINO_API_SET
    private const char NEWLINE_CHAR = '\n';
    private SerialDeviceManager serialDeviceManager;
    private SerialPort port = null;
    private void Awake()
    {
        serialDeviceManager = GetComponent<SerialDeviceManager>();
        port = serialDeviceManager.serialPort;
    }

    /// <summary>
    /// Sends a message followed by defined newline char across the open serial device.
    /// </summary>
    /// <param name="message">Message to send across the serial port.</param>
    /// <exception cref="NullReferenceException"></exception>
    public void SerialWriteLine(string message)
    {
        if (port == null)
            throw new NullReferenceException("Serial Port cannot be null.");

        port.Write(message + NEWLINE_CHAR);
    }
#endif
}
