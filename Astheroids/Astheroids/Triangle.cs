using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Ica_04_PPP
{
    class Triangle : ShapeBase
    {
        //vitices of a triangle
        private const int VERTICES = 3;

        private static readonly GraphicsPath gpTri;

        static Triangle()
        {
            //get the model of the triangle 
            gpTri = GetModel(VERTICES,1); 
        }

        public Triangle(PointF pos, Color col)
            : base(pos, col)
        {

        }
        /// <summary>
        /// Returns the graphics path of the triangle based on the latest co-ordinates and rotation
        /// </summary>
        /// <returns>graphics path of the triangle</returns>
        internal override GraphicsPath GetPath()
        {
            GraphicsPath temp = (GraphicsPath)gpTri.Clone();

            Matrix mat = new Matrix();

            mat.Rotate(m_Rotation);

            mat.Translate(Pos.X, Pos.Y, MatrixOrder.Append);

            temp.Transform(mat);
            return temp;
        }
    }
}
