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
    public void SerialWriteLine(string message)
    {
        SerialWrite(message + NEWLINE_CHAR);
    }
    /// <summary>
    /// Sends a message alone the open serial device with no newline char.
    /// </summary>
    /// <param name="message">Message t send across the serial port.</param>
    /// <exception cref="NullReferenceException"></exception>
    public void SerialWrite(string message)
    {
        if (port == null)
            throw new NullReferenceException("Serial Port cannot be null");

        port.Write(message);
    }
    /// <summary>
    /// Reads the serial port unitl the newline char \n is read or until timeout.
    /// </summary>
    /// <returns>Data read in the serial port. Or null if timed out.</returns>
    /// <exception cref="NullReferenceException"></exception>
    public string SerialReadLine()
    {
        if (port == null)
            throw new NullReferenceException("Serial port cannot be null");

        string data;
        try
        {
            data = port.ReadLine();
        }
        catch (TimeoutException e)
        {
            return null;
        }
        return data;
    }
#endif
}
