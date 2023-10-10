
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

#if !MIGRATION
using Windows.Foundation;
#endif

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    /// <summary>
    ///  The SizeChangedinfo class is used as a parameter to OnSizeRenderChanged.
    /// </summary>
    internal sealed class SizeChangedInfo
    {
        /// <summary>
        ///     Initializes a new instance of the SizeChangedinfo class.
        /// </summary>
        /// <param name="element">
        ///     The element which size is changing.
        /// </param>
        /// <param name="previousSize">
        ///     The size of the object before update. New size is element.RenderSize
        /// </param>
        /// <param name="widthChanged">
        /// The flag indicating that width component of the size changed. Note that due to double math 
        /// effects, the it may be (previousSize.Width != newSize.Width) and widthChanged = true.
        /// This may happen in layout when sizes of objects are fluctuating because of a precision "jitter" of
        /// the input parameters, but the overall scene is considered to be "the same" so no visible changes 
        /// will be detected. Typically, the handler of SizeChangedEvent should check this bit to avoid 
        /// invalidation of layout if the dimension didn't change.
        /// </param>
        /// <param name="heightChanged">
        /// The flag indicating that height component of the size changed. Note that due to double math 
        /// effects, the it may be (previousSize.Height != newSize.Height) and heightChanged = true.
        /// This may happen in layout when sizes of objects are fluctuating because of a precision "jitter" of
        /// the input parameters, but the overall scene is considered to be "the same" so no visible changes 
        /// will be detected. Typically, the handler of SizeChangedEvent should check this bit to avoid 
        /// invalidation of layout if the dimension didn't change.
        /// </param>
        public SizeChangedInfo(UIElement element, Size previousSize, bool widthChanged, bool heightChanged)
        {
            Element = element;
            PreviousSize = previousSize;
            WidthChanged = widthChanged;
            HeightChanged = heightChanged;
        }

        /// <summary>
        /// Read-only access to the previous Size
        /// </summary>
        public Size PreviousSize { get; }

        /// <summary>
        /// Read-only access to the new Size
        /// </summary>
        public Size NewSize => Element.RenderSize;

        /// <summary>
        /// Read-only access to the flag indicating that Width component of the size changed.
        /// Note that due to double math 
        /// effects, the it may be (previousSize.Width != newSize.Width) and widthChanged = true.
        /// This may happen in layout when sizes of objects are fluctuating because of a precision "jitter" of
        /// the input parameters, but the overall scene is considered to be "the same" so no visible changes 
        /// will be detected. Typically, the handler of SizeChangedEvent should check this bit to avoid 
        /// invalidation of layout if the dimension didn't change.
        /// </summary>
        public bool WidthChanged { get; private set; }

        /// <summary>
        /// Read-only access to the flag indicating that Height component of the size changed.
        /// Note that due to double math 
        /// effects, the it may be (previousSize.Height != newSize.Height) and heightChanged = true.
        /// This may happen in layout when sizes of objects are fluctuating because of a precision "jitter" of
        /// the input parameters, but the overall scene is considered to be "the same" so no visible changes 
        /// will be detected. Typically, the handler of SizeChangedEvent should check this bit to avoid 
        /// invalidation of layout if the dimension didn't change.
        /// </summary>
        public bool HeightChanged { get; private set; }

        //this method is used by UIElement to "accumulate" several cosequitive layout updates
        //into the single args object cahced on UIElement. Since the SizeChanged is deferred event,
        //there could be several size changes before it will actually fire.
        internal void Update(bool widthChanged, bool heightChanged)
        {
            WidthChanged |= widthChanged;
            HeightChanged |= heightChanged;
        }

        internal UIElement Element { get; }

        internal SizeChangedInfo Next;
    }
}
