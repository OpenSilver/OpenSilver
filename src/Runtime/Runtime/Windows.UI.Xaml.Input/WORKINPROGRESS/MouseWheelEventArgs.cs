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
using System.Threading.Tasks;


#if MIGRATION
namespace System.Windows.Input
#else
namespace Windows.UI.Xaml.Input
#endif
{
#if WORKINPROGRESS
    [OpenSilver.NotImplemented]
#if MIGRATION
    public partial class MouseWheelEventArgs : MouseEventArgs
#else
    public partial class MouseWheelEventArgs
#endif
    {
        [OpenSilver.NotImplemented]
        public int Delta { get; private set; }
    }
#endif
}
