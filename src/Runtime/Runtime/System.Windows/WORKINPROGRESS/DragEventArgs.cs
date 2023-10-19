
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

namespace System.Windows
{
    //
    // Summary:
    //     Provides data for drag-and-drop events in Silverlight.
	[OpenSilver.NotImplemented]
    public sealed partial class DragEventArgs : RoutedEventArgs
    {
        //
        // Summary:
        //     Gets a data object (implements System.Windows.IDataObject) that contains the
        //     data associated with the corresponding drag event. This value is not useful in
        //     all event cases; see Remarks.
        //
        // Returns:
        //     The data object that contains the data that is associated with the corresponding
        //     drag event.
		[OpenSilver.NotImplemented]
        public IDataObject Data { get; private set; }
        //
        // Summary:
        //     Gets or sets a value that indicates the present state of the event handling for
        //     a routed event as it travels the route.
        //
        // Returns:
        //     true if the event is marked handled; otherwise, false. The default value is false.
		[OpenSilver.NotImplemented]
        public bool Handled { get; set; }

        //
        // Summary:
        //     Returns a drop point that is relative to a specified System.Windows.UIElement.
        //
        // Parameters:
        //   relativeTo:
        //     The System.Windows.UIElement for which to get a relative drop point.
        //
        // Returns:
        //     A drop point that is relative to the element specified in relativeTo.
		[OpenSilver.NotImplemented]
        public Point GetPosition(UIElement relativeTo)
        {
            return new Point();
        }
    }
}
