using System.Drawing;

namespace Craftplacer.Windows.VisualStyles
{
    public static class NinePatchExtensions
    {
        public static void DrawNinePatch(this Graphics graphics, Image image, NinePatchGeometry geometry, Rectangle destinationRectangle, bool tile = false)
        {
            var srcRects = geometry.GetRectangles();

            var dstGeometry = new NinePatchGeometry(geometry.Padding, destinationRectangle);
            var dstRects = dstGeometry.GetRectangles();

            for (int i = 0; i < 9; i++)
            {
                var srcRect = srcRects[i];
                var dstRect = dstRects[i];

                if (srcRect.Width == 0 || srcRect.Height == 0)
                {
                    continue;
                }

                graphics.DrawImage(image, dstRect, srcRect, GraphicsUnit.Pixel);
            }
        }
    }
}
