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

        public IniSection this[string sectionName]
        {
            get => Sections.First(section => section.Name == sectionName);
        }
    }
}