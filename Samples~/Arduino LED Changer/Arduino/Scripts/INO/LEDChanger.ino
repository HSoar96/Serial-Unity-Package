#include <Arduino.h>

#define DIGITAL_IN 7

void setup() 
{
  Serial.begin(115200);
  Serial.setTimeout(10);
  pinMode(LED_BUILTIN, OUTPUT);
  pinMode(DIGITAL_IN, INPUT);
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

  if(digitalRead(DIGITAL_IN) == HIGH)
  {
    Serial.println(String(DIGITAL_IN) + " HIGH");
  }
}
