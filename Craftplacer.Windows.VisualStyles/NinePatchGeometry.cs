using System;
using System.Drawing;

namespace Craftplacer.Windows.VisualStyles
{
    /// <summary>
    /// Contains numeric data for a nine-patch and properties for calculating these values.
    /// </summary>
    public struct NinePatchGeometry
    {
        public Padding Padding;
        public Rectangle Rectangle;

        public int[] Horizontals => new int[]
        {
            Rectangle.X,
            Rectangle.Left + Padding.Left,
            Rectangle.Right - Padding.Right,
            Rectangle.Right
        };

        public int[] Verticals => new int[]
        {
            Rectangle.Y,
            Rectangle.Top + Padding.Top,
            Rectangle.Bottom - Padding.Bottom,
            Rectangle.Bottom
        };

        public NinePatchGeometry(Padding padding, Rectangle rectangle)
        {
            Padding = padding;
            Rectangle = rectangle;
        }

        public Rectangle[] GetRectangles()
        {
            var rectangles = new Rectangle[9];
            var h = Horizontals;
            var v = Verticals;
            var i = 0;
            for (int y = 0; y < 3; y++)
            {
                for (int x = 0; x < 3; x++)
                {
                    var left = Math.Max(0, h[x]);
                    var top = Math.Max(0, v[y]);
                    var right = Math.Max(0, h[x + 1]);
                    var bottom = Math.Max(0, v[y + 1]);

                    rectangles[i] = Rectangle.FromLTRB(left, top, right, bottom);
                    i++;
                }
            }

            return rectangles;
        }
    }
}
