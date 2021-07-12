using Craftplacer.Windows.VisualStyles.Enums;

using System;
using System.Drawing;

using Vanara.PInvoke;

using static Vanara.PInvoke.ComCtl32;
using static Vanara.PInvoke.Gdi32;

namespace Craftplacer.Windows.VisualStyles
{
    public static class Extensions
    {
        public static int Get0BGR(Color rgbColor)
        {
            // Return a zero-alpha 24-bit BGR color integer
            return (0 << 24) + (rgbColor.B << 16) + (rgbColor.G << 8) + rgbColor.R;
        }

        public static void DrawString(this Graphics graphics, Element element, string s, int x, int y, int w, int h)
        {
            if (element.TextShadowType == TextShadowType.Continuous)
            {
                var textColor = (uint)Get0BGR(element.TextColor.Value);
                var shadowColor = (uint)Get0BGR(element.TextShadowColor.Value);

                var rect = new RECT(x, y, x + w, y + h);
                var hdc = graphics.GetHdc();

                SelectObject(hdc, element.Font.ToHfont());
                SetTextColor(hdc, textColor);
                DrawShadowText(hdc, s, (uint)s.Length, rect, 0, textColor, shadowColor, element.TextShadowOffset.X, element.TextShadowOffset.Y);
                graphics.ReleaseHdc(hdc);
            }
            else
            {
                using var textBrush = new SolidBrush(element.TextColor.Value);
                graphics.DrawString(s, element.Font, textBrush, x, y);
            }
        }

        public static void DrawElement(this Graphics graphics, Element element, Rectangle bounds, int? imageIndex = null)
        {
            var bitmap = element.Image;

            var srcRect = new Rectangle(Point.Empty, bitmap.Size);

            if (imageIndex.HasValue)
            {
                switch (element.ImageLayout)
                {
                    case Orientation.Horizontal:
                        throw new NotImplementedException();

                    case Orientation.Vertical:
                        var actualHeight = bitmap.Height / element.ImageCount;
                        var top = actualHeight * imageIndex.Value;
                        srcRect = new Rectangle(0, top, bitmap.Width, actualHeight);
                        break;
                }
            }

            var geometry = new NinePatchGeometry(element.SizingMargins.Value, srcRect);
            graphics.DrawNinePatch(bitmap, geometry, bounds, element.SizingType == SizingType.Tile);
        }
    }
}
