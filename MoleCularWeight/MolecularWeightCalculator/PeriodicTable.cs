using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace MolecularWeightCalculator
{
    internal class PeriodicTable
    {
        //There can only be one periodic table
        private static Element[] elements = new Element[100];
        /// <summary>
        /// Perform periodic table data collection statically
        /// </summary>
        static PeriodicTable()
        {
            //Elements received from https://www.kaggle.com/jwaitze/tablesoftheelements
            List<string[]> rawRows = new List<string[]>();
            string[] lines = Properties.Resources.periodic_table.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            //Start at 1 to skip header lines
            for (int i = 1; i < lines.Length; i++)
            {
                rawRows.Add(lines[i].Split(','));
            }
            //Only get first 100 elements
            rawRows = rawRows.GetRange(0, 100);
            //Linq build elements into master array
            elements = (from x in rawRows select new Element(int.Parse(x[0]), x[1], x[2], float.Parse(x[3]))).ToArray();
        }
        /// <summary>
        /// Returns entire array of elements
        /// </summary>
        /// <returns></returns>
        public static Element[] GetAllElements()
        {
            return elements;
        }
        /// <summary>
        /// Determines whether the symbol exists in master
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public static bool ElementExists(string symbol)
        {
            return elements.Contains(new Element(symbol));
        }
        /// <summary>
        /// Get element from master table
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public static Element GetElement(string symbol)
        {
            return elements.FirstOrDefault(x => x.Symbol.Equals(symbol));
        }
    }
}
