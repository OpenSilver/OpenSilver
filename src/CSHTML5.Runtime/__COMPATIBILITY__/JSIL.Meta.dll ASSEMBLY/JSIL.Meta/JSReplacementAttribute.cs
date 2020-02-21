using System;

namespace JSIL.Meta
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property)]
    public class JSReplacementAttribute : Attribute
    {
        public JSReplacementAttribute(string expression)
        {
        }
    }
}