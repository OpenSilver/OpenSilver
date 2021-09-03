using System;

namespace System.Windows.Markup
{
    public partial interface IXamlTypeResolver
    {
        Type Resolve(string @qualifiedTypeName);
    }
}
