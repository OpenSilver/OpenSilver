﻿

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
using System.Linq;
using System.Text;

#if MIGRATION
namespace System.Windows.Data
#else
namespace Windows.UI.Xaml.Data
#endif
{
    internal partial interface IPropertyPathNode
    {
        IPropertyPathNode Next {get; set;}
        object Value { get; }
        bool IsBroken { get; }
        //internal IType ValueType;
        void SetSource(object source);
        void SetValue(object value);
        void Listen(IPropertyPathNodeListener listener);
        void Unlisten(IPropertyPathNodeListener listener);
    }
}
