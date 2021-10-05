

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


#if !MIGRATION
using System;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    [OpenSilver.NotImplemented]
    public class FloatableWindow : ContentControl
    {
        [OpenSilver.NotImplemented]
        public bool? DialogResult { get; set; }

        [OpenSilver.NotImplemented]
        protected virtual void OnClosed(EventArgs e)
        {
        }

        [OpenSilver.NotImplemented]
        public Panel ParentLayoutRoot { get; set; }

        [OpenSilver.NotImplemented]
        public ResizeMode ResizeMode { get; set; }

        [OpenSilver.NotImplemented]
        public void ShowDialog()
        {
        }

        public event EventHandler Closed;

        [OpenSilver.NotImplemented]
        public void Close()
        {
        }

        [OpenSilver.NotImplemented]
        public object Title { get; set; }
    }
}
