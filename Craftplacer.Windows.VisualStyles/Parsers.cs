using System;
using System.Drawing;
using System.Linq;

namespace Craftplacer.Windows.VisualStyles
{
    internal class Parsers
    {
        private static readonly char[] separatorValues = new char[2] { ',', ' ' };

        internal static bool ParseBool(string value, bool @default)
        {
            if (bool.TryParse(value, out var result))
            {
                return result;
            }

            return @default;
        }

        internal static Color? ParseColor(string value)
        {
            if (value == null)
            {
                return null;
            }

            var values = value
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select((v) => byte.Parse(v))
                .ToArray();

            return Color.FromArgb(values[0], values[1], values[2]);
        }

        internal static int ParseInt(string value, int @default)
        {
            if (int.TryParse(value, out var result))
            {
                return result;
            }

            return @default;
        }

        internal static Point ParseOffset(string value)
        {
            if (value == null)
            {
                return Point.Empty;
            }

            var values = value.Split(separatorValues, StringSplitOptions.RemoveEmptyEntries);

            if (int.TryParse(values[0], out var x) && int.TryParse(values[1], out var y))
            {
                return new Point(x, y);
            }

            return Point.Empty;
        }

        internal static Padding? ParsePadding(string value)
        {
            if (value == null)
            {
                return null;
            }

            var values = value
                .Split(separatorValues, StringSplitOptions.RemoveEmptyEntries)
                .Select((v) => int.Parse(v))
                .ToArray();

            return new Padding(values[0], values[2], values[1], values[3]);
        }
    }
}