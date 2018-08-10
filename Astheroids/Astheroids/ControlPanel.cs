/*
 * Control panel class to create a modless dialog and send data to and fro the main form
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Astheroids
{
    //delegate to send info to the main form
    public delegate void delVoidInfo(int difficulity, Color ShipCol, Color RockCol, Color backCol);
    public partial class ControlPanel : Form
    {
        //instance of the delegate
        public delVoidInfo getInfo = null;
        public int selectedDifficulity;
        public bool gameStart = false;
        //color variables to store return values
        public Color shipCol;
        public Color rockCol;
        public Color backCol;
        public int score = 0;
        //new binding source instance
        private BindingSource bs = new BindingSource();
        //dictionary to store the scores of the players
        public Dictionary<string, int> scores = new Dictionary<string, int>();
        public ControlPanel()
        {
            InitializeComponent();
            //initialize the difficulty combobox
            DifficulityCombo.SelectedIndex = 0;
            selectedDifficulity = DifficulityCombo.SelectedIndex;
            //initialize the color variables
            shipCol = Color.Yellow;
            rockCol = Color.Red;
            backCol = Color.Black;
            //set the source of the biding source to the dictionary
            bs.DataSource = scores;
            //reda in the scores
            Read();
            //display the rules
            label3.Text = "Rules:\n" + "- Asteroids move around in the game window\n- You move your ship around and shoot the asteroids for points" +
                          "\n- You have three lives, bump into an asteroid and you lose one";
            //set data grid view to the binding source
            dataGridView1.DataSource = bs;
            //display the difficulty text 
            DifficulityLabelText();
            //method to invoke the delgate 
            ReturnInfo();
        }
        /// <summary>
        /// This method sets the color for the labels and invokes the delegate with it's method
        /// </summary>
        public void ReturnInfo()
        {
            //set the color of the labels
            shipColLabel.BackColor = shipCol;
            rockColLabel.BackColor = rockCol;
            backColLabel.BackColor = backCol;
            if (getInfo != null)
                getInfo.Invoke(selectedDifficulity, shipCol, rockCol, backCol);
        }
        /// <summary>
        /// selected index changed event of the combo box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DifficulityCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            //set the selected difficulty to selected index of the combobox
            selectedDifficulity = DifficulityCombo.SelectedIndex;
            ReturnInfo();
            DifficulityLabelText();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                shipCol = colorDialog1.Color;
            }
            ReturnInfo();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                rockCol = colorDialog1.Color;
            }
            ReturnInfo();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                backCol = colorDialog1.Color;
            }
            ReturnInfo();
        }
        /// <summary>
        /// Form's closing events
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ControlPanel_FormClosing(object sender, FormClosingEventArgs e)
        {
            //write the bin file
            Write();
            //cancel the closing event
            MessageBox.Show("Cannot Close the control panel while the game is running");
            e.Cancel = true;
        }
        /// <summary>
        /// set the tect of the label according to the selected difficulty
        /// </summary>
        private void DifficulityLabelText()
        {
            if (selectedDifficulity == 0)
            {
                label4.Text = "- Big Rock: 5 pts\n- Medium Rock: 10 pts\n- Small Rock: 15 pts\n Asteroids spawn every 2 seconds";
            }
            if (selectedDifficulity == 1)
            {
                label4.Text = "- Big Rock: 10 pts\n- Medium Rock: 15 pts\n- Small Rock: 20 pts\n Asteroids spawn every second";
            }
            if (selectedDifficulity == 2)
            {
                label4.Text = "- Big Rock: 15 pts\n- Medium Rock: 20 pts\n- Small Rock: 25 pts\n Asteroids spawn every half a second";
            }
        }

        public void GetScore(int thisScore)
        {
            score = thisScore;
            textBox1.Clear();
            textBox1.Enabled = true;
            saveBtn.Enabled = true;
            label5.Text = "Your Score : " + score.ToString();
        }

        public void NewGame()
        {
            Read();
            textBox1.Text = "Enter Your Name";
            textBox1.Enabled = false;
            saveBtn.Enabled = false;
            label5.Text = "";
        }
        /// <summary>
        /// write the scores t a binary file
        /// </summary>
        private void Write()
        {
            FileStream fs = new FileStream("scores.bin", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, scores);
            fs.Close();
            Read();
        }
        /// <summary>
        /// Read the binary file
        /// </summary>
        private void Read()
        {
            FileStream fs = new FileStream("scores.bin", FileMode.Open, FileAccess.Read);
            BinaryFormatter bf = new BinaryFormatter();
            scores = (Dictionary<string, int>)bf.Deserialize(fs);
            bs.DataSource = from n in scores.ToList() orderby -n.Value select new { Player = n.Key, Score = n.Value };
            fs.Close();
        }
        /// <summary>
        /// save button click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveBtn_Click(object sender, EventArgs e)
        {
            //add the new score to the dictionary
            scores.Add(textBox1.Text, score);
            //write the dict to the bin file
            Write();
            textBox1.Enabled = false;
            saveBtn.Enabled = false;
        }
    }
}
