
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
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Represents a time picker popup that allows choosing time through 3 sliders: Hours, Minutes and seconds.
    /// </summary>
    [OpenSilver.NotImplemented]
    public class RangeTimePickerPopup : TimePickerPopup
    {
        /// <summary>
        /// Gets or sets the Style applied to the sliders in the RangeTimePickerPopup control.
        /// </summary>
        [OpenSilver.NotImplemented]
        public Style SliderStyle
        {
            get { return (Style)GetValue(SliderStyleProperty); }
            set { SetValue(SliderStyleProperty, value); }
        }

        public static readonly DependencyProperty SliderStyleProperty =
            DependencyProperty.Register("SliderStyle", typeof(Style), typeof(RangeTimePickerPopup), null);

        /// <summary>
        /// Gets or sets the Style applied to the buttons that represent hours, minutes and seconds.
        /// </summary>
        [OpenSilver.NotImplemented]
        public Style TimeButtonStyle
        {
            get { return (Style)GetValue(TimeButtonStyleProperty); }
            set { SetValue(TimeButtonStyleProperty, value); }
        }

        public static readonly DependencyProperty TimeButtonStyleProperty =
            DependencyProperty.Register("TimeButtonStyle", typeof(Style), typeof(RangeTimePickerPopup), null);
    }
}
