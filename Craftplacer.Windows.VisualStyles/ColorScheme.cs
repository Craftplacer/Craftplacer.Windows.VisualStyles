using IniParser.Model;

using System;
using System.Collections.Generic;

using static Craftplacer.Windows.VisualStyles.Helpers;

namespace Craftplacer.Windows.VisualStyles
{
    public class ColorScheme
    {
        private readonly IniData ini;
        private Dictionary<string, Element> sectionCache = new();

        public VisualStyle VisualStyle { get; }

        internal ColorScheme(VisualStyle visualStyle, IniData ini)
        {
            VisualStyle = visualStyle;
            this.ini = ini ?? throw new ArgumentNullException(nameof(ini));
        }

        public Element this[string section]
        {
            get
            {
                section = section.ToLowerInvariant();

                if (!sectionCache.ContainsKey(section))
                {
                    sectionCache[section] = CreateElement(ini[section]);
                }

                return sectionCache[section];
            }
        }

        private Element CreateElement(KeyDataCollection data)
        {
            var element = new Element(this);
            DeserializeFromIni(element, data);
            return element;
        }
    }
}
