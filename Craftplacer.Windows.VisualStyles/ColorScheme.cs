using Craftplacer.Windows.VisualStyles.Ini;

using System;
using System.Collections.Generic;
using System.Linq;
using static Craftplacer.Windows.VisualStyles.Helpers;

namespace Craftplacer.Windows.VisualStyles
{
    public class ColorScheme
    {
        private readonly IniFile ini;
        private Dictionary<string, Element> sectionCache = new();

        public VisualStyle VisualStyle { get; }

        internal ColorScheme(VisualStyle visualStyle, IniFile ini)
        {
            VisualStyle = visualStyle;
            this.ini = ini ?? throw new ArgumentNullException(nameof(ini));
        }

        public Element this[string sectionName]
        {
            get
            {
                sectionName = sectionName.ToLowerInvariant();

                if (!sectionCache.ContainsKey(sectionName))
                {
                    sectionCache[sectionName] = CreateElement(ini[sectionName]);
                }

                return sectionCache[sectionName];
            }
        }

        private Element CreateElement(IniSection section)
        {
            var element = new Element(this);
            DeserializeFromIni(element, section.Values);
            return element;
        }
    }
}
