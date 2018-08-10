#include <hidef.h>      		/* common defines and macros 		*/
#include "derivative.h"
#include "LCD_lib.h"

void LCD_Init(void){//8-bit, 2-line, 5x8 chars, disp on, curs on, blink off, inc curs mode, no shift, clear, home
  
  PTH = 0b00000000;
  DDRH = 0b11111111;
  PORTK &= 0b11111000;
  DDRK |= 0b00000111;
  
  asm PSHD;
  asm LDD#0;            //49.15ms delay
  asm DBNE D,*;
  asm DBNE D,*;
  asm PULD;
  
  PTH = 0b00111000;
  
  PORTK |= 0b00000001;
  PORTK &= 0b11111000;
  
  asm PSHD;
  asm LDD #11000;         //4.125ms delay
  asm DBNE D,*;
  asm PULD;
  
  PORTK |= 0b00000001;  //RS low, R/W low, EN high to write a control
  PORTK &= 0b11111000;  //resting state
  
  asm PSHD;
  asm LDD #267;         //100us delay
  asm DBNE D,*;
  asm PULD;
  
  PORTK |= 0b00000001;  //RS low, R/W low, EN high to write a control
  PORTK &= 0b11111000;  //resting state
  
  asm PSHD;
  asm LDD #267;         //100us delay
  asm DBNE D,*;
  asm PULD;
  
  LCD_Ctrl(0b00111000); //same as above but using LCD_Ctrl (Busy is active)
  
  LCD_Ctrl(0b00001110); //display controls
  
  LCD_Ctrl(0b00000001); //clear display, home postion
  
  LCD_Ctrl(0b00000110); //mode controls
   
}

void LCD_Ctrl(unsigned char cCommand){
   
   while (LCD_Busy() != 0);
   PTH = cCommand;
   
   PORTK |= 0b00000001;
   PORTK &= 0b11111000;
}

unsigned char LCD_Busy(void){
   
   unsigned char cBusy;
   
   DDRH = 0b0000000; //data bus as inputs for read
   
   PORTK |= 0b00000011;
   
   PORTK &= 0b11111000;  //resting state
   
   cBusy = PTH & 0b10000000;  //busy flag is MSB of status register
   
   DDRH = 0b11111111; //data bus returned to outputs for next write 
   
   return cBusy;
}


void LCD_Char(unsigned char cCommand){
   while (LCD_Busy() != 0);
   PTH = cCommand;
   
   PORTK |= 0b00000101;
   PORTK &= 0b11111000;
}



void LCD_Addr(unsigned char cAddr){ //raw LCD DDRAM address -- requires knowledge of device
     cAddr |= 0b10000000;
     LCD_Ctrl(cAddr);

}

void LCD_Pos(unsigned char cRow,unsigned char cCol){  //Row and Column, zero based; out of range values go to home location
     if(cRow > 3 || cCol > 19)
        LCD_Addr(0);
     else{
        
        switch (cRow){
          case 0:
          LCD_Addr(cCol);
          break;
          
          case 1:
          LCD_Addr(cCol + 64);
          break;
          
          case 2:
          LCD_Addr(cCol + 20);
          break;
          
          case 3:
          LCD_Addr(cCol + 84);
          break;
          
        }
            
     }


}

void LCD_String(char * cString){  //requires a NULL-terminated string of ASCII characters in main program
   while(*cString != 0)  //watch for null terminator
    LCD_Char(*cString++); //send next char

}
