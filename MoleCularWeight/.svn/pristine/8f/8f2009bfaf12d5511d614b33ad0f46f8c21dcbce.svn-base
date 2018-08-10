using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MolecularWeightCalculator
{  
    internal class ElementParser
    {
        //Total raw string
        private string sRawInput;
        //Valid elements according to dictionary build
        private List<string> sValid = new List<string>();
        //Invalid elements according to dictionary build
        private List<string> sInvalid = new List<string>();
        /// <summary>
        /// Constructor forces initialization to empty string, will also take a placeholder
        /// </summary>
        /// <param name="text"></param>
        public ElementParser(string text = "")
        {
            sRawInput = text;
        }
        /// <summary>
        /// Set the parser's processing string from outside the class
        /// </summary>
        public string SetString
        {
            set
            {
                //Remove all whitespace
                sRawInput = Regex.Replace(value, @"\s+", "");
            }
        }
        /// <summary>
        /// List of valid elements build from GetElements()
        /// </summary>
        public List<string> GetValidElementStrings
        {
            get
            {
                return sValid;
            }
        }
        /// <summary>
        /// List of invalid elements built from GetElements();
        /// </summary>
        public List<string> GetInvalidElementStrings
        {
            get
            {
                return sInvalid;
            }
        }

        //Process input string to dictionary keyed by element, value is total count
        public Dictionary<Element, int> GetElements()
        {
            Dictionary<Element, int> elementCount = new Dictionary<Element, int>();
            /*
             * Split string into valid matches including:
             *      -Must have upper case letter
             *      -Optional lower case letter
             *      -Optional number
             *      -Optional two digit number
            */
            var segments = from n in Regex.Split(sRawInput, @"([A-Z][a-z]\d+)|([A-Z][a-z])|([A-Z]\d+)|([A-Z])")
                           where !n.Length.Equals(0)
                           select n;

            //Process valid matches into their separate parts using regular expressions, if there is no number then then count is 1, create groups based on unique string identifiers
            var rawGrouping = from q in segments
                           select new { Symbol = Regex.Match(q, @"([A-Z][a-z]|[A-Z])").Value, Count = string.IsNullOrEmpty(Regex.Match(q, @"\d+").Value) ? 1 : int.Parse(Regex.Match(q, @"\d+").Value) }
                           into r group r by r.Symbol;

            sValid.Clear();
            sInvalid.Clear();
            foreach (var selection in rawGrouping)
            {
                //Only build the dictionary from valid elements
                if (PeriodicTable.ElementExists(selection.Key))
                {
                    sValid.Add(selection.Key);
                    int sum = 0;
                    //Add up the sums of each subgrouping
                    foreach (var item in selection)
                    {
                        sum += item.Count;
                    }
                    elementCount.Add(PeriodicTable.GetElement(selection.Key), sum);
                }
                else
                {
                    sInvalid.Add(selection.Key);
                }
            }
            return elementCount;
        }
    }
}
