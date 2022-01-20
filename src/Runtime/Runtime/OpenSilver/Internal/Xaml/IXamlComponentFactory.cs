
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

using System.ComponentModel;

namespace OpenSilver.Internal.Xaml
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IXamlComponentFactory
    {
        object CreateComponent();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IXamlComponentFactory<T> : IXamlComponentFactory
    {
        new T CreateComponent();
    }
}
