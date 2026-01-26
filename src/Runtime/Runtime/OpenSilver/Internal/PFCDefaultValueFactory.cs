
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
using System.Windows;

namespace OpenSilver.Internal;

internal sealed class PFCDefaultValueFactory<T> : DefaultValueFactory
{
    private readonly Func<PresentationFrameworkCollection<T>> _getPrototypeDelegate;
    private readonly Func<DependencyObject, DependencyProperty, PresentationFrameworkCollection<T>> _createDefaultValueDelegate;

    public PFCDefaultValueFactory(
        Func<PresentationFrameworkCollection<T>> getPrototypeDelegate,
        Func<DependencyObject, DependencyProperty, PresentationFrameworkCollection<T>> createDefaultValueDelegate)
    {
        ArgumentNullException.ThrowIfNull(getPrototypeDelegate);
        ArgumentNullException.ThrowIfNull(createDefaultValueDelegate);

        _getPrototypeDelegate = getPrototypeDelegate;
        _createDefaultValueDelegate = createDefaultValueDelegate;
    }

    internal override object DefaultValue => _getPrototypeDelegate();

    internal override object CreateDefaultValue(DependencyObject owner, DependencyProperty property)
        => _createDefaultValueDelegate(owner, property);
}
