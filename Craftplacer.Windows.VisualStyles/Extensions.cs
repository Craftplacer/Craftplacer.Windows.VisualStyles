using System.Drawing;

using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Gdi;

namespace Craftplacer.Windows.VisualStyles
{
    public static class Extensions
    {
        public static void DrawElement(this Graphics graphics, Element element, Rectangle bounds, int? imageIndex = null)
        {
            Bitmap bitmap;

            if (imageIndex.HasValue)
            {
                bitmap = element.GetBitmaps()[imageIndex.Value];
            }
            else
            {
                bitmap = element.Image;
            }

            Rectangle rect = new Rectangle(Point.Empty, bitmap.Size);
            NinePatchGeometry geometry = new NinePatchGeometry(element.SizingMargins.Value, rect);
            graphics.DrawNinePatch(bitmap, geometry, bounds, element.SizingType == SizingType.Tile);
        }

        public static void DrawString(this Graphics graphics, Element element, string s, int x, int y, int w, int h)
        {
            if (element.TextShadowType == TextShadowType.Continuous)
            {
                uint textColor = (uint)Get0BGR(element.TextColor.Value);
                uint shadowColor = (uint)Get0BGR(element.TextShadowColor.Value);

                HDC hdc = new HDC(graphics.GetHdc());
                HGDIOBJ hFont = new HGDIOBJ(element.Font.ToHfont());

                try
                {
                    PInvoke.SelectObject(hdc, hFont);
                    PInvoke.SetTextColor(hdc, textColor);

                    RECT rect = new RECT
                    {
                        left = x,
                        top = y,
                        right = x + w,
                        bottom = y + h
                    };

                    unsafe
                    {
                        fixed (char* pszText = s)
                        {
                            PInvoke.DrawShadowText(
                                hdc,
                                pszText,
                                (uint)s.Length,
                                &rect,
                                0,
                                textColor,
                                shadowColor,
                                element.TextShadowOffset.X,
                                element.TextShadowOffset.Y);
                        }
                    }
                }
                finally
                {
                    PInvoke.DeleteObject(hFont);
                    graphics.ReleaseHdc(hdc);
                }
            }
            else
            {
                using var textBrush = new SolidBrush(element.TextColor.Value);
                graphics.DrawString(s, element.Font, textBrush, x, y);
            }
        }

        /// <summary>
        /// Returns a zero-alpha 24-bit BGR color integer
        /// </summary>
        public static int Get0BGR(Color color) => (0 << 24) + (color.B << 16) + (color.G << 8) + color.R;
    }
}