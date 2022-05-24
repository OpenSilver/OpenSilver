
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

#if !MIGRATION
using Windows.Foundation;
#endif

#if MIGRATION
namespace System.Windows.Media.Animation
#else
namespace Windows.UI.Xaml.Media.Animation
#endif
{
    /// <summary>
    /// Animates the value of a <see cref="Point"/> property between two target values
    /// using linear interpolation over a specified <see cref="Timeline.Duration"/>.
    /// </summary>
    public sealed partial class PointAnimation : AnimationTimeline
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PointAnimation"/>
        /// class.
        /// </summary>
        public PointAnimation()
        {
        }

        /// <summary>
        /// Identifies the <see cref="By"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ByProperty =
            DependencyProperty.Register(
                nameof(By),
                typeof(Point?),
                typeof(PointAnimation),
                new PropertyMetadata((object)null));

        /// <summary>
        /// Gets or sets the total amount by which the animation changes its starting value.
        /// The default is null.
        /// </summary>
        public Point? By
        {
            get { return (Point?)this.GetValue(ByProperty); }
            set { this.SetValue(ByProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="EasingFunction"/> dependency
        /// property.
        /// </summary>
        public static readonly DependencyProperty EasingFunctionProperty =
            DependencyProperty.Register(
                nameof(EasingFunction),
                typeof(IEasingFunction),
                typeof(PointAnimation),
                new PropertyMetadata((object)null));

        /// <summary>
        /// Gets or sets the easing function you are applying to the animation.
        /// The default is null.
        /// </summary>
        public IEasingFunction EasingFunction
        {
            get { return (EasingFunctionBase)GetValue(EasingFunctionProperty); }
            set { SetValue(EasingFunctionProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="From"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FromProperty = 
            DependencyProperty.Register(
                nameof(From), 
                typeof(Point?), 
                typeof(PointAnimation),
                new PropertyMetadata((object)null));

        /// <summary>
        /// Gets or sets the animation's starting value. The default is null.
        /// </summary>
        public Point? From
        {
            get { return (Point?)this.GetValue(FromProperty); }
            set { this.SetValue(FromProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="To"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ToProperty =
            DependencyProperty.Register(
                nameof(To),
                typeof(Point?),
                typeof(PointAnimation),
                new PropertyMetadata((object)null));

        /// <summary>
        /// Gets or sets the animation's ending value. The default is null.
        /// </summary>
        public Point? To
        {
            get { return (Point?)this.GetValue(ToProperty); }
            set { this.SetValue(ToProperty, value); }
        }

        internal override void GetTargetInformation(IterationParameters parameters)
        {
            _parameters = parameters;
            DependencyObject target;
            PropertyPath propertyPath;

            GetTargetElementAndPropertyInfo(parameters, out target, out propertyPath);

            _propertyContainer = target;
            _targetProperty = propertyPath;
            _propDp = GetProperty(_propertyContainer, _targetProperty);
            _target = Storyboard.GetTarget(this);
            _targetName = Storyboard.GetTargetName(this);
        }

        internal override void Apply(IterationParameters parameters, bool isLastLoop)
        {
            if (To != null)
            {
                OnAnimationCompleted(parameters, isLastLoop, To.Value, _propertyContainer, _targetProperty);
            }
        }

        private void OnAnimationCompleted(IterationParameters parameters, bool isLastLoop, object value, DependencyObject target, PropertyPath propertyPath)
        {
            if (!this._isUnapplied)
            {
                if (isLastLoop)
                {
                    AnimationHelpers.ApplyValue(target, propertyPath, value);
                }

                OnIterationCompleted(parameters);
            }
        }
    }
}
