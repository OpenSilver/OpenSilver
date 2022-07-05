using System.Collections.Generic;
using System.Globalization;
using System.Windows.Media.Animation;

namespace System.Windows.Controls.DataVisualization
{
    /// <summary>
    /// Represents a control that can animate the transitions between its specified
    /// dependency property.
    /// </summary>
    internal static class DependencyPropertyAnimationHelper
    {
        /// <summary>
        /// Number of key frames per second to generate the date time animations.
        /// </summary>
        public const int KeyFramesPerSecond = 20;
        /// <summary>
        /// The pattern used to ensure unique keys for the storyboards stored in
        /// a framework element's resource dictionary.
        /// </summary>
        private const string StoryboardKeyPattern = "__{0}__";

        /// <summary>Returns a unique key for a storyboard.</summary>
        /// <param name="propertyPath">The property path of the property that
        /// the storyboard animates.</param>
        /// <returns>A unique key for a storyboard.</returns>
        private static string GetStoryboardKey(string propertyPath)
        {
            return string.Format((IFormatProvider)CultureInfo.InvariantCulture, "__{0}__", new object[1]
            {
        (object) propertyPath
            });
        }

        /// <summary>
        /// Starts animating a dependency property of a framework element to a
        /// target value.
        /// </summary>
        /// <param name="target">The element to animate.</param>
        /// <param name="animatingDependencyProperty">The dependency property to
        /// animate.</param>
        /// <param name="propertyPath">The path of the dependency property to
        /// animate.</param>
        /// <param name="targetValue">The value to animate the dependency
        /// property to.</param>
        /// <param name="timeSpan">The duration of the animation.</param>
        /// <param name="easingFunction">The easing function to uses to
        /// transition the data points.</param>
        public static void BeginAnimation(this FrameworkElement target, DependencyProperty animatingDependencyProperty, string propertyPath, object targetValue, TimeSpan timeSpan, IEasingFunction easingFunction)
        {
            Storyboard storyBoard = target.Resources[(object)DependencyPropertyAnimationHelper.GetStoryboardKey(propertyPath)] as Storyboard;
            if (storyBoard != null)
            {
                object obj = target.GetValue(animatingDependencyProperty);
                storyBoard.Stop();
                target.SetValue(animatingDependencyProperty, obj);
                target.Resources.Remove(DependencyPropertyAnimationHelper.GetStoryboardKey(propertyPath));
            }
            storyBoard = DependencyPropertyAnimationHelper.CreateStoryboard(target, animatingDependencyProperty, propertyPath, ref targetValue, timeSpan, easingFunction);
            storyBoard.Completed += (EventHandler)((source, args) =>
            {
                storyBoard.Stop();
                target.SetValue(animatingDependencyProperty, targetValue);
                target.Resources.Remove(DependencyPropertyAnimationHelper.GetStoryboardKey(propertyPath));
            });
            target.Resources.Add(DependencyPropertyAnimationHelper.GetStoryboardKey(propertyPath), (object)storyBoard);
            storyBoard.Begin();
        }

        /// <summary>
        /// Creates a story board that animates a dependency property to a
        /// value.
        /// </summary>
        /// <param name="target">The element that is the target of the
        /// storyboard.</param>
        /// <param name="animatingDependencyProperty">The dependency property
        /// to animate.</param>
        /// <param name="propertyPath">The property path of the dependency
        /// property to animate.</param>
        /// <param name="toValue">The value to animate the dependency property
        /// to.</param>
        /// <param name="durationTimeSpan">The duration of the animation.</param>
        /// <param name="easingFunction">The easing function to use to
        /// transition the data points.</param>
        /// <returns>The story board that animates the property.</returns>
        private static Storyboard CreateStoryboard(FrameworkElement target, DependencyProperty animatingDependencyProperty, string propertyPath, ref object toValue, TimeSpan durationTimeSpan, IEasingFunction easingFunction)
        {
            object obj = target.GetValue(animatingDependencyProperty);
            Storyboard storyboard = new Storyboard();
            Storyboard.SetTarget((Timeline)storyboard, (DependencyObject)target);
            Storyboard.SetTargetProperty((Timeline)storyboard, new PropertyPath(propertyPath, new object[0]));
            if (obj != null && toValue != null)
            {
                double doubleValue1;
                double doubleValue2;
                if (ValueHelper.TryConvert(obj, out doubleValue1) && ValueHelper.TryConvert(toValue, out doubleValue2))
                {
                    DoubleAnimation doubleAnimation = new DoubleAnimation();
                    doubleAnimation.EasingFunction = easingFunction;
                    doubleAnimation.Duration = (Duration)durationTimeSpan;
                    doubleAnimation.To = new double?(ValueHelper.ToDouble(toValue));
                    toValue = (object)doubleAnimation.To;
                    storyboard.Children.Add((Timeline)doubleAnimation);
                }
                else
                {
                    DateTime dateTimeValue1;
                    DateTime dateTimeValue2;
                    if (ValueHelper.TryConvert(obj, out dateTimeValue1) && ValueHelper.TryConvert(toValue, out dateTimeValue2))
                    {
                        ObjectAnimationUsingKeyFrames animationUsingKeyFrames = new ObjectAnimationUsingKeyFrames();
                        animationUsingKeyFrames.Duration = (Duration)durationTimeSpan;
                        long count = (long)(durationTimeSpan.TotalSeconds * 20.0);
                        if (count < 2L)
                            count = 2L;
                        IEnumerable<TimeSpan> intervalsInclusive = ValueHelper.GetTimeSpanIntervalsInclusive(durationTimeSpan, count);
                        foreach (DiscreteObjectKeyFrame discreteObjectKeyFrame in EnumerableFunctions.Zip<DateTime, TimeSpan, DiscreteObjectKeyFrame>(ValueHelper.GetDateTimesBetweenInclusive(dateTimeValue1, dateTimeValue2, count), intervalsInclusive, (Func<DateTime, TimeSpan, DiscreteObjectKeyFrame>)((dateTime, timeSpan) =>
                        {
                            return new DiscreteObjectKeyFrame()
                            {
                                Value = (object)dateTime,
                                KeyTime = (KeyTime)timeSpan
                            };
                        })))
                        {
                            animationUsingKeyFrames.KeyFrames.Add((ObjectKeyFrame)discreteObjectKeyFrame);
                            toValue = discreteObjectKeyFrame.Value;
                        }
                        storyboard.Children.Add((Timeline)animationUsingKeyFrames);
                    }
                }
            }
            if (storyboard.Children.Count == 0)
            {
                ObjectAnimationUsingKeyFrames animationUsingKeyFrames = new ObjectAnimationUsingKeyFrames();
                DiscreteObjectKeyFrame discreteObjectKeyFrame1 = new DiscreteObjectKeyFrame();
                discreteObjectKeyFrame1.Value = toValue;
                discreteObjectKeyFrame1.KeyTime = (KeyTime)new TimeSpan(0, 0, 0);
                DiscreteObjectKeyFrame discreteObjectKeyFrame2 = discreteObjectKeyFrame1;
                animationUsingKeyFrames.KeyFrames.Add((ObjectKeyFrame)discreteObjectKeyFrame2);
                storyboard.Children.Add((Timeline)animationUsingKeyFrames);
            }
            return storyboard;
        }
    }
}

