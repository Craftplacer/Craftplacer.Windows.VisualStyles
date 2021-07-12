using Craftplacer.Windows.VisualStyles.Attributes;
using Craftplacer.Windows.VisualStyles.Enums;

using System;
using System.Drawing;

namespace Craftplacer.Windows.VisualStyles
{
    public class Element : IDisposable
    {
        private Bitmap _image;

        private Font font;

        internal Element(ColorScheme colorScheme)
        {
            ColorScheme = colorScheme;
        }

        [IniProperty]
        public Color? AccentColorHint { get; init; }

        [IniProperty("BgType", DefaultValue = "None")]
        public BackgroundType BackgroundType { get; init; }

        [IniProperty]
        public Color? BorderColorHint { get; init; }

        public ColorScheme ColorScheme { get; }

        [IniProperty]
        public Padding? ContentMargins { get; init; }

        [IniProperty]
        public Color? FillColorHint { get; init; }

        [IniProperty("Font")]
        public string FontName { get; init; }

        public Font Font
        {
            get
            {
                if (font == null)
                {
                    var split = FontName.Split(", ", StringSplitOptions.RemoveEmptyEntries);
                    
                    var fontFamily = split[0];
                    var fontSize = float.Parse(split[1]);
                    var fontStyle = FontStyle.Regular;

                    if (split.Length >= 3)
                    {
                        fontStyle = Enum.Parse<FontStyle>(split[2], true);
                    }


                    font = new Font(fontFamily, fontSize, fontStyle, GraphicsUnit.Point);
                }
                return font;
            }
        }

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
                        if (TransparentColor.HasValue)
                        {
                            bitmap.MakeTransparent(TransparentColor.Value);
                        }
                        else
                        {
                            bitmap.MakeTransparent();
                        }
                    }

                    _image = bitmap;
                }

                return _image;
            }
        }

        [IniProperty(DefaultValue = "1")]
        public int ImageCount { get; init; }

        [IniProperty]
        public string ImageFile { get; init; }

        [IniProperty(DefaultValue = "Horizontal")]
        public Orientation ImageLayout { get; init; }

        [IniProperty]
        public Point Offset { get; init; }

        [IniProperty]
        public OffsetType OffsetType { get; init; }

        [IniProperty]
        public Padding? SizingMargins { get; init; }

        [IniProperty(DefaultValue = "Stretch")]
        public SizingType SizingType { get; init; }

        [IniProperty]
        public Color? TextColor { get; init; }

        [IniProperty]
        public Color? TextShadowColor { get; init; }

        [IniProperty]
        public Point TextShadowOffset { get; init; }

        [IniProperty(DefaultValue = "Single")]
        public TextShadowType TextShadowType { get; init; }

        [IniProperty(DefaultValue = "False")]
        public bool Transparent { get; init; }

        [IniProperty]
        public Color? TransparentColor { get; init; }

        public void Dispose()
        {
            Image?.Dispose();
        }

        public Bitmap[] GetBitmaps()
        {
            var fullBitmap = (Bitmap)Image.Clone();

            if (Transparent)
            {
                if (TransparentColor.HasValue)
                {
                    fullBitmap.MakeTransparent(TransparentColor.Value);
                }
                else
                {
                    fullBitmap.MakeTransparent();
                }
            }

            var bitmapArray = new Bitmap[ImageCount];
            var actualHeight = fullBitmap.Height / ImageCount;

            for (int i = 0; i < ImageCount; i++)
            {
                switch (ImageLayout)
                {
                    case Orientation.Horizontal:
                        throw new NotImplementedException();

                    case Orientation.Vertical:
                        var top = actualHeight * i;
                        var rect = new Rectangle(0, top, fullBitmap.Width, actualHeight);
                        bitmapArray[i] = fullBitmap.Clone(rect, fullBitmap.PixelFormat);
                        break;
                }
            }

            fullBitmap?.Dispose();

            return bitmapArray;
        }

    }
}