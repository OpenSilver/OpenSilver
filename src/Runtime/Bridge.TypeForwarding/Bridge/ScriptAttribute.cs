using System;

namespace Bridge
{
    public sealed class ScriptAttribute : Attribute
    {
        public ScriptAttribute(params string[] lines)
        {
        }
    }
}