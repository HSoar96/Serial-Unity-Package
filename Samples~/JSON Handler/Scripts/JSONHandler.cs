using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class JSONHandler : MonoBehaviour
{

    public SerialCommunicationManager commManager;

    private MouseInput input = new MouseInput { x = 0, y = 0 };

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            input.x = Map(Input.mousePosition.x, 0, Screen.width, -200, 200);
            input.y = Map(Input.mousePosition.y, 0, Screen.height, 0, 200);

            string json = JsonConvert.SerializeObject(input, Formatting.None);
            commManager.SerialWrite(json);
        }

        // Listen for response and print it to the user.
        string s = commManager.SerialReadLine();
        if(s != null)
        {
            Debug.Log(s);
        }

    }

    /// <summary>
    /// Maps a float in one range to another range similar to the Arduino Map function.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="fromLow"></param>
    /// <param name="fromHigh"></param>
    /// <param name="toLow"></param>
    /// <param name="toHigh"></param>
    /// <returns>A float between the defined bounds.</returns>
    private static float Map(float value, float fromLow, float fromHigh, float toLow, float toHigh)
    {
        return (value - fromLow) * (toHigh - toLow) / (fromHigh - fromLow) + toLow;
    }

    // A simple class is used instead of a Vector2 due to
    // ease of intergration with newtonsoft.JSON.
    class MouseInput
    {
        public float x;
        public float y;
    }
}