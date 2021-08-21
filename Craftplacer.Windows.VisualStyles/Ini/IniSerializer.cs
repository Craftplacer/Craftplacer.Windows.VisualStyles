using Craftplacer.Windows.VisualStyles.Attributes;

using System;
using System.Collections.Generic;
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

                dynamic propertyValue = ParseValue(value, property.PropertyType);

                if (propertyValue == null)
                {
                    continue;
                }

                property.SetValue(obj, propertyValue);
            }
        }

        private static dynamic ParseValue(string value, Type propertyType)
        {
            if (propertyType == typeof(bool) && bool.TryParse(value, out var result))
            {
                return result;
            }

            if (propertyType.IsEnum)
            {
                return Enum.Parse(propertyType, value, true);
            }

            if (propertyType == typeof(string))
            {
                return value;
            }

            return null;
        }
    }
}