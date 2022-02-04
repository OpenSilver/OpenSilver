#if MIGRATION

using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace Microsoft.Expression.Interactivity.Core
{
    //
    // Summary:
    //     An action that will change the state of a targeted storyboard when invoked.
    [OpenSilver.NotImplemented]
    public partial class ControlStoryboardAction : StoryboardAction
    {
        private bool isPaused;

        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty ControlStoryboardProperty =
            DependencyProperty.Register(nameof(ControlStoryboardOption),
                                        typeof(ControlStoryboardOption),
                                        typeof(ControlStoryboardAction),
                                        null);

        [OpenSilver.NotImplemented]
        public ControlStoryboardOption ControlStoryboardOption
        {
            get { return (ControlStoryboardOption)GetValue(ControlStoryboardProperty); }
            set { SetValue(ControlStoryboardProperty, value); }
        }

        //
        // Summary:
        //     This method is called when some criteria is met and the action should be invoked.
        //     This method will attempt to change the targeted storyboard in a way defined by
        //     the ControlStoryboardOption.
        //
        // Parameters:
        //   parameter:
        protected override void Invoke(object parameter)
        {
            if (AssociatedObject == null || Storyboard == null)
                return;
            switch (ControlStoryboardOption)
            {
                case ControlStoryboardOption.Play:
                    Storyboard.Begin();
                    break;
                case ControlStoryboardOption.Stop:
                    Storyboard.Stop();
                    break;
                case ControlStoryboardOption.TogglePlayPause:
                    ClockState clockState = ClockState.Stopped;
                    try
                    {
                        clockState = Storyboard.GetCurrentState();
                    }
                    catch (InvalidOperationException ex)
                    {
                    }
                    if (clockState == ClockState.Stopped)
                    {
                        isPaused = false;
                        Storyboard.Begin();
                        break;
                    }
                    if (isPaused)
                    {
                        isPaused = false;
                        Storyboard.Resume();
                        break;
                    }
                    isPaused = true;
                    Storyboard.Pause();
                    break;
                case ControlStoryboardOption.Pause:
                    Storyboard.Pause();
                    break;
                case ControlStoryboardOption.Resume:
                    Storyboard.Resume();
                    break;
                case ControlStoryboardOption.SkipToFill:
                    Storyboard.SkipToFill();
                    break;
            }
        }

        protected override void OnStoryboardChanged(DependencyPropertyChangedEventArgs args)
        {
            isPaused = false;
        }
    }
}

#endif
