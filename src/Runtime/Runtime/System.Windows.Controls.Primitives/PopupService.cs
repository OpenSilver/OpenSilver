
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
using System.Windows.Input;
#else
using Windows.Foundation;
using MouseEventArgs = Windows.UI.Xaml.Input.PointerRoutedEventArgs;
using MouseEventHandler = Windows.UI.Xaml.Input.PointerEventHandler;
#endif

#if MIGRATION
namespace System.Windows.Controls.Primitives
#else
namespace Windows.UI.Xaml.Controls.Primitives
#endif
{
    internal static class PopupService
    {
        internal static FrameworkElement RootVisual { get; private set; }

        /// <summary>
        /// Place the ToolTip relative to this point 
        /// </summary>
        internal static Point MousePosition { get; set; }

        internal static PositionsWatcher PositionsWatcher { get; } = new();

        internal static void SetRootVisual()
        {
            if (RootVisual == null && Application.Current != null)
            {
                RootVisual = Application.Current.RootVisual as FrameworkElement;
                if (RootVisual != null)
                {
                    // keep caching mouse position because we can't query it from Silverlight 
#if MIGRATION
                    RootVisual.MouseMove += new MouseEventHandler(OnRootMouseMove);
#else
                    RootVisual.PointerMoved += new MouseEventHandler(OnRootMouseMove);
#endif
                }
            }
        }

        private static void OnRootMouseMove(object sender, MouseEventArgs e)
        {
#if MIGRATION
            MousePosition = e.GetPosition(null);
#else
            MousePosition = e.GetCurrentPoint(null).Position;
#endif
        }
    }
}
