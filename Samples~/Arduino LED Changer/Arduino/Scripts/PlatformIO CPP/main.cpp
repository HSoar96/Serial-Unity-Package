#include <Arduino.h>
void setup() 
{
  Serial.begin(9600);
  pinMode(LED_BUILTIN, OUTPUT);
}

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
}