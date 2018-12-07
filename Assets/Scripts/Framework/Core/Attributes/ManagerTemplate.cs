using System;

namespace Framework.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class ManagerTemplateAttribute : Attribute
    {
        public bool AutoRegister {get; set;}
        public ManagerTemplateAttribute()
        {
            AutoRegister = false;
        }
    }
}