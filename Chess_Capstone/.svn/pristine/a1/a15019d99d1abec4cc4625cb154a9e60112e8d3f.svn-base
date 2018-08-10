//Miscellaneous generally-useful routines
//Processor:  MC9S12XDP512
//Crystal:  16 MHz
//by Carter Kozakevich
//September 2017

//Binary-Coded Decimal conversion routines

#include <hidef.h>      		/* common defines and macros 		*/
#include "derivative.h"
#include "Misc_Lib.h"

unsigned int HexToBCD(unsigned int iHexIn){//integer math; result is BCD - not converted to ASCII; make it 4-digits for sev-seg display
    unsigned int iBCDOut = 0;
    unsigned int iPow = 1;
    unsigned char cCount;
    
    if(iHexIn > 9999)
      return 0;
    
    for(cCount = 0;cCount < 4; cCount++){
      iBCDOut += (iHexIn % 10) * iPow;
      iHexIn /= 10;
      iPow *= 16;
    }
    return iBCDOut;


}
unsigned int BCDToHex(unsigned int iBCDIn){ //integer math; requires BCD - not ASCII equivalent; make it 4-digits to complement HexToBCD
    unsigned  int iHexOut = 0;
    unsigned  char cCount = 0;
    unsigned  char cDigit;
    unsigned  int iPow = 1;
    
    while(cCount < 4){
      cDigit = iBCDIn % 16;
      if(cDigit < 10){
        
        iHexOut += cDigit * iPow; 
        iBCDIn /= 16;
        cCount++;
        iPow *= 10;
      } else{
          iHexOut = 0;
          cCount = 4;
      }
    }
    return iHexOut;
}

//ASCII-Code handling routines

unsigned char ToUpper(unsigned char cChar){

     if(cChar <= 'z' && cChar >= 'a')
        cChar -= 0x20;
     return cChar;
    
}


unsigned char ToLower(unsigned char cChar){
    
    if(cChar <= 'Z' && cChar >= 'A')
        cChar += 0x20;
     return cChar;
}
    


unsigned char HexToASCII(unsigned char cChar){  //single character converter
    if(cChar <= 0x09){
      return cChar += 0x30;   
    }      
    if(cChar <= 0x0f && cChar >= 0x0a)
      return cChar += 0x37;
    return cChar;
}
unsigned char ASCIIToHex(unsigned char cChar){   //single character converter
    
    if(cChar >= '0' && cChar <= '9')
      cChar -= 0x30;
    return cChar;
}


//9S12X simple timer routines

void TimInit125ns(void){
    TSCR1 |= 0b10000000;   //turn timer on
    
    TSCR2 &= 0b11111000;   //set prescale to Bus/64
    TSCR2 |= 0b00000000;
    
    TIOS  |= 0b00000001;  //channel 0 to output compare
    
    TCTL2 &= 0b11111110;  //timer0 toggles port T0 - pin9    
    TCTL2 |= 0b00000001;   
    
    TFLG1 |= 0b00000001; //clear any prev. event on timer0
}


void TimInit8us(void){
    
    TSCR1 |= 0b10000000;   //turn timer on
    
    TSCR2 &= 0b11111000; //set prescale to Bus/64
    TSCR2 |= 0b00000110;
    
    TIOS  |= 0b00000001;  //channel 0 to output compare
    
    TCTL2 &= 0b11111110;  //timer0 toggles port T0 - pin9    
    TCTL2 |= 0b00000001;   
    
    TFLG1 |= 0b00000001; //clear any prev. event on timer0
}


void Sleep_ms(unsigned int iTime){    //requires TimInit8us setup; blocking delay
      unsigned int iCount;
      TC0 = TCNT + 125;  //1ms
      for( iCount = 1; iCount <= iTime; iCount++){
          TFLG1 |= 0b00000001;          //clear flag
          while((TFLG1&0b00000001)==0);  //wait for flag
          TC0+=125;    
      }
}
void msDelay(unsigned int msDelayAmnt){
    
    unsigned int loop = msDelayAmnt / 10; 

  
    asm                 LDY loop;     //Loop input times 	  
  	asm    InnerLoop:   LDX #26664; //26667 * 125ns = 10ms -4 from 26667 to compensate for outter loop
  	asm                 DBNE X,*; 
  	asm    DBNE Y,InnerLoop;	   
}
