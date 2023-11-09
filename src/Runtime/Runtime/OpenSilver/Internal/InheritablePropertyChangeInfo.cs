
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

using System.Windows;

namespace OpenSilver.Internal;

/// <summary>
/// This is the data that is passed through the DescendentsWalker
/// during an inheritable property change tree-walk.
/// </summary>
internal readonly struct InheritablePropertyChangeInfo
{
    internal InheritablePropertyChangeInfo(
        DependencyObject rootElement,
        DependencyProperty property,
        object oldValue,
        object newValue)
    {
        RootElement = rootElement;
        Property = property;
        OldValue = oldValue;
        NewValue = newValue;
    }

    internal DependencyObject RootElement { get; }

    internal DependencyProperty Property { get; }

    internal object OldValue { get; }

    internal object NewValue { get; }
}
