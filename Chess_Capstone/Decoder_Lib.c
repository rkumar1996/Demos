//include the required header files 
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include "Decoder_Lib.h"

//struct to save the points on the board

//char arrays to check the string against
char * xPos = "abcdefgh";
char * yPos = "12345678";

//check the incoming character for the end termination
char CheckChar(char c)
{
   if(c == '*'){
      return 0;
   }
   return c;
}

int CheckCastle(char* move)
{
   if(move[2] == 'c')
   {
      return 1;
   }
   return 0;
}

//check if the incoming move is a death move
int CheckDeath(char * move)
{
   if(move[2] == 'x')
   {
      return 1;
   }
   return 0;
}

char* GetCastle(char* move){

  if(move[0] == 'a'){
    
      if(move[1] == '1'){
        return "e1-c1y";
      }
      if(move[1] == '8'){
        return "e8-c8y";
      }
      
  }
  if(move[0] == 'h'){
    
      if(move[1] == '1'){
        return "e1-g1y";
      }
      if(move[1] == '8'){
        return "e8-g8y";
      }
  }
  return "";

}


//decode the start position in the move and return the start point
struct Point DecodeStartPos(char* move)
{
   struct Point startPos;
   //the x position if start point
   int i = 0;
   int j = 0;
   for(i = 0; i < strlen(xPos); i++)
   {
      if(move[0] == xPos[i])
      {
        startPos.X = i + 1;
      }
   }
   
   //the y position if start point
   
   
   for(j = 0; j < strlen(yPos); j++)
   {
      if(move[1] == yPos[j])
      {
        startPos.Y = j + 1;
      }
   }
   return startPos;
}

//decode the end position of the move
struct Point DecodeEndPos(char* move)
{
    struct Point endPos;
   
   //the x position if start point
   int i = 0;
   int j = 0;
   
   for(i = 0; i < strlen(xPos); i++)
   {
      if(move[3] == xPos[i])
      {
        endPos.X = i + 1;
      }
   }
   
   //the y position if start point
   
   
   for(j = 0; j < strlen(yPos); j++)
   {
      if(move[4] == yPos[j])
      {
        endPos.Y = j + 1;
      }
   }
   return endPos;
};