
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

namespace System.Windows.Media.Animation
{
    /// <summary>
    /// Represents an easing function that creates an animated bouncing effect.
    /// </summary>
    [OpenSilver.NotImplemented]
    public class BounceEase : EasingFunctionBase
    {
        /// <summary>
        /// Identifies the <see cref="BounceEase.Bounces"/> dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty BouncesProperty =
            DependencyProperty.Register(
                "Bounces",
                typeof(int),
                typeof(BounceEase),
                new PropertyMetadata(3));

        /// <summary>
        /// Identifies the <see cref="BounceEase.Bounciness"/> dependency
        /// property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty BouncinessProperty =
            DependencyProperty.Register(
                "Bounciness",
                typeof(double),
                typeof(BounceEase),
                new PropertyMetadata(2d));

        /// <summary>
        /// Initializes a new instance of the <see cref="BounceEase"/> class.
        /// </summary>
        [OpenSilver.NotImplemented]
        public BounceEase()
        {
        }
   
        /// <summary>
        /// Gets or sets the number of bounces.
        /// The value must be greater or equal to zero. Negative values will resolve 
        /// to zero. The default is 3.
        /// </summary>
        [OpenSilver.NotImplemented]
        public int Bounces
        {
            get { return (int)this.GetValue(BouncesProperty); }
            set { this.SetValue(BouncesProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value that specifies how bouncy the bounce animation is. Low values
        /// of this property result in bounces with little lose of height between bounces
        /// (more bouncy) while high values result in dampened bounces (less bouncy).
        /// This value must be positive. The default value is 2.
        /// </summary>
        [OpenSilver.NotImplemented]
        public double Bounciness
        {
            get { return (double)this.GetValue(BouncinessProperty); }
            set { this.SetValue(BouncinessProperty, value); }
        }

        /// <summary>
        /// Provides the logic portion of the easing function that you can override to produce
        /// the <see cref="EasingMode.EaseIn"/> mode of the custom easing
        /// function.
        /// </summary>
        /// <param name="normalizedTime">
        /// Normalized time (progress) of the animation.
        /// </param>
        /// <returns>
        /// A double that represents the transformed progress.
        /// </returns>
        [OpenSilver.NotImplemented]
        protected override double EaseInCore(double normalizedTime)
        {
            return 0d;
        }
    }
}
