

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
using Microsoft.Windows;
using System.Windows;
#else
using Windows.UI.Xaml.Input;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Input;
using System.Windows;
using Windows.UI.Xaml.Controls;
#endif

#if MIGRATION
namespace Microsoft.Windows
#else
namespace System.Windows
#endif
{
    /// <summary>
    /// Provides data for drag-and-drop events in Silverlight.
    /// </summary>
    public sealed partial class DragEventArgs : ExtendedRoutedEventArgs
    {
#if MIGRATION
        internal MouseEventArgs MouseEventArgs { get; set; }
#else
        internal PointerRoutedEventArgs PointerRoutedEventArgs { get; set; }
#endif

        /// <summary>
        /// Initializes a new instance of the DragEventArgs class.
        /// </summary>
        internal DragEventArgs(IDataObject data)
        {
            this.Data = data;
        }

        /// <summary>
        /// Initializes a new instance of the DragEventArgs class.
        /// </summary>
#if MIGRATION
        internal DragEventArgs(IDataObject data, MouseEventArgs e) //todo: check whether this should be "MouseButtonEventArgs"
#else
        internal DragEventArgs(IDataObject data, PointerRoutedEventArgs e)
#endif
        {
            this.Data = data;
#if MIGRATION
            this.MouseEventArgs = e;
#else
            this.PointerRoutedEventArgs = e;
#endif
        }

        /// <summary>
        /// Gets or sets the target drop-and-drop operation.
        /// </summary>
        public DragDropEffects Effects { get; set; }

        /// <summary>
        /// Gets a data object (implements IDataObject) that contains
        /// the data associated with the corresponding drag event. This value is not
        /// useful in all event cases; see Remarks.
        /// </summary>
        public IDataObject Data { get; private set; }

        /// <summary>
        /// Returns a drop point that is relative to a specified UIElement
        /// </summary>
        /// <param name="relativeTo">The UIElement for which to get a relative drop point.</param>
        /// <returns> A drop point that is relative to the element specified in relativeTo.</returns>
#if MIGRATION
        public Point GetPosition(UIElement relativeTo)
        {
            return MouseEventArgs.GetPosition(relativeTo);
        }
#else
        public PointerPoint GetPosition(UIElement relativeTo)
        {
            return PointerRoutedEventArgs.GetCurrentPoint(relativeTo);
        }
#endif

        /// <summary>
        /// Gets a member of the System.Windows.DragDropEffects enumeration that specifies
        /// which operations are allowed by the originator of the drag event.
        /// </summary>
        public DragDropEffects AllowedEffects { get; internal set; }
    }
}
