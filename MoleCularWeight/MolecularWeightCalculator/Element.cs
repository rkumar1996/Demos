using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MolecularWeightCalculator
{
    class Element
    {
        //Element's atomic number based on periodic table
        private int iAtomicNumber;
        //Element's full name
        private string sName;
        //Element's symbolic abbreviation
        private string sSymbol;
        //Element's molar mass (g/mol)
        private float fAtomicMass;

        /// <summary>
        /// Element constructor
        /// </summary>
        /// <param name="atomicNumber">Element's atomic number</param>
        /// <param name="name">Element's name</param>
        /// <param name="symbol">Element's symbol</param>
        public Element(int atomicNumber, string name, string symbol, float atomicWeight)
        {
            iAtomicNumber = atomicNumber;
            sName = name;
            sSymbol = symbol;
            fAtomicMass = atomicWeight;
        }
        public Element(string symbol)
        {
            sSymbol = symbol;
        }
        //Atomic number property ensures valid number
        public int AtomicNumber
        {
            get
            {
                return iAtomicNumber;
            }
            private set
            {
                if (value <= 0)
                    throw new Exception("Atomic number value must be a natural number");
                iAtomicNumber = value;
            }
        }
        //Properties for name and symbol ensure valid string
        public string Name
        {
            get
            {
                return sName;
            }
            private set
            {
                if (value.Count() < 1)
                    throw new Exception("Name can not be null or empty");
            }
        }
        public string Symbol
        {
            get
            {
                return sSymbol;
            }
            private set
            {
                if (value.Count() < 1)
                    throw new Exception("Symbol can not be null or empty");
            }
        }
        //Property ensures numbers are not negative or zero
        public float AtomicMass
        {
            get
            {
                return fAtomicMass;
            }
            private set
            {
                if (value <= 0)
                    throw new Exception("Atomic weight must be a positive non-zero real number");
            }
        }
        /// <summary>
        /// Equals override
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Element))
                return false;
            Element other = (Element)obj;
            return sSymbol.Equals(other.sSymbol);
        }
        /// <summary>
        /// Hash code will simply be name string's hash
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return sName.GetHashCode();
        }
        public override string ToString()
        {
            return Name;
        }
    }
}
