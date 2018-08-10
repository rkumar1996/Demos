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

namespace DesignModels
{
    class ShipModel : ShapeBase
    {
        public const int SHIPRAD = 40;
        private static  GraphicsPath gpTri;
        public ShipModel(PointF pos, Color col)
            : base(pos, col)
        {
            gpTri = GetShipModel();
        }
        
        private GraphicsPath GetShipModel()
        {
            GraphicsPath gpTri = new GraphicsPath();
            List<PointF> ptList = new List<PointF>();
            Matrix mat = new Matrix();

            //Points for the model of the ship that  start at origion
            ptList.Add(new PointF(0, 0));
            ptList.Add(new PointF(SHIPRAD * (float)Math.Cos(Math.PI/3), SHIPRAD * (float)Math.Sin(Math.PI / 3)));
            ptList.Add(new PointF(0, SHIPRAD/2));
            ptList.Add(new PointF(SHIPRAD * (float)Math.Cos(2*Math.PI / 3), SHIPRAD * (float)Math.Sin(2*Math.PI / 3)));

            //create the required shape
            gpTri.AddPolygon(ptList.ToArray());
            //rotate the ship 90 degrees 
            mat.Rotate(90);
            gpTri.Transform(mat);
            return gpTri;
        }

        internal override void Tick(Size rect, float yDir, float rotChange)
        {
            float x = -(float)Math.Cos(m_Rotation) * yDir * 5;
            float y = -(float)Math.Sin(m_Rotation) * yDir * 5;

            //add the speed component
            Pos = new PointF(Pos.X + x, Pos.Y + y);
            //add the rotation
            m_Rotation += rotChange / 10;

            if (Pos.X > rect.Width)
            {
                Pos = new PointF(0, Pos.Y);
            }
            if (Pos.X < 0)
            {
                Pos = new PointF(rect.Width, Pos.Y);
            }
            if (Pos.Y < 0)
            {
                Pos = new PointF(Pos.X, rect.Height);
            }
            if (Pos.Y > rect.Height)
            {
                Pos = new PointF(Pos.X, 0);
            }
        }
        internal override GraphicsPath GetPath()
        {
            GraphicsPath temp = (GraphicsPath)gpTri.Clone();
            Matrix mat = new Matrix();
            mat.Rotate((float)(m_Rotation * 180 / Math.PI));
            mat.Translate(Pos.X, Pos.Y, MatrixOrder.Append);
            temp.Transform(mat);
            return temp;
        }
        /// <summary>
        /// Assign the new color to the ship
        /// </summary>
        /// <param name="c"></param>
        public void NewCol(Color c)
        {
            m_Color = c;
        }

        public float RetRotation()
        {
            return m_Rotation;
        }
        public PointF ShootPoint()
        {
            return GetPath().PathPoints[0];
        }

        public PointF CenterPoint()
        {
            return GetPath().PathPoints[0];
        }
    }
}