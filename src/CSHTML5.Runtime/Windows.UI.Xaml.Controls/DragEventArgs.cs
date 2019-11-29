
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



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
    public sealed class DragEventArgs : ExtendedRoutedEventArgs
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
        /// Gets or sets a value that indicates the present state of the event handling
        /// for a routed event as it travels the route.
        /// </summary>
        public bool Handled { get; set; }

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

#if unsupported
        /// <summary>
        /// Gets a member of the System.Windows.DragDropEffects enumeration that specifies
        /// which operations are allowed by the originator of the drag event.
        /// </summary>
        public DragDropEffects AllowedEffects { get; internal set; }
#endif
    }
}
