
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

#if MIGRATION
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
#else
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
#endif

#if MIGRATION
namespace System.Windows.Automation.Peers
#else
namespace Windows.UI.Xaml.Automation.Peers
#endif
{
    /// <summary>
    /// Exposes <see cref="ScrollBar"/> types to UI automation.
    /// </summary>
    public class ScrollBarAutomationPeer : RangeBaseAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScrollBarAutomationPeer"/> class.
        /// </summary>
        /// <param name="owner">
        /// The <see cref="ScrollBar"/> to associate with the <see cref="ScrollBarAutomationPeer"/>.
        /// </param>
        public ScrollBarAutomationPeer(ScrollBar owner)
            : base(owner)
        {
        }

        /// <summary>
        /// Gets the control type for the <see cref="ScrollBar"/> that
        /// is associated with this <see cref="ScrollBarAutomationPeer"/>.
        /// This method is called by <see cref="AutomationPeer.GetAutomationControlType"/>.
        /// </summary>
        /// <returns>
        /// A value of the enumeration.
        /// </returns>
        protected override AutomationControlType GetAutomationControlTypeCore()
            => AutomationControlType.ScrollBar;

        /// <summary>
        /// Gets the name of the class that is associated with this <see cref="ScrollBarAutomationPeer"/>.
        /// This method is called by <see cref="AutomationPeer.GetClassName"/>.
        /// </summary>
        /// <returns>
        /// The class name.
        /// </returns>
        protected override string GetClassNameCore() => "ScrollBar";

        /// <summary>
        /// Gets a clickable point for this control.
        /// </summary>
        /// <returns>
        /// A point that is clickable within the control, or a point with <see cref="double.NaN"/>
        /// coordinates, if no point is clickable within the control.
        /// </returns>
        protected override Point GetClickablePointCore() => new Point(double.NaN, double.NaN);

        /// <summary>
        /// Gets the orientation of the <see cref="ScrollBar"/> control associated with this peer.
        /// </summary>
        /// <returns>
        /// The orientation of the scroll bar, as a value of the enumeration.
        /// </returns>
        protected override AutomationOrientation GetOrientationCore()
            => ((ScrollBar)Owner).Orientation == Orientation.Horizontal ?
            AutomationOrientation.Horizontal :
            AutomationOrientation.Vertical;
    }
}
