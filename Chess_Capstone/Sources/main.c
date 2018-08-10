/********************************************************************/
// HC12 Program: ICA19
// Processor:		MC9S12XDP512
// Xtal Speed:		16 MHz
// Author:			Reeshav Kumar & Carter Kozakevich   

/********************************************************************/


#include <hidef.h>      		/* common defines and macros 		*/
#include "derivative.h"      	/* derivative-specific definitions 	*/

#include <stdio.h>
#include <string.h>

/********************************************************************/
//		Library includes
/********************************************************************/

#include "LCD_lib.h"
#include "SwLeds_lib.h"
#include "SCI0_Lib.h"
#include "Misc_Lib.h"

#include "Movement.h"
#include "Decoder_Lib.h"


/********************************************************************/
//		Prototypes
/********************************************************************/
 int buildMove(void);

/********************************************************************/
//		Variables
/********************************************************************/

int count = 0;
int startGame = 0;
char in = 0;
char read[5] = "";
struct Point startPos;
struct Point endPos;
struct Point lastDeath;
struct Point castleMove;
struct Point botLeft;
struct Point mid;
int check = -1;
int castleCheck = -1;
/********************************************************************/
//		Lookups
/********************************************************************/


void main(void)
{
// main entry point
_DISABLE_COP();

/********************************************************************/
// initializations
/********************************************************************/

  //for port m use ddrm fo rsetting address at bit 7
  //amd ptm for turning it on or off
  
  
  SwLED_Init(); 
  SCI0_Init19200();
  LCD_Init();
  TimInit8us();
    
  lastDeath.X = 0;
  lastDeath.Y = 1; 
  startPos.X = 0;
  startPos.Y = 0;
  endPos.X = 0;
  endPos.Y = 0;
  botLeft.X = 1;
  botLeft.Y = 1;
  mid.X = 5;
  mid.Y = 5;
  LCD_Pos(1, 0);
  LCD_String("Press Center Button to Start!");

  
	for (;;)
	{
  
  //Half Square distance == 3.4cm
  //2.1cm == 100ms delay (full step) double when micro stepping
    while(startGame == 0){ 
      if(PT1AD1&0b00000001){        
       LCD_Ctrl(0b00000001);
       LCD_Pos(1, 0);
       LCD_String("Initializing...");
       MoveInit(129, 4);
       startGame = 1;
       LCD_Ctrl(0b00000001);
       LCD_Pos(1, 0);
       //LCD_String("Play the Game!");
      }
      
    }
    if(PT1AD1 &0b00001000) //down press; go to start
    {
      Move(mid,botLeft);
    }
  
    if(buildMove() == 1) 
    {
      SCI0_TxChar('w');
      startPos = DecodeStartPos(read);
      endPos = DecodeEndPos(read);
      LCD_String(read);
      check = CheckDeath(read);
      
         if(check == 0){
          StandardMove(startPos,endPos);
         }
         if(check == 1){
          LCD_String("Die");
          lastDeath = DeathMove(startPos,endPos,lastDeath);
         }
         
      castleCheck = CheckCastle(read);
      if(castleCheck == 1)
      {
        startPos = DecodeStartPos(GetCastle(read));
        endPos = DecodeEndPos(GetCastle(read));
        StandardMove(startPos,endPos);
        castleCheck = 0;
      }
      
      SCI0_TxChar('n');
    }
  }
}


/********************************************************************/
//		Functions
/********************************************************************/

 int buildMove()
 {
    in = SCI0_RxChar();
    if(in != 0)
    {   
      if(in == 'y')   
      {
        //LCD_String(read);
         count = 0;
         return 1;
      }
      else
      {       
        read[count++] = in;
        return 0;
      }
	  }
    return 0;
 }


/********************************************************************/
//		Interrupt Service Routines
/********************************************************************/

