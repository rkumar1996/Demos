/********************************************************************/
//		Library includes
/********************************************************************/
#include <hidef.h>      		/* common defines and macros 		*/
#include "derivative.h"      	/* derivative-specific definitions 	*/

#include <stdio.h>
#include <string.h>

#include "Decoder_Lib.h"
#include "SwLeds_lib.h"
#include "Misc_Lib.h"


//Helper Method for turning on specified Motor and Led for direction
//for the specified amount of time 
//direction +1 == forward, -1 == backwards
//xORy expects 'x' or 'y' for direction (lowercase)
//how long == number of half squares to move
void ActivateLedMotor(char, int, float);
void ToCenter(char);
void Move(struct Point, struct Point);
//Half Square distance == 3.4cm
//2.1cm == 100ms delay (full step) double when micro stepping
  
  
//Green LED (5) == Speaker (channel 6) -- X-direction
//Red LED (8) == BackLight (Channel 3) -- Y-direction
//Yellow LED (7) == Actuator

static int msHalfSquare;      //ms delay to move a half square 
static int stepSize;          //step size to use
static struct Point homePos;  //where magnet starts and ends
int deathAlt;

void StandardMove(struct Point start, struct Point end){
     Move(homePos, start);
     ToCenter('p');
     Move(start, end);
     ToCenter('d');
     Move(end,homePos);
}

void ToCenter(char PickOrDrop) {
     
     if(PickOrDrop != 'p' && PickOrDrop != 'd') {
        return; //Bad input, just return
     }
     
     //X-Dir
     ActivateLedMotor('x', 1, 1.1); //Move 1 half square forward
     
     //Y-Dir
     ActivateLedMotor('y', 1, 1.1);  //Move 1 half square forward
    
     //Activate Actuator
     if(PickOrDrop == 'p'){
        PT1AD1 |= 0b01000000;     
        PTM |= 0b10000000;        //Turn on bit 7 of mosfet to pick up
     } else if(PickOrDrop == 'd'){
        PT1AD1 &= 0b10111111;        
        PTM &= 0b01111111;         //Turn bit 7 of mosfet to drop  
     }    
     
     //Y-Dir Moving back
     ActivateLedMotor('y', -1, 1.1);    //Move 1 half square back

     //X-Dir Moving Back
     ActivateLedMotor('x', -1, 1.1);   //Move 1 half square back
    
}
void Move(struct Point start, struct Point end){
  
    int xDistance = 0;
    int yDistance = 0;
    
    //Distances between points (in half block lengths by * 2)
    xDistance = (end.X - start.X) * 2;  
    yDistance = (end.Y - start.Y) * 2;  
       
    if(yDistance > 0) {      
       ActivateLedMotor('y', 1, yDistance); 
    }else if(yDistance < 0){        
       ActivateLedMotor('y', -1, yDistance * -1); 
       }   
	
    if(xDistance > 0) {      
       ActivateLedMotor('x', 1, xDistance);  
    }else if(xDistance < 0){        
       ActivateLedMotor('x', -1, xDistance * -1); 
       }
  
}

void MoveInit(int halfSquare, int step){

    struct Point start;
      
    start.X = 1;
    start.Y = 1;
    
      
    DDRM |= 0b10000000;
    PTM &= 0b01111111;
    
    
    PWMPOL = 0b11111111;  //positive polarity-all channels
    PWMCLK |= 0b01001000; //use SB as clock for channel 3,6
      
    PWMPRCLK &= 0b00001111; 
    PWMPRCLK |= 0b00100010; //Pre-B = 2^4  PRE-A = 2^3
      
    PWMSCLB = 4; //scale B = 2 * 21
    PWMSCLA = 4;
      
    PWMPER6 = 250; //period = 200
    PWMDTY6 = PWMPER6 / 2; 
      
    PWMPER3 = 250; //period = 100
    PWMDTY3 = PWMPER3 / 2; //50% dty cycle 
    
    deathAlt = 0;
      
    homePos.X = 5;
    homePos.Y = 5;
      
    msHalfSquare = halfSquare;
    stepSize = step;
       
     
    Move(start, homePos);
  
}

struct Point DeathMove(struct Point startPos, struct Point endPos, struct Point deathArea){
  
  struct Point temp = deathArea;
  //temp.X = deathArea.X;
  //temp.Y = deathArea.Y;
  
  Move(homePos,endPos);
  
  ToCenter('p');
  
  Move(endPos, deathArea);
  
  if(deathAlt == 1) {
    PTM &= 0b01111111;  //Drop piece
    temp.Y += 1;
    deathAlt = 0;
  }else if(deathAlt == 0){
    ToCenter('d');
    deathAlt = 1;    
  } 

  
  Move(deathArea, startPos);
  ToCenter('p');
  Move(startPos,endPos);
  ToCenter('d');
  Move(endPos,homePos);
  
  return temp;
}


void ActivateLedMotor(char xORy, int direction, float howLong){
     
     //How long to delay for                          
     int distance = msHalfSquare * stepSize * howLong;
     
     if(xORy == 'x'){
      if(direction == 1){
         //X-Dir forward
         PT1AD1 |= 0b00100000;               //Green LED forward = 5V
         PWME |= 0b01000000;                 //Turn on motor
         Sleep_ms(distance); 
         PWME &= 0b10111111;                 //Turn off motor
      }else if(direction == -1) {        
         //X-Dir Moving Back
         PT1AD1 &= 0b11011111;               //Green LED negative-dir = 0V
         PWME |= 0b01000000;                 //Turn on motor
         Sleep_ms(distance); //distance
         PWME &= 0b10111111;                 //Turn off motor
      }
    }else if(xORy == 'y') {
       if(direction == 1){
         //Y-Dir forward
         PT1AD1 |= 0b10000000;    //Red LED forward = 5V
         PWME |= 0b00001000;      //Turn on motor for 1 half square
         Sleep_ms(distance);      //How long to move for
         PWME &= 0b11110111;      //Turn off motor
      }else if(direction == -1) {        
         //Y-Dir Moving Back
         PT1AD1 &= 0b01111111;   //Red LED negative-dir = 0V
         PWME |= 0b00001000;                 
         Sleep_ms(distance);  
         PWME &= 0b11110111;
      }
      
    }
}

