
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
using System.ComponentModel;
using System.Diagnostics;

#if MIGRATION
using System.Windows;
using System.Windows.Markup;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;
#endif

namespace OpenSilver.Internal.Xaml
{
    /// <summary>
    /// DO NOT USE THIS CLASS IN YOUR CODE. 
    /// This class is publicly exposed because we need to give access to some 
    /// internal features of OpenSilver when converting xaml files to C#.
    /// We reserve ourselves the right to change this class or delete it in
    /// later releases.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class RuntimeHelpers
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static T GetPropertyValue<T>(Type xamlType, string propertyName, string xamlValue, Func<T> fallbackValueFunc)
        {
            ReflectedPropertyData property = TypeConverterHelper.GetProperties(xamlType)[propertyName];
            if (property == null)
            {
                throw new InvalidOperationException(
                    $"Property '{propertyName}' was not found in type '{xamlType}'."
                );
            }

            Debug.Assert(property.PropertyType == typeof(T));

            // Note: here we do not want to use the Converter property as it would return a non-null
            // converter for several known types for which object instantiation can be optimized at
            // compilation and we would waste time parsing strings.
            TypeConverter converter = property.InternalConverter;
            if (converter != null)
            {
                return (T)converter.ConvertFromInvariantString(xamlValue);
            }

            if (fallbackValueFunc != null)
            {
                return fallbackValueFunc();
            }

            throw new InvalidOperationException(
                $"Failed to create a '{typeof(T)}' from the text '{xamlValue}'."
            );
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static object GetPropertyValue(Type xamlType, string propertyName, string xamlValue, Func<object> fallbackValueFunc)
        {
            ReflectedPropertyData property = TypeConverterHelper.GetProperties(xamlType)[propertyName];
            if (property == null)
            {
                throw new InvalidOperationException(
                    $"Property '{propertyName}' was not found in type '{xamlType}'."
                );
            }

            // Note: here we do not want to use the Converter property as it would return a non-null
            // converter for several known types for which object instantiation can be optimized at
            // compilation and we would waste time parsing strings.
            TypeConverter converter = property.InternalConverter;
            if (converter != null)
            {
                return converter.ConvertFromInvariantString(xamlValue);
            }

            if (fallbackValueFunc != null)
            {
                return fallbackValueFunc();
            }

            throw new InvalidOperationException(
                $"Failed to create a '{property.PropertyType}' from the text '{xamlValue}'."
            );
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void InitializeNameScope(DependencyObject dependencyObject)
        {
            NameScope.SetNameScope(dependencyObject, new NameScope());
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void RegisterName(DependencyObject dependencyObject, string name, object scopedElement)
        {
            INameScope nameScope = FrameworkElement.FindScope(dependencyObject);
            if (nameScope != null)
            {
                nameScope.RegisterName(name, scopedElement);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void SetTemplatedParent(FrameworkElement element, DependencyObject templatedParent)
        {
            element.TemplatedParent = templatedParent;
        }
    }
}
