using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityAssetLib
{
    public partial class Rectf : Object
    {
        public float x;
        public float y;
        public float width;
        public float height;
        public Rectf() { }
        public Rectf(float x, float y, float width, float height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }
        public Rectf(RectangleF rectangleF) : this(rectangleF.X, rectangleF.Y, rectangleF.Width, rectangleF.Height) { }
        public RectangleF ToRectangleF()
        {
            return new RectangleF(x, y, width, height);
        }
    }
}
