
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
using System.Collections.Generic;

#if MIGRATION
using System.Windows;
using System.Windows.Markup;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;
#endif

namespace OpenSilver.Internal
{
    internal class NameScope : INameScope
    {
        private Dictionary<string, object> _nameMap;

        public object FindName(string name)
        {
            if (_nameMap == null || name == null || name == string.Empty)
            {
                return null;
            }

            if (_nameMap.TryGetValue(name, out object obj))
            {
                return obj;
            }

            return null;
        }

        public void RegisterName(string name, object scopedElement)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (scopedElement == null)
            {
                throw new ArgumentNullException(nameof(scopedElement));
            }

            if (name == string.Empty)
            {
                throw new ArgumentException("Name cannot be an empty string.", nameof(name));
            }

            if (_nameMap == null)
            {
                _nameMap = new Dictionary<string, object>();
                _nameMap[name] = scopedElement;
            }
            else
            {
                if (_nameMap.TryGetValue(name, out object nameContext))
                {
                    if (scopedElement != nameContext)
                    {
                        throw new ArgumentException(
                            string.Format("Cannot register duplicate name '{0}' in this scope.", name)
                        );
                    }
                }
                else
                {
                    _nameMap[name] = scopedElement;
                }
            }
        }

        public void UnregisterName(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (name == string.Empty)
            {
                throw new ArgumentException("Name cannot be an empty string.", nameof(name));
            }

            if (_nameMap == null || !_nameMap.Remove(name))
            {
                throw new ArgumentException(string.Format("Name '{0}' was not found.", name), nameof(name));
            }
        }

        /// <summary>
        /// NameScope property. This is an attached property. 
        /// </summary>
        public static readonly DependencyProperty NameScopeProperty =
            DependencyProperty.RegisterAttached(
                "NameScope",
                typeof(INameScope),
                typeof(NameScope),
                null);

        /// <summary>
        /// Helper for setting NameScope property on a DependencyObject. 
        /// </summary>
        /// <param name="dependencyObject">Dependency Object  to set NameScope property on.</param>
        /// <param name="value">NameScope property value.</param>
        public static void SetNameScope(DependencyObject dependencyObject, INameScope value)
        {
            if (dependencyObject == null)
            {
                throw new ArgumentNullException(nameof(dependencyObject));
            }

            dependencyObject.SetValue(NameScopeProperty, value);
        }

        /// <summary>
        /// Helper for reading NameScope property from a DependencyObject.
        /// </summary>
        /// <param name="dependencyObject">DependencyObject to read NameScope property from.</param>
        /// <returns>NameScope property value.</returns>
        public static INameScope GetNameScope(DependencyObject dependencyObject)
        {
            if (dependencyObject == null)
            {
                throw new ArgumentNullException(nameof(dependencyObject));
            }

            return (INameScope)dependencyObject.GetValue(NameScopeProperty);
        }
    }
}
