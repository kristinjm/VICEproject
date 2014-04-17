#include <stdio.h>
#include <OneWire.h>
//////////////////////////VFD Controls/////////////////////////////////////
struct msg
{
   byte message[6];
   byte crc[2];
} msg;
char message0[] = {0x02, 0x06, 0x00, 0x00, 0x00, 0x01, 0x48, 0x39};  // switch VFD on
//////////////////////////Send Data/////////////////////////////////////
char incomingByte[128];
int len = 0;
int readlen = 0;
////////////////////////////////////////////////////////////////////////
///////////////////////////////VFD State////////////////////////////////
int state=3;  //Sets VFD State to Off
////////////////////////////////////////////////////////////////////////
////////////////////////////FlowSensor/////////////////////////////////
volatile int NbTopsFan; //measuring the rising edges of the signal
float Calc;                               
//int hallsensor = 2;    //The pin location of the sensor
////////////////////////////////////////////////////////////////////////
////////////////////////////TemperatureSensor///////////////////////////
OneWire  ds(56);  // on pin 54 which is A0 for the Arduino Due
////////////////////////////////////////////////////////////////////////
float freq1 = 60;
int state1 = 1;

void rpm ()     //This is the function that the interupt calls 
{ 
  NbTopsFan++;  //This function measures the rising and falling edge of the sensors signal
} 

void setup() 
{
  memset(incomingByte, 0, sizeof(incomingByte));
  pinMode(2, INPUT);        //Sets pin 2 as input--Flow Sensor
  pinMode(3, OUTPUT);       //Sets pin 3 as output--VFD relay
  pinMode(4, OUTPUT);       //Sets pin 4 as output--EWO relay
  Serial.begin(9600);       //PC
  Serial1.begin(9600);      //XBee
  Serial1.setTimeout(200);
  Serial1.flush();
  Serial2.begin(9600);      //VFD
  attachInterrupt(2, rpm, RISING);   //set interupt on pin--Flow Sensor
  memset(incomingByte, 0, sizeof(incomingByte));
  
  Serial.write(message0, 8);
  //give time for the startup message to be recognized
  delay(500);
  vfdControl();
}

void loop() 
{
  String sendmsg;
  float flow;
  float pressure;
  float temperature;
  if(Serial1.available() > 0)
  {
    //Serial.println("data available");
    readlen = Serial1.readBytesUntil('\n', incomingByte, 128);
    incomingByte[0] = '<';
    //Serial.print(incomingByte);
    incomingByte[readlen] = '\0';
    //Serial.println(readlen);
    if (readlen > 0)
    {
      if(!strcmp(incomingByte, "<REQ>"))
      {
        flow = flowsensor();
        pressure = pressuresensor();
        temperature = temperaturesensor();
        sendmsg = parseData(flow,freq1,temperature,pressure,state1);
      }
      else
      {
        //Serial.println("inside else");
        //relayControl(z);
        parseMessage();
        //Serial.println("after parseMessage");
        flow = flowsensor();
        pressure = pressuresensor();
        temperature = temperaturesensor();
        sendmsg = parseData(flow,freq1,temperature,pressure,state1);
      }
      //Serial.println(sendmsg);
      Serial1.print(sendmsg);
    }
  }
}

String parseData(float f, float h, float t, float p, int s)
{
  String ff = String(f,2);
  String hh = String(h,2);
  String tt = String(t,2);
  String pp = String(p,2);
  String s2 = String(s);
  
  String fs = String("F=" + ff + ",");
  String hs = String("H=" + hh + ",");
  String ts = String("T=" + tt + ",");
  String ps = String("P=" + pp + ",");
  String ss = String("VS=" + s2);
  
  String message = "<"+fs+hs+ts+ps+ss+">";
  return message;
}



void vfdControl()
{
   struct msg data;
   data.message[0] = 0x02; 
   data.message[1] = 0x06;
   data.message[2] = 0x00;
   data.message[3] = 0x01;
   data.message[4] = 0x00;
   data.message[5] = 0x00;
   data.crc[0] = 0x00;
   data.crc[1] = 0x00;
   
   word crcReturn = 0;
   int freqtimes10 = (int)(freq1 *10);
   data.message[4] = (freqtimes10 & 0xff00) >> 8;
   data.message[5] = (freqtimes10 & 0x00ff);
   crcReturn = CRC16(data.message, (word)6);
   data.crc[0] = (crcReturn & 0xFF00)>>8;
   data.crc[1] = (crcReturn & 0x00FF);
   Serial.write(data.message, sizeof(data.message));
   Serial.write(data.crc, sizeof(data.crc));
}

String relayControl(String z)
{
  state = z.toInt();               // State becomes user input from serial monitor
  if(state==1)
  {
    digitalWrite(4, HIGH);         // EWO backup line is OPEN
    delay(10000);                  // Safety delay to ensure motor is no longer spinning before closing VFD line
    digitalWrite(3, LOW);          // VFD line is CLOSED, EWO line is OPEN  
  }
  
  if(state==2)
  {
    digitalWrite(3, HIGH);         // VFD line is OPEN, EWO line is CLOSED
    digitalWrite(4, LOW);          // EWO backup line is CLOSED
  }
  
  if(state==3)
  {
    digitalWrite(4, HIGH);         // EWO backup line is OPEN
    digitalWrite(3, HIGH);         // EWO line is CLOSED VFD line is OPEN
  }
  //Serial.println(state); 
}

//////////////////////////////////Flow Sensor/////////////////////////////////////
float flowsensor()
{
  NbTopsFan = 0;                     // Set NbTops to 0 ready for calculations
  //interrupts();
  //delay (1000);                      // Wait 1 second
  //noInterrupts();
  Calc = ((NbTopsFan/ 7.5)*.264172); // (Pulse frequency)/ 7.5Q, = flow rate 
  //Serial.print (Calc, 2);          // Prints the number calculated above
  //Serial.print (" gal/min\r\n");     // Prints "gal/min" and returns a  new line
  return Calc;
}
///////////////////////////////////////////////////////////////////////////////////

//////////////////////////////////Fressure Sensor//////////////////////////////////
float pressuresensor()
{
  float PSI;
  float slope = (1000/3.3);               // 1000 PSI / our span of 3.3V
  // read the input on analog pin 0:
  int sensorValue = analogRead(A0);
  // Convert the analog reading (which goes from 0 - 1023) to a voltage span (0 - 3.3V):
  float Vo = sensorValue * (3.3 / 1023.0);
  
  if (sensorValue <= 525)
  {
    PSI = 0;
  }  
  
  if (sensorValue > 525)
  {
    PSI = ((slope * Vo) - (slope*1.7));  // y = Mx+B
  }
  
  //Serial.print("The Sensor Voltage is ");
  //Serial.println(Vo,4);
  //Serial.print("The PSI is ");
  //Serial.println(PSI);
  //Serial.print("The Sensor Value is ");
  //Serial.println(sensorValue);
  //delay(5000);
  return PSI;
}
///////////////////////////////////////////////////////////////////////////////////

///////////////////////////////Temperature Sensor//////////////////////////////////
float temperaturesensor() 
{
  byte i;
  byte present = 0;
  byte type_s;
  byte data[12];
  byte addr[8];
  float celsius, fahrenheit;
  
//  if ( !ds.search(addr)) {
//    Serial.println("No more addresses.");
//    Serial.println();
//    ds.reset_search();
//    delay(250);
//    //return;
//  }
  
//  Serial.print("ROM =");
//  for( i = 0; i < 8; i++) {
//    Serial.write(' ');
//    Serial.print(addr[i], HEX);
//  }

//  if (OneWire::crc8(addr, 7) != addr[7]) {
//      Serial.println("CRC is not valid!");
//      //return;
//  }
  //Serial.println();
 
  // the first ROM byte indicates which chip
  switch (addr[0]) {
    case 0x10:
      //Serial.println("  Chip = DS18S20");  // or old DS1820
      type_s = 1;
      break;
    case 0x28:
      //Serial.println("  Chip = DS18B20");
      type_s = 0;
      break;
    case 0x22:
      //Serial.println("  Chip = DS1822");
      type_s = 0;
      break;
    //default:
      //Serial.println("Device is not a DS18x20 family device.");
      //return;
  } 

  ds.reset();
  ds.select(addr);
  ds.write(0x44,1);         // start conversion, with parasite power on at the end
  
  //delay(1000);     // maybe 750ms is enough, maybe not
  // we might do a ds.depower() here, but the reset will take care of it.
  
  present = ds.reset();
  ds.select(addr);    
  ds.write(0xBE);         // Read Scratchpad

  //Serial.print("  Data = ");
  //Serial.print(present,HEX);
  //Serial.print(" ");
  for ( i = 0; i < 9; i++) {           // we need 9 bytes
    data[i] = ds.read();
    //Serial.print(data[i], HEX);
    //Serial.print(" ");
  }
  //Serial.print(" CRC=");
  //Serial.print(OneWire::crc8(data, 8), HEX);
  //Serial.println();

  // convert the data to actual temperature

  unsigned int raw = (data[1] << 8) | data[0];
  if (type_s) {
    raw = raw << 3; // 9 bit resolution default
    if (data[7] == 0x10) {
      // count remain gives full 12 bit resolution
      raw = (raw & 0xFFF0) + 12 - data[6];
    }
  } else {
    byte cfg = (data[4] & 0x60);
    if (cfg == 0x00) raw = raw << 3;  // 9 bit resolution, 93.75 ms
    else if (cfg == 0x20) raw = raw << 2; // 10 bit res, 187.5 ms
    else if (cfg == 0x40) raw = raw << 1; // 11 bit res, 375 ms
    // default is 12 bit resolution, 750 ms conversion time
  }
  celsius = (float)raw / 16.0;
  fahrenheit = celsius * 1.8 + 32.0;
  //Serial.print("  Temperature = ");
  //Serial.print(celsius);
  //Serial.print(" Celsius, ");
  //Serial.print(fahrenheit);
  //Serial.println(" Fahrenheit");
  return fahrenheit;
}
///////////////////////////////////////////////////////////////////////////////////

void parseMessage()
{
int id = 0;
char valstr[32];
memset(valstr, 0, sizeof(valstr));

char* token = strtok(incomingByte, "<>,");
while(token != NULL)
{
  char id[4];
  memset(id, 0, sizeof(id));
  int i = 0;
  int j = 0;
  
  while(token[i] != '=')
  {
    id[i] = token[i];
    i++;
  }
  i++;
  while(token[i] != '\0')
  {
    valstr[j] = token[i];
    i++; j++;
  }
  if(!strcmp(id, "H"))
  {
    freq1 = atof(valstr);
    vfdControl();
  }
  else if(!strcmp(id, "S"))
  {
    state1 = atoi(valstr);
  }
  memset(id, 0, sizeof(id));
  memset(valstr, 0, sizeof(valstr));
  token = strtok(NULL, "<>,");
}
}

////////////////////////////////////CRC Function//////////////////////////////////////////
word CRC16(const byte *nData, word wLength)
{
   static const word wCRCTable[] = {
   0X0000, 0XC0C1, 0XC181, 0X0140, 0XC301, 0X03C0, 0X0280, 0XC241,
   0XC601, 0X06C0, 0X0780, 0XC741, 0X0500, 0XC5C1, 0XC481, 0X0440,
   0XCC01, 0X0CC0, 0X0D80, 0XCD41, 0X0F00, 0XCFC1, 0XCE81, 0X0E40,
   0X0A00, 0XCAC1, 0XCB81, 0X0B40, 0XC901, 0X09C0, 0X0880, 0XC841,
   0XD801, 0X18C0, 0X1980, 0XD941, 0X1B00, 0XDBC1, 0XDA81, 0X1A40,
   0X1E00, 0XDEC1, 0XDF81, 0X1F40, 0XDD01, 0X1DC0, 0X1C80, 0XDC41,
   0X1400, 0XD4C1, 0XD581, 0X1540, 0XD701, 0X17C0, 0X1680, 0XD641,
   0XD201, 0X12C0, 0X1380, 0XD341, 0X1100, 0XD1C1, 0XD081, 0X1040,
   0XF001, 0X30C0, 0X3180, 0XF141, 0X3300, 0XF3C1, 0XF281, 0X3240,
   0X3600, 0XF6C1, 0XF781, 0X3740, 0XF501, 0X35C0, 0X3480, 0XF441,
   0X3C00, 0XFCC1, 0XFD81, 0X3D40, 0XFF01, 0X3FC0, 0X3E80, 0XFE41,
   0XFA01, 0X3AC0, 0X3B80, 0XFB41, 0X3900, 0XF9C1, 0XF881, 0X3840,
   0X2800, 0XE8C1, 0XE981, 0X2940, 0XEB01, 0X2BC0, 0X2A80, 0XEA41,
   0XEE01, 0X2EC0, 0X2F80, 0XEF41, 0X2D00, 0XEDC1, 0XEC81, 0X2C40,
   0XE401, 0X24C0, 0X2580, 0XE541, 0X2700, 0XE7C1, 0XE681, 0X2640,
   0X2200, 0XE2C1, 0XE381, 0X2340, 0XE101, 0X21C0, 0X2080, 0XE041,
   0XA001, 0X60C0, 0X6180, 0XA141, 0X6300, 0XA3C1, 0XA281, 0X6240,
   0X6600, 0XA6C1, 0XA781, 0X6740, 0XA501, 0X65C0, 0X6480, 0XA441,
   0X6C00, 0XACC1, 0XAD81, 0X6D40, 0XAF01, 0X6FC0, 0X6E80, 0XAE41,
   0XAA01, 0X6AC0, 0X6B80, 0XAB41, 0X6900, 0XA9C1, 0XA881, 0X6840,
   0X7800, 0XB8C1, 0XB981, 0X7940, 0XBB01, 0X7BC0, 0X7A80, 0XBA41,
   0XBE01, 0X7EC0, 0X7F80, 0XBF41, 0X7D00, 0XBDC1, 0XBC81, 0X7C40,
   0XB401, 0X74C0, 0X7580, 0XB541, 0X7700, 0XB7C1, 0XB681, 0X7640,
   0X7200, 0XB2C1, 0XB381, 0X7340, 0XB101, 0X71C0, 0X7080, 0XB041,
   0X5000, 0X90C1, 0X9181, 0X5140, 0X9301, 0X53C0, 0X5280, 0X9241,
   0X9601, 0X56C0, 0X5780, 0X9741, 0X5500, 0X95C1, 0X9481, 0X5440,
   0X9C01, 0X5CC0, 0X5D80, 0X9D41, 0X5F00, 0X9FC1, 0X9E81, 0X5E40,
   0X5A00, 0X9AC1, 0X9B81, 0X5B40, 0X9901, 0X59C0, 0X5880, 0X9841,
   0X8801, 0X48C0, 0X4980, 0X8941, 0X4B00, 0X8BC1, 0X8A81, 0X4A40,
   0X4E00, 0X8EC1, 0X8F81, 0X4F40, 0X8D01, 0X4DC0, 0X4C80, 0X8C41,
   0X4400, 0X84C1, 0X8581, 0X4540, 0X8701, 0X47C0, 0X4680, 0X8641,
   0X8201, 0X42C0, 0X4380, 0X8341, 0X4100, 0X81C1, 0X8081, 0X4040 };

   byte nTemp;
   word wCRCWord = 0xFFFF;

      while (wLength--)
      {
         nTemp = *nData++ ^ wCRCWord;
         wCRCWord >>= 8;
         wCRCWord ^= wCRCTable[nTemp];
      }
      
      // Swap the bytes since it sends Least significant byte first
      wCRCWord = ((wCRCWord & 0xFF00) >> 8) | ((wCRCWord & 0x00FF) << 8);
      return wCRCWord;
}
