using System.Drawing;
using System.Drawing.Drawing2D;

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

                if (tile)
                {
                    Bitmap bitmap = (Bitmap)image;
                    Bitmap bitmapCrop = bitmap.Clone(srcRect, image.PixelFormat);
                    using (var textureBrush = new TextureBrush(bitmapCrop, WrapMode.Tile))
                    {
                        // We use transforms so the results are identical:
                        //
                        // If we would run this method without transforms
                        // on two different positions, the start of the
                        // bitmap would look different. This is because
                        // each paint function is acting like a stencil
                        // for a Brush (in this case TextureBrush).

                        graphics.TranslateTransform(dstRect.X, dstRect.Y);

                        Rectangle rect = new Rectangle(Point.Empty, dstRect.Size);
                        graphics.FillRectangle(textureBrush, rect);

                        graphics.ResetTransform();
                    }
                }
                else
                {
                    graphics.DrawImage(image, dstRect, srcRect, GraphicsUnit.Pixel);
                }
            }
        }
    }
}