using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class XYController : MonoBehaviour
{
    public SerialCommunicationManager commManager;
    public GameObject cube;

    public int xAnalogPin = 16;
    public int yAnalogPin = 19;

    void Update()
    {
#if SUP_API_SET
        string[] serialData = commManager.ReadPinData(' ');

        if(serialData != null)
        {
            int pin = Int32.Parse(serialData[0]);
            Vector3 transform = cube.transform.position;

            if(pin == xAnalogPin)
            {
                float mappedValue = Map(Int32.Parse(serialData[1]), 0, 1023, -10, 10);
                cube.transform.position = new Vector3(transform.x, mappedValue, transform.z);
            }
            else if(pin == yAnalogPin)
            {
                float mappedValue = Map(Int32.Parse(serialData[1]), 0, 1023, -10, 10);
                cube.transform.position = new Vector3(mappedValue, transform.y, transform.z);
            }
            else
            {
                Debug.LogWarning($"Data was unable to be parsed");
                Debug.LogWarning($"{serialData[0]} {serialData[1]}");      
            }
        }
#endif
    }

    private static float Map(float value, float fromLow, float fromHigh, float toLow, float toHigh)
    {
        return (value - fromLow) * (toHigh - toLow) / (fromHigh - fromLow) + toLow;
    }
}
