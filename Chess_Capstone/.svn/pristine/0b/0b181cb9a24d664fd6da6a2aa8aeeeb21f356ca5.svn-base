 //Com port SCI0 initialization and commands
//Processor:  MC9S12XDP512
//Crystal:  16 MHz
//by Carter Kozakevich
//Oct 2017

#include <hidef.h>
#include "derivative.h"
#include "SCI0_Lib.h"


void SCI0_Init(unsigned long uLong){ //any valid baud rate can be passed to this; 8-bit, 1 Stop, No parity, no interrupts
    unsigned char cIRHold = SCI0BDH & 0b11100000;
    SCI0BD = 8000000/16/uLong;
    SCI0BDH |= cIRHold;
    SCI0CR1 = 0b00000000;
    SCI0CR2 = 0b00001100;
}
void SCI0_Init9600(void){//8-bit, 1 Stop, No parity, no interrupts
    unsigned char cIRHold = SCI0BDH & 0b11100000;
    SCI0BD = 13;
    SCI0BDH |= cIRHold;
    SCI0CR1 = 0b00000000;
    SCI0CR2 = 0b00001100;
}
void SCI0_Init19200(void){  //8-bit, 1 stop, No parity, no interrupts
    unsigned char cIRHold = SCI0BDH & 0b11100000;
    SCI0BD = 26;
    SCI0BDH |= cIRHold;
    SCI0CR1 = 0b00000000;
    SCI0CR2 = 0b00001100;
    
}
void SCI0_TxChar(unsigned char cByte){
   
   while(!(SCI0SR1 & 0b10000000));
   
    SCI0DRL = cByte;
   
}
unsigned char SCI0_RxChar(void)	//Non-blocking; returns NULL if no new valid character is available
{
  if(SCI0SR1 & 0b00100000){
       return SCI0DRL;
  } else return 0;
}


void SCI0_TxString(char *cString){  //Requires a NULL-terminated ASCII string in the main program
    
    while(*cString != 0)
      SCI0_TxChar(*cString++);
}



