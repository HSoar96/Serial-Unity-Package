#include <Arduino.h>
#include <ArduinoJson.h>

struct vector2
{
  float x;
  float y;
};

void setup() {
  Serial.begin(115200);
  Serial1.begin(9600);
  Serial.setTimeout(10);
}

void loop() {
  String data;
  StaticJsonDocument<200> doc;
  while (Serial.available() == 0)
  {
    // Wait for input.
  }

  data = Serial.readStringUntil('}');
  data += '}';

  DeserializationError error = deserializeJson(doc, data);

  // Test if parsing succeeds.
  if (error) {
    Serial.print(F("deserializeJson() failed: "));
    Serial.println(error.f_str());
    digitalWrite(LED_BUILTIN, HIGH);
    return;
  }

  vector2 vec;
  vec.x = doc["x"];
  vec.y = doc["y"];

  Serial.print("X Position = ");
  Serial.print(vec.x);
  Serial.print ("   Y Position = ");
  Serial.println(vec.y);
 }