#if MIGRATION
namespace System.Windows.Controls
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
            DependencyProperty.Register("SliderStyle", typeof(Style), typeof(RangeTimePickerPopup), new PropertyMetadata());

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
            DependencyProperty.Register("TimeButtonStyle", typeof(Style), typeof(RangeTimePickerPopup), new PropertyMetadata());
    }
}
#endif
