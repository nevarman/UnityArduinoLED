// Arduino setup that reads lines A,B,C etc to do light animations with Unity
// You can find the examples inside Adafruit/strandtest
#include <Adafruit_NeoPixel.h>
#define PIN 6 // Adjust with your pin on your arduino input
#define NUM_LEDS strip.numPixels()
Adafruit_NeoPixel strip = Adafruit_NeoPixel(50, PIN, NEO_RGB + NEO_KHZ800);

char messageFromPC[32] = {0};
char compare[32] = {'A'};
int integerFromPC = 0;
String inputString = "";         // a String to hold incoming data
boolean stringComplete = false;  // whether the string is complete
char receivedData;
void setup() {
  Serial.begin(9600);
    // reserve 200 bytes for the inputString:
 // inputString.reserve(200);
  strip.begin(); // prep the neopixel
  strip.show();
  Serial.println("<Arduino is ready>");
}

/*
  SerialEvent occurs whenever a new data comes in the hardware serial RX. This
  routine is run between each time loop() runs, so using delay inside loop can
  delay response. Multiple bytes of data may be available.
*/
void serialEvent() {
  while (Serial.available()) {
    // get the new byte:
    char inChar = (char)Serial.read();
    // add it to the inputString:
    inputString += inChar;
    // if the incoming character is a newline, set a flag so the main loop can
    // do something about it:
    if (inChar == '\n') {
      stringComplete = true;
    }
  }
}

void loop() {
  handleReceivedData();
  if (stringComplete) {
    Serial.println(inputString);

    // Length (with one extra character for the null terminator)
    int str_len = inputString.length() + 1; 

    // Prepare the character array (the buffer) 
    char char_array[str_len];

    // Copy it over 
    inputString.toCharArray(char_array, str_len);
    parseCharData(char_array);
    // clear the string:
    inputString = "";
    stringComplete = false;
  }
  //strip.show(); //output to the neopixel
  //delay(20); //for safety
}

void parseCharData(char data[]) {
    // split the data into its parts
  char * strtokIndx; // this is used by strtok() as an index
  
  strtokIndx = strtok(data,",");      // get the first part - the string
  strcpy(messageFromPC, strtokIndx); // copy it to messageFromPC
  
  strtokIndx = strtok(NULL, ",");     // this continues where the previous call left off
  int r = atoi(strtokIndx);     // convert this part to an integer
  
  strtokIndx = strtok(NULL, ","); 
  int g = atoi(strtokIndx);     
  
  strtokIndx = strtok(NULL, ","); 
  int b = atoi(strtokIndx);    

  Serial.println(messageFromPC);
  Serial.println( r);
  Serial.println( g);
  Serial.println( b);
  receivedData = messageFromPC[0];
  if(messageFromPC[0] == 'A'){
    setAll(r,g,b);
  }
  else if(messageFromPC[0] == 'B'){
    setAllRandom();
  }
  else if(receivedData == 'D'){
    colorWipe(strip.Color(r,g,b), 20);
  }
}

void handleReceivedData(){
  if(receivedData == 'C'){
    rainbow(1);
  }else if(receivedData == 'E'){
    rainbowCycle(1);
  }
}

void setAll(int r, int g, int b){
  for (int i=0; i<NUM_LEDS; i++) {
      strip.setPixelColor(i,strip.Color( r,g,b));
      strip.show();
  } 
}

void setAllRandom(){
  for (int i=0; i<NUM_LEDS; i++) {
      strip.setPixelColor(i,strip.Color( random(0,255),random(0,255),random(0,255)));
      strip.show();
  }  
}

// Fill the dots one after the other with a color
void colorWipe(uint32_t c, uint8_t wait) {
  for(uint16_t i=0; i<strip.numPixels(); i++) {
    strip.setPixelColor(i, c);
    strip.show();
    delay(wait);
  }
}

void rainbow(uint8_t wait) {
  uint16_t i, j;

  for(j=0; j<256; j++) {
    for(i=0; i<strip.numPixels(); i++) {
      strip.setPixelColor(i, Wheel((i+j) & 255));
    }
    strip.show();
    delay(wait);
  }
}

// Slightly different, this makes the rainbow equally distributed throughout
void rainbowCycle(uint8_t wait) {
  uint16_t i, j;

  for(j=0; j<256*5; j++) { // 5 cycles of all colors on wheel
    for(i=0; i< strip.numPixels(); i++) {
      strip.setPixelColor(i, Wheel(((i * 256 / strip.numPixels()) + j) & 255));
    }
    strip.show();
    delay(wait);
  }
}

// Input a value 0 to 255 to get a color value.
// The colours are a transition r - g - b - back to r.
uint32_t Wheel(byte WheelPos) {
  WheelPos = 255 - WheelPos;
  if(WheelPos < 85) {
    return strip.Color(255 - WheelPos * 3, 0, WheelPos * 3);
  }
  if(WheelPos < 170) {
    WheelPos -= 85;
    return strip.Color(0, WheelPos * 3, 255 - WheelPos * 3);
  }
  WheelPos -= 170;
  return strip.Color(WheelPos * 3, 255 - WheelPos * 3, 0);
}
