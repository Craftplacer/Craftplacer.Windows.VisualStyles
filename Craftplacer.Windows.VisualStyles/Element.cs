using Craftplacer.Windows.VisualStyles.Attributes;
using Craftplacer.Windows.VisualStyles.Enums;

using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Craftplacer.Windows.VisualStyles
{
    public class Element : IDisposable
    {
        private Font _font;
        private Bitmap _image;

        internal Element(ColorScheme colorScheme)
        {
            ColorScheme = colorScheme;
        }

        [IniProperty]
        public Color? AccentColorHint { get; set; }

        [IniProperty("BgType", DefaultValue = "None")]
        public BackgroundType BackgroundType { get; set; }

        [IniProperty]
        public Color? BorderColorHint { get; set; }

        public ColorScheme ColorScheme { get; }

        [IniProperty]
        public Padding? ContentMargins { get; set; }

        [IniProperty]
        public Color? FillColorHint { get; set; }

        public Font Font
        {
            get
            {
                if (_font == null)
                {
                    var split = FontName.Split(", ", StringSplitOptions.RemoveEmptyEntries);

                    var fontFamily = split[0];
                    var fontSize = float.Parse(split[1]);
                    var fontStyle = FontStyle.Regular;

                    if (split.Length >= 3)
                    {
                        fontStyle = Enum.Parse<FontStyle>(split[2], true);
                    }

                    _font = new Font(fontFamily, fontSize, fontStyle, GraphicsUnit.Point);
                }
                return _font;
            }
        }

        [IniProperty("Font")]
        public string FontName { get; set; }

        public Bitmap Image
        {
            get
            {
                if (_image == null)
                {
                    var resourceName = VisualStyle.GetResourceName(ImageFile);
                    var bitmap = ColorScheme.VisualStyle.LoadBitmap(resourceName);

                    if (Transparent)
                    {
                        if (Bitmap.GetPixelFormatSize(bitmap.PixelFormat) == 32)
                        {
                            bitmap = Helpers.ChangePixelFormat(bitmap, PixelFormat.Format32bppArgb);
                        }
                        else
                        {
                            bitmap = bitmap.RemoveTransparencyKey(TransparentColor);
                        }
                    }

                    _image = bitmap;
                }

                return _image;
            }
            private set => _image = value;
        }

        [IniProperty(DefaultValue = "1")]
        public int ImageCount { get; set; }

        [IniProperty]
        public string ImageFile { get; set; }

        [IniProperty(DefaultValue = "Horizontal")]
        public Orientation ImageLayout { get; set; }

        [IniProperty]
        public Point Offset { get; set; }

        [IniProperty]
        public OffsetType OffsetType { get; set; }

        [IniProperty]
        public Padding? SizingMargins { get; set; }

        public SizingType SizingType { get; set; }

        [IniProperty]
        public Color? TextColor { get; set; }

        [IniProperty]
        public Color? TextShadowColor { get; set; }

        [IniProperty]
        public Point TextShadowOffset { get; set; }

        [IniProperty]
        public TextShadowType TextShadowType { get; set; }

        [IniProperty]
        public bool Transparent { get; set; }

        [IniProperty]
        public Color? TransparentColor { get; set; }

        public void Dispose()
        {
            if (_image != null)
            {
                _image.Dispose();
                _image = null;
            }

            if (_font != null)
            {
                _font.Dispose();
                _font = null;
            }

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Gets an array of sliced bitmaps.
        /// </summary>
        public Bitmap[] GetBitmaps()
        {
            Bitmap bitmap = (Bitmap)Image.Clone();
            Bitmap[] bitmaps = bitmap.SliceBitmaps(ImageCount, ImageLayout);
            bitmap?.Dispose();
            return bitmaps;
        }
    }
}