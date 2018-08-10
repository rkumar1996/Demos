//Switches and LEDs 
//Processor:  MC9S12XDP512 
//Crystal:  16 MHz 


void SwLED_Init(void);    //LEDs as outputs, switches as inputs, dig inputs enabled 
void LED_On(char);        //accepts R, G, Y, A (for all) 
void LED_Off(char);       //accepts R, G, Y, A (for all) 
void LED_Tog(char);       //accepts R, G, Y, A (for all) 
char SwCk(void);