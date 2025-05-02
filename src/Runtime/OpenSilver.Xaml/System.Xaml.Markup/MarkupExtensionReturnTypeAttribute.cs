
//
// Copyright (C) 2010 Novell Inc. http://novell.com
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

namespace System.Windows.Markup;

/// <summary>
/// Reports the type that a markup extension can return.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class MarkupExtensionReturnTypeAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MarkupExtensionReturnTypeAttribute"/> class.
    /// </summary>
    public MarkupExtensionReturnTypeAttribute()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MarkupExtensionReturnTypeAttribute"/> class using the 
    /// provided <see cref="Type"/>.
    /// </summary>
    /// <param name="returnType">
    /// The return type that this attribute reports.
    /// </param>
    public MarkupExtensionReturnTypeAttribute(Type returnType)
    {
        ReturnType = returnType;
    }

    /// <summary>
    /// Deprecated; do not use.
    /// </summary>
    /// <param name="returnType">
    /// The return type that this .NET attribute reports.
    /// </param>
    /// <param name="expressionType">
    /// Deprecated; do not use.
    /// </param>
    [Obsolete("Unused. Use MarkupExtensionReturnTypeAttribute(Type) or XamlSetMarkupExtensionAttribute.")]
    public MarkupExtensionReturnTypeAttribute(Type returnType, Type expressionType)
            : this(returnType)
    {
        ExpressionType = expressionType;
    }

    /// <summary>
    /// Gets the <see cref="MarkupExtension"/> return type that this .NET attribute reports.
    /// </summary>
    /// <returns>
    /// The type-safe return type of the specific <see cref="MarkupExtension.ProvideValue(IServiceProvider)"/>
    /// implementation of the markup extension where the <see cref="MarkupExtensionReturnTypeAttribute"/> .NET 
    /// attribute is applied.
    /// </returns>
    public Type ReturnType { get; private set; }

    /// <summary>
    /// Deprecated; do not use.
    /// </summary>
    /// <returns>
    /// Deprecated; do not use.
    /// </returns>
    [ObsoleteAttribute("Unused. Use XamlSetMarkupExtensionAttribute functionality instead.")]
    public Type ExpressionType { get; private set; }
}
