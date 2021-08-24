using Craftplacer.Windows.VisualStyles.Attributes;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;

namespace Craftplacer.Windows.VisualStyles.Ini
{
    public static class IniSerializer
    {
        public static void DeserializeFromIni<T>(T obj, Dictionary<string, string> data)
        {
            var properties = typeof(T).GetProperties();

            foreach (var property in properties)
            {
                var attribute = property.GetCustomAttribute<IniPropertyAttribute>();

                if (attribute == null)
                {
                    continue;
                }

                var key = attribute.Name ?? property.Name;
                string value = Helpers.CaseInsensitiveGet(data, key);

                if (value == null)
                {
                    if (attribute.DefaultValue != null)
                    {
                        value = attribute.DefaultValue;
                    }
                    else
                    {
                        continue;
                    }
                }

                dynamic propertyValue = ParseValue(value, property.PropertyType);

                if (propertyValue.Equals(null))
                {
                    continue;
                }

                property.SetValue(obj, propertyValue);
            }
        }

        private static Color ParseColor(string value)
        {
            var values = value
                .Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select((v) => byte.Parse(v))
                .ToArray();

            return Color.FromArgb(values[0], values[1], values[2]);
        }

        private static Padding ParsePadding(string value)
        {
            var values = value
                .Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select((v) => int.Parse(v))
                .ToArray();

            return new(values[0], values[2], values[1], values[3]);
        }

        private static Point ParsePoint(string value)
        {
            if (value == null)
            {
                return Point.Empty;
            }

            var values = value
               .Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (int.TryParse(values[0], out var x) && int.TryParse(values[1], out var y))
            {
                return new Point(x, y);
            }

            return Point.Empty;
        }

        private static dynamic ParseValue(string value, Type propertyType)
        {
            var nulledType = Nullable.GetUnderlyingType(propertyType);
            if (nulledType != null)
            {
                propertyType = nulledType;
            }

            if (propertyType == typeof(bool) && bool.TryParse(value, out var boolResult))
            {
                return boolResult;
            }

            if (propertyType.IsEnum)
            {
                return Enum.Parse(propertyType, value, true);
            }

            if (propertyType == typeof(string))
            {
                return value;
            }

            if (propertyType == typeof(Color))
            {
                return ParseColor(value);
            }

            if (propertyType == typeof(Padding))
            {
                return ParsePadding(value);
            }

            if (propertyType == typeof(Point))
            {
                return ParsePoint(value);
            }

            if (propertyType == typeof(int) && int.TryParse(value, out var intResult))
            {
                return intResult;
            }

            Debugger.Break();

            return null;
        }
    }
}