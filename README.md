# Serial Unity Package - SUP

A Unity package to enable easy 2 way serial communication between the Unity Editor and Arduino based microcontrollers. 

This package only works on Windows however Unix compatibility is in the works.

## Installation

Use [Unity Package Manager](https://docs.unity3d.com/Manual/upm-ui.html) to add Serial Unity Package to your project. You can either do this via the GIT URL option or via disk.

## Usage
### Unity
Create a Serial Manager by placing ```SerialDeviceManager.cs``` and ```SerialCommunicationManager.cs```on an empty ```GameObject```, or import and use the sample manager.

Select your new manager in the hierarchy and follow the prompts to update your API Compatibility level. This is required to use the ```System.IO.Ports``` namespace.

There are 3 public Methods in  ```SerialCommunicationManager.cs``` that you can use to communicate to your serial device.

#### SerialWriteLine
Sends a message over the chosen serial port followed by the chosen newline character. 
#### SerialWrite
Sends a message over the chosen serial port.
#### SerialReadLine 
Reads incoming data from the serial port up until the chosen new line character or until it times out.
#### ReadPinData
Reads incoming data from the serial port and then splits it using the defined delimiter. Returning an array of strings.  It is recommended to use this to receive data from specific pins. This becomes useful when sending data from lots of pins and allows for easy comparisons.

### Arduino
If required by your specific device e.g. Teensy's set the operation mode to Serial.

#### Serial Setup
In your main ```.ino``` or ```.cpp``` file setup serial communication.  Some example code using the default settings is shown below.

It is important to set the correct baudrate and timeout, the default baudrate is 115200 and timeout is 10ms.

You can change the baudrate and timeout to whatever you prefer however you must do it both in your arduino code and in your unity project via your Serial Manager.

```cpp
void setup() 
{
  	Serial.begin(115200);
  	Serial.setTimeout(10);
}
```

#### Runtime
Below is some basic example code for the arduino followed by a corresponding script for unity, this turns on and off the built-in LED when W or S are pressed at runtime in unity. It also sends data to unity and displays it in the debug log when pin D4 is high. 
```cpp
void loop() 
{
  	String s = Serial.readStringUntil('\n');
  	if(s == "LED ON")
  	{
   		digitalWrite(LED_BUILTIN, HIGH);
  	}
  	else if(s == "LED OFF")
  	{
   		 digitalWrite(LED_BUILTIN, LOW);
  	}
	
  	if(digitalRead(D4) == HIGH)
  	{
  		Serial.println("D4 HIGH");
  	}
}
```

```c#
    public SerialCommunicationManager communicationManager;

    private void Update()
    {
     	if (Input.GetKeyDown(KeyCode.W))
        	communicationManager.SerialWriteLine("LED ON");
        
    	if (Input.GetKeyDown(KeyCode.S))
        	communicationManager.SerialWriteLine("LED OFF");

     	if (communicationManager.SerialReadLine() == "D4 HIGH")
        	Debug.Log("Digital pin D4 is currently HIGH");
    }
```

## Samples
There are currently three samples that can be imported through the Unity Package Manager.

- Sample Serial Manager Prefab
- Builtin LED Changer
- Simple XY Controller
- Simple JSON Parser and Generator

#### Serial Manager Prefab
The sample serial manager prefab contains only the serial manager and no scene.<br>
***IMPORTANT: When choosing your device ensure you do it in the prefab not in the object in the hierarchy because this will be reset on play***

#### LED Changer
The LED Changer turns on/off the arduino’s built-in LED when W/S is pressed at runtime in the unity scene. Including both a unity scene and example arduino scripts in both ```.cpp`` and ```.ino```

#### XY Controller
The XY Controller includes both a functioning unity scene and example arduino scripts in both a ```.cpp``` and ```.ino``` format.

#### JSON Parser/Generator
A scene containing a Serial Manager, and a JSONHandler object that sends a mapped mouse position to an arduino and awaits a response. As well as corresponding PlatformIO ```.cpp```  and Arduino ```.ino``` files.

The samples have currently been tested on the following devices.

- Teensy 4.1, 4.0, 3.6 & 3.2
- Seeduino V4.2
- NodeMCU V3

## License
[MIT](https://github.com/HSoar96/Serial-Unity-Package/blob/main/LICENSE.md)