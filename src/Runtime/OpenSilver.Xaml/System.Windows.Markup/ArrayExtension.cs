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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xaml.Markup;

namespace System.Windows.Markup;

/// <summary>
/// Implements x:Array support for .NET XAML Services.
/// </summary>
[MarkupExtensionReturnType(typeof(Array))]
[ContentProperty(nameof(Items))]
public class ArrayExtension : MarkupExtension
{
    private readonly List<object> items;

    /// <summary>
    /// Initializes a new instance of the <see cref="ArrayExtension"/> class. This creates an empty array.
    /// </summary>
    public ArrayExtension()
    {
        items = [];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ArrayExtension"/> class based on the provided raw array.
    /// </summary>
    /// <param name="elements">The array content that populates the created array.</param>
    /// <exception cref="ArgumentNullException"><paramref name="elements"/> is <see langword="null"/>.</exception>
    /// <remarks>This method supports markup extension behavior and is not typically
    /// called by user code, unless that user code implements XAML processing behavior.</remarks>
    public ArrayExtension(Array elements)
    {
        ArgumentNullException.ThrowIfNull(elements);

        Type = elements.GetType().GetElementType();
        items = [.. elements];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ArrayExtension"/> class and initializes the type of the array.
    /// </summary>
    /// <param name="arrayType">The object type of the new array.</param>
    /// <exception cref="ArgumentNullException"><paramref name="arrayType"/> is <see langword="null"/>.</exception>
    /// <remarks>This method supports markup extension behavior and is not typically
    /// called by user code, unless that user code implements XAML processing behavior.</remarks>
    public ArrayExtension(Type arrayType)
    {
        ArgumentNullException.ThrowIfNull(arrayType);

        Type = arrayType;
        items = [];
    }

    /// <summary>
    /// Gets or sets the type of array to be created when calling <see cref="ProvideValue(IServiceProvider)"/>.
    /// </summary>
    [ConstructorArgument("arrayType")]
    public Type Type { get; set; }

    /// <summary>
    /// Gets the contents of the array. Settable in XAML through XAML collection syntax.
    /// </summary>
    public IList Items => items;

    /// <summary>
    /// Appends the supplied object to the end of the array.
    /// </summary>
    /// <param name="value">The object to add to the end of the array.</param>
    public void AddChild(object value)
    {
        // null is allowed.
        Items.Add(value);
    }

    /// <summary>
    /// Adds a text node as a new array item.
    /// </summary>
    /// <param name="text">The text to add to the end of the array.</param>
    public void AddText(string text)
    {
        // null is allowed.
        Items.Add(text);
    }

    /// <summary>
    /// Returns an array that is sized to the number of objects supplied in the <see cref="Items"/> values.
    /// </summary>
    /// <param name="serviceProvider">An object that can provide services for the markup extension.</param>
    /// <returns>The created array, or null.</returns>
    /// <exception cref="InvalidOperationException">
    /// Processed an array that did not provide a valid <see cref="Type"/>.
    /// -or-
    /// There is a type mismatch between the declared <see cref="Type"/> of the array 
    /// and one or more of its <see cref="Items"/> values.
    /// </exception>
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        if (Type == null)
            throw new InvalidOperationException("Type property must be set before calling ProvideValue method");

        bool invalid = false;
        foreach (var item in Items)
        {
            if (item == null)
            {
                if (Type.GetTypeInfo().IsValueType)
                    invalid = true;
            }
            else if (!Type.GetTypeInfo().IsAssignableFrom(item.GetType().GetTypeInfo()))
                invalid = true;
            if (invalid)
                throw new InvalidOperationException(string.Format("Item in the array must be an instance of '{0}'", Type));
        }
        Array a = Array.CreateInstance(Type, Items.Count);
        Items.CopyTo(a, 0);
        return a;
    }
}
