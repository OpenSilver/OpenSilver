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

#if MIGRATION
namespace System.Windows.Data
#else
namespace Windows.UI.Xaml.Data
#endif
{
    public abstract class BindingExpressionBase : Expression
    {
        internal BindingExpressionBase() { }

        [OpenSilver.NotImplemented]
        // Using Binding instead of BindingBase due to BindingOperations.SetBinding doing the same
        public Binding ParentBindingBase { get; }
    }
}
