
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

namespace System.Windows
{
    [OpenSilver.NotImplemented]
    public sealed partial class WindowSettings : DependencyObject
    {
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty WindowStyleProperty = DependencyProperty.Register("WindowStyle", typeof(WindowStyle), typeof(WindowSettings), null);

        [OpenSilver.NotImplemented]
        public WindowStyle WindowStyle
        {
            get { return (WindowStyle)this.GetValue(WindowStyleProperty); }
            private set { this.SetValue(WindowStyleProperty, value); }
        }
    }
}
