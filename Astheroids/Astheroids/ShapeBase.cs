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
    public abstract class ShapeBase
    {
        internal const int RADIUS = 20;
        internal const double RotMaxMin = 6.00;
        internal const double XYMaxMin = 5.00;
        protected float m_Rotation { get; set; }
        protected static Random s_Rnd = new Random();
        public Color m_Color { get; set; }

        public bool IsMarkedForDeath { get; set; }
        //position of the shape on the canvas
        public PointF Pos { get; set; }

        public ShapeBase(PointF pos, Color col)
        {
            m_Color = col;
            Pos = pos;
            m_Rotation = 0;
        }

        //method to return the graphics path of the shape
        internal abstract GraphicsPath GetPath();
        /// <summary>
        /// This method draws the shape on the window
        /// </summary>
        /// <param name="bg">graphics buffer to draw the shape on</param>
        public void Render(BufferedGraphics bg)
        {
            GraphicsPath x = GetPath();
            bg.Graphics.FillPath(new SolidBrush(m_Color), x);
        }
        /// <summary>
        /// This method creates the shape based on the arguments provides and returns its graphics path
        /// </summary>
        /// <param name="vertices">No of vertices for the shape</param>
        /// <param name="percentChange">% of radii required(1 means full radii)</param>
        /// <returns></returns>
        protected static GraphicsPath GetModel(int vertices, float percentChange, int pixelSize)
        {
            float variance = 0;
            
            GraphicsPath gpTri = new GraphicsPath();
            List<PointF> ptList = new List<PointF>();
            float x, y = 0;
            //for the required amount of vertices
            for (int i = 0; i < vertices; i++)
            {
                //calculate the random varience
                variance = (float)((s_Rnd.NextDouble() * (1 - percentChange)) + percentChange);

                //find out the x and y co-ordinates of the point on the eclosing circle to get the corner points of the shape
                x = (float)(Math.Cos((2 * Math.PI / vertices) * (i + 1)) * pixelSize) * variance;
                y = (float)(Math.Sin((2 * Math.PI / vertices) * (i + 1)) * pixelSize) * variance;
                //add the points to a list
                ptList.Add(new PointF(x, y));
            }
            //create the required shape
            gpTri.AddPolygon(ptList.ToArray());
            return gpTri;
        }
        
        //returns the region of the shape based on the given graphics path
        public Region GetRegion()
        {
            return new Region(GetPath());
        }

        internal abstract void Tick(Size rect, float yDir = 0, float rotChange = 0);
    }
}