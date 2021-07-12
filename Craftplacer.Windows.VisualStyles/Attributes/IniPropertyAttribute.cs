using System;

namespace Craftplacer.Windows.VisualStyles.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    sealed class IniPropertyAttribute : Attribute
    {
        public IniPropertyAttribute()
        {
        }

        public IniPropertyAttribute(string name)
        {
            Name = name;
        }

        public readonly string Name;

        public string DefaultValue { get; set; }
    }
}
