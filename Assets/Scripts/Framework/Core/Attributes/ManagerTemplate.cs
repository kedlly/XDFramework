using System;

namespace Framework.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class ManagerTemplateAttribute : Attribute
    {
        public string Name {get; private set;}
        public bool AutoRegister {get; set;}
        public ManagerTemplateAttribute(string name)
        {
            Name = name;
            AutoRegister = false;
        }
    }
}