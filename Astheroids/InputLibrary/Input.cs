using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace InputLibrary
{
    public class Input
    {
        //pause bool
        public bool I_pausePressed { get; private set; } = false;

        public bool I_shoot { get; private set; } = false;

        public bool I_start { get; private set; } = false; 

        //floats fo the movement of the ship
        public float I_Yval { get; private set; } = 0;
        public float I_rot { get; private set; } = 0;
        public bool GameAlive { get; private set; } = false;

        //xbo
        private Thread I_pollXThread;

        private bool xboxIN = false;
        private bool transition = false;

        public Input()
        {
            I_pollXThread = new Thread(new ThreadStart(PollXbox))
            {
                IsBackground = true
            };
            I_pollXThread.Start();
            GameAlive = true;
        }
        /// <summary>
        /// Xbox poll thread method
        /// </summary>
        private void PollXbox()
        {

            while (GameAlive)
            {
                GamePadState controllerState = GamePad.GetState(PlayerIndex.One);

                if (xboxIN = controllerState.IsConnected)
                {
                    transition = true;
                    //get the y valoe of the thumbstick
                    if(controllerState.ThumbSticks.Left.Y > 0)
                    {
                        I_Yval = -1;
                    }
                    else if (controllerState.ThumbSticks.Left.Y < 0)
                    {
                        I_Yval = 1;
                    }else
                    {
                        I_Yval = 0;
                    }
                    if (controllerState.ThumbSticks.Left.X > 0)
                    {
                        I_rot = 1;
                    }
                    else if (controllerState.ThumbSticks.Left.X < 0)
                    {
                        I_rot = -1;
                    }
                    else
                    {
                        I_rot = 0;
                    }
                    //pause
                    I_pausePressed = controllerState.IsButtonDown(Buttons.Start);
                    //shoot check - true on down
                    I_shoot = controllerState.IsButtonDown(Buttons.A);

                }
                else if (transition)
                {
                    transition = false;
                    I_Yval = 0;
                    I_rot = 0;
                }
                Thread.Sleep(15);
            }
        }

        public void InputKeyDown(System.Windows.Forms.KeyEventArgs keyEvent)
        {
            if (!xboxIN)
            {
                if (keyEvent.KeyCode == System.Windows.Forms.Keys.W)
                    I_Yval = -1;
                if (keyEvent.KeyCode == System.Windows.Forms.Keys.S)
                    I_Yval = 1;
                if (keyEvent.KeyCode == System.Windows.Forms.Keys.A)
                    I_rot = -1;
                if (keyEvent.KeyCode == System.Windows.Forms.Keys.D)
                    I_rot = 1;
                if (keyEvent.KeyCode == System.Windows.Forms.Keys.ShiftKey)
                    I_shoot = true;
                if (keyEvent.KeyCode == System.Windows.Forms.Keys.P)
                    I_pausePressed = true;
            }
        }

        public void InputKeyUp(System.Windows.Forms.KeyEventArgs keyEvent)
        {
            if (!xboxIN)
            {
                if (keyEvent.KeyCode == System.Windows.Forms.Keys.W)
                    I_Yval = 0;
                if (keyEvent.KeyCode == System.Windows.Forms.Keys.S)
                    I_Yval = 0;
                if (keyEvent.KeyCode == System.Windows.Forms.Keys.A)
                    I_rot = 0;
                if (keyEvent.KeyCode == System.Windows.Forms.Keys.D)
                    I_rot = 0;
                if (keyEvent.KeyCode == System.Windows.Forms.Keys.ShiftKey)
                    I_shoot = false;
                if (keyEvent.KeyCode == System.Windows.Forms.Keys.P)
                    I_pausePressed = false;
            }
        }
        public void Close()
        {
            GameAlive = false;
        }
    }
}
