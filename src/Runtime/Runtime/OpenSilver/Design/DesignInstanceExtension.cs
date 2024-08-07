

/*===================================================================================
* 
*   Copyright (c) Userware/OpenSilver.net
*      
*   This file is part of the OpenSilver Runtime (https://opensilver.net), which is
*   licensed under the MIT license: https://opensource.org/licenses/MIT
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/



using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Markup;

namespace OpenSilver.Design;

/// <summary>
/// Create design-time instances of a specified type.
/// </summary>
[ContentProperty(nameof(Type))]
public class DesignInstanceExtension : MarkupExtension
{
    private const int ListCount = 3;

    /// <summary>
    /// Gets or sets the type of instances to create.
    /// </summary>
    public Type Type { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the design tool should actually
    /// create an instance of the class, which implies that the class has a public default constructor.
    /// Default is <see langword="true"/>.
    /// </summary>
    public bool IsDesignTimeCreatable { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to create a collection of instances.
    /// </summary>
    public bool CreateList { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DesignInstanceExtension"/> class.
    /// </summary>
    public DesignInstanceExtension()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DesignInstanceExtension"/> class with the specified type.
    /// </summary>
    /// <param name="type">The type of the instance to create.</param>
    public DesignInstanceExtension(Type type)
    {
        Type = type;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        if (Type == null)
        {
            throw new InvalidOperationException("Type must be set for DesignInstanceExtension.");
        }

        if (!IsDesignTimeCreatable)
        {
            // todo: parse the class for its bindable properties, for example to display empty DataGrid but with columns
            // https://learn.microsoft.com/en-us/windows/uwp/data-binding/displaying-data-in-the-designer#sample-data-from-class-and-design-time-attributes
            return null;
        }

        try
        {
            if (CreateList)
            {
                var listType = typeof(List<>).MakeGenericType(Type);
                var list = (IList)Activator.CreateInstance(listType);

                for (int i = 0; i < ListCount; i++)
                {
                    var item = Activator.CreateInstance(Type);
                    list.Add(item);
                }

                return list;
            }

            var instance = Activator.CreateInstance(Type);
            return instance;
        }
        catch (Exception ex)
        {
            // Handle potential exceptions, such as missing parameterless constructors
            Debug.WriteLine($"DesignInstanceExtension error: {ex.Message}");
            return null;
        }
    }
}
