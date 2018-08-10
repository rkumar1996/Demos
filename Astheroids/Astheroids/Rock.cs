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
    class Rock : ShapeBase
    {
        private const float PercentChange = (float)0.75;
        public int RockSize { get;  set; }
        public int ColAlpha { get; private set; }
        public int RockSides { get; private set; }
        public bool iMirror { get; set; } = false;
        protected float m_RotationChange;
        public bool isDangerous { get;  private set; } = false;
        private float m_Xspeed;
        private float m_Yspeed;
        private GraphicsPath gpRock;
        
        public Rock(PointF pos, Color color, int size, int sides,int colorAlpha, bool dangerous) 
            : base(pos, color)
        {
            ColAlpha = colorAlpha;
            RockSize = size;
            RockSides = sides;
            gpRock = GetModel(sides, PercentChange,size);
            isDangerous = dangerous;
            m_Rotation = (float)((s_Rnd.NextDouble() * RotMaxMin) - (RotMaxMin / 2));
            m_RotationChange = (float)((s_Rnd.NextDouble() * RotMaxMin) - (RotMaxMin / 2));
            m_Xspeed = (float)((s_Rnd.NextDouble() * XYMaxMin) - (XYMaxMin / 2));
            m_Yspeed = (float)((s_Rnd.NextDouble() * XYMaxMin) - (XYMaxMin / 2));
        }
        /// <summary>
        /// Returns the graphics path of the Rock based on the latest co-ordinates and rotation
        /// </summary>
        /// <returns>graphics path of the rock</returns>
        internal override GraphicsPath GetPath()
        {
            GraphicsPath temp = (GraphicsPath)gpRock.Clone();

            Matrix mat = new Matrix();

            mat.Rotate(m_Rotation);

            mat.Translate(Pos.X, Pos.Y, MatrixOrder.Append);

            temp.Transform(mat);
            return temp;
        }
        /// <summary>
        /// Add the alpha to the color component
        /// </summary>
        /// <param name="change">the int to change the color with</param>
        public void AddAlpha(int change)
        {
            ColAlpha += change;
            m_Color = Color.FromArgb(ColAlpha > 255 ? 255 : ColAlpha, m_Color);
        }
        internal override void Tick(Size rect, float yDir, float rotChange = 0)
        {
            //add the speed component
            Pos = new PointF(Pos.X + m_Xspeed, Pos.Y + m_Yspeed);
            //add the rotation component
            m_Rotation += m_RotationChange;

            //if the rock is out of bounds of the window then set the rock to die
            if (Pos.X - RockSize > rect.Width)
                IsMarkedForDeath = true;
            if (Pos.X + RockSize < 0)
                IsMarkedForDeath = true;
            if (Pos.Y + RockSize < 0)
                IsMarkedForDeath = true;
            if (Pos.Y - RockSize > rect.Height)
                IsMarkedForDeath = true;
        }
        /// <summary>
        /// This method mirrors the rock and return it
        /// </summary>
        /// <param name="newPoint">the point where the where new rock is supposed to be added</param>
        /// <returns>A mirror of the rock</returns>
        public Rock MirrorRock(PointF newPoint)
        {
            Rock retRock = new Rock(newPoint, m_Color, RockSize, RockSides, ColAlpha, isDangerous);
            retRock.m_Xspeed = m_Xspeed;
            retRock.m_Yspeed = m_Yspeed;
            retRock.m_Rotation = m_Rotation;
            retRock.m_RotationChange = m_RotationChange;
            retRock.iMirror = true;
            return retRock;
        }
        /// <summary>
        /// set the rock to be dangerous
        /// </summary>
        public void IsDangerous()
        {
            isDangerous = true;
        }
    }
}
