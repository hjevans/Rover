/*
  Software serial multple serial test

 Receives from the hardware serial, sends to software serial.
 Receives from software serial, sends to hardware serial.

 The circuit:
 * RX is digital pin 10 (connect to TX of other device)
 * TX is digital pin 11 (connect to RX of other device)

 Note:
 Not all pins on the Mega and Mega 2560 support change interrupts,
 so only the following can be used for RX:
 10, 11, 12, 13, 50, 51, 52, 53, 62, 63, 64, 65, 66, 67, 68, 69

 Not all pins on the Leonardo support change interrupts,
 so only the following can be used for RX:
 8, 9, 10, 11, 14 (MISO), 15 (SCK), 16 (MOSI).

 created back in the mists of time
 modified 25 May 2012
 by Tom Igoe
 based on Mikal Hart's example

 This example code is in the public domain.

 */
#include <SoftwareSerial.h>
#include <String.h> 
SoftwareSerial mySerial(10, 11); // RX, TX

int pot0 = A0;
int pot1 = A1;
int valPot0 = 0;
int valPot1 = 0;
String toHero;
void setup() {
  // Open serial communications and wait for port to open:
  Serial.begin(57600);
  while (!Serial) {
    ; // wait for serial port to connect. Needed for native USB port only
  }
  
  
  //Serial.println("Goodnight moon");
  // set the data rate for the SoftwareSerial port
  mySerial.begin(115200);
  //mySerial.println("Hello, world?");
}

void loop() { // run over and over
  int theta0;
  int theta1;
  valPot0 = analogRead(pot0);
  valPot1 = analogRead(pot1);
  theta0 = map(valPot0, 0, 1023, 0, 360*10);
  theta1 = map(valPot1, 0, 1023, 0, 360*10);
  //Serial.print("sensor0: ");
  //Serial.print(valPot0);
  //Serial.print("\t angle0(deg): "); 
  //Serial.write(theta0);
  //Serial.print("\n");
  //Serial.print("sensor1: ");
  //Serial.print(valPot1);
  //Serial.print("\t angle1(deg): "); 
  //Serial.print("\n");
  String s0 = String(theta0);
  String s1 = String(theta1);
  //toHero = s0 + " " + s1 + " ";
  if (s0.length() < 4){
    for (int j = s0.length(); j < 4; ++j){
      s0 = '0'+ s0;
    }
  }
  if (s1.length() < 4){
    for (int k = s1.length(); k < 4; ++k){
      s1 = '0' + s1;
    }
  }
  toHero = "E-"+s0 + "-" + s1+"-;" ;
  //for (int i = 0; i < toHero.length(); i++){
  for (int i = 0; i < 13; i++){
    mySerial.write(toHero[i]);
    Serial.write(toHero[i]);
  }
  //toHero = 'E'+toHero;
  //Serial.println(toHero);
  delay(10);
  /*
  if (mySerial.available()) {
    mySerial.write("Hello, HERO!");
    Serial.write(mySerial.read());
  }
  */
  //if (Serial.available()) {
    //mySerial.write(Serial.read());
  //}
}

