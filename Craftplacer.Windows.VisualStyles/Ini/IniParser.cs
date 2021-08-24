using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Craftplacer.Windows.VisualStyles.Ini
{
    internal static class IniParser
    {
        private static readonly Regex SectionRegex = new Regex(@"^\[(.*)\]", RegexOptions.Compiled);

        public static IniFile Parse(string iniData) => new IniFile(ParseSections(iniData));

        private static IEnumerable<IniSection> ParseSections(string iniData)
        {
            var lines = iniData.Split(new char[] { '\r', '\n' });

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
                if (sectionMatch.Success)
                {
                    if (finishSection(out var finalizedSection))
                    {
                        yield return finalizedSection;
                    }

                    sectionName = sectionMatch.Groups[1].Value;
                    values = new Dictionary<string, string>();
                }
                else
                {
                    var split = line.Split('=', 2);
                    var key = split[0].Trim();
                    var value = split[1].Split(';', 2).First().Trim();

                    values[key] = value;
                }
            }

            if (finishSection(out var endSection))
            {
                yield return endSection;
            }
        }
    }
}