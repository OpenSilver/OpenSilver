

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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <exclude/>
    public partial class UIElementCollection : RangeObservableCollection<UIElement>
    {
        public UIElementCollection()
        { 
        
        }

#if WORKINPROGRESS
        [OpenSilver.NotImplemented]
        public object SyncRoot
        {
            get
            {
                return this;
            }
        }
#endif
    }
}
