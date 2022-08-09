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
    private void Start()
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
            throw new NullReferenceException("Serial Port cannot be null.\nAre you sure you chose a device in Serial Device Manager?");

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
            throw new NullReferenceException("Serial Port cannot be null.\nAre you sure you chose a device in Serial Device Manager?");

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

    /// <summary>
    /// Reads the serial port unitl the newline char \n is read or until timeout. Then splits the data.
    /// </summary>
    /// <param name="seperator">A character array that delimits the substrings in the read string</param>
    /// <returns>Data read in the serial port. Or null if timed out.</returns>
    /// <exception cref="NullReferenceException"></exception>
    public string[] ReadPinData(char seperator)
    {
        string rawData = SerialReadLine();

        if (rawData == null)
            return null;
        else
            return rawData.Split(seperator);
    }
#endif
}
