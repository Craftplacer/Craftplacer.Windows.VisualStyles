using System.Collections.Generic;
using System.Diagnostics;

namespace Craftplacer.Windows.VisualStyles.Ini
{
    [DebuggerDisplay("{Name} ({Values.Count} items)")]
    public class IniSection
    {
        public IniSection(string name, Dictionary<string, string> values)
        {
            Name = name;
            Values = values;
        }

        public string Name { get; }
        public Dictionary<string, string> Values { get; }

        public string this[string key] => Helpers.CaseInsensitiveGet(Values, key);
    }
}