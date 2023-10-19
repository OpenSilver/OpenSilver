
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

namespace System.Windows;

internal sealed class ReadOnlyPropertyMetadata : PropertyMetadata
{
    public ReadOnlyPropertyMetadata(object defaultValue, GetReadOnlyValueCallback getValueCallback)
        : base(defaultValue)
    {
        GetReadOnlyValueCallback = getValueCallback;
    }

    public ReadOnlyPropertyMetadata(
        object defaultValue,
        GetReadOnlyValueCallback getValueCallback,
        PropertyChangedCallback propertyChangedCallback)
        : base(defaultValue, propertyChangedCallback)
    {
        GetReadOnlyValueCallback = getValueCallback;
    }

    internal override GetReadOnlyValueCallback GetReadOnlyValueCallback { get; }
}
