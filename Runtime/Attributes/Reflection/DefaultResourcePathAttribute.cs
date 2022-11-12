using System;

namespace Baracuda.Utilities
{
    public class DefaultResourcePathAttribute : Attribute
    {
        public string Path { get; }

        public DefaultResourcePathAttribute(string path)
        {
            Path = path;
        }
    }
}
