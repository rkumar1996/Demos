//Switches and LEDs 
//Processor:  MC9S12XDP512 
//Crystal:  16 MHz 
//by Carter Kozakevich
//September 2017 


#include <hidef.h>
#include "derivative.h"
#include "SwLeds_lib.h"
#include "Misc_Lib.h"

void SwLED_Init(void)    //LEDs as outputs, switches as inputs, dig inputs enabled
{
  PT1AD1 &= 0b00011111; //turn off LEDs 
  DDR1AD1 = 0b11100000; //Leds out, switches in
  ATD1DIEN1 = 0b00011111; //enable switches as inputs
}
 

void LED_On(char myLED) //upper case letter of LED and A for all, turn ON that LED
{
  switch(myLED)
  {
    case'R':
    PT1AD1|=0b10000000;               //turn on red LED
    break;
    
    case'Y':
    PT1AD1|=0b01000000;                //turn on yellow LED
    break;
    
    case'G':
    PT1AD1|=0b00100000;                //turn on green LED
    break;
    
    case'A':
    PT1AD1|=0b11100000;                //turn on all LEDs
    break;
  }
}

void LED_Off(char myLED) //upper case letter of LED and A for all, turn OFF that LED
{
     switch(myLED)
  {
    case'R':
    PT1AD1&=0b01111111;               //turn off red LED
    break;
    
    case'Y':
    PT1AD1&=0b10111111;                //turn off yellow LED
    break;
    
    case'G':
    PT1AD1&=0b11011111;                //turn off green LED
    break;
    
    case'A':
    PT1AD1&=0b00011111;                //turn off all LEDs
    break;
  }

} 
void LED_Tog(char myLED)  //upper case letter of LED and A for all, toggle that LED
{
    switch(myLED)
  {
    case'R':
    PT1AD1^=0b10000000;               //toggle red LED
    break;
    
    case'Y':
    PT1AD1^=0b01000000;                //toggle yellow LED
    break;
    
    case'G':
    PT1AD1^=0b00100000;                //toggle green LED
    break;
    
    case'A':
    PT1AD1^=0b11100000;                //toggle all LEDs
    break;
  }
} 

char SwCk(void){
   char cSample1 = 1;
   char cSample2 = 2;
   
   while(cSample1 != cSample2){
      cSample1 = PT1AD1 & 0b00011111;
      msDelay(10);
      cSample2 = PT1AD1 & 0b00011111;
   }
   return cSample1;
}