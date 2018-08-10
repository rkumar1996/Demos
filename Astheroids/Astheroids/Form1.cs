/*
 * Program     : Astheroids 
 * Description : This game allows the user to move the ship around the form while trying to evade the moving asteroids around the screen. The 
 *               player has the ability to shoot bullets and kill asteroids. The inputs are taken care of by the input library
 * Date:         3/13/2018
 * Author:       Reeshav Kumar
 * Course:       CMPE2800
 * Class:        A02
 * Instructor:   Simon Walker
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using InputLibrary;
using DesignModels;
using System.Diagnostics;

namespace Astheroids
{
    public partial class Form1 : Form
    {
        private const int MAXLIVES = 3;
        private const int BULLETSPAWN = 200; //bullets spawn every 200 milliseconds
        private const int ALPHACHANGE = 5; //change in alpa of the rock color evry tick
        private const int MAXBULLETS = 8;


        //control panel of the game to chage gameplay settings
        private ControlPanel cp = new ControlPanel();

        //Diffuculty of the game that specifies the spawn rate of the asteroid
        enum Difficulity { Easy = 2000, Medium = 1000, Hard = 500 }
        //Enum for size of the rock to be added to the game
        enum RockSize { Full = 40, Med = 20, Small = 10 }
        //max possible tranparency for the color
        private const int MAXALPHA = 255;
        //keep track og the score in the game
        private static int score = 0;

        //bullet list to keep track of all the bullets
        private static List<Bullet> bulletList = new List<Bullet>();
        //Rock list to store all the rocks present in the game
        private static List<Rock> rockList = new List<Rock>();
        //store the instance of the ship
        private static ShipModel myShip;
        //create an instance of the inout class
        private static Input myInput = new Input();
        //stopwatches to manage spawn rate of the bullets and rocks
        private static Stopwatch bulletTime = new Stopwatch();
        private static Stopwatch rockTime = new Stopwatch();
        //random number generator to create rocks at random positions
        private static Random rnd = new Random();
        //number of lives the player has
        private static int lives = 0;
        //bool to check if a new rock is to be added
        private static bool newRock = false;
        //floats for the position of the new rock
        private static float xRockPos = 0;
        private static float yRockPos = 0;

        //bools to check the pause transition
        private static bool transition = false;
        private static bool pauseState = false;
        //bool to check if a game is started or not
        private static bool start = false;
        //color variables for capturing the colors coming in from the modless dialog
        private static Color shipCol;
        private static Color rockCol;
        private static Color backCol;
        //int for selected difficulity of the game
        private int selectedDifficulity = (int)Difficulity.Easy;

        public Form1()
        {
            InitializeComponent();
            //start the timers
            bulletTime.Start();
            rockTime.Start();
            //set the live to the maximum 
            lives = MAXLIVES;
            //initialize the x and y ppos of the rock to be added to a random number
            xRockPos = (float)rnd.NextDouble() * ClientSize.Width;
            yRockPos = (float)rnd.NextDouble() * ClientSize.Height;
            //initialize the color properties
            shipCol = Color.Yellow;
            rockCol = Color.Red;
            backCol = Color.Black;
            //show the modless dialog
            cp.Show();
            //create the ship model at the center of the screen
            myShip = new ShipModel(new PointF(ClientRectangle.Width / 2, ClientRectangle.Height / 2), shipCol);
        }
        /// <summary>
        /// Timer event that happens every 25 ms
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer1_Tick(object sender, EventArgs e)
        {
            //use the method to assign new method to the cp delegate
            ModlessDelgateMethod();
            Graphics gr = CreateGraphics();
            using (BufferedGraphicsContext bgc = new BufferedGraphicsContext())
            {
                using (BufferedGraphics bg = bgc.Allocate(gr, ClientRectangle))
                {
                    //turn anti aliasing on
                    bg.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    //clear the backgroung and set color to black
                    bg.Graphics.Clear(backCol);

                    //if the game is in focus
                    if (ContainsFocus)
                    {
                        //check the pause state
                        if (myInput.I_pausePressed && !transition)
                        {
                            transition = true;
                            pauseState = !pauseState;
                        }
                        else if (transition && !myInput.I_pausePressed)
                            transition = false;

                        //shoot animation - change the color of ship evry second
                        if (myInput.I_shoot && bulletTime.ElapsedMilliseconds > BULLETSPAWN)
                        {
                            AddBullet();
                            bulletTime.Restart();
                        }
                        //if the game is not about to start 
                        if (!start)
                        {
                            //display the start game page
                            GameStart(bg);
                        }
                        else
                        {
                            //if the game is not paused and the user has not ran out of lives
                            if (!pauseState && lives > 0)
                            {
                                //Use rock manager to add new rocks to the game
                                RockManager();
                                //use mirroring to create mirrors of the rocks floating out of the window
                                Mirroring();
                                //Check for bullet and rock collision
                                BulletVsRock(gr);
                                //remove all bullets that are marked for death
                                bulletList.RemoveAll(q => q.IsMarkedForDeath);
                                //tick each bullet
                                bulletList.ForEach(q => q.Tick(ClientSize));
                                //remove all rocks that are maked for death
                                rockList.RemoveAll(q => q.IsMarkedForDeath);
                                //tick each rock
                                rockList.ForEach(q => q.Tick(ClientSize, 5, 0));
                                //tick the ship with values of the inputs
                                myShip.Tick(ClientSize, myInput.I_Yval, myInput.I_rot);
                                //check intersection between ship and the rock
                                ShipVsRock(gr);
                                //if the ship is marked for death
                                if (myShip.IsMarkedForDeath)
                                {
                                    //decrement the life counter
                                    lives--;
                                    //set the death bool to false
                                    myShip.IsMarkedForDeath = false;
                                }
                                //render all the shapes
                                bulletList.ForEach(q => q.Render(bg));
                                rockList.ForEach(q => q.Render(bg));
                                myShip.Render(bg);
                                //diaple the status of the gameplay
                                DisplayStats(bg);
                            }
                            //pause state
                            else if (pauseState && lives > 0)
                            {
                                Pause(bg);
                            }
                            //game over state
                            else if (lives <= 0)
                            {
                                GameOver(bg);
                            }
                        }
                    }
                    else
                    {
                        NoFocus(bg);
                    }
                    //render the back buffer
                    bg.Render();
                }
            }
        }
        /// <summary>
        /// Hit detection between rock and the ship
        /// </summary>
        /// <param name="gr">graphics object to check the regions on</param>
        private void ShipVsRock(Graphics gr)
        {
            PointF shipCenter = myShip.CenterPoint();
            for (int i = 0; i < rockList.Count; i++)
            {
                PointF thisRock = rockList[i].Pos;
                //if the rock is dangerous
                if (rockList[i].isDangerous)
                {
                    //check if the shapes are near each other
                    if (GetDistance(shipCenter, thisRock) <= (ShipModel.SHIPRAD / 2 + rockList[i].RockSize))
                    {
                        //intersect the regions 
                        Region shipReg = myShip.GetRegion();
                        Region rockReg = rockList[i].GetRegion();
                        shipReg.Intersect(rockReg);
                        if (!shipReg.IsEmpty(gr))
                        {
                            //if the regions intersect set the death bool of the ship and the rock to true
                            myShip.IsMarkedForDeath = rockList[i].IsMarkedForDeath = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This method adds rocks to the game and sets the dangerous bool of qualifying rocks
        /// </summary>
        private void RockManager()
        {
            //add rocks at the game at the selected spawn rate
            if (rockTime.ElapsedMilliseconds >= selectedDifficulity)
            {
                AddRock();
                rockTime.Restart();
            }
            //if the rocks have fully faded in the set it to be dangerous;
            rockList.FindAll(q => q.RockSize == (int)RockSize.Full && q.ColAlpha == MAXALPHA).ForEach(q => q.IsDangerous());
            //increase the alpha component of the rock
            rockList.ForEach(q => q.AddAlpha(ALPHACHANGE));
        }

        /// <summary>
        /// Method to assign the control panel delegate a target method
        /// </summary>
        private void ModlessDelgateMethod()
        {
            cp.getInfo = new delVoidInfo(SelectDifficulity);
        }

        /// <summary>
        /// This method iterates through the rock list and check if any mirror are required
        /// </summary>
        private void Mirroring()
        {
            //mirror variables to hold the position
            float mirrorX = 0;
            float mirrorY = 0;
            
            for (int i = 0; i < rockList.Count; i++)
            {
                Rock thisRock = rockList[i];
                bool thisMirror = false;
                //assign the position variables to the initial posiition of the rock
                mirrorX = thisRock.Pos.X;
                mirrorY = thisRock.Pos.Y;

                //if the rock excedes the right edge
                if (thisRock.Pos.X + thisRock.RockSize > ClientSize.Width)
                {
                    //set the position to the eopposite edge
                    mirrorX = (thisRock.Pos.X - ClientSize.Width);
                    thisMirror = true;
                }
                //if the rock excedes the left edge
                else if (thisRock.Pos.X - thisRock.RockSize < 0)
                {
                    //set the position to the eopposite edge
                    mirrorX = (ClientSize.Width + thisRock.Pos.X);
                    thisMirror = true;
                }
                //if the rock excedds top edge
                else if (thisRock.Pos.Y - thisRock.RockSize < 0)
                {
                    //set the position to the eopposite edge
                    mirrorY = (ClientSize.Height + thisRock.Pos.Y);
                    thisMirror = true;
                }
                //if the rock exceeds bottom edge
                else if (thisRock.Pos.Y + thisRock.RockSize > ClientSize.Height)
                {
                    //set the position to the eopposite edge
                    mirrorY = (thisRock.Pos.Y - ClientSize.Height);
                    thisMirror = true;
                }
                //else set the mirror bool of the rock to false
                else
                {
                    rockList[i].iMirror = false;
                }
                //if the rock doesn's already have a mirror
                if (!rockList[i].iMirror)
                {
                    //check if this rock warrents a mirror
                    if (thisMirror)
                    {
                        //add the mirror of the rock to the list
                        rockList.Add(rockList[i].MirrorRock(new PointF(mirrorX, mirrorY)));
                        rockList[i].iMirror = true;
                    }
                }
            }
        }
        /// <summary>
        /// Hit detection b/w Rock and Bullet
        /// </summary>
        /// <param name="gr">Drawing surface to check the inrtersrcion on</param>
        private void BulletVsRock(Graphics gr)
        {
            for (int i = 0; i < bulletList.Count; i++)
            {
                PointF thisBullet = bulletList[i].Pos;
                for (int j = 0; j < rockList.Count; j++)
                {
                    PointF thisRock = rockList[j].Pos;
                    //if they are in the vicinity of each other
                    if (GetDistance(thisBullet, thisRock) <= (Bullet.BulletSize + rockList[j].RockSize))
                    {
                        //spin up their regions and check intersects
                        Region rockRegion = rockList[j].GetRegion();
                        rockRegion.Intersect(bulletList[i].GetRegion());

                        if (!rockRegion.IsEmpty(gr))
                        {
                            //set the bullet to die
                            bulletList[i].IsMarkedForDeath = true;
                            //for full size rocks
                            if (rockList[j].RockSize == (int)RockSize.Full)
                            {
                                //add 3 med rocks
                                for (int a = 0; a < 3; a++)
                                {
                                    rockList.Add(new Rock(rockList[j].Pos, rockList[j].m_Color, (int)RockSize.Med, rnd.Next(6, 10), MAXALPHA, true));
                                }
                                //score the player
                                GivePoints(5, 10, 15);
                            }
                            //for med sized rocks
                            else if (rockList[j].RockSize == (int)RockSize.Med)
                            {
                                //add 2 small rocks
                                for (int b = 0; b < 2; b++)
                                {
                                    rockList.Add(new Rock(rockList[j].Pos, rockList[j].m_Color, (int)RockSize.Small, rnd.Next(6, 10), MAXALPHA, true));
                                }
                                //score the player
                                GivePoints(15,20,25);
                            }
                            //for small rocks
                            else if (rockList[j].RockSize == (int)RockSize.Small)
                            {
                                //score the player
                                GivePoints(20,25,30);
                            }
                            //set the wounded rock to die
                            rockList[j].IsMarkedForDeath = true;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Adds the rock to the list of rocks in the game
        /// </summary>
        private void AddRock()
        {
            //get the sides for this rock (min = 6, max = 10)
            int tempRockSides = rnd.Next(6, 10);
            if (newRock)
            {
                //if a new rock is supposed to be added
                //then get the random position in the game window
                xRockPos = (float)rnd.NextDouble() * ClientSize.Width;
                yRockPos = (float)rnd.NextDouble() * ClientSize.Height;
                newRock = false;
            }
            else
            {
                //add the rock with 0 alpha 
                rockList.Add(new Rock(new PointF(xRockPos, yRockPos), Color.FromArgb(0, rockCol), (int)RockSize.Full, tempRockSides, 0, false));
                newRock = true;
            }
        }
        /// <summary>
        /// Add the bullet to the game 
        /// </summary>
        private void AddBullet()
        {
            if (bulletList.Count < MAXBULLETS)
                bulletList.Add(new Bullet(myShip.ShootPoint(), myShip.RetRotation()));
        }
        /// <summary>
        /// Key down even of the form that uses the input manager's key down method to 
        /// get the key event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            //invoke the input manager's key down methd
            myInput.InputKeyDown(e);
        }
        /// <summary>
        /// Clod=sing events of the form calls input manager's close method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            myInput.Close();
        }
        /// <summary>
        /// form's key up event calls input manager's key up method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            myInput.InputKeyUp(e);
        }
        /// <summary>
        /// Delegate's target method to retrieve control panel's data and 
        /// set the values 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="ship"></param>
        /// <param name="rock"></param>
        /// <param name="back"></param>
        private void SelectDifficulity(int index, Color ship, Color rock, Color back)
        {
            // for the selected index of the difficulity set the selected difficulty int
            if (index == 0)
            {
                selectedDifficulity = (int)Difficulity.Easy;
            }
            if (index == 1)
            {
                selectedDifficulity = (int)Difficulity.Medium;
            }
            if (index == 2)
            {
                selectedDifficulity = (int)Difficulity.Hard;
            }
            //set the color variables of the form to the incoming colors
            shipCol = ship;
            myShip.m_Color = ship;
            rockCol = rock;
            backCol = back;
        }

        /// <summary>
        /// Returns the distance between two points in a cartesian plane
        /// </summary>
        /// <param name="p1">point 1</param>
        /// <param name="p2">point 2</param>
        /// <returns>distance as a float</returns>
        private float GetDistance(PointF p1, PointF p2)
        {
            return (float)Math.Sqrt(Math.Pow((p2.X - p1.X), 2) + Math.Pow((p2.Y - p1.Y), 2));
        }

        /// <summary>
        /// Displays the text for the start of the game
        /// </summary>
        /// <param name="bg">buffered graphics to draw the text on</param>
        private void GameStart(BufferedGraphics bg)
        {
            bg.Graphics.DrawString("Astheroids", new Font(FontFamily.GenericSerif, 50, FontStyle.Bold), new SolidBrush(Color.Blue), new RectangleF(ClientSize.Width / 2 - 175, 100, ClientSize.Width, 100));
            bg.Graphics.DrawString("Click Here to start game.", new Font(FontFamily.GenericSerif, 20, FontStyle.Underline), new SolidBrush(Color.Blue), new RectangleF(ClientSize.Width / 2 - 150, 200, ClientSize.Width, 30));
        }
        /// <summary>
        /// Displays the text when the control loses focus
        /// </summary>
        /// <param name="bg">buffered graphics to draw the text on</param>
        private void NoFocus(BufferedGraphics bg)
        {
            bg.Graphics.DrawString("The control panel has the focus.\n Please click to resume.", new Font(FontFamily.GenericSerif, 20, FontStyle.Bold), new SolidBrush(Color.FromArgb(128, Color.Green)), new RectangleF(ClientSize.Width / 2 - 175, 100, ClientSize.Width, 100));
        }
        /// <summary>
        /// Displays the text when the game is paused
        /// </summary>
        /// <param name="bg">buffered graphics to draw the text on</param>
        private void Pause(BufferedGraphics bg)
        {
            bg.Graphics.DrawString("Paused!\nScore : " + score.ToString() + "\nLives Left : " + lives.ToString(), new Font(FontFamily.GenericSerif, 50, FontStyle.Bold), new SolidBrush(Color.FromArgb(128, Color.Green)), new RectangleF(ClientSize.Width / 2 - 175, 200, ClientSize.Width, 250));
        }
        /// <summary>
        /// Displays the text when the game is over
        /// </summary>
        /// <param name="bg">buffered graphics to draw the text on</param>
        private void GameOver(BufferedGraphics bg)
        {
            bg.Graphics.DrawString("Game Over!\nScore : " + score.ToString(), new Font(FontFamily.GenericSerif, 50, FontStyle.Bold), new SolidBrush(Color.FromArgb(128, Color.Green)), new RectangleF(ClientSize.Width / 2 - 175, 200, ClientSize.Width, 250));
            bg.Graphics.DrawString("Click Here to play again", new Font(FontFamily.GenericSerif, 20, FontStyle.Underline), new SolidBrush(Color.Blue), new RectangleF(ClientSize.Width / 2 - 150, 450, ClientSize.Width, 200));
            //call the get score method of the modless dialog
            cp.GetScore(score);
        }
        /// <summary>
        /// Displays the stats of the game
        /// </summary>
        /// <param name="bg">buffered graphics to draw the text on</param>
        private void DisplayStats(BufferedGraphics bg)
        {
            bg.Graphics.DrawString("Score : " + score.ToString(), new Font(FontFamily.GenericSerif, 20, FontStyle.Bold), new SolidBrush(Color.Blue), 5, 5);
            bg.Graphics.DrawString("Lives Left " + lives.ToString(), new Font(FontFamily.GenericSerif, 20, FontStyle.Bold), new SolidBrush(Color.Blue), 5, 30);
        }
        /// <summary>
        /// Method to add points to the user's score
        /// </summary>
        /// <param name="easy">points for easy difficulty</param>
        /// <param name="med">points for med difficulty</param>
        /// <param name="hard">points for hard difficulty</param>
        private void GivePoints(int easy, int med , int hard)
        {
            if (selectedDifficulity == (int)Difficulity.Easy)
                score += easy;
            if (selectedDifficulity == (int)Difficulity.Medium)
                score += med;
            if (selectedDifficulity == (int)Difficulity.Hard)
                score += hard;
        }
        /// <summary>
        /// mouse click event of the form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Mouse event that occured</param>
        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            //if the game is about to start
            if (!start)
            {
                RectangleF rect = new RectangleF(ClientSize.Width / 2 - 200, 200, ClientSize.Width, 100);
                //if the click point is in the rectangle
                if (rect.Contains(e.Location))
                {
                    //start the game
                    start = true;
                    cp.NewGame();
                }
            }
            else
            {
                //if the game is starting after finishing the last one
                RectangleF rect = new RectangleF(ClientSize.Width / 2 - 150, 450, ClientSize.Width, 200);
                if (rect.Contains(e.Location))
                {
                    //reset the lives 
                    lives = MAXLIVES;
                    score = 0;
                    //get the new model og the ship
                    myShip = new ShipModel(new PointF(ClientRectangle.Width / 2, ClientRectangle.Height / 2), shipCol);
                    //clear the rock and bullet list
                    rockList.Clear();
                    bulletList.Clear();
                    //set the game to about to be startred
                    start = false;
                }
            }
        }
    }
}