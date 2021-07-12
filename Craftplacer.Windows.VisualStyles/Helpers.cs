using Craftplacer.Windows.VisualStyles.Attributes;

using IniParser.Model;

using System;
using System.Reflection;

namespace Craftplacer.Windows.VisualStyles
{
    internal static class Helpers
    {
        public static void DeserializeFromIni<T>(T obj, KeyDataCollection data)
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
                string value;

                if (data.ContainsKey(key))
                {
                    value = data[key];
                }
                else if (attribute.DefaultValue != null)
                {
                    value = attribute.DefaultValue;
                }
                else
                {
                    continue;
                }

                object propertyValue;

                if (property.PropertyType == typeof(bool) && bool.TryParse(value, out var result))
                {
                    propertyValue = result;
                }
                else if (property.PropertyType.IsEnum)
                {
                    propertyValue = Enum.Parse(property.PropertyType, value, true);
                }
                else if (property.PropertyType == typeof(string))
                {
                    propertyValue = value;
                }
                else
                {
                    continue;
                }

                property.SetValue(obj, propertyValue);
            }
        }
    }
}
