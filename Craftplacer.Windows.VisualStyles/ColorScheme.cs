using Craftplacer.Windows.VisualStyles.Ini;

using System;
using System.Collections.Generic;

namespace Craftplacer.Windows.VisualStyles
{
    public class ColorScheme
    {
        private readonly IniFile _ini;
        private readonly Dictionary<string, Element> _sectionCache = new Dictionary<string, Element>();

        internal ColorScheme(VisualStyle visualStyle, IniFile ini, string colorName, string sizeName)
        {
            VisualStyle = visualStyle;
            _ini = ini ?? throw new ArgumentNullException(nameof(ini));
            ColorName = colorName;
            SizeName = sizeName;
        }

        public VisualStyle VisualStyle { get; }

        public string ColorName { get; }

        public string SizeName { get; }

        public Element this[string sectionName]
        {
            get
            {
                sectionName = sectionName.ToLowerInvariant();

                if (!_sectionCache.ContainsKey(sectionName))
                {
                    var section = _ini[sectionName];

                    if (section == null)
                    {
                        _sectionCache[sectionName] = null;
                    }
                    else
                    {
                        _sectionCache[sectionName] = CreateElement(section);
                    }
                }

                return _sectionCache[sectionName];
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