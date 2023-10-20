namespace System.Windows.Media.Animation
{
    /// <summary>
    /// This class implements an easing function that backs up before going to the destination.
    /// </summary>
    [OpenSilver.NotImplemented]
    public partial class BackEase : EasingFunctionBase
    {
        //
        // Summary:
        //     Identifies the System.Windows.Media.Animation.BackEase.Amplitude dependency property.
        //
        // Returns:
        //     The identifier for the System.Windows.Media.Animation.BackEase.Amplitude dependency
        //     property.
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty AmplitudeProperty =
            DependencyProperty.Register("Amplitude",
                                        typeof(double),
                                        typeof(BackEase),
                                        new PropertyMetadata(1.0));

        //
        // Summary:
        //     Gets or sets the amplitude of retraction associated with a System.Windows.Media.Animation.BackEase
        //     animation.
        //
        // Returns:
        //     The amplitude of retraction associated with a System.Windows.Media.Animation.BackEase
        //     animation. This value must be greater than or equal to 0. The default is 1.
        [OpenSilver.NotImplemented]
        public double Amplitude
        {
            get { return (double)GetValue(AmplitudeProperty); }
            set { SetValue(AmplitudeProperty, value); }
        }

        //
        // Summary:
        //     Initializes a new instance of the System.Windows.Media.Animation.BackEase class.
        [OpenSilver.NotImplemented]
        public BackEase()
        {

        }

        //
        // Summary:
        //     Provides the logic portion of the easing function that you can override to produce
        //     the System.Windows.Media.Animation.EasingMode.EaseIn mode of the custom easing
        //     function.
        //
        // Parameters:
        //   normalizedTime:
        //     Normalized time (progress) of the animation.
        //
        // Returns:
        //     A double that represents the transformed progress.
        [OpenSilver.NotImplemented]
        protected override double EaseInCore(double normalizedTime)
        {
            return default(double);
        }
    }
}
