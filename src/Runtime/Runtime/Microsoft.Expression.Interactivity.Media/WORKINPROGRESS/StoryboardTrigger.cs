using System.Windows.Interactivity;

#if MIGRATION
using System.Windows;
using System.Windows.Media.Animation;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;
#endif

namespace Microsoft.Expression.Interactivity.Media
{
    /// <summary>
    /// An abstract class that provides the ability to target a Storyboard.
    /// </summary>
    /// <remarks>
    /// For Trigger authors, this class provides a standard way to target a Storyboard. Design tools may choose to provide a 
    /// special editing experience for classes that inherit from this trigger, thereby improving the designer experience. 
    /// </remarks>
    [OpenSilver.NotImplemented]
    public abstract class StoryboardTrigger : TriggerBase<DependencyObject>
    {
        /// <summary>
        /// The targeted Storyboard. This is a dependency property.
        /// </summary>
        public Storyboard Storyboard
        {
            get { return (Storyboard)GetValue(StoryboardProperty); }
            set { SetValue(StoryboardProperty, value); }
        }

        public static readonly DependencyProperty StoryboardProperty =
            DependencyProperty.Register("Storyboard", typeof(Storyboard), typeof(StoryboardTrigger),
                new FrameworkPropertyMetadata(new PropertyChangedCallback(OnStoryboardChanged)));

        private static void OnStoryboardChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            StoryboardTrigger storyboardTrigger = sender as StoryboardTrigger;
            if (storyboardTrigger != null)
            {
                storyboardTrigger.OnStoryboardChanged(args);
            }
        }

        /// <summary>
        /// This method is called when the Storyboard property is changed.
        /// </summary>
        protected virtual void OnStoryboardChanged(DependencyPropertyChangedEventArgs args)
        {
        }
    }
}