using System.Collections.Generic;

namespace Craftplacer.Windows.VisualStyles.Ini
{
    public record IniSection
    {
        public string Name { get; }
        public Dictionary<string, string> Values { get; }

        public string this[string key] => Values[key];

        public IniSection(string name, Dictionary<string, string> values)
        {
            Name = name;
            Values = values;
        }
    }
}