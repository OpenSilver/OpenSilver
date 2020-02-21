using System;

namespace JSIL.Meta
{
    [AttributeUsage(AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface)]
    public class JSIgnoreAttribute : Attribute
    {
        public JSIgnoreAttribute()
        {
        }
    }
}