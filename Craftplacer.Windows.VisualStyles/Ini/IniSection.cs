using System.Collections.Generic;

namespace Craftplacer.Windows.VisualStyles.Ini
{
    public class IniSection
    {
        public IniSection(string name, Dictionary<string, string> values)
        {
            Name = name;
            Values = values;
        }

        public string Name { get; }
        public Dictionary<string, string> Values { get; }

        public string this[string key] => Values[key];
    }
}