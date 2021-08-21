using Craftplacer.Windows.VisualStyles.Ini;

using System;
using System.Collections.Generic;

namespace Craftplacer.Windows.VisualStyles
{
    public class ColorScheme
    {
        private readonly IniFile ini;
        private readonly Dictionary<string, Element> sectionCache = new Dictionary<string, Element>();

        internal ColorScheme(VisualStyle visualStyle, IniFile ini)
        {
            VisualStyle = visualStyle;
            this.ini = ini ?? throw new ArgumentNullException(nameof(ini));
        }

        public VisualStyle VisualStyle { get; }

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
            IniSerializer.DeserializeFromIni(element, section.Values);
            return element;
        }
    }
}