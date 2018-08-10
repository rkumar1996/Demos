/*********************************************************
 *  Assignment: Molecular Mass Calculator
 *  Written by: Karun Kakulphimp
 *              Reeshav Kumar
 *              Felipe Restrepo Rubio
 *  Description:
 *      This program allows the user to input the formula
 *      for a chemical compound which will then be parsed
 *      into its separate elements and a total g/mol is
 *      calculated and displayed for the user.
**********************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MolecularWeightCalculator
{
    public partial class MolecularWeightCalculator : Form
    {
        private ElementParser inputParser = new ElementParser();
        private BindingSource binder = new BindingSource();

        //dictionary to store the elements parsed in from the string
        private Dictionary<Element, int> workingElements = new Dictionary<Element, int>();

        public MolecularWeightCalculator()
        {
            InitializeComponent();
            UI_dataGridView.DataSource = binder;
            Blank();
        }
        /// <summary>
        /// Sort elements by name and display them in the data grid view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSortName_Click(object sender, EventArgs e)
        {
            var finalElements = from q in PeriodicTable.GetAllElements()
                                orderby q.Name
                                select q;
            //Show data
            binder.DataSource = finalElements;
        }
        /// <summary>
        /// Sort elements by name and display them in the data grid view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCharSymbols_Click(object sender, EventArgs e)
        {
            var finalElements = from q in PeriodicTable.GetAllElements()
                                orderby q.Symbol
                                select q;
            //Show data
            binder.DataSource = finalElements;
        }
        /// <summary>
        /// Sort elements by Atomic mass and display them in the data grid view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSortAtomic_Click(object sender, EventArgs e)
        {
            var finalElements = from q in PeriodicTable.GetAllElements()
                                orderby q.AtomicMass
                                select q;
            //Show data
            binder.DataSource = finalElements;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender">Textbox containing the input string</param>
        /// <param name="e"></param>
        private void txBxChemFormIn_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty((sender as TextBox).Text))
            {
                //If there's no text put in default display
                Blank();
            }
            else
            {
                DisableBtns();
                //Assign the input text to the parser
                inputParser.SetString = (sender as TextBox).Text;
                //get the final elements
                workingElements = inputParser.GetElements();
                //Get final elements into viewable format
                var finalElements = from q in workingElements
                                    select new { Element = q.Key, Count = q.Value, q.Key.AtomicMass, TotalMass = q.Value * q.Key.AtomicMass };
                //Show data
                binder.DataSource = finalElements;
                //Display feedback text
                UI_labelFeedback.Text = $"Valid elements: " +
                                        ((inputParser.GetValidElementStrings.Count > 0) ? string.Join(", ", inputParser.GetValidElementStrings) : "None") +
                                        "\nInvalid elements: " +
                                        ((inputParser.GetInvalidElementStrings.Count > 0) ? string.Join(", ", inputParser.GetInvalidElementStrings) : "None");
                //Show final calculation as long as the formula is valid
                UI_txbxMolarMassOut.Text = inputParser.GetInvalidElementStrings.Count.Equals(0) ? finalElements.Sum(x => x.TotalMass).ToString() + "g/mol" : "Formula is invalid";
                //Determine foreground color based on validity
                UI_txbxMolarMassOut.ForeColor = inputParser.GetInvalidElementStrings.Count.Equals(0) ? Color.DarkGreen : Color.Red;
            }
        }
        /// <summary>
        /// Default blank display
        /// </summary>
        private void Blank()
        {
            UI_btnCharSymbols.Enabled = true;
            UI_btnSortAtomic.Enabled = true;
            UI_btnSortName.Enabled = true;
            binder.DataSource = PeriodicTable.GetAllElements();
            UI_labelFeedback.Text = "";
            UI_txbxMolarMassOut.Text = "Enter formula";
            UI_txbxMolarMassOut.ForeColor = Color.Black;
        }
        /// <summary>
        /// Disable the ui buttns
        /// </summary>
        private void DisableBtns()
        {
            UI_btnCharSymbols.Enabled = false;
            UI_btnSortAtomic.Enabled = false;
            UI_btnSortName.Enabled = false;
        }

        private void UI_chkDebug_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as CheckBox).Checked)
                UI_labelFeedback.Visible = true;
            else
                UI_labelFeedback.Visible = false;
        }
    }
}
