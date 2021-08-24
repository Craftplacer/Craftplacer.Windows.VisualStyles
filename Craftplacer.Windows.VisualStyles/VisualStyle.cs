using Craftplacer.Windows.VisualStyles.Ini;

using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.LibraryLoader;

namespace Craftplacer.Windows.VisualStyles
{
    /// <summary>
    /// Represents a Windows visual style
    /// </summary>
    public class VisualStyle : IDisposable
    {
        private readonly Encoding _encoding = Encoding.Unicode;
        private readonly HINSTANCE hModule;
        private readonly IniFile themeIni;
        private bool disposedValue;

        public VisualStyle(string vsPath)
        {
            Path = vsPath;

            unsafe
            {
                fixed (char* lpLibFileName = Path)
                {
                    hModule = PInvoke.LoadLibraryEx(lpLibFileName, new HANDLE(), LOAD_LIBRARY_FLAGS.LOAD_LIBRARY_AS_DATAFILE);
                }
            }

            if (hModule.IsNull)
            {
                throw new ArgumentException("Couldn't load the specified visual style.", nameof(vsPath));
            }

            var buffer = LoadResource("THEMES_INI", "TEXTFILE");
            var text = _encoding.GetString(buffer);
            text = FilterComments(text);
            themeIni = IniParser.Parse(text);
            var docsSection = themeIni["documentation"];
            DisplayName = docsSection["DisplayName"];
            ToolTip = docsSection["ToolTip"];
            Author = docsSection["Author"];
            Company = docsSection["Company"];
            Copyright = docsSection["Copyright"];
            Description = docsSection["Description"];
            ThemeName = docsSection["ThemeName"];
            WmpSkinName = docsSection["WmpSkinName"];

            // Special parsing
            // LastUpdated = DateTime.Parse(docsSection["LastUpdated"]);
            // URL = new Uri(docsSection["URL"]);
        }

        ~VisualStyle()
        {
            Dispose(disposing: false);
        }

        public string Author { get; private set; }

        public string[] ColorNames
        {
            get
            {
                var buffer = LoadResource("#1", "COLORNAMES");
                var text = _encoding.GetString(buffer);
                var colorNames = text.Split('\0', StringSplitOptions.RemoveEmptyEntries);
                return colorNames;
            }
        }

        public string Company { get; private set; }

        public string Copyright { get; private set; }

        public string Description { get; private set; }

        public string DisplayName { get; private set; }

        public DateTime LastUpdated { get; private set; }

        public string Path { get; }

        public string[] SizeNames
        {
            get
            {
                var buffer = LoadResource("#1", "SIZENAMES");
                var text = _encoding.GetString(buffer);
                var colorNames = text.Split('\0', StringSplitOptions.RemoveEmptyEntries);
                return colorNames;
            }
        }

        public string ThemeName { get; private set; }

        public string ToolTip { get; private set; }

        public Uri URL { get; private set; }

        /// <summary>
        /// Name of the accompanying Windows Media Player skin.
        /// </summary>
        public string WmpSkinName { get; private set; }

        /// <summary>
        /// Converts a visual style file path into a resource name.
        /// </summary>
        /// <param name="filePath">A file path (like "Blue\button.bmp")</param>
        /// <returns>The translated resource name (like "BLUE_BUTTON_BMP")</returns>
        public static string GetResourceName(string filePath)
        {
            var invalidChars = System.IO.Path.GetInvalidFileNameChars().Append('.');
            var filteredFilePathChars = filePath.Select((c) => invalidChars.Contains(c) ? '_' : c);
            var filteredFilePath = new string(filteredFilePathChars.ToArray());
            return filteredFilePath.ToUpperInvariant();
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public (string DisplayName, string ToolTip) GetColorDisplay(string colorName)
        {
            var section = themeIni["ColorScheme." + colorName];
            return (section["DisplayName"], section["ToolTip"]);
        }

        public ColorScheme GetColorScheme(string iniFileName, string colorName, string sizeName)
        {
            var buffer = LoadResource(iniFileName, "TEXTFILE");
            var text = _encoding.GetString(buffer);

            text = FilterComments(text);

            var colorSchemeIni = IniParser.Parse(text);

            return new ColorScheme(this, colorSchemeIni, colorName, sizeName);
        }

        public ColorScheme GetColorScheme(string colorName, string sizeName)
        {
            var fileResName = GetIniName(colorName, sizeName);
            return GetColorScheme(fileResName, colorName, sizeName);
        }

        public ColorScheme[] GetColorSchemes()
        {
            var colorSchemes = new ColorScheme[ColorNames.Length * SizeNames.Length];
            var i = 0;
            foreach (var colorName in ColorNames)
            {
                foreach (var sizeName in SizeNames)
                {
                    colorSchemes[i] = GetColorScheme(colorName, sizeName);
                    i++;
                }
            }
            return colorSchemes;
        }

        public (string DisplayName, string ToolTip) GetSizeDisplay(string sizeName)
        {
            var section = themeIni["Size." + sizeName];
            return (section["DisplayName"], section["ToolTip"]);
        }

        public Bitmap LoadBitmap(string filePath)
        {
            var resourceName = GetResourceName(filePath);
            return Bitmap.FromResource(hModule.Value, resourceName);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                if (!PInvoke.FreeLibrary(hModule))
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }

                disposedValue = true;
            }
        }

        // HACK: fuck ini parsers not handling comments correctly
        private static string FilterComments(string value)
        {
            return Regex.Replace(value, @";(.*)", string.Empty);
        }

        private string GetIniName(string colorName, string sizeName)
        {
            var colorIndex = Array.IndexOf(ColorNames, colorName);
            var sizeIndex = Array.IndexOf(SizeNames, sizeName);
            var fileResIndex = sizeIndex + (colorIndex * SizeNames.Length);

            var buffer = LoadResource("#1", "FILERESNAMES");
            var text = Encoding.Unicode.GetString(buffer);
            var fileResNames = text.Split('\0', StringSplitOptions.RemoveEmptyEntries);

            return fileResNames[fileResIndex];
        }

        private byte[] LoadResource(string resourceName, string resourceType)
        {
            HRSRC hResInfo;

            unsafe
            {
                fixed (char* lpName = resourceName)
                fixed (char* lpType = resourceType)
                {
                    hResInfo = PInvoke.FindResource(hModule, lpName, lpType);
                }
            }

            var hResData = PInvoke.LoadResource(hModule, hResInfo);
            var resSize = PInvoke.SizeofResource(hModule, hResInfo);

            var buffer = new byte[resSize];
            Marshal.Copy((IntPtr)hResData.ToInt32(), buffer, 0, buffer.Length);

            return buffer;
        }
    }
}