using System;
using System.Collections.Generic;
using System.Linq;

namespace Craftplacer.Windows.VisualStyles.Ini
{
    public class IniFile
    {
        public IEnumerable<IniSection> Sections;

        public IniFile(IEnumerable<IniSection> sections)
        {
            Sections = sections;
        }

        public IniSection this[string sectionName] => Sections.FirstOrDefault(section =>
        {
            return section.Name.Equals(sectionName, StringComparison.OrdinalIgnoreCase);
        });
    }
}