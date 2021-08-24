using Craftplacer.Windows.VisualStyles.Enums;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;

namespace Craftplacer.Windows.VisualStyles
{
    internal static class Helpers
    {
        public static T CaseInsensitiveGet<T>(this Dictionary<string, T> dictionary, string key)
        {
            return dictionary.FirstOrDefault(kv =>
            {
                return kv.Key.Equals(key, StringComparison.OrdinalIgnoreCase);
            }).Value;
        }

        public static Bitmap ChangePixelFormat(this Bitmap bitmap, PixelFormat pixelFormat)
        {
            Bitmap result = new Bitmap(bitmap.Width, bitmap.Height, pixelFormat);
            Rectangle bmpBounds = new Rectangle(Point.Empty, bitmap.Size);
            BitmapData srcData = bitmap.LockBits(bmpBounds, ImageLockMode.ReadOnly, bitmap.PixelFormat);
            BitmapData resData = result.LockBits(bmpBounds, ImageLockMode.WriteOnly, result.PixelFormat);

            long srcScan0 = srcData.Scan0.ToInt64();
            long resScan0 = resData.Scan0.ToInt64();
            int srcStride = srcData.Stride;
            int resStride = resData.Stride;
            int rowLength = Math.Abs(srcData.Stride);
            try
            {
                byte[] buffer = new byte[rowLength];
                for (int y = 0; y < srcData.Height; y++)
                {
                    IntPtr sourcePtr = new IntPtr(srcScan0 + (y * srcStride));
                    Marshal.Copy(sourcePtr, buffer, 0, rowLength);

                    IntPtr resPtr = new IntPtr(resScan0 + (y * resStride));
                    Marshal.Copy(buffer, 0, resPtr, rowLength);
                }
            }
            finally
            {
                bitmap.UnlockBits(srcData);
                result.UnlockBits(resData);
            }

            return result;
        }

        public static Bitmap RemoveTransparencyKey(this Bitmap bitmap, Color? transparencyKey = null)
        {
            if (!transparencyKey.HasValue)
            {
                transparencyKey = Color.Magenta;
            }

            var rect = new Rectangle(Point.Empty, bitmap.Size);
            bitmap = bitmap.Clone(rect, PixelFormat.Format32bppArgb);
            var bitmapData = bitmap.LockBits(rect, ImageLockMode.ReadWrite, bitmap.PixelFormat);
            var bitsPerPixel = Image.GetPixelFormatSize(bitmapData.PixelFormat);
            var size = bitmapData.Stride * bitmapData.Height;
            var data = new byte[size];

            Marshal.Copy(bitmapData.Scan0, data, 0, size);

            for (int i = 0; i < size; i += bitsPerPixel / 8)
            {
                // var magnitude = 1 / 3d * (data[i] + data[i + 1] + data[i + 2]);

                //data[i] is the first of 3 bytes of color
                if (transparencyKey.Value.B == data[i] &&
                    transparencyKey.Value.G == data[i + 1] &&
                    transparencyKey.Value.R == data[i + 2])
                {
                    data[i + 3] = 0;
                }
            }

            Marshal.Copy(data, 0, bitmapData.Scan0, data.Length);
            bitmap.UnlockBits(bitmapData);

            return bitmap;
        }

        public static Bitmap[] SliceBitmaps(this Bitmap bitmap, int bitmapCount, Orientation orientation)
        {
            Bitmap[] bitmaps = new Bitmap[bitmapCount];

            int z = orientation switch
            {
                Orientation.Horizontal => bitmap.Width / bitmapCount,
                Orientation.Vertical => bitmap.Height / bitmapCount,
                _ => throw new NotImplementedException(),
            };

            for (int i = 0; i < bitmapCount; i++)
            {
                var rect = orientation switch
                {
                    Orientation.Horizontal => new Rectangle(z, 0, z, bitmap.Height),
                    Orientation.Vertical => new Rectangle(0, z * i, bitmap.Width, z),
                    _ => throw new NotImplementedException(),
                };

                bitmaps[i] = bitmap.Clone(rect, bitmap.PixelFormat);
            }

            return bitmaps;
        }
    }
}