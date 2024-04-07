using System;

namespace Baracuda.Utilities.Reflection
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AddressablesGroupAttribute : Attribute
    {
        public bool CreateLabel { get; set; }
        public string GroupName { get; }

        public AddressablesGroupAttribute(string groupName)
        {
            GroupName = groupName;
        }
    }
}