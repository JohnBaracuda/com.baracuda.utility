using System;

namespace Baracuda.Utility.Reflection
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