# Unity Arduino Package

A Unity package to enable easy 2 way serial communication between the Unity Editor and Arduino based microcontrollers. 

This package only works on Windows however Unix compatibility is in the works.

## Installation

Use [Unity Package Manager](https://docs.unity3d.com/Manual/upm-ui.html) to add Unity Arduino Package to your project. You can either do this via the GIT URL option or via disk.

## Usage
### Unity
Create a Serial Manager by placing ```SerialDeviceManager.cs``` and ```SerialCommunicationManager.cs```on an empty ```GameObject```, or import and use the sample manager.

Select your new manager in the heirachy and follow the prompts to update your API Compatibility level. This is required to use the ```System.IO.Ports``` namespace.

There are 3 public Methods in  ```SerialCommunicationManager.cs``` that you can use to communicate to your serial device.

#### SerialWriteLine
Sends a message over the chosen serial port followed by the chosen newline character. 
#### SerialWrite
Sends a message over the chosen serial port.
### SerialReadLine 
Reads incomming data from the serial port up until the chosen new line character or until it times out.

### Arduino
If required by your specific device e.g. Teensy's set the operation mode to Serial.

#### Serial Setup
In your main ```.ino``` or ```.cpp``` file setup serial communication.  Some example code using the default settings is shown below.

It is important to set the correct baudrate and timeout, the default baudrate is 115200 and timeout is 10ms.

You can change the baudrate and timeout to whatever you prefer however you must do it both in your arduino code and in your unity project via your Serial Manager.

```
void setup() 
{
  	Serial.begin(115200);
  	Serial.setTimeout(10);
}
```

#### Runtime
Below is some basic example code for the arduino followed by a corrosponding script for unity, this turns on and off the bultin LED when W or S are pressed at runtime in unity. It also sends data to unity and displays it in the debug log when pin D4 is high. 
```
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

```
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
There are currently two samples that can be imported through the Unity Package Manager.

A sample Serial Manager prefab and a an LED Changer.

The LED Changer includes both a functioning unity scene and example arduino scripts in both a ```.cpp``` and ```.ino``` format.

The example turns on the arduinos built in LED when W or S is pressed at runtime in the unity scene.

It has currently been tested on the following devices.

- Teensy 4.1, 4.0, 3.6 & 3.2
- Seeduino V4.2
- NodeMCU V3

## License
[MIT](https://choosealicense.com/licenses/mit/)
