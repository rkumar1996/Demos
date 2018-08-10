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
    class Bullet : ShapeBase
    {
        public const int BulletSize = 3;
        private static GraphicsPath gpBullet;
        public Bullet(PointF pos, float shipRot)
            : base(pos, Color.Goldenrod)
        {
            gpBullet = GetModel(3, 1, BulletSize);
            m_Rotation = shipRot;
        }

        private GraphicsPath Model()
        {
            GraphicsPath gpTri = new GraphicsPath();

            return gpTri;
        }

        internal override GraphicsPath GetPath()
        {
            GraphicsPath temp = (GraphicsPath)gpBullet.Clone();
            Matrix mat = new Matrix();
            mat.Rotate((float)(m_Rotation * 180 / Math.PI));
            mat.Translate(Pos.X, Pos.Y, MatrixOrder.Append);
            temp.Transform(mat);
            return temp;
        }


        internal override void Tick(Size rect, float yDir = 0, float rotChange = 0)
        {
            float x = (float)Math.Cos(m_Rotation) * 10;
            float y = (float)Math.Sin(m_Rotation) * 10;

            //add the speed component
            Pos = new PointF(Pos.X + x, Pos.Y + y);

            if (Pos.X > rect.Width || Pos.X < 0 || Pos.Y < 0 || Pos.Y > rect.Height)
            {
                IsMarkedForDeath = true;
            }
        }
    }
}
