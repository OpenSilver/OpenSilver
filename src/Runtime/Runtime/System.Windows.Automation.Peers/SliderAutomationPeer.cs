
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

using System.Windows.Controls;

namespace System.Windows.Automation.Peers
{
    /// <summary>
    /// Exposes <see cref="Slider"/> types to UI automation.
    /// </summary>
    public class SliderAutomationPeer : RangeBaseAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SliderAutomationPeer"/> class.
        /// </summary>
        /// <param name="owner">
        /// The <see cref="Slider"/> to associate with the <see cref="SliderAutomationPeer"/>.
        /// </param>
        public SliderAutomationPeer(Slider owner)
            : base(owner)
        {
        }

        /// <summary>
        /// Gets the control type for the element that is associated with this <see cref="SliderAutomationPeer"/>.
        /// This method is called by <see cref="AutomationPeer.GetAutomationControlType"/>.
        /// </summary>
        /// <returns>
        /// A value of the enumeration.
        /// </returns>
        protected override AutomationControlType GetAutomationControlTypeCore()
            => AutomationControlType.Slider;

        /// <summary>
        /// Gets the name of the class that is associated with this <see cref="SliderAutomationPeer"/>.
        /// This method is called by <see cref="AutomationPeer.GetClassName"/>.
        /// </summary>
        /// <returns>
        /// The class name.
        /// </returns>
        protected override string GetClassNameCore() => "Slider";

        /// <summary>
        /// Gets a clickable point on the pwner control.
        /// </summary>
        /// <returns>
        /// A <see cref="Point"/> structure containing a point on the owner control that
        /// is clickable.
        /// </returns>
        protected override Point GetClickablePointCore() => new Point(double.NaN, double.NaN);
    }
}
