using System;

namespace Craftplacer.Windows.VisualStyles.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    internal sealed class IniPropertyAttribute : Attribute
    {
        public readonly string Name;

        public IniPropertyAttribute()
        {
        }

        public IniPropertyAttribute(string name)
        {
            Name = name;
        }

        public string DefaultValue { get; set; }
    }
}