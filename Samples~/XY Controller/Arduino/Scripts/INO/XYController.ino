#define X_AXIS 16
#define Y_AXIS 19

int yValue = -1;
int xValue = -1;
int digitalVal = 0;

void setup() 
{
  Serial.begin(115200);
  Serial.setTimeout(10);
  pinMode(LED_BUILTIN, OUTPUT);
  pinMode(Y_AXIS, INPUT);
  pinMode(X_AXIS, INPUT);
}

void loop() 
{
  if(analogRead(Y_AXIS) != yValue)
  {
    yValue = analogRead(Y_AXIS);
    Serial.println(String(Y_AXIS) + " " + String(yValue));
  }
  if(analogRead(X_AXIS) != xValue)
  {
    xValue = analogRead(X_AXIS);
    Serial.println(String(X_AXIS) + " " + String(xValue));
  }
}
