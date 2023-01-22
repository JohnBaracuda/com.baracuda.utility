using System;

namespace Baracuda.Utilities
{
    public class DefaultResourceAttribute : Attribute
    {
        public string FileName { get; set; }
        public string Path { get; }

        public DefaultResourceAttribute(string path)
        {
            Path = path;
        }
    }
}
