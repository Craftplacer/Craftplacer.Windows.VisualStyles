using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Craftplacer.Windows.VisualStyles.Ini
{
    internal static class IniParser
    {
        private static readonly Regex SectionRegex = new Regex(@"^\[(\w*)\]", RegexOptions.Compiled);

        public static IniFile Parse(string iniData) => new IniFile(ParseSections(iniData));

        private static IEnumerable<IniSection> ParseSections(string iniData)
        {
            var lines = iniData.Split(Environment.NewLine);

            string sectionName = null;
            Dictionary<string, string> values = new Dictionary<string, string>();

            bool finishSection(out IniSection section)
            {
                if (values.Count == 0)
                {
                    section = null;
                    return false;
                }
                else
                {
                    section = new IniSection(sectionName, values);
                    return true;
                }
            }

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                if (string.IsNullOrWhiteSpace(line) || line[0] == ';')
                {
                    continue;
                }

                var sectionMatch = SectionRegex.Match(line);
                if (sectionMatch != null)
                {
                    if (finishSection(out var finalizedSection))
                    {
                        yield return finalizedSection;
                    }

                    sectionName = sectionMatch.Captures.First().Value;
                    values = new Dictionary<string, string>();

                    continue;
                }

                var split = line.Split('=', 2);
                var key = split[0];
                var value = split[1].Split(';', 2).First();

                values[key] = value;
            }

            if (finishSection(out var endSection))
            {
                yield return endSection;
            }
        }
    }
}