
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
using System.ComponentModel;
using OpenSilver.Internal;

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    [Obsolete(Helper.ObsoleteMemberMessage)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public delegate List<object> ValueToMultipleValuesHtmlConverter(DependencyObject d, object value);
}